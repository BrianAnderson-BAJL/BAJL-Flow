namespace FlowEngineDesigner
{
  partial class frmSettingNumber
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
      btnOk = new Button();
      txtDescription = new TextBox();
      lblName = new Label();
      nudValue = new NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)nudValue).BeginInit();
      SuspendLayout();
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(259, 172);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(116, 42);
      btnOk.TabIndex = 7;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // txtDescription
      // 
      txtDescription.Location = new Point(12, 35);
      txtDescription.Multiline = true;
      txtDescription.Name = "txtDescription";
      txtDescription.ReadOnly = true;
      txtDescription.Size = new Size(363, 74);
      txtDescription.TabIndex = 6;
      txtDescription.TabStop = false;
      // 
      // lblName
      // 
      lblName.AutoSize = true;
      lblName.Location = new Point(15, 10);
      lblName.Name = "lblName";
      lblName.Size = new Size(26, 15);
      lblName.TabIndex = 5;
      lblName.Text = "Key";
      // 
      // nudValue
      // 
      nudValue.Location = new Point(14, 123);
      nudValue.Maximum = new decimal(new int[] { 2147000000, 0, 0, 0 });
      nudValue.Minimum = new decimal(new int[] { 2147000000, 0, 0, int.MinValue });
      nudValue.Name = "nudValue";
      nudValue.Size = new Size(361, 23);
      nudValue.TabIndex = 8;
      // 
      // frmSettingNumber
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(387, 226);
      Controls.Add(nudValue);
      Controls.Add(btnOk);
      Controls.Add(txtDescription);
      Controls.Add(lblName);
      FormBorderStyle = FormBorderStyle.Fixed3D;
      Name = "frmSettingNumber";
      Text = "frmSettingNumber";
      Load += frmSettingNumber_Load;
      ((System.ComponentModel.ISupportInitialize)nudValue).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button btnOk;
    private TextBox txtDescription;
    private Label lblName;
    private NumericUpDown nudValue;
  }
}