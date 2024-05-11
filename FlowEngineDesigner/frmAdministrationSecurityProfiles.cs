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
  public partial class frmAdministrationSecurityProfiles : Form
  {
    private SecurityProfile.SECURITY_ACCESS_LEVEL Access;
    public frmAdministrationSecurityProfiles()
    {
      InitializeComponent();
    }

    private void frmAdministrationSecurityProfiles_Activated(object sender, EventArgs e)
    {
      cServer.RefreshSecurityProfiles(Callback_SecurityProfilesGet);
    }

    private void Callback_SecurityProfilesGet(FlowEngineCore.Administration.EventArgsPacket e)
    {
      lstData.Items.Clear();
      if (cServer.SecurityProfiles is null)
        return;

      for (int x = 0; x < cServer.SecurityProfiles.Count; x++)
      {
        ListViewItem lvi = lstData.Items.Add(cServer.SecurityProfiles[x].Name);
        lvi.SubItems.Add(cServer.SecurityProfiles[x].AdministrationUsers.ToString());
        lvi.SubItems.Add(cServer.SecurityProfiles[x].AdministrationSecurityProfiles.ToString());
        lvi.SubItems.Add(cServer.SecurityProfiles[x].AdministrationFlows.ToString());
        lvi.Tag = cServer.SecurityProfiles[x];
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      frmAdministrationSecurityProfileProperties f = new frmAdministrationSecurityProfileProperties(FORM_MODE.Add);
      f.Show();
    }

    private void btnEditView_Click(object sender, EventArgs e)
    {
      EditSelected(FORM_MODE.Edit);
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      EditSelected(FORM_MODE.Delete);
    }

    private void EditSelected(FORM_MODE mode)
    {
      if (lstData.SelectedItems.Count == 0)
        return;
      FlowEngineCore.SecurityProfile? item = lstData.SelectedItems[0].Tag as FlowEngineCore.SecurityProfile;
      if (item is not null)
      {
        if (mode == FORM_MODE.Edit && Access == SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
          mode = FORM_MODE.ReadOnly;
        frmAdministrationSecurityProfileProperties f = new frmAdministrationSecurityProfileProperties(mode, item);
        f.Show();
      }
    }

    private void lstData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      EditSelected(FORM_MODE.Edit);
    }

    private void frmAdministrationSecurityProfiles_Load(object sender, EventArgs e)
    {
      Access = cServer.AccessLevelForUserLoggedIn(SecurityProfile.SECURITY_AREA.SecurityProfiles);
      if (Access < SecurityProfile.SECURITY_ACCESS_LEVEL.Full)
      {
        btnAdd.Enabled = false;
        btnDelete.Enabled = false;
      }
      if (Access == SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
      {
        btnEditView.Text = "View";
      }
      if (Access == SecurityProfile.SECURITY_ACCESS_LEVEL.None)  //How did you get here?
      {
        btnEditView.Enabled = false;
      }

    }
  }
}
