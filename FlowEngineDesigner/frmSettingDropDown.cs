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
      if (setting.Options is null)
        return;


      for (int x = 0; x < setting.Options.Count; x++)
      {
        cmbDropDown.Items.Add(setting.Options[x]);
      }

      Global.ComboBoxSetIndex(cmbDropDown, setting.Value.ToString()!);
    }

    private void frmSettingDropDown_Load(object sender, EventArgs e)
    {

    }

    private void cmbDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (mSetting.Options is null)
        return;

      mSetting.Value = mSetting.Options[cmbDropDown.SelectedIndex];
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
