using Core.Administration.Messages;
using Core.Administration;
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
        cServer.SendAndResponse(userLogin.GetPacket(), Callback_UserLogin, Packet.PACKET_TYPE.UserLoginResponse);
      }
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

    private void frmServerConnect_Load(object sender, EventArgs e)
    {
      if (cOptions.AdministrationAutoConnect == true)
      {
        btnConnect_Click(sender, e);
      }
    }
  }
}
