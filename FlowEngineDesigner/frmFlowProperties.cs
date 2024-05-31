using FlowEngineCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowEngineDesigner
{
  public partial class frmFlowProperties : Form
  {
    private cFlowWrapper Flow;
    private List<ucParameter> userControls = new List<ucParameter>();

    public frmFlowProperties(cFlowWrapper flow)
    {
      Flow = flow;
      InitializeComponent();
      WinApi.SetTabWidth(txtSampleData, 4);
    }

    private void frmFlowProperties_Load(object sender, EventArgs e)
    {
      string[] startTypes = Enum.GetNames(typeof(FlowEngineCore.FlowRequest.START_TYPE));
      for (int x = 0; x < startTypes.Length; x++)
      {
        cmbDebugStartType.Items.Add(startTypes[x]);
      }
      Global.ComboBoxSetIndex(cmbDebugStartType, Flow.DebugStartType.ToString());
      cmbDebugStartType.SelectedIndexChanged += CmbDebugStartType_SelectedIndexChanged;
      for (int x = 0; x < PluginManager.Plugins.Count; x++)
      {
        cmbPlugins.Items.Add(PluginManager.Plugins[x]);
        if (Flow.StartPlugin is not null && PluginManager.Plugins[x].Name == Flow.StartPlugin.Name)
        {
          cmbPlugins.SelectedIndex = x;
        }
      }
      cmbPlugins.SelectedIndexChanged += cmbPlugins_SelectedIndexChanged;
      txtCreatedDateTime.Text = Flow.CreateDateTime.ToString();
      txtModifiedLastDateTime.Text = Flow.ModifiedLastDateTime.ToString();
      if (Flow.SampleDataFormat == DATA_FORMAT.Json)
      {
        rbSampleDataFormatJson.Checked = true;
      }
      else if (Flow.SampleDataFormat == DATA_FORMAT.Xml)
        rbSampleDataFormatXml.Checked = true;
      txtSampleData.Text = Flow.SampleData;

      AddFlowStartCommands();
    }

    private void CmbDebugStartType_SelectedIndexChanged(object? sender, EventArgs e)
    {
      Flow.DebugStartType = System.Enum.Parse<FlowEngineCore.FlowRequest.START_TYPE>(cmbDebugStartType.Text);
    }

    private void label3_Click(object sender, EventArgs e)
    {

    }

    private void cmbPlugins_SelectedIndexChanged(object? sender, EventArgs e)
    {
      Flow.StartPlugin = cmbPlugins.SelectedItem as FlowEngineCore.Plugin;
      if (Flow.StartPlugin is not null && Flow.StartPlugin.SampleStartData is not null)
      {
        Flow.StartCommands = Flow.StartPlugin.FlowStartCommands.ToParmVars();
        Flow.SampleData = Flow.StartPlugin.SampleStartData.ToJson();
      }
      AddFlowStartCommands();

      if (Flow.SampleData is null)
        return;

      rbSampleDataFormatJson.Checked = true;
      txtSampleData.Text += Flow.SampleData;
    }

    private void AddFlowStartCommands()
    {
      if (Flow.StartPlugin is null)
        return;

      for (int x = 0; x < userControls.Count; x++)
      {
        gbStartOptions.Controls.Remove(userControls[x]);
      }
      userControls.Clear();


      int StartX = 10;
      int StartY = 20;
      for (int x = 0; x < Flow.StartCommands.Count; x++)
      {

        PARM_VAR parmVar = Flow.StartCommands[x];


        if (parmVar.Parm.DataType == DATA_TYPE.Integer)
        {
          ucParameterInteger uc = new ucParameterInteger(parmVar, null, Flow);
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          gbStartOptions.Controls.Add(uc);
          userControls.Add(uc);
        }
        if ((parmVar.Parm.DataType == DATA_TYPE.String && parmVar.Parm.StringSubType != STRING_SUB_TYPE.DropDownList) || parmVar.Parm.DataType == DATA_TYPE.Object)
        {
          ucParameterString uc = new ucParameterString(parmVar, null, Flow);
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          gbStartOptions.Controls.Add(uc);
          userControls.Add(uc);

        }
        if (parmVar.Parm.DataType == DATA_TYPE.Decimal)
        {
          //txtValue.Text = ParmDec.Value.ToString();
        }
        if (parmVar.Parm.DataType == DATA_TYPE.Boolean)
        {

        }
        if (parmVar.Parm.DataType == DATA_TYPE.String && parmVar.Parm.StringSubType == STRING_SUB_TYPE.DropDownList)
        {
          ucParameterDropDownList uc = new ucParameterDropDownList(parmVar);
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          gbStartOptions.Controls.Add(uc);
          userControls.Add(uc);
        }
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      Flow.SampleData = txtSampleData.Text;
      Flow.PopulateSampleVariablesFromPlugin();

      for (int x = 0; x < userControls.Count; x++)
      {
        ucParameter parm = userControls[x] as ucParameter;
        if (parm is not null)
        {
          parm.UpdateValues();
        }
      }
      this.Close();
    }

    private void rbSampleDataFormatNone_CheckedChanged(object sender, EventArgs e)
    {
      Flow.SampleDataFormat = DATA_FORMAT._None;
      txtSampleData.ReadOnly = true;
    }

    private void rbSampleDataFormatJson_CheckedChanged(object sender, EventArgs e)
    {
      Flow.SampleDataFormat = DATA_FORMAT.Json;
      txtSampleData.ReadOnly = false;
    }

    private void rbSampleDataFormatXml_CheckedChanged(object sender, EventArgs e)
    {
      Flow.SampleDataFormat = DATA_FORMAT.Xml;
      txtSampleData.ReadOnly = false;
    }

    private void txtSampleData_TextChanged(object sender, EventArgs e)
    {
      Flow.SampleData = txtSampleData.Text;
      if (Flow.SampleDataFormat == DATA_FORMAT.Json)
      {
        string temp = Flow.SampleData;
        try
        {
          Variable? v = Variable.JsonParse(ref temp);
          rbSampleDataFormatJson.ForeColor = Color.Green;
        }
        catch
        {
          rbSampleDataFormatJson.ForeColor = Color.Red;
        }
      }
    }
  }
}
