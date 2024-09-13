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
using static FlowEngineCore.Administration.Messages.FlowDebugAlways;

namespace FlowEngineDesigner
{
  public partial class frmServerConnect : Form
  {
    private cServerInformation? selectedServer = null;
    private static bool AutoConnectedOnce = false;
    public frmServerConnect()
    {
      InitializeComponent();
    }

    private void btnConnect_Click(object sender, EventArgs e)
    {
      bool needToSave = false;
      string Error = "";
      if (cmbProfile.Text == "")
        Error += "Profile name required!" + Environment.NewLine;
      if (txtUrl.Text == "")
        Error += "Url / IP required!" + Environment.NewLine;
      if (txtLoginId.Text == "")
        Error += "Login Id required!" + Environment.NewLine;
      if (txtPassword.Text == "")
        Error += "Password required!" + Environment.NewLine;
      if (txtServerPrivateKey.Text == "")
        Error += "Server private key required!" + Environment.NewLine;

      if (Error != "")
      {
        MessageBox.Show(Error, "Missing values!");
        return;
      }

      if (selectedServer is null)
      {
        selectedServer = new cServerInformation();
        cServer.Servers.Add(selectedServer);
        needToSave = true;
      }
      else
      {
        if (selectedServer.Profile != cmbProfile.Text)
          needToSave = true;
        if (selectedServer.Url != txtUrl.Text)
          needToSave = true;
        if (selectedServer.Port != (int)nudPort.Value)
          needToSave = true;
        if (selectedServer.LoginId != txtLoginId.Text)
          needToSave = true;
        if (selectedServer.Password != txtPassword.Text)
          needToSave = true;
        if (selectedServer.RememberLoginId != chkRememberMe.Checked)
          needToSave = true;
        if (selectedServer.RememberPassword != chkRememberPassword.Checked)
          needToSave = true;
        if (selectedServer.PrivateKey != txtServerPrivateKey.Text)
          needToSave = true;
        if (selectedServer.AutoConnect != chkAutoConnect.Checked)
          needToSave = true;
        if (selectedServer.DebugAlways != chkDebugAlways.Checked)
          needToSave = true;
      }

      //Update all the values for the selected
      selectedServer.Profile = cmbProfile.Text;
      selectedServer.Url = txtUrl.Text;
      selectedServer.Port = (int)nudPort.Value;
      selectedServer.LoginId = txtLoginId.Text;
      selectedServer.Password = txtPassword.Text;
      selectedServer.RememberLoginId = chkRememberMe.Checked;
      selectedServer.RememberPassword = chkRememberPassword.Checked;
      selectedServer.PrivateKey = txtServerPrivateKey.Text;
      selectedServer.AutoConnect = chkAutoConnect.Checked;
      selectedServer.DebugAlways = chkDebugAlways.Checked;

      bool resp = cServer.Connect(selectedServer);
      if (resp == true)
      {
        UserLogin userLogin = new UserLogin(cServer.Info.PrivateKey, "", txtLoginId.Text, txtPassword.Text);
        cServer.SendAndResponse(userLogin.GetPacket(), Callback_UserLogin);
      }

      if (needToSave == true)
        cServer.SaveProfiles();
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
      Global.FormMain!.tsLoggedInAs.Text = $"as {loginResponse.LoginId}";
      this.Close();
      if (cServer.UserLoggedIn.NeedToChangePassword == true)
      {
        frmAdministrationUserChangePassword f = new frmAdministrationUserChangePassword();
        f.ShowDialog();
      }
      cServer.GetServerSettings();
      cServer.RefreshSecurityProfiles();

      if (cServer.Info.DebugAlways == true)
      {
        FlowDebugAlways flowDebugAlways = new FlowDebugAlways(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey, FlowDebugAlways.DEBUG_ALWAYS.Yes);
        cServer.SendAndResponse(flowDebugAlways.GetPacket(), Global.FormMain.Callback_FlowDebugAlways);
      }
    }

    private void frmServerConnect_Load(object sender, EventArgs e)
    {
      int selectedIndex = -1;
      for (int x = 0; x < cServer.Servers.Count; x++)
      {
        cmbProfile.Items.Add(cServer.Servers[x]);
        if (cServer.Servers[x].WasLastConnected == true)
        {
          selectedIndex = x;
        }
      }
      cmbProfile.Items.Add("< Add new >");
      if (selectedIndex >= 0)
        cmbProfile.SelectedIndex = selectedIndex;
      else
        AutoConnectedOnce = true;

      if (cServer.Info.AutoConnect == true && AutoConnectedOnce == false)
      {
        btnConnect_Click(sender, e);
        AutoConnectedOnce = true;
      }
    }

    private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowPassword.Checked == true)
        txtPassword.PasswordChar = '\0';
      else
        txtPassword.PasswordChar = '*';
    }

    private void cmbProfile_SelectedIndexChanged(object sender, EventArgs e)
    {
      cServerInformation? server = cmbProfile.SelectedItem as cServerInformation;
      if (server is not null)
      {
        selectedServer = server;
        txtUrl.Text = selectedServer.Url;
        txtLoginId.Text = selectedServer.LoginId;
        txtPassword.Text = selectedServer.Password;
        chkRememberMe.Checked = selectedServer.RememberLoginId;
        chkRememberPassword.Checked = selectedServer.RememberPassword;
        chkAutoConnect.Checked = selectedServer.AutoConnect;
        chkDebugAlways.Checked = selectedServer.DebugAlways;
        txtServerPrivateKey.Text = selectedServer.PrivateKey;
        nudPort.Value = selectedServer.Port;
      }
      else if (cmbProfile.SelectedIndex == cServer.Servers.Count)
      {
        selectedServer = null;
        AutoConnectedOnce = true;
      }

    }
  }
}
