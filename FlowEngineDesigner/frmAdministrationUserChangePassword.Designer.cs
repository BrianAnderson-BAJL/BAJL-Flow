namespace FlowEngineDesigner
{
  partial class frmAdministrationUserChangePassword
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
      chkShowPassword = new CheckBox();
      label3 = new Label();
      txtOldPassword = new TextBox();
      label4 = new Label();
      txtLoginId = new TextBox();
      btnChangePassword = new Button();
      label1 = new Label();
      txtNewPassword = new TextBox();
      SuspendLayout();
      // 
      // chkShowPassword
      // 
      chkShowPassword.AutoSize = true;
      chkShowPassword.Location = new Point(387, 78);
      chkShowPassword.Name = "chkShowPassword";
      chkShowPassword.Size = new Size(108, 19);
      chkShowPassword.TabIndex = 30;
      chkShowPassword.Text = "Show Password";
      chkShowPassword.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(69, 78);
      label3.Name = "label3";
      label3.Size = new Size(82, 15);
      label3.TabIndex = 28;
      label3.Text = "Old Password:";
      // 
      // txtOldPassword
      // 
      txtOldPassword.Location = new Point(157, 74);
      txtOldPassword.Name = "txtOldPassword";
      txtOldPassword.PasswordChar = '*';
      txtOldPassword.Size = new Size(224, 23);
      txtOldPassword.TabIndex = 27;
      txtOldPassword.Text = "123";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(98, 48);
      label4.Name = "label4";
      label4.Size = new Size(53, 15);
      label4.TabIndex = 26;
      label4.Text = "Login Id:";
      // 
      // txtLoginId
      // 
      txtLoginId.Location = new Point(157, 45);
      txtLoginId.Name = "txtLoginId";
      txtLoginId.Size = new Size(224, 23);
      txtLoginId.TabIndex = 25;
      txtLoginId.Text = "Banderson";
      // 
      // btnChangePassword
      // 
      btnChangePassword.Location = new Point(208, 147);
      btnChangePassword.Name = "btnChangePassword";
      btnChangePassword.Size = new Size(92, 35);
      btnChangePassword.TabIndex = 24;
      btnChangePassword.Text = "&Change";
      btnChangePassword.UseVisualStyleBackColor = true;
      btnChangePassword.Click += btnChangePassword_Click;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(64, 106);
      label1.Name = "label1";
      label1.Size = new Size(87, 15);
      label1.TabIndex = 32;
      label1.Text = "New Password:";
      // 
      // txtNewPassword
      // 
      txtNewPassword.Location = new Point(157, 103);
      txtNewPassword.Name = "txtNewPassword";
      txtNewPassword.PasswordChar = '*';
      txtNewPassword.Size = new Size(224, 23);
      txtNewPassword.TabIndex = 31;
      txtNewPassword.Text = "123";
      // 
      // frmAdministrationUserChangePassword
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(519, 245);
      Controls.Add(label1);
      Controls.Add(txtNewPassword);
      Controls.Add(chkShowPassword);
      Controls.Add(label3);
      Controls.Add(txtOldPassword);
      Controls.Add(label4);
      Controls.Add(txtLoginId);
      Controls.Add(btnChangePassword);
      Name = "frmAdministrationUserChangePassword";
      Text = "frmAdministrationUserChangePassword";
      FormClosing += frmAdministrationUserChangePassword_FormClosing;
      Load += frmAdministrationUserChangePassword_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private CheckBox chkShowPassword;
    private Label label3;
    private TextBox txtOldPassword;
    private Label label4;
    private TextBox txtLoginId;
    private Button btnChangePassword;
    private Label label1;
    private TextBox txtNewPassword;
  }
}