namespace FlowEngineDesigner
{
  partial class frmSettingColor
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
      colorDialog1 = new ColorDialog();
      btnOk = new Button();
      lblLabel = new Label();
      pictureBox1 = new PictureBox();
      ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
      SuspendLayout();
      // 
      // btnOk
      // 
      btnOk.Location = new Point(313, 67);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(109, 36);
      btnOk.TabIndex = 5;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // lblLabel
      // 
      lblLabel.Location = new Point(21, 27);
      lblLabel.Name = "lblLabel";
      lblLabel.Size = new Size(137, 23);
      lblLabel.TabIndex = 3;
      lblLabel.Text = "label1";
      lblLabel.TextAlign = ContentAlignment.MiddleRight;
      // 
      // pictureBox1
      // 
      pictureBox1.Cursor = Cursors.Hand;
      pictureBox1.Location = new Point(170, 26);
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Size = new Size(252, 28);
      pictureBox1.TabIndex = 6;
      pictureBox1.TabStop = false;
      pictureBox1.Click += pictureBox1_Click;
      // 
      // frmSettingColor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(441, 119);
      Controls.Add(pictureBox1);
      Controls.Add(btnOk);
      Controls.Add(lblLabel);
      Name = "frmSettingColor";
      Text = "frmSettingColor";
      Load += frmSettingColor_Load;
      ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private ColorDialog colorDialog1;
    private Button btnOk;
    private Label lblLabel;
    private PictureBox pictureBox1;
  }
}