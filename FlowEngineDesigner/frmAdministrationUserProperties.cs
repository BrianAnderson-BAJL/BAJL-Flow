using FlowEngineCore.Administration;
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
  public partial class frmAdministrationUserProperties : Form
  {
    private FORM_MODE Mode;
    private FlowEngineCore.User User;
    private System.Timers.Timer Tim = new System.Timers.Timer();
    private string PreviousLoginId = "";
    public frmAdministrationUserProperties(FORM_MODE mode, FlowEngineCore.User? user)
    {
      InitializeComponent();
      Mode = mode;
      if (Mode == FORM_MODE.Add || user is null)
      {
        User = new FlowEngineCore.User();
      }
      else
      {
        User = user;
      }

    }

    private void frmAdministrationUserProperties_Load(object sender, EventArgs e)
    {
      if (cServer.SecurityProfiles is not null)
      {
        for (int x = 0; x < cServer.SecurityProfiles.Count; x++)
        {
          cmbSecurityProfile.Items.Add(cServer.SecurityProfiles[x].Name);
        }
      }
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
        this.Text = User.LoginId;
        PreviousLoginId = User.LoginId;
        txtLoginId.Text = User.LoginId;
        txtPassword.Text = "";
        txtNameFirst.Text = User.NameFirst;
        txtNameSur.Text = User.NameSur;
        Global.ComboBoxSetIndex(cmbSecurityProfile, User.SecurityProfile.Name);
      }

      Tim.Interval = 1000;
      Tim.Elapsed += Tim_Elapsed;

      if (Mode == FORM_MODE.Delete)
      {
        btnAction.Text = "Delete";
        this.Text = $"Delete User [{txtLoginId.Text}]";
      }
      else if (Mode == FORM_MODE.ReadOnly)
      {
        btnAction.Text = "Ok";
        this.Text = $"View User [{txtLoginId.Text}]";
      }
      else if (Mode == FORM_MODE.Add)
      {
        btnAction.Text = "Add";
        this.Text = $"Add User";
        Tim.Start();
      }
      else if (Mode == FORM_MODE.Edit)
      {
        txtPassword.PlaceholderText = "Leave blank to not change password";
        btnAction.Text = "Edit";
        this.Text = $"Edit User [{txtLoginId.Text}]";
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
        FlowEngineCore.Administration.Messages.UserLoginIdCheck u = new FlowEngineCore.Administration.Messages.UserLoginIdCheck(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, txtLoginId.Text);
        cServer.SendAndResponse(u.GetPacket(), Callback_UserLoginIdCheck);
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
        FlowEngineCore.Administration.Messages.UserAdd u = new FlowEngineCore.Administration.Messages.UserAdd(cOptions.AdministrationPrivateKey, SessionKey, txtLoginId.Text, txtPassword.Text, txtNameFirst.Text, txtNameSur.Text, cmbSecurityProfile.Text);
        cServer.SendAndResponse(u.GetPacket(), Callback_User);
      }
      if (Mode == FORM_MODE.Edit)
      {
        FlowEngineCore.Administration.Messages.UserEdit u = new FlowEngineCore.Administration.Messages.UserEdit(cOptions.AdministrationPrivateKey, SessionKey, User.LoginId, txtLoginId.Text, txtPassword.Text, txtNameFirst.Text, txtNameSur.Text, cmbSecurityProfile.Text);
        cServer.SendAndResponse(u.GetPacket(), Callback_User);
      }
      if (Mode == FORM_MODE.Delete)
      {
        FlowEngineCore.Administration.Messages.UserDelete u = new FlowEngineCore.Administration.Messages.UserDelete(cOptions.AdministrationPrivateKey, SessionKey, User.LoginId);
        cServer.SendAndResponse(u.GetPacket(), Callback_User);
      }
      if (Mode == FORM_MODE.ReadOnly)
        this.Close();
    }

    private void Callback_User(FlowEngineCore.Administration.EventArgsPacket e)
    {
      BaseResponse response = new BaseResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        this.Close();
      }
    }


    private void Callback_UserLoginIdCheck(FlowEngineCore.Administration.EventArgsPacket e)
    {
      BaseResponse.RESPONSE_CODE rc = e.Packet.PeekResponseCode();
      if (rc != BaseResponse.RESPONSE_CODE.Success && rc != BaseResponse.RESPONSE_CODE.Duplicate)
        return;

      string LabelText = "";
      Color LabelColor = Color.Black;
      UserLoginIdCheckResponse response = new UserLoginIdCheckResponse(e.Packet);
      if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
      {
        LabelText = "Valid LoginId";
        LabelColor = Color.Green;
      }
      else if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Duplicate)
      {
        LabelText = $"Duplicate use [{response.LoginIdSuggestion}]";
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

    private void frmAdministrationUserProperties_FormClosing(object sender, FormClosingEventArgs e)
    {
      Tim.Stop();
    }
  }
}
