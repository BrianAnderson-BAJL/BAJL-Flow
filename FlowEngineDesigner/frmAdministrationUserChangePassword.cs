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

namespace FlowEngineDesigner
{
  public partial class frmAdministrationUserChangePassword : Form
  {
    public frmAdministrationUserChangePassword()
    {
      InitializeComponent();
    }

    private void btnChangePassword_Click(object sender, EventArgs e)
    {
      if (txtNewPassword.Text == txtOldPassword.Text)
      {
        MessageBox.Show("New password can not be the same as the old password!");
        return;
      }
      if (cServer.UserLoggedIn is null)
      {
        MessageBox.Show("No user signed in!");
        return;
      }
      FlowEngineCore.Administration.Messages.UserChangePassword changePassword = new FlowEngineCore.Administration.Messages.UserChangePassword(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, cServer.UserLoggedIn.LoginId, txtOldPassword.Text, txtNewPassword.Text);
      cServer.SendAndResponse(changePassword.GetPacket(), Callback_UserChangePassword);
    }

    private void Callback_UserChangePassword(FlowEngineCore.Administration.EventArgsPacket e)
    {
      BaseResponse response = new BaseResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        this.Close();
      }
    }

    private void frmAdministrationUserChangePassword_Load(object sender, EventArgs e)
    {

    }

    private void frmAdministrationUserChangePassword_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (cServer.UserLoggedIn is null)
      {
        cServer.Disconnect();
        return;
      }

      if (cServer.UserLoggedIn.NeedToChangePassword == true)
      {
        e.Cancel = true;
        if (MessageBox.Show("You have to change your password before you can use the Administration area.\n\nDo you want to continue to change your password?", "Change Password?", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
        {
          cServer.Disconnect();
          e.Cancel = false;
        }
      }
    }

    private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowPassword.Checked == true)
      {
        txtNewPassword.PasswordChar = '\0';
        txtOldPassword.PasswordChar = '\0';
      }
      else
      {
        txtNewPassword.PasswordChar = '*';
        txtOldPassword.PasswordChar = '*';
      }
    }
  }
}
