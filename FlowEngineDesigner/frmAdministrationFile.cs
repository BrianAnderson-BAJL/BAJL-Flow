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

    public delegate void FlowWrapperChanged(cFlowWrapper flowWrapper);

    private FILE_MODE FileMode = FILE_MODE.Open;
    private cFlowWrapper FlowWrapper;
    private FlowWrapperChanged Call_Back;
    public frmAdministrationFile(FILE_MODE fileMode, cFlowWrapper flowWrapper, FlowWrapperChanged call_back)
    {
      InitializeComponent();
      FileMode = fileMode;
      FlowWrapper = flowWrapper;
      Call_Back = call_back;
    }

    private void frmAdministrationFile_Load(object sender, EventArgs e)
    {
      btnAction.Text = FileMode.ToString();
      SecurityProfile.SECURITY_ACCESS_LEVEL accessLevel = cServer.AccessLevelForUserLoggedIn(SecurityProfile.SECURITY_AREA.Flows);
      if (accessLevel >= SecurityProfile.SECURITY_ACCESS_LEVEL.Full)
      {
        chkDeployLive.Enabled = true;
      }
      else
      {
        chkDeployLive.Enabled = false;
      }
      if (FlowWrapper is not null)
      {
        txtFileName.Text = FlowWrapper.FileName;
      }
      FlowsGet message = new FlowsGet(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn!.SessionKey);
      cServer.SendAndResponse(message.GetPacket(), Callback_Files);

    }

    private void frmAdministrationFile_Activated(object sender, EventArgs e)
    {
      //FlowsGet message = new FlowsGet(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn!.SessionKey);
      //cServer.SendAndResponse(message.GetPacket(), Callback_Files);

    }


    private void Callback_Files(Core.Administration.EventArgsPacket e)
    {
      //TreeNode? previousSelectedNode = tvDirectories.SelectedNode;
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
        if (tvDirectories.GetNodeCount(true) <= 30)
          tvDirectories.Nodes[0].ExpandAll();
        else
          tvDirectories.Nodes[0].Expand();

        if (cOptions.AdministrationLastFilePath != "") //We want to select the same directory that was already selected if the treeview was refreshed
          tvDirectories.SelectedNode = Global.TreeViewSelectNodeByText(tvDirectories.Nodes, cOptions.AdministrationLastFilePath);
        else
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
          newNode = tvDirectories.Nodes.Add("/");
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

      cOptions.AdministrationLastFilePath = e.Node.Text;
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


    private void btnAction_Click(object sender, EventArgs e)
    {
      if (FlowWrapper is null && FileMode == FILE_MODE.Save)
      {
        MessageBox.Show("No flow was passed to dialog, can't save.");
        return;
      }
      if (txtFileName.Text == "")
      {
        MessageBox.Show("No file name entered, can't save.");
        txtFileName.Focus();
        return;
      }
      for (int x = 0; x < Core.Global.IllegalFileNameCharacters.Length; x++)
      {
        if (txtFileName.Text.Contains(Core.Global.IllegalFileNameCharacters[x]) == true)
        {
          MessageBox.Show("Illegal characters detected, file name can't contain '" + Core.Global.IllegalFileNameCharacters[x] + "'");
          txtFileName.Focus();
          return;
        }
      }
      for (int x = 0; x < Core.Global.IllegalFileNameStartCharacters.Length; x++)
      {
        if (txtFileName.Text.StartsWith(Core.Global.IllegalFileNameStartCharacters[x]) == true)
        {
          MessageBox.Show("Illegal characters at beginning of file name, file name can't start with '" + Core.Global.IllegalFileNameStartCharacters[x] + "'");
          txtFileName.Focus();
          return;
        }
      }
      if (txtFileName.Text.Contains(".") == true && txtFileName.Text.ToLower().Contains(".flow") == false)
      {
        MessageBox.Show("Illegal extension for file name, file name must end in '.flow' or leave off to have the .flow extension added automatically.");
        txtFileName.Focus();
        return;
      }
      if (txtFileName.Text.ToLower().EndsWith(".flow") == false)
      {
        txtFileName.Text += ".flow";
      }
      if (FileMode == FILE_MODE.Save)
      {
        for (int x = 0; x < lvFiles.Items.Count; x++)
        {
          if (txtFileName.Text == lvFiles.Items[x].Text)
          {
            //Replicate the normal overwrite file save as message box
            if (MessageBox.Show(txtFileName.Text + " already exists.\nDo you want to replace it?", "Confirm Save As", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
            {
              return;
            }
            break;
          }
        }
      }

      string fileName = txtFileName.Text;
      if (tvDirectories.SelectedNode is not null && tvDirectories.SelectedNode.Text != "/") //Only append the path if something is selected and it isn't the root path /
      {
        fileName = tvDirectories.SelectedNode.Text + "/" + fileName;
      }

      if (FileMode == FILE_MODE.Save)
      {
        FlowSave fs = new FlowSave(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, fileName, chkDeployLive.Checked, FlowWrapper!.XmlWriteMemory()); //Stupid parser doesn't understand that FlowWrapper can't be null here, so the !
        cServer.SendAndResponse(fs.GetPacket(), Callback_FileSave);
      }
      if (FileMode == FILE_MODE.Open)
      {
        FlowOpen fo = new FlowOpen(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, fileName);
        cServer.SendAndResponse(fo.GetPacket(), Callback_FileOpen);
      }
    }

    private void Callback_FileSave(Core.Administration.EventArgsPacket e)
    {
      BaseResponse response = new BaseResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        this.Close();
      }
    }

    private void Callback_FileOpen(Core.Administration.EventArgsPacket e)
    {
      FlowOpenResponse response = new FlowOpenResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        string flowXml = response.FlowXml;
        //if (FlowWrapper is not null)
        //{
        FlowWrapper = new cFlowWrapper();
        FlowWrapper.XmlRead(ref flowXml);
        Call_Back(FlowWrapper);
        //}
        //else
        //{
        //  FlowWrapper = new cFlowWrapper();
        //  FlowWrapper.XmlRead(ref flowXml);
        //  //frmFlow f = new frmFlow(FlowWrapper);
        //  //f.Show();
        //}
        this.Close();
      }
    }

    private void lvFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      if (lvFiles.SelectedItems.Count <= 0)
      {
        return;
      }
      txtFileName.Text = lvFiles.SelectedItems[0].Text;
    }

    private void lvFiles_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      btnAction.PerformClick();
    }
  }
}
