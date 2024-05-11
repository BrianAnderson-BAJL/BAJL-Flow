using FlowEngineCore.Administration.Messages;
using FlowEngineCore.Administration;
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
  public partial class frmServerConnect : Form
  {
    private static bool AutoConnectedOnce = false;
    public frmServerConnect()
    {
      InitializeComponent();
    }

    private void btnConnect_Click(object sender, EventArgs e)
    {
      bool resp = cServer.Connect(txtDomainName.Text, (int)nudPort.Value);
      if (resp == true)
      {
        UserLogin userLogin = new UserLogin(cOptions.AdministrationPrivateKey, "", txtLoginId.Text, txtPassword.Text);
        cServer.SendAndResponse(userLogin.GetPacket(), Callback_UserLogin);
      }
    }

    private void Callback_UserLogin(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() != BaseResponse.RESPONSE_CODE.Success)
        return;

      UserLoginResponse loginResponse = new UserLoginResponse(e.Packet);
      cServer.UserLoggedIn = new FlowEngineCore.User();
      cServer.UserLoggedIn.LoginId = loginResponse.LoginId;
      cServer.UserLoggedIn.SecurityProfileNameTemp = loginResponse.SecurityProfile;
      cServer.UserLoggedIn.NameFirst = loginResponse.NameFirst;
      cServer.UserLoggedIn.NameSur = loginResponse.NameSur;
      cServer.UserLoggedIn.SessionKey = loginResponse.SessionKey;
      cServer.UserLoggedIn.NeedToChangePassword = loginResponse.NeedToChangePassword;
      Global.FormMain!.tsLoggedInAs.Text = loginResponse.LoginId;
      this.Close();
      if (cServer.UserLoggedIn.NeedToChangePassword == true)
      {
        frmAdministrationUserChangePassword f = new frmAdministrationUserChangePassword();
        f.ShowDialog();
      }

      cServer.RefreshSecurityProfiles();

      Global.FormMain.debugAlwaysToolStripMenuItem.Checked = cOptions.AdministrationDebugAlways;
    }

    private void frmServerConnect_Load(object sender, EventArgs e)
    {
      if (cOptions.AdministrationAutoConnect == true && AutoConnectedOnce == false)
      {
        btnConnect_Click(sender, e);
        AutoConnectedOnce = true;
      }
    }

    private void chkRememberMe_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void label3_Click(object sender, EventArgs e)
    {
    }

    private void txtPassword_TextChanged(object sender, EventArgs e)
    {
    }

    private void label4_Click(object sender, EventArgs e)
    {
    }

    private void txtLoginId_TextChanged(object sender, EventArgs e)
    {
    }

    private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
    {
    }
  }
}
