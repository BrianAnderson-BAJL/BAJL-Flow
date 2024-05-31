namespace FlowEngineDesigner
{
  partial class frmCommentProperties
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      label1 = new Label();
      txtComment = new TextBox();
      btnOk = new Button();
      SuspendLayout();
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(12, 17);
      label1.Name = "label1";
      label1.Size = new Size(64, 15);
      label1.TabIndex = 0;
      label1.Text = "Comment:";
      // 
      // txtComment
      // 
      txtComment.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      txtComment.Location = new Point(12, 35);
      txtComment.Multiline = true;
      txtComment.Name = "txtComment";
      txtComment.Size = new Size(445, 158);
      txtComment.TabIndex = 1;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(362, 199);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(95, 45);
      btnOk.TabIndex = 2;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // frmCommentProperties
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(469, 256);
      Controls.Add(btnOk);
      Controls.Add(txtComment);
      Controls.Add(label1);
      Name = "frmCommentProperties";
      StartPosition = FormStartPosition.Manual;
      Text = "Comment properties";
      Load += frmCommentProperties_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox txtComment;
    private Button btnOk;
  }
}