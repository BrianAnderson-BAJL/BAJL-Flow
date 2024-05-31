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
  public partial class frmQuickTip : Form
  {
    public static void ShowQuickTip(string text)
    {
      frmQuickTip f = new frmQuickTip(text);
      f.Location = Cursor.Position;
      f.Show();

    }
    public frmQuickTip(string text)
    {
      InitializeComponent();
      this.txtQuickTip.Text = text;
    }

    private void frmQuickTip_Deactivate(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
