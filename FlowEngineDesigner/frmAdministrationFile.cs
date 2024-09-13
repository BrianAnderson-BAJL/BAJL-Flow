using FlowEngineCore;
using FlowEngineCore.Administration.Messages;
using System.Diagnostics.Eventing.Reader;

namespace FlowEngineDesigner
{
  public partial class frmAdministrationFile : Form
  {
    public enum FILE_TYPE
    {
      Flow,
      Template,
    }
    private struct FLOW_FILE_INFO
    {
      public string FileName;
      public DateTime ModifiedDateTime;
      public string PluginStarting;
      public string StartCommands;
    }

    public delegate void FlowWrapperChanged(cFlowWrapper flowWrapper, string fileName = "");

    private FILE_MODE FileMode = FILE_MODE.Open;
    private FILE_TYPE FileType = FILE_TYPE.Flow;
    private cFlowWrapper FlowWrapper;
    private FlowWrapperChanged Call_Back;
    private string SaveAsFileName = "";
    public frmAdministrationFile(FILE_MODE fileMode, cFlowWrapper flowWrapper, FlowWrapperChanged call_back, FILE_TYPE fileType = FILE_TYPE.Flow)
    {
      InitializeComponent();
      FileMode = fileMode;
      FileType = fileType;
      FlowWrapper = flowWrapper;
      Call_Back = call_back;
    }

    private void frmAdministrationFile_Load(object sender, EventArgs e)
    {
      btnAction.Text = FileMode.ToString();

      if (FileType == FILE_TYPE.Flow)
      {
        GetFlowFiles();
      }
      else
      {
        GetTemplateFiles();
      }
    }

    private void GetTemplateFiles()
    {
      chkDeployLive.Visible = false;
      TemplatesGet message = new TemplatesGet(cServer.Info.PrivateKey, cServer.UserLoggedIn!.SessionKey);
      cServer.SendAndResponse(message.GetPacket(), Callback_Files);
    }
    private void GetFlowFiles()
    {
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
      FlowsGet message = new FlowsGet(cServer.Info.PrivateKey, cServer.UserLoggedIn!.SessionKey);
      cServer.SendAndResponse(message.GetPacket(), Callback_Files);
      if (FileMode == FILE_MODE.Open)
      {
        chkDeployLive.Enabled = false;
      }
    }

    private void frmAdministrationFile_Activated(object sender, EventArgs e)
    {
      //FlowsGet message = new FlowsGet(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn!.SessionKey);
      //cServer.SendAndResponse(message.GetPacket(), Callback_Files);

    }


    private void Callback_Files(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() != BaseResponse.RESPONSE_CODE.Success)
        return;
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

        if (cServer.Info.LastFilePath != "") //We want to select the same directory that was already selected if the treeview was refreshed
          tvDirectories.SelectedNode = Global.TreeViewSelectNodeByText(tvDirectories.Nodes, cServer.Info.LastFilePath);
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

      Flows = Flows.OrderBy(ffi => ffi.FileName).ToList(); //TODO: Properly sort the columns when clicking on them, this is just a Quick fix to order the names by the file name
      cServer.Info.LastFilePath = e.Node.Text;
      cServer.SaveProfiles();
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
      for (int x = 0; x < FlowEngineCore.Global.IllegalFileNameCharacters.Length; x++)
      {
        if (txtFileName.Text.Contains(FlowEngineCore.Global.IllegalFileNameCharacters[x]) == true)
        {
          MessageBox.Show("Illegal characters detected, file name can't contain '" + FlowEngineCore.Global.IllegalFileNameCharacters[x] + "'");
          txtFileName.Focus();
          return;
        }
      }
      for (int x = 0; x < FlowEngineCore.Global.IllegalFileNameStartCharacters.Length; x++)
      {
        if (txtFileName.Text.StartsWith(FlowEngineCore.Global.IllegalFileNameStartCharacters[x]) == true)
        {
          MessageBox.Show("Illegal characters at beginning of file name, file name can't start with '" + FlowEngineCore.Global.IllegalFileNameStartCharacters[x] + "'");
          txtFileName.Focus();
          return;
        }
      }
      if (FileType == FILE_TYPE.Flow)
      {
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

      SaveAsFileName = txtFileName.Text;
      if (tvDirectories.SelectedNode is not null && tvDirectories.SelectedNode.Text != "/") //Only append the path if something is selected and it isn't the root path /
      {
        SaveAsFileName = BuildPathFromTree(SaveAsFileName);
      }

      if (FileType == FILE_TYPE.Flow)
      {
        if (FileMode == FILE_MODE.Save)
        {
          FlowSave fs = new FlowSave(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey, SaveAsFileName, chkDeployLive.Checked, FlowWrapper!.XmlWriteMemory()); //Stupid code parser doesn't understand that FlowWrapper can't be null here, so the !
          cServer.SendAndResponse(fs.GetPacket(), Callback_FileSave);
        }
        if (FileMode == FILE_MODE.Open)
        {
          FlowOpen fo = new FlowOpen(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey, SaveAsFileName);
          cServer.SendAndResponse(fo.GetPacket(), Callback_FileOpen);
        }
      }
      else
      {
        Call_Back(FlowWrapper!, txtFileName.Text);
        this.Close();
      }
    }

    private string BuildPathFromTree(string fileName)
    {
      string path = "";
      fileName = Path.GetFileName(fileName);
      TreeNode currentNode = tvDirectories.SelectedNode;
      while (currentNode is not null)
      {
        path = currentNode.Text + path;
        currentNode = currentNode.Parent;
        if (currentNode.Text == "/")
          break;
      }
      return path + "/" + fileName;
    }

    private void Callback_FileSave(FlowEngineCore.Administration.EventArgsPacket e)
    {
      BaseResponse response = new BaseResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        FlowWrapper.FileName = SaveAsFileName;
        Call_Back(FlowWrapper);
        this.Close();
      }
    }

    private void Callback_FileOpen(FlowEngineCore.Administration.EventArgsPacket e)
    {
      FlowOpenResponse response = new FlowOpenResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        string flowXml = response.FlowXml;

        //if (FlowWrapper is not null)
        //{
        FlowWrapper = new cFlowWrapper(cFlowWrapper.INCLUDE_START_STEP.EXCLUDE);
        FlowWrapper.XmlRead(ref flowXml);
        FlowWrapper.FileName = response.FileName;

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
      else
      {
        MessageBox.Show("Server open returned a non success response, Trace window may have more information.");
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


    private void txtFileName_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnAction.PerformClick();
      }
    }
  }
}
