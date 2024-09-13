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
using static FlowEngineCore.Administration.Messages.FlowDebugAlways;

namespace FlowEngineDesigner
{
  public partial class frmMain : Form
  {
    public frmMain()
    {
      InitializeComponent();
    }

    private void toolboxToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmToolbox f = new frmToolbox();
      f.Owner = this;
      Global.Layout.ExecuteLayout(f, cLayoutForm.LAYOUT_FORM.Toolbox);
      f.Show();
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
      Global.FormMain = this;
      Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
      cPluginManagerWrapper.LoadPlugins(cOptions.GetFullPath(cOptions.PluginPath));
      cPluginManagerWrapper.CreateAllGrphics();
      Global.LoadStaticImages();
      cLayoutForm formlayout1 = new cLayoutForm();
      formlayout1.layoutForm = cLayoutForm.LAYOUT_FORM.Main;
      formlayout1.state = FormWindowState.Normal;
      formlayout1.size = new System.Numerics.Vector2(1757, 97);
      formlayout1.position = new System.Numerics.Vector2(22, 115);

      cLayoutForm formlayout2 = new cLayoutForm();
      formlayout2.layoutForm = cLayoutForm.LAYOUT_FORM.Toolbox;
      formlayout2.open = true;
      formlayout2.state = FormWindowState.Normal;
      formlayout2.size = new System.Numerics.Vector2(300, 800);
      formlayout2.position = new System.Numerics.Vector2(22, 212);

      cLayoutForm formlayout3 = new cLayoutForm();
      formlayout3.layoutForm = cLayoutForm.LAYOUT_FORM.Flow;
      formlayout3.open = true;
      formlayout3.state = FormWindowState.Normal;
      formlayout3.size = new System.Numerics.Vector2(1457, 800);
      formlayout3.position = new System.Numerics.Vector2(322, 212);

      cLayoutForm formlayout4 = new cLayoutForm();
      formlayout4.layoutForm = cLayoutForm.LAYOUT_FORM.Tracer;
      formlayout4.open = true;
      formlayout4.state = FormWindowState.Normal;
      formlayout4.size = new System.Numerics.Vector2(1457, 300);
      formlayout4.position = new System.Numerics.Vector2(322, 1020);

      Global.Layout = new cLayout()
      {
        FormLayouts = { formlayout1, formlayout2, formlayout3, formlayout4 }
      };

      Global.Layout.ExecuteLayout(this);


      //TEST AREA

      //cSqlParser parser = new cSqlParser();
      //parser.ParseSql("((1 = 1) (OR) (2 = 2))");
      //frmSqlEditor f1 = new frmSqlEditor();
      //f1.Show();

