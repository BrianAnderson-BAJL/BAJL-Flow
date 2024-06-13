namespace FlowEngineDesigner
{
  partial class frmSettingDropDown
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
      lblLabel = new Label();
      cmbDropDown = new ComboBox();
      btnOk = new Button();
      SuspendLayout();
      // 
      // lblLabel
      // 
      lblLabel.Location = new Point(12, 35);
      lblLabel.Name = "lblLabel";
      lblLabel.Size = new Size(137, 23);
      lblLabel.TabIndex = 0;
      lblLabel.Text = "label1";
      lblLabel.TextAlign = ContentAlignment.MiddleRight;
      // 
      // cmbDropDown
      // 
      cmbDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbDropDown.FormattingEnabled = true;
      cmbDropDown.Location = new Point(155, 35);
      cmbDropDown.Name = "cmbDropDown";
      cmbDropDown.Size = new Size(258, 23);
      cmbDropDown.TabIndex = 1;
      cmbDropDown.SelectedIndexChanged += cmbDropDown_SelectedIndexChanged;
      // 
      // btnOk
      // 
      btnOk.Location = new Point(304, 75);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(109, 36);
      btnOk.TabIndex = 2;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // frmSettingDropDown
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(441, 119);
      Controls.Add(btnOk);
      Controls.Add(cmbDropDown);
      Controls.Add(lblLabel);
      FormBorderStyle = FormBorderStyle.Fixed3D;
      Name = "frmSettingDropDown";
      Text = "frmSettingDropDown";
      Load += frmSettingDropDown_Load;
      ResumeLayout(false);
    }

    #endregion

    private Label lblLabel;
    private ComboBox cmbDropDown;
    private Button btnOk;
  }
}