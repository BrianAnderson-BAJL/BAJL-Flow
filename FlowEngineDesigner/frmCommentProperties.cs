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
  public partial class frmCommentProperties : Form
  {
    private FlowEngineCore.Comment Comment;
    private static int[]? CustomColors;
    public frmCommentProperties(FlowEngineCore.Comment comment)
    {
      InitializeComponent();
      Comment = comment;
      picBack.BackColor = Comment.ColorBackground;
      picFont.BackColor = Comment.ColorText;
      colorDialog1.CustomColors = CustomColors;
    }

    private void frmCommentProperties_Load(object sender, EventArgs e)
    {
      txtComment.Text = Comment.Text;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      Comment.Text = txtComment.Text;
      this.Close();
    }

    private void picBack_Click(object sender, EventArgs e)
    {
      colorDialog1.Color = Comment.ColorBackground;
      DialogResult dr = colorDialog1.ShowDialog();
      if (dr == DialogResult.OK)
      {
        picBack.BackColor = colorDialog1.Color;
        Comment.ColorBackground = colorDialog1.Color;
        CustomColors = colorDialog1.CustomColors;
      }
    }

    private void picFont_Click(object sender, EventArgs e)
    {
      colorDialog1.Color = Comment.ColorText;
      DialogResult dr = colorDialog1.ShowDialog();
      if (dr == DialogResult.OK)
      {
        picFont.BackColor = colorDialog1.Color;
        Comment.ColorText = colorDialog1.Color;
        CustomColors = colorDialog1.CustomColors;
      }

    }
  }
}
