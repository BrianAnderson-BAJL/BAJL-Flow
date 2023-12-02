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
  public partial class frmSettingNumber : Form
  {
    private Setting mSetting;

    public frmSettingNumber(Setting setting)
    {
      InitializeComponent();
      mSetting = setting;
    }

    private void frmSettingNumber_Load(object sender, EventArgs e)
    {
      this.Text = "Setting [" + mSetting.Key + "]";
      lblName.Text = mSetting.Key;
      txtDescription.Text = mSetting.Description;
      nudValue.Value = Convert.ToDecimal(mSetting.Value);
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      mSetting.Value = nudValue.Value;
      this.Close();

    }
  }
}
