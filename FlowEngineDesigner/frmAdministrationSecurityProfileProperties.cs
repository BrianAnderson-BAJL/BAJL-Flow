using FlowEngineCore;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace FlowEngineDesigner
{
  public partial class frmAdministrationSecurityProfileProperties : Form
  {
    private FlowEngineCore.SecurityProfile? Profile = null;
    private FORM_MODE Mode = FORM_MODE.ReadOnly;
    private string OldName = "";
    public frmAdministrationSecurityProfileProperties(FORM_MODE mode, FlowEngineCore.SecurityProfile? profile = null)
    {
      InitializeComponent();
      Profile = profile;
      Mode = mode;
    }

    private void frmAdministrationSecurityProfileProperties_Load(object sender, EventArgs e)
    {
      FlowEngineCore.SecurityProfile.SECURITY_ACCESS_LEVEL[] vals = Enum.GetValues<FlowEngineCore.SecurityProfile.SECURITY_ACCESS_LEVEL>();
      for (int x = 0; x < vals.Length; x++)
      {
        cmbUsers.Items.Add(vals[x].ToString());
        cmbSecurityProfiles.Items.Add(vals[x].ToString());
        cmbFlows.Items.Add(vals[x].ToString());
        cmbServerSettings.Items.Add(vals[x].ToString());
      }
      cmbStatistics.Items.Add(SecurityProfile.SECURITY_ACCESS_SIMPLE.None.ToString());
      cmbStatistics.Items.Add(SecurityProfile.SECURITY_ACCESS_SIMPLE.Access.ToString());
      cmbTemplates.Items.Add(SecurityProfile.SECURITY_ACCESS_SIMPLE.None.ToString());
      cmbTemplates.Items.Add(SecurityProfile.SECURITY_ACCESS_SIMPLE.Access.ToString());

      if (Profile is not null)
      {
        OldName = Profile.Name;
        txtName.Text = Profile.Name;
        Global.ComboBoxSetIndex(cmbUsers, Profile.AdministrationUsers.ToString());
        Global.ComboBoxSetIndex(cmbSecurityProfiles, Profile.AdministrationSecurityProfiles.ToString());
        Global.ComboBoxSetIndex(cmbFlows, Profile.AdministrationFlows.ToString());
        Global.ComboBoxSetIndex(cmbServerSettings, Profile.ServerSettings.ToString());
        Global.ComboBoxSetIndex(cmbStatistics, Profile.Statistics.ToString());
        Global.ComboBoxSetIndex(cmbTemplates, Profile.Templates.ToString());
      }

      if (Mode == FORM_MODE.ReadOnly || Mode == FORM_MODE.Delete)
      {
        txtName.ReadOnly = true;
        cmbUsers.Enabled = false;
        cmbSecurityProfiles.Enabled = false;
        cmbFlows.Enabled = false;
        cmbServerSettings.Enabled = false;
        cmbStatistics.Enabled = false;
        cmbTemplates.Enabled = false;
      }

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
      }
      else if (Mode == FORM_MODE.Edit)
      {
        btnAction.Text = "Edit";
      }

    }

    private void btnAction_Click(object sender, EventArgs e)
    {
      if (cServer.UserLoggedIn is null)
        return;

      SecurityProfile.SECURITY_ACCESS_LEVEL users = Enum.Parse<SecurityProfile.SECURITY_ACCESS_LEVEL>(cmbUsers.Text);
      SecurityProfile.SECURITY_ACCESS_LEVEL sp = Enum.Parse<SecurityProfile.SECURITY_ACCESS_LEVEL>(cmbSecurityProfiles.Text);
      SecurityProfile.SECURITY_ACCESS_LEVEL flows = Enum.Parse<SecurityProfile.SECURITY_ACCESS_LEVEL>(cmbFlows.Text);
      SecurityProfile.SECURITY_ACCESS_LEVEL serverSettings = Enum.Parse<SecurityProfile.SECURITY_ACCESS_LEVEL>(cmbServerSettings.Text);
      SecurityProfile.SECURITY_ACCESS_SIMPLE stats = Enum.Parse<SecurityProfile.SECURITY_ACCESS_SIMPLE>(cmbStatistics.Text);
      SecurityProfile.SECURITY_ACCESS_SIMPLE template = Enum.Parse<SecurityProfile.SECURITY_ACCESS_SIMPLE>(cmbTemplates.Text);

      if (Mode == FORM_MODE.Add)
      {
        FlowEngineCore.Administration.Messages.SecurityProfileAdd u = new FlowEngineCore.Administration.Messages.SecurityProfileAdd(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, txtName.Text, users, sp, flows, serverSettings, stats, template);
        cServer.SendAndResponse(u.GetPacket(), Callback);
      }
      if (Mode == FORM_MODE.Edit)
      {
        FlowEngineCore.Administration.Messages.SecurityProfileEdit u = new FlowEngineCore.Administration.Messages.SecurityProfileEdit(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, OldName, txtName.Text, users, sp, flows, serverSettings, stats, template);
        cServer.SendAndResponse(u.GetPacket(), Callback);
      }
      if (Mode == FORM_MODE.Delete)
      {
        FlowEngineCore.Administration.Messages.SecurityProfileDelete u = new FlowEngineCore.Administration.Messages.SecurityProfileDelete(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey, txtName.Text);
        cServer.SendAndResponse(u.GetPacket(), Callback);
      }
      if (Mode == FORM_MODE.ReadOnly)
        this.Close();

    }

    private void Callback(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() == BaseResponse.RESPONSE_CODE.Success)
      {
        this.Close();
      }
    }

  }
}
