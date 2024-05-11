using FlowEngineCore;
using FlowEngineCore.Administration.Messages;
using FlowEngineCore.Administration.Packets;
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
  public partial class frmAdministrationUsers : Form
  {
    private SecurityProfile.SECURITY_ACCESS_LEVEL AccessUsers = SecurityProfile.SECURITY_ACCESS_LEVEL.None;
    public frmAdministrationUsers()
    {
      InitializeComponent();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      frmAdministrationUserProperties f = new frmAdministrationUserProperties(FORM_MODE.Add, null);
      f.Show();
    }

    private void frmAdministrationUsers_Load(object sender, EventArgs e)
    {
      AccessUsers = cServer.AccessLevelForUserLoggedIn(SecurityProfile.SECURITY_AREA.Users);
      if (AccessUsers < SecurityProfile.SECURITY_ACCESS_LEVEL.Full)
      {
        btnAdd.Enabled = false;
        btnDelete.Enabled = false;
      }
      if (AccessUsers == SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
      {
        btnEditView.Text = "View";
      }
      if (AccessUsers == SecurityProfile.SECURITY_ACCESS_LEVEL.None)  //How did you get here
      {
        btnEditView.Enabled = false;
      }
    }

    private void Callback_UsersGet(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() == BaseResponse.RESPONSE_CODE.Success)
      {
        UsersGetResponse response = new UsersGetResponse(e.Packet);
        if (response.ResponseCode == BaseResponse.RESPONSE_CODE.Success)
        {
          lstUsers.Items.Clear();
          for (int x = 0; x < response.Users.Count; x++)
          {
            ListViewItem lvi = lstUsers.Items.Add(response.Users[x].LoginId);
            response.Users[x].SecurityProfile = cServer.FindSecurityProfileByName(response.Users[x].SecurityProfileNameTemp);
            lvi.SubItems.Add(response.Users[x].NameFirst);
            lvi.SubItems.Add(response.Users[x].NameSur);
            lvi.SubItems.Add(response.Users[x].SecurityProfile.Name);
            lvi.Tag = response.Users[x];
          }
        }
      }
    }

    private void frmAdministrationUsers_Activated(object sender, EventArgs e)
    {
      if (cServer.UserLoggedIn is not null)
      {
        UsersGet usersGet = new UsersGet(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey);
        cServer.SendAndResponse(usersGet.GetPacket(), Callback_UsersGet);
      }
    }

    private void btnEditView_Click(object sender, EventArgs e)
    {
      EditSelectedUser();
    }

    private void EditSelectedUser()
    {
      if (lstUsers.SelectedItems.Count == 0)
        return;
      FlowEngineCore.User? user = lstUsers.SelectedItems[0].Tag as FlowEngineCore.User;
      if (user is not null)
      {
        FORM_MODE mode = FORM_MODE.Edit;
        if (AccessUsers == SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
          mode = FORM_MODE.ReadOnly;
        frmAdministrationUserProperties f = new frmAdministrationUserProperties(mode, user);
        f.Show();
      }
    }


    private void lstUsers_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      EditSelectedUser();
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (lstUsers.SelectedItems.Count == 0)
        return;
      FlowEngineCore.User? user = lstUsers.SelectedItems[0].Tag as FlowEngineCore.User;
      if (user is not null)
      {
        frmAdministrationUserProperties f = new frmAdministrationUserProperties(FORM_MODE.Delete, user);
        f.Show();
      }

    }
  }
}
