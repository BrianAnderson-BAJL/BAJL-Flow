using Core.Administration.Messages;
using Core.Administration.Packets;
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
    }

    private void Callback_UsersGet(Core.Administration.EventArgsPacket e)
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
            lvi.SubItems.Add(response.Users[x].NameFirst);
            lvi.SubItems.Add(response.Users[x].NameSur);
            lvi.SubItems.Add(response.Users[x].SecurityProfile);
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
        cServer.SendAndResponse(usersGet.GetPacket(), Callback_UsersGet, Core.Administration.Packet.PACKET_TYPE.UsersGetResponse);
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
      Core.User? user = lstUsers.SelectedItems[0].Tag as Core.User;
      if (user is not null)
      {
        frmAdministrationUserProperties f = new frmAdministrationUserProperties(FORM_MODE.Edit, user);
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
      Core.User? user = lstUsers.SelectedItems[0].Tag as Core.User;
      if (user is not null)
      {
        frmAdministrationUserProperties f = new frmAdministrationUserProperties(FORM_MODE.Delete, user);
        f.Show();
      }

    }
  }
}
