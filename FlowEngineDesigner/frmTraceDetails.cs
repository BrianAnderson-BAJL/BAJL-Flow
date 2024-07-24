using FlowEngineCore;
using FlowEngineCore.Administration.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FlowEngineDesigner
{
  public partial class frmTraceDetails : Form
  {
    private int StepId = -1;
    private const string NO_NAME = "[NO NAME]";
    public frmTraceDetails()
    {
      InitializeComponent();
    }

    private void frmTraceDetails_Load(object sender, EventArgs e)
    {
      string xmlString = txtXml.Text;
      xmlString = Xml.GetXMLChunk(ref xmlString, "Trace");
      txtFileName.Text = Xml.GetXMLChunk(ref xmlString, "FileName");
      StepId = Xml.GetXMLChunkAsInt(ref xmlString, "StepId");
      txtStepName.Text = Xml.GetXMLChunk(ref xmlString, "StepName") + $"[{StepId}]";
      bool Success = Xml.GetXMLChunkAsBool(ref xmlString, "Success");

      if (Success == true)
      {
        txtError.Text = "";
        lblSuccess.Text = "Success";
        lblSuccess.ForeColor = Color.Green;
      }
      else
      {
        txtError.Text = Xml.GetXMLChunk(ref xmlString, "ErrorNumber") + " - " + Xml.GetXMLChunk(ref xmlString, "ErrorDescription");
        lblSuccess.Text = "Failure";
        lblSuccess.ForeColor = Color.Red;
      }

      TreeNode paramNode = tvVariables.Nodes.Add("Parameters");
      TreeNode varNode = tvVariables.Nodes.Add("Flow Variables");
      Variable parmNodeVar = new Variable("Parameters");
      Variable VarNodeVar = new Variable("Flow Variables");
      paramNode.Tag = parmNodeVar;
      varNode.Tag = VarNodeVar;

      //Populate the parameters
      for (int x = 0; x < int.MaxValue; x++) //Will break out when it runs out of parameters
      {
        string valXml = Xml.GetXMLChunk(ref xmlString, "parm" + x.ToString(), Xml.BAJL_ENCODE.Base64Encoding);
        if (valXml == "")
          break;
        Variable? var = Variable.JsonParse(ref valXml);
        if (var is null)
          continue;

        parmNodeVar.SubVariableAdd(var);
        PopulateNode(paramNode, var);
      }

      string flowVarsXmlString = Xml.GetXMLChunk(ref xmlString, "FlowVariables");
      //Populate the flow variables
      for (int x = 0; x < int.MaxValue; x++) //Will break out when it runs out of parameters
      {
        string valXml = Xml.GetXMLChunk(ref flowVarsXmlString, "Variable", Xml.BAJL_ENCODE.Base64Encoding);
        if (valXml == "")
          break;
        string junk = valXml;
        Variable? var = Variable.JsonParse(ref valXml);
        if (var is null)
          continue;

        VarNodeVar.SubVariableAdd(var);
        PopulateNode(varNode, var);
      }
      paramNode.Expand();
      varNode.Expand();
    }

    private void btnShowFlow_Click(object sender, EventArgs e)
    {
      Form? form = Global.FindOpenFormByTitleText($"Flow - [{txtFileName.Text}]");
      if (form is null)
      {
        FlowOpen fo = new FlowOpen(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, txtFileName.Text);
        cServer.SendAndResponse(fo.GetPacket(), Callback_FileOpen);
      }
      else
      {
        form.BringToFront();
      }
    }


    private void Callback_FileOpen(FlowEngineCore.Administration.EventArgsPacket e)
    {
      FlowOpenResponse response = new FlowOpenResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        string flowXml = response.FlowXml;

        cFlowWrapper flowWrapper = new cFlowWrapper(cFlowWrapper.INCLUDE_START_STEP.EXCLUDE);
        flowWrapper.FileName = txtFileName.Text;
        flowWrapper.XmlRead(ref flowXml);
        flowWrapper.PopulateSampleVariablesFromPlugin();
        frmFlow f = new frmFlow(flowWrapper);
        flowWrapper.Center(f.Camera, f.pictureBox1);

        Global.Layout.ExecuteLayout(f, cLayoutForm.LAYOUT_FORM.Flow);

        f.Show();
      }
      else
      {
        MessageBox.Show("Server open returned a non success response, Trace window may have more information.");
      }
    }

    private void btnHighlightStep_Click(object sender, EventArgs e)
    {
      Form? form = Global.FindOpenFormByTitleText($"Flow - [{txtFileName.Text}]");
      if (form is null)
        return;

      form.BringToFront();
      frmFlow? flowForm = form as frmFlow;
      if (flowForm is null)
        return;

      flowForm.HighlightStep(StepId);
    }


    private void lvParameters_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        lvVariables.Tag = e.Location;
        contextMenuStrip1.Show(lvVariables, e.Location);

      }
    }

    private void copyToolStripMenuItem_Click_1(object sender, EventArgs e)
    {
      if (lvVariables.SelectedItems.Count == 0)
        return;
      Variable? var = lvVariables.SelectedItems[0].Tag as Variable;
      if (var is not null)
      {
        Clipboard.SetText(var.ToJson());
        return;
      }
      Point location = (Point)lvVariables.Tag;
      var hitTest = lvVariables.HitTest(location);
      if (hitTest.Item != null && hitTest.SubItem != null)
      {
        Clipboard.SetText(hitTest.SubItem.Text);

      }
    }

    private void tvVariables_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {

    }

    private void PopulateNode(TreeNode node, Variable var)
    {
      string tmpName = var.Name;
      if (tmpName == "")
        tmpName = NO_NAME;
      node = node.Nodes.Add(tmpName);
      node.Tag = var;
      for (int x = 0; x < var.SubVariables.Count; x++)
      {
        //TreeNode tn = node.Nodes.Add(var.SubVariables[x].Name);
        PopulateNode(node, var.SubVariables[x]);
      }
    }

    private void tvVariables_AfterSelect(object sender, TreeViewEventArgs e)
    {
      tvVariablesSelected();
    }

    private void tvVariablesSelected()
    {
      if (tvVariables.SelectedNode is null)
        return;
      TreeNode node = tvVariables.SelectedNode;
      Variable? var = node.Tag as Variable;
      if (var is null)
        return;

      lvVariables.Items.Clear();
      if (var.DataType == DATA_TYPE._None)
      {
        for (int x = 0; x < var.SubVariables.Count; x++)
        {
          string tmpName = var.SubVariables[x].Name;
          if (tmpName == "")
            tmpName = NO_NAME;

          ListViewItem lvi = lvVariables.Items.Add(tmpName);
          lvi.Tag = node.Nodes[x];
          if (var.SubVariables[x].DataType == DATA_TYPE._None)
          {
            lvi.SubItems.Add($"{var.SubVariables[x].SubVariables.Count} - [SUB VARIABLES]");
          }
          else
          {
            string tempValue = var.SubVariables[x].GetValueAsString();
            lvi.SubItems.Add(tempValue);
          }
        }
      }
      else
      {
        ListViewItem lvi = lvVariables.Items.Add(var.Name);
        lvi.Tag = node;
        lvi.SubItems.Add(var.Value.ToString());
      }
    }

    private void lvVariables_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (lvVariables.SelectedItems.Count == 0)
        return;

      ListViewItem lvi = lvVariables.SelectedItems[0];

      TreeNode? node = lvi.Tag as TreeNode;
      if (node is null)
        return;

      tvVariables.SelectedNode = node;
      node.Expand();
      tvVariablesSelected();
    }
  }
}
