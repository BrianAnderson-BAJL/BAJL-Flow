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
      f.Show();
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
      Global.FormMain = this;
      Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
      cPluginManager.LoadPlugins(cOptions.GetFullPath(cOptions.PluginPath));
      cPluginManager.CreateAllGrphics();
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

      cLayout Layout = new cLayout()
      {
        FormLayouts = { formlayout1, formlayout2, formlayout3, formlayout4 }
      };

      Layout.ExecuteLayout();

      setFocusOnMouseEnterToolStripMenuItem.Checked = cOptions.FocusOnMouseEnter;
      highlightStepsOnExecutionToolStripMenuItem.Checked = cOptions.HighlightStepsOnExecution;
      //string json = "{\"gameBoardId\":77,\"score\":0,\"powerUpTimeAddUsed\":0,\"powerUpHintUsed\":0,\"words\":[]}";
      //string json = "{\r\n  \"gameBoardId\": 352,\r\n  \"score\": 250,\r\n  \"powerUpTimeAddUsed\": 0,\r\n  \"powerUpHintUsed\": 1,\r\n  \"words\": [\r\n    {\r\n      \"word\": \"DATE\",\r\n      \"time\": 9.2\r\n    },\r\n    {\r\n      \"word\": \"MATE\",\r\n      \"time\": 15.44\r\n    },\r\n    {\r\n      \"word\": \"RITE\",\r\n      \"time\": 19.62\r\n    },\r\n    {\r\n      \"word\": \"DAM\",\r\n      \"time\": 30.29\r\n    },\r\n    {\r\n      \"word\": \"ELATE\",\r\n      \"time\": 33.14\r\n    },\r\n    {\r\n      \"word\": \"HAD\",\r\n      \"time\": 40.71\r\n    },\r\n    {\r\n      \"word\": \"HEAL\",\r\n      \"time\": 45.4\r\n    },\r\n    {\r\n      \"word\": \"TEAL\",\r\n      \"time\": 48.22\r\n    },\r\n    {\r\n      \"word\": \"REAL\",\r\n      \"time\": 50.93\r\n    },\r\n    {\r\n      \"word\": \"DEAL\",\r\n      \"time\": 57.53\r\n    },\r\n    {\r\n      \"word\": \"HID\",\r\n      \"time\": 63.54\r\n    },\r\n    {\r\n      \"word\": \"MAD\",\r\n      \"time\": 65.29\r\n    },\r\n    {\r\n      \"word\": \"DAD\",\r\n      \"time\": 66.51\r\n    },\r\n    {\r\n      \"word\": \"GLIM\",\r\n      \"time\": 76.5\r\n    },\r\n    {\r\n      \"word\": \"MIL\",\r\n      \"time\": 79.96\r\n    },\r\n    {\r\n      \"word\": \"RIM\",\r\n      \"time\": 85.4\r\n    },\r\n    {\r\n      \"word\": \"LAM\",\r\n      \"time\": 101.85\r\n    },\r\n    {\r\n      \"word\": \"HAM\",\r\n      \"time\": 116.34\r\n    },\r\n    {\r\n      \"word\": \"HELL\",\r\n      \"time\": 119.0\r\n    }\r\n  ]\r\n}";
      //string json = "{\r\n\"employee\":{\"name\":\"John\", \"age\":30, \"alive\":true, \"dead\":false, \"salary\":55.26, \"city\":\"New York\"},\"employees\":[\"John\", \"Anna\", \"Peter\"],\"employees\":[500, 1000, 100.99]\r\n}";
      //Variable? v = Variable.JsonParse(ref json);

      if (cOptions.AdministrationAutoConnect == true)
      {
        frmServerConnect f = new frmServerConnect();
        f.Show(); //The form load event will auto login if AdministrationAutoConnect is true.
      }
    }

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmFlow f = new frmFlow();
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
      f.Show();
    }

    private void frmMain_MouseEnter(object sender, EventArgs e)
    {
      if (cOptions.FocusOnMouseEnter == true)
      {
        this.Focus();
      }
    }

    private void setFocusOnMouseEnterToolStripMenuItem_Click(object sender, EventArgs e)
    {
      cOptions.FocusOnMouseEnter = !cOptions.FocusOnMouseEnter;
      setFocusOnMouseEnterToolStripMenuItem.Checked = cOptions.FocusOnMouseEnter;
    }

    private void highlightStepsOnExecutionToolStripMenuItem_Click(object sender, EventArgs e)
    {
      cOptions.HighlightStepsOnExecution = !cOptions.HighlightStepsOnExecution;
      highlightStepsOnExecutionToolStripMenuItem.Checked = cOptions.HighlightStepsOnExecution;
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

    private void loginToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmAdministrationUserLogin f = new frmAdministrationUserLogin();
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
      frmAdministrationFile f = new frmAdministrationFile(FILE_MODE.Open);
      f.Show();
    }

  }
}
