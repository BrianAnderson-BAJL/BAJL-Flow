using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;
using Core.Administration.Messages;

namespace FlowEngineDesigner
{
  public partial class frmAdministrationFile : Form
  {
    private struct FLOW_FILE_INFO
    {
      public string FileName;
      public DateTime ModifiedDateTime;
      public string PluginStarting;
      public string StartCommands;
    }

    private FILE_MODE FileMode = FILE_MODE.Open;
    public frmAdministrationFile(FILE_MODE fileMode)
    {
      InitializeComponent();
      FileMode = fileMode;
    }

    private void frmAdministrationFile_Load(object sender, EventArgs e)
    {

    }

    private void frmAdministrationFile_Activated(object sender, EventArgs e)
    {
      FlowsGet message = new FlowsGet(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn!.SessionKey);
      cServer.SendAndResponse(message.GetPacket(), Callback_Files);

    }


    private void Callback_Files(Core.Administration.EventArgsPacket e)
    {
      FlowsGetResponse data = new FlowsGetResponse(e.Packet);
      tvDirectories.Nodes.Clear();
      string xmlStr = data.FlowsXml;
      string directories = Xml.GetXMLChunk(ref xmlStr, "Directories");
      string directory = Xml.GetXMLChunk(ref directories, "Directory0");
      if (directory.Length > 0)
      {
        DirectoryAdd(ref directory, null, 1);
      }
      if (tvDirectories.Nodes.Count > 0)
      {
        tvDirectories.Nodes[0].Expand();
        tvDirectories.SelectedNode = tvDirectories.Nodes[0];
      }
    }

    private void DirectoryAdd(ref string directory, TreeNode? parentNode, int depth)
    {
      while (directory.Length > 0)
      {
        TreeNode? newNode;
        string path = Xml.GetXMLChunk(ref directory, "Path");
        if (parentNode is null)
        {
          newNode = tvDirectories.Nodes.Add("/ [root]");
        }
        else
        {
          newNode = parentNode.Nodes.Add(path);
        }
        List<FLOW_FILE_INFO> files = new List<FLOW_FILE_INFO>(128);
        string fileXml = Xml.GetXMLChunk(ref directory, "File", "Directory" + depth.ToString());
        while (fileXml.Length > 0)
        {
          FLOW_FILE_INFO flowInfo = new FLOW_FILE_INFO();
          flowInfo.FileName = Xml.GetXMLChunk(ref fileXml, "FileName");
          flowInfo.ModifiedDateTime = Xml.GetXMLChunkAsDateTime(ref fileXml, "ModifiedDateTime");
          flowInfo.PluginStarting = Xml.GetXMLChunk(ref fileXml, "PluginStarting");
          flowInfo.StartCommands = Xml.GetXMLChunk(ref fileXml, "StartCommands");
          files.Add(flowInfo);
          fileXml = Xml.GetXMLChunk(ref directory, "File", "Directory" + depth.ToString());
        }

        newNode.Tag = files;
        directory = Xml.GetXMLChunk(ref directory, "Directory" + depth.ToString());
        if (directory.Length > 0)
        {
          DirectoryAdd(ref directory, newNode, depth + 1);
        }
      }
    }

    private void tvDirectories_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (e.Node is null)
        return;

      List<FLOW_FILE_INFO>? Flows = e.Node.Tag as List<FLOW_FILE_INFO>;
      if (Flows is null)
        return;

      lvFiles.Items.Clear();
      for (int x = 0; x < Flows.Count; x++)
      {
        FLOW_FILE_INFO flow = Flows[x];
        ListViewItem lvi = lvFiles.Items.Add(flow.FileName);
        lvi.SubItems.Add(flow.ModifiedDateTime.ToString());
        lvi.SubItems.Add(flow.PluginStarting);
        lvi.SubItems.Add(flow.StartCommands);
      }
    }
  }
}
