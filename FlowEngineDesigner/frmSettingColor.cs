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
  public partial class frmSettingColor : Form
  {
    private Setting mSetting;
    public frmSettingColor(Setting setting)
    {
      InitializeComponent();
      mSetting = setting;
      pictureBox1.BackColor = (Color)mSetting.Value;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
      colorDialog1.Color = (Color)mSetting.Value;
      if (colorDialog1.ShowDialog() == DialogResult.OK)
      {
        mSetting.Value = colorDialog1.Color;
        pictureBox1.BackColor = (Color)mSetting.Value;
      }
    }

    private void frmSettingColor_Load(object sender, EventArgs e)
    {

    }
  }
}
