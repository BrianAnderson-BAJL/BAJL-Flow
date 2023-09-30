using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Administration;
using Core.Administration.Messages;

namespace FlowEngineDesigner
{
  public partial class frmAdministrationUserLogin : Form
  {
    public frmAdministrationUserLogin()
    {
      InitializeComponent();
    }

    private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowPassword.Checked)
      {
        txtPassword.PasswordChar = '*';
      }
      else
      {
        txtPassword.PasswordChar = '\0';
      }
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
      UserLogin userLogin = new UserLogin(cOptions.AdministrationPrivateKey, "", txtLoginId.Text, txtPassword.Text);
      cServer.SendAndResponse(userLogin.GetPacket(), Callback_UserLogin, Packet.PACKET_TYPE.UserLoginResponse);
    }

    private void Callback_UserLogin(Core.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() != BaseResponse.RESPONSE_CODE.Success)
        return;

      UserLoginResponse loginResponse = new UserLoginResponse(e.Packet);
      cServer.UserLoggedIn = new Core.User();
      cServer.UserLoggedIn.LoginId = loginResponse.LoginId;
      cServer.UserLoggedIn.SecurityProfile = loginResponse.SecurityProfile;
      cServer.UserLoggedIn.NameFirst = loginResponse.NameFirst;
      cServer.UserLoggedIn.NameSur = loginResponse.NameSur;
      cServer.UserLoggedIn.SessionKey = loginResponse.SessionKey;

      this.Close();
    }

  }
}
