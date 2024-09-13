namespace FlowEngineDesigner
{
  partial class frmSettingString
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
      lblName = new Label();
      txtDescription = new TextBox();
      txtValue = new TextBox();
      btnOk = new Button();
      SuspendLayout();
      // 
      // lblName
      // 
      lblName.AutoSize = true;
      lblName.Location = new Point(17, 21);
      lblName.Name = "lblName";
      lblName.Size = new Size(26, 15);
      lblName.TabIndex = 0;
      lblName.Text = "Key";
      // 
      // txtDescription
      // 
      txtDescription.Location = new Point(14, 46);
      txtDescription.Multiline = true;
      txtDescription.Name = "txtDescription";
      txtDescription.ReadOnly = true;
      txtDescription.ScrollBars = ScrollBars.Vertical;
      txtDescription.Size = new Size(363, 74);
      txtDescription.TabIndex = 1;
      txtDescription.TabStop = false;
      // 
      // txtValue
      // 
      txtValue.Location = new Point(15, 133);
      txtValue.Multiline = true;
      txtValue.Name = "txtValue";
      txtValue.Size = new Size(363, 63);
      txtValue.TabIndex = 0;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(262, 219);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(116, 42);
      btnOk.TabIndex = 3;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // frmSettingString
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(393, 273);
      Controls.Add(btnOk);
      Controls.Add(txtValue);
      Controls.Add(txtDescription);
      Controls.Add(lblName);
      FormBorderStyle = FormBorderStyle.Fixed3D;
      Name = "frmSettingString";
      Text = "Setting - ";
      Load += frmSettingString_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lblName;
    private TextBox txtDescription;
    private TextBox txtValue;
    private Button btnOk;
  }
}