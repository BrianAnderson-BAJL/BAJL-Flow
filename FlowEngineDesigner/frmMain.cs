using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
      formlayout2.size = new System.Numerics.Vector2(300, 600);
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

      Global.Layout.ExecuteLayout();


      //TEST AREA

      //cSqlParser parser = new cSqlParser();
      //parser.ParseSql("((1 = 1) (OR) (2 = 2))");
      //frmSqlEditor f1 = new frmSqlEditor();
      //f1.Show();

      //TEST AREA

      if (cOptions.AdministrationAutoConnect == true)
      {
        frmServerConnect f = new frmServerConnect();
        f.Show(); //The form load event will auto login if AdministrationAutoConnect is true.
      }
    }

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmFlow f = new frmFlow();
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
      Global.Layout.ExecuteLayout(f, cLayoutForm.LAYOUT_FORM.Tracer);
      f.Show();
    }




    private void connectToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmServerConnect f = new frmServerConnect();
      f.Show();
    }

    private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      cServer.Disconnect();
    }

    private void usersToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      frmAdministrationUsers f = new frmAdministrationUsers();
      f.Show();
    }


    private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAdministrationUserChangePassword f = new frmAdministrationUserChangePassword();
      f.ShowDialog();
    }

    private void securityProfilesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAdministrationSecurityProfiles f = new frmAdministrationSecurityProfiles();
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
      f.Show();
    }

    private void flowOpenToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAdministrationFile f = new frmAdministrationFile(FILE_MODE.Open, new cFlowWrapper(), FlowWrapperChanged_Callback);
      f.Show();
    }

    private void FlowWrapperChanged_Callback(cFlowWrapper flowWrapper)
    {
      frmFlow f = new frmFlow(flowWrapper);
      f.Show();
      flowWrapper.PopulateSampleVariablesFromPlugin();
      f.pictureBox1.Refresh();
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
        cFlowWrapper flow = new cFlowWrapper();
        flow.XmlReadFile(openFileDialog1.FileName);
        frmFlow f = new frmFlow(flow);
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
  }
}