      //TEST AREA
      cServer.LoadProfiles();
      if (cServer.Info.AutoConnect == true)
      {
        frmServerConnect f = new frmServerConnect();
        f.Owner = this;
        f.Show(); //The form load event will auto login if AdministrationAutoConnect is true.
      }

    }

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmFlow f = new frmFlow();
      f.Owner = this;
      Global.Layout.ExecuteLayout(f, cLayoutForm.LAYOUT_FORM.Flow);
      f.Show();
    }

    private void layout1ToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void frmMain_ResizeEnd(object sender, EventArgs e)
    {
      int width = this.Left;
      int height = this.Top;
    }

    private void tracerToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmTracer f = new frmTracer();
      f.Owner = this;
      Global.Layout.ExecuteLayout(f, cLayoutForm.LAYOUT_FORM.Tracer);
      f.Show();
    }




    private void connectToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmServerConnect f = new frmServerConnect();
      f.Owner = this;
      f.Show();
    }

    private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      cServer.Disconnect();
    }

    private void usersToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      frmAdministrationUsers f = new frmAdministrationUsers();
      f.Owner = this;
      f.Show();
    }


    private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAdministrationUserChangePassword f = new frmAdministrationUserChangePassword();
      f.Owner = this;
      f.ShowDialog();
    }

    private void securityProfilesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAdministrationSecurityProfiles f = new frmAdministrationSecurityProfiles();
      f.Owner = this;
      f.Show();
    }

    private void serverToolStripMenuItem_Click(object sender, EventArgs e)
    {
    }

    private void serverToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
    {
      usersToolStripMenuItem1.Enabled = false;
      securityProfilesToolStripMenuItem.Enabled = false;

      SecurityProfile.SECURITY_ACCESS_LEVEL accessUsers = cServer.AccessLevelForUserLoggedIn(SecurityProfile.SECURITY_AREA.Users);
      SecurityProfile.SECURITY_ACCESS_LEVEL accessSecurityProfiles = cServer.AccessLevelForUserLoggedIn(SecurityProfile.SECURITY_AREA.SecurityProfiles);

      if (accessSecurityProfiles >= SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
        securityProfilesToolStripMenuItem.Enabled = true;
      if (accessUsers >= SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
        usersToolStripMenuItem1.Enabled = true;
    }

    private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
    {
      cServer.Disconnect();
    }

    private void tsLoggedInAs_DoubleClick(object sender, EventArgs e)
    {
      frmAdministrationUserProperties f = new frmAdministrationUserProperties(FORM_MODE.ReadOnly, cServer.UserLoggedIn);
      f.Owner = this;
      f.Show();
    }

    private void flowOpenToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAdministrationFile f = new frmAdministrationFile(FILE_MODE.Open, new cFlowWrapper(), FlowWrapperChanged_Callback);
      f.Owner = this;
      f.Show();
    }

    private void FlowWrapperChanged_Callback(cFlowWrapper flowWrapper, string filename)
    {
      frmFlow f = new frmFlow(flowWrapper);
      Global.Layout.ExecuteLayout(f, cLayoutForm.LAYOUT_FORM.Flow);
      flowWrapper.PopulateSampleVariablesFromPlugin();
      flowWrapper.Center(f.Camera, f.pictureBox1);
      f.Owner = this;
      f.TitleText();
      f.pictureBox1.Refresh();
      f.Show();
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      openFileDialog1.Filter = "Flow file (*.flow)|*.flow|All files (*.*)|*.*";
      openFileDialog1.Multiselect = true;
      openFileDialog1.FileName = "";
      openFileDialog1.DefaultExt = "flow";
      DialogResult dr = openFileDialog1.ShowDialog();
      if (dr == DialogResult.OK)
      {
        cFlowWrapper flow = new cFlowWrapper(cFlowWrapper.INCLUDE_START_STEP.EXCLUDE);
        flow.XmlReadFile(openFileDialog1.FileName);
        frmFlow f = new frmFlow(flow);
        f.Owner = this;
        Global.Layout.ExecuteLayout(f, cLayoutForm.LAYOUT_FORM.Flow);
        f.Show();
      }
    }

    /// <summary>
    /// I use this to activate the form if it isn't the active window. If this isn't there it will take 2 clicks to activate a menu or toolstrip button, makes it rather annoying.
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
      const int WM_PARENTNOTIFY = 0x0210;
      if (this.Focused == false && m.Msg == WM_PARENTNOTIFY)
      {
        // Make this form auto-grab the focus when menu/controls are clicked
        this.Activate();
      }
      base.WndProc(ref m);
    }

    private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void debugAlwaysToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
    {
    }


    public void Callback_FlowDebugAlways(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() == BaseResponse.RESPONSE_CODE.Success)
      {
        FlowDebugAlwaysResponse response = new FlowDebugAlwaysResponse(e.Packet);
        if (response.DebugAlways == DEBUG_ALWAYS.Yes)
          cServer.Info.DebugAlways = true;
        else
          cServer.Info.DebugAlways = false;

        debugAlwaysToolStripMenuItem.Checked = cServer.Info.DebugAlways;
      }
    }

    private void debugAlwaysToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FlowDebugAlways.DEBUG_ALWAYS debugAlways = FlowDebugAlways.DEBUG_ALWAYS.Yes;

      if (cServer.Info.DebugAlways == true) //If it is already Yes/true, we want to toggle it to false/No
        debugAlways = FlowDebugAlways.DEBUG_ALWAYS.No;

      FlowDebugAlways flowDebugAlways = new FlowDebugAlways(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey, debugAlways);
      cServer.SendAndResponse(flowDebugAlways.GetPacket(), Callback_FlowDebugAlways);
    }

    private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      //Server settings are retrieved from the server after a user connects to the server, in cServer.GetServerSettings()

      frmSettings f = new frmSettings(Options.GetSettings, "Server settings", frmSettings.SOURCE.RemoteServer);
      f.Owner = this;
      f.Show();
    }

    private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Form? found = Global.FindOpenFormByTitleText("Statistics");
      if (found is not null)
      {
        found.BringToFront();
      }
      else
      {
        frmStatistics f = new frmStatistics();
        f.Owner = this;
        f.Show();
      }
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAbout f = new frmAbout();
      f.Owner = this;
      f.Show();
    }

    private void frmMain_Shown(object sender, EventArgs e)
    {
    }

    private void frmMain_Enter(object sender, EventArgs e)
    {

    }

    //private bool inActivated = false;
    private void frmMain_Activated(object sender, EventArgs e)
    {

      //if (inActivated == true)
      //  return;
      //inActivated = true;
      //cEventManager.RaiseEventTracer(cEventManager.SENDER.Compiler, "Main Activated");
      //Form? f = Global.FindOpenFormByName("frmTracer");
      //if (f is not null)
      //  f.BringToFront();
      //f = Global.FindOpenFormByName("frmToolbox");
      //if (f is not null)
      //  f.BringToFront();
      //f = Global.FindOpenFormByName("frmFlow");
      //if (f is not null)
      //  f.BringToFront();
      ////this.BringToFront();
      //inActivated = false;
    }

    private void connectRecentToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }
  }
}
