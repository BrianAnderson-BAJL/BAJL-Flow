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
      colorDialog1 = new ColorDialog();
      picBack = new PictureBox();
      label2 = new Label();
      label3 = new Label();
      picFont = new PictureBox();
      ((System.ComponentModel.ISupportInitialize)picBack).BeginInit();
      ((System.ComponentModel.ISupportInitialize)picFont).BeginInit();
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
      // picBack
      // 
      picBack.BorderStyle = BorderStyle.FixedSingle;
      picBack.Location = new Point(107, 199);
      picBack.Name = "picBack";
      picBack.Size = new Size(91, 23);
      picBack.TabIndex = 3;
      picBack.TabStop = false;
      picBack.Click += picBack_Click;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(35, 203);
      label2.Name = "label2";
      label2.Size = new Size(67, 15);
      label2.TabIndex = 4;
      label2.Text = "Back Color:";
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(35, 232);
      label3.Name = "label3";
      label3.Size = new Size(66, 15);
      label3.TabIndex = 6;
      label3.Text = "Font Color:";
      // 
      // picFont
      // 
      picFont.BorderStyle = BorderStyle.FixedSingle;
      picFont.Location = new Point(107, 228);
      picFont.Name = "picFont";
      picFont.Size = new Size(91, 23);
      picFont.TabIndex = 5;
      picFont.TabStop = false;
      picFont.Click += picFont_Click;
      // 
      // frmCommentProperties
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(469, 256);
      Controls.Add(label3);
      Controls.Add(picFont);
      Controls.Add(label2);
      Controls.Add(picBack);
      Controls.Add(btnOk);
      Controls.Add(txtComment);
      Controls.Add(label1);
      Name = "frmCommentProperties";
      StartPosition = FormStartPosition.Manual;
      Text = "Comment properties";
      Load += frmCommentProperties_Load;
      ((System.ComponentModel.ISupportInitialize)picBack).EndInit();
      ((System.ComponentModel.ISupportInitialize)picFont).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox txtComment;
    private Button btnOk;
    private ColorDialog colorDialog1;
    private PictureBox picBack;
    private Label label2;
    private Label label3;
    private PictureBox picFont;
  }
}