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
    public frmCommentProperties(FlowEngineCore.Comment comment)
    {
      InitializeComponent();
      Comment = comment;
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
  }
}
