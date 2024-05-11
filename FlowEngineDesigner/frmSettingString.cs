using FlowEngineCore;
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
  public partial class frmSettingString : Form
  {
    private Setting mSetting;

    public frmSettingString(Setting s)
    {
      InitializeComponent();
      mSetting = s;
    }

    private void frmSettingString_Load(object sender, EventArgs e)
    {
      this.Text = "Setting [" + mSetting.Key + "]";
      lblName.Text = mSetting.Key;
      txtDescription.Text = mSetting.Description;
      if (mSetting.Value is null)
        return;
      txtValue.Text = mSetting.Value.ToString();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      mSetting.Value = txtValue.Text;
      this.Close();
    }
  }
}
