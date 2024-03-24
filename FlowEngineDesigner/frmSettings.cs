using Core;
using Google.Protobuf.WellKnownTypes;
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
  public partial class frmSettings : Form
  {
    Core.Plugin mPlugin;
    internal frmSettings(Core.Plugin plugin)
    {
      InitializeComponent();
      mPlugin = plugin;

    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      mPlugin.SaveSettings();
      this.Close();
    }

    private void frmSettings_Load(object sender, EventArgs e)
    {
      PopulateSettings();
    }

    private void PopulateSettings()
    {
      lstSettings.Items.Clear();
      lstSettings.Groups.Clear();
      this.Text = $"Plugin Settings [{mPlugin.Name}]";
      for (int x = 0; x < mPlugin.Settings.Count; x++)
      {
        Core.Setting s = mPlugin.Settings[x];
        AddSettingToListView(s);

        if (s.DataType == DATA_TYPE.String && s.StringSubType == STRING_SUB_TYPE.DropDownList)
        {
          for (int y = 0; y < s.SubSettings.Count; y++)
          {
            if (s.Value is null)
              continue;
            if ((s.DropDownGroupName + s.Value.ToString()) == s.SubSettings[y].DropDownGroupName)
            {
              AddSettingToListView(s.SubSettings[y]);
            }
          }
        }

      }
    }

    private void AddSettingToListView(Setting s)
    {
      ListViewItem lvi = lstSettings.Items.Add(s.Key);
      lvi.Tag = s;
      lvi.SubItems.Add(s.DataType.ToString());
      if (s.Value is null)
        return;
      lvi.SubItems.Add(s.Value.ToString());
      if (s.DataType == Core.DATA_TYPE.Color)
      {
        lvi.UseItemStyleForSubItems = false;
        lvi.SubItems[2].BackColor = (Color)s.Value;
      }

      //Search for group
      if (s.GroupName.Length > 0)
      {
        ListViewGroup? lvg = FindGroup(s.GroupName);
        if (lvg is null)
        {
          lvg = new ListViewGroup(s.GroupName, s.GroupName);
          lstSettings.Groups.Add(lvg);
        }
        lvi.Group = lvg;
      }
    }


    private ListViewGroup? FindGroup(string groupName)
    {
      for (int y = 0; y < lstSettings.Groups.Count; y++)
      {
        ListViewGroup lvg = lstSettings.Groups[y];
        if (lvg.Name == groupName)
        {
          return lvg;
        }
      }
      return null;
    }

    private void lstSettings_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void lstSettings_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (lstSettings.SelectedItems.Count > 0)
      {
        Setting? s = lstSettings.SelectedItems[0].Tag as Setting;
        if (s is not null)
        {
          if (s.DataType == DATA_TYPE.String && s.StringSubType == STRING_SUB_TYPE.DropDownList)
          {
            frmSettingDropDown f = new frmSettingDropDown(s);
            f.ShowDialog();
          }
          else if (s.DataType == DATA_TYPE.Color)
          {
            frmSettingColor f = new frmSettingColor(s);
            f.ShowDialog();
          }
          else if (s.DataType == DATA_TYPE.String)
          {
            frmSettingString f = new frmSettingString(s);
            f.ShowDialog();
          }
          else if (s.DataType == DATA_TYPE.Integer || s.DataType == DATA_TYPE.Decimal)
          {
            frmSettingNumber f = new frmSettingNumber(s);
            f.ShowDialog();
          }
          PopulateSettings();
          mPlugin.SaveSettings();
        }
      }
    }

    private void lstSettings_MouseHover(object sender, EventArgs e)
    {
    }
  }
}
