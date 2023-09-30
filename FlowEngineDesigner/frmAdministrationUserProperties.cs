using Core.Administration;
using Core.Administration.Messages;
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
  public partial class frmAdministrationUserProperties : Form
  {
    private FORM_MODE Mode;
    private Core.User User;
    private System.Timers.Timer Tim = new System.Timers.Timer();
    private string PreviousLoginId = "";
    public frmAdministrationUserProperties(FORM_MODE mode, Core.User? user)
    {
      InitializeComponent();
      Mode = mode;
      if (Mode == FORM_MODE.Add || user is null)
      {
        User = new Core.User();
      }
      else
      {
        User = user;
      }

    }

    private void frmAdministrationUserProperties_Load(object sender, EventArgs e)
    {
      cmbSecurityProfile.Items.Add("Admin");
      cmbSecurityProfile.Items.Add("Everything except Admin");
      cmbSecurityProfile.Items.Add("Readonly");

      lblDuplicate.Visible = false;
      if (Mode == FORM_MODE.Delete || Mode == FORM_MODE.ReadOnly)
      {
        txtLoginId.ReadOnly = true;
        txtPassword.ReadOnly = true;
        txtNameFirst.ReadOnly = true;
        txtNameSur.ReadOnly = true;
        cmbSecurityProfile.Enabled = false;
      }
      if (Mode == FORM_MODE.Edit || Mode == FORM_MODE.ReadOnly || Mode == FORM_MODE.Delete && User is not null)
      {
        PreviousLoginId = User.LoginId;
        txtLoginId.Text = User.LoginId;
        txtPassword.Text = "";
        txtNameFirst.Text = User.NameFirst;
        txtNameSur.Text = User.NameSur;
        Global.ComboBoxSetIndex(cmbSecurityProfile, User.SecurityProfile);
      }

      Tim.Interval = 1000;
      Tim.Elapsed += Tim_Elapsed;

      if (Mode == FORM_MODE.Delete)
      {
        btnAction.Text = "Delete";
      }
      else if (Mode == FORM_MODE.ReadOnly)
      {
        btnAction.Text = "Ok";
      }
      else if (Mode == FORM_MODE.Add)
      {
        btnAction.Text = "Add";
        Tim.Start();
      }
      else if (Mode == FORM_MODE.Edit)
      {
        txtPassword.PlaceholderText = "Leave blank to not change password";
        btnAction.Text = "Edit";
        Tim.Start();
      }


    }


    private void txtLoginId_TextChanged(object sender, EventArgs e)
    {
      Tim.Stop();
      Tim.Start();
      lblDuplicate.Visible = false;

    }
    private void Tim_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
      if (txtLoginId.Text.ToLower() != PreviousLoginId.ToLower() && txtLoginId.Text.ToLower() != User.LoginId.ToLower() && cServer.UserLoggedIn is not null)
      {
        PreviousLoginId = txtLoginId.Text;
        Core.Administration.Messages.UserLoginIdCheck u = new Core.Administration.Messages.UserLoginIdCheck(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, txtLoginId.Text);
        cServer.SendAndResponse(u.GetPacket(), Callback_UserLoginIdCheck, Packet.PACKET_TYPE.UserLoginIdCheckResponse);
      }
    }

    private void btnAction_Click(object sender, EventArgs e)
    {
      string SessionKey = "";
      if (cServer.UserLoggedIn is not null)
      {
        SessionKey = cServer.UserLoggedIn.SessionKey;
      }

      if (Mode == FORM_MODE.Add)
      {
        Core.Administration.Messages.UserAdd u = new Core.Administration.Messages.UserAdd(cOptions.AdministrationPrivateKey, SessionKey, txtLoginId.Text, txtPassword.Text, txtNameFirst.Text, txtNameSur.Text, cmbSecurityProfile.Text);
        cServer.SendAndResponse(u.GetPacket(), Callback_User, Packet.PACKET_TYPE.UserAddResponse);
      }
      if (Mode == FORM_MODE.Edit)
      {
        Core.Administration.Messages.UserEdit u = new Core.Administration.Messages.UserEdit(cOptions.AdministrationPrivateKey, SessionKey, User.LoginId, txtLoginId.Text, txtPassword.Text, txtNameFirst.Text, txtNameSur.Text, cmbSecurityProfile.Text);
        cServer.SendAndResponse(u.GetPacket(), Callback_User, Packet.PACKET_TYPE.UserEditResponse);
      }
      if (Mode == FORM_MODE.Delete)
      {
        Core.Administration.Messages.UserDelete u = new Core.Administration.Messages.UserDelete(cOptions.AdministrationPrivateKey, SessionKey, User.LoginId);
        cServer.SendAndResponse(u.GetPacket(), Callback_User, Packet.PACKET_TYPE.UserDeleteResponse);
      }
    }

    private void Callback_User(Core.Administration.EventArgsPacket e)
    {
      BaseResponse response = new BaseResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        this.Close();
      }
    }
   

    private void Callback_UserLoginIdCheck(Core.Administration.EventArgsPacket e)
    {
      string LabelText = "";
      Color LabelColor = Color.Black;
      UserLoginIdCheckResponse response = new UserLoginIdCheckResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        LabelText = "Valid LoginId";
        LabelColor = Color.Green;
      }
      else if (response.ResponseCode == BaseResponse.RESPONSE_CODE.LoginIdDuplicate)
      {
        LabelText = String.Format("Duplicate use [{0}]", response.LoginIdSuggestion);
        LabelColor = Color.Red;
      }

      //this.lblDuplicate.Invoke((MethodInvoker)delegate
      //{
      lblDuplicate.Visible = true;
      lblDuplicate.Text = LabelText;
      lblDuplicate.ForeColor = LabelColor;
      lblDuplicate.Tag = response.LoginIdSuggestion;
      lblDuplicate.Cursor = Cursors.Hand;
      //});
    }

    private void label3_Click(object sender, EventArgs e)
    {

    }

    private void txtPassword_TextChanged(object sender, EventArgs e)
    {
    }

    private void label2_Click(object sender, EventArgs e)
    {
    }

    private void txtLoginId_Enter(object sender, EventArgs e)
    {
      Tim.Start();
    }

    private void lblDuplicate_Click(object sender, EventArgs e)
    {
      if (lblDuplicate.Tag is not null)
        txtLoginId.Text = lblDuplicate.Tag.ToString();
      lblDuplicate.Visible = false;
    }
  }
}
