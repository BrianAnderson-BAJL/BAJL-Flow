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
  public partial class frmSettingDropDown : Form
  {
    Setting mSetting;
    public frmSettingDropDown(Setting setting)
    {
      InitializeComponent();
      mSetting = setting;
      lblLabel.Text = setting.Key;
      for (int x = 0; x < setting.SubSettings.Count; x++)
      {
        cmbDropDown.Items.Add(setting.SubSettings[x].Value.ToString());
      }
      for (int x = 0; x < cmbDropDown.Items.Count; x++)
      {
        if (cmbDropDown.Items[x].ToString() == setting.Value.ToString())
        {
          cmbDropDown.SelectedIndex = x;
        }
      }
    }

    private void frmSettingDropDown_Load(object sender, EventArgs e)
    {

    }

    private void cmbDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
      mSetting.Value = mSetting.SubSettings[cmbDropDown.SelectedIndex].Value;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
