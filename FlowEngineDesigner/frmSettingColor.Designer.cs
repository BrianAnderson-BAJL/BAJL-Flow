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
      lblName = new Label();
      pictureBox1 = new PictureBox();
      txtDescription = new TextBox();
      ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
      SuspendLayout();
      // 
      // btnOk
      // 
      btnOk.Location = new Point(320, 122);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(109, 36);
      btnOk.TabIndex = 5;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // lblName
      // 
      lblName.Location = new Point(28, 82);
      lblName.Name = "lblName";
      lblName.Size = new Size(137, 23);
      lblName.TabIndex = 3;
      lblName.Text = "label1";
      lblName.TextAlign = ContentAlignment.MiddleRight;
      // 
      // pictureBox1
      // 
      pictureBox1.BorderStyle = BorderStyle.FixedSingle;
      pictureBox1.Cursor = Cursors.Hand;
      pictureBox1.Location = new Point(177, 81);
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Size = new Size(252, 28);
      pictureBox1.TabIndex = 6;
      pictureBox1.TabStop = false;
      pictureBox1.Click += pictureBox1_Click;
      // 
      // txtDescription
      // 
      txtDescription.Location = new Point(28, 12);
      txtDescription.Multiline = true;
      txtDescription.Name = "txtDescription";
      txtDescription.ReadOnly = true;
      txtDescription.ScrollBars = ScrollBars.Vertical;
      txtDescription.Size = new Size(401, 63);
      txtDescription.TabIndex = 7;
      txtDescription.TabStop = false;
      // 
      // frmSettingColor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(441, 167);
      Controls.Add(txtDescription);
      Controls.Add(pictureBox1);
      Controls.Add(btnOk);
      Controls.Add(lblName);
      FormBorderStyle = FormBorderStyle.Fixed3D;
      Name = "frmSettingColor";
      Text = "frmSettingColor";
      Load += frmSettingColor_Load;
      ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private ColorDialog colorDialog1;
    private Button btnOk;
    private Label lblName;
    private PictureBox pictureBox1;
    private TextBox txtDescription;
  }
}