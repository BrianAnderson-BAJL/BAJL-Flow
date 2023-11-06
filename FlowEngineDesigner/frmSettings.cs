using Core;
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
      this.Text = $"Plugin Settings [{mPlugin.Name}]";
      for (int x = 0; x < mPlugin.Settings.Count; x++)
      {
        Core.Setting s = mPlugin.Settings[x];
        ListViewItem lvi = lstSettings.Items.Add(s.Key);
        lvi.Tag = s;
        lvi.SubItems.Add(s.DataType.ToString());
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
          if (lvg == null)
          {
            lvg = new ListViewGroup(s.GroupName, s.GroupName);
            lstSettings.Groups.Add(lvg);
          }
          lvi.Group = lvg;
        }
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
        if (s != null)
        {
          if (s.DataType == DATA_TYPE.DropDownList)
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
          PopulateSettings();
          mPlugin.SaveSettings();
        }
      }
    }

    private void lstSettings_MouseHover(object sender, EventArgs e)
    {
      //Point mousePos = lstSettings.PointToClient(MousePosition);
      //ListViewHitTestInfo hit = lstSettings.HitTest(mousePos);
      //if (hit.Item != null)
      //{
      //  Setting? s = hit.Item.Tag as Setting;
      //  if (s != null)
      //  {

      //    toolTip1.SetToolTip(lstSettings, s.ToolTip);
      //    System.Diagnostics.Debug.WriteLine(s.ToolTip);
      //  }
      //}
    }
  }
}
