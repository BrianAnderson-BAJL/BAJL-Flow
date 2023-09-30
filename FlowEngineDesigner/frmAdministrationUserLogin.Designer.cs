namespace FlowEngineDesigner
{
  partial class frmAdministrationUserLogin
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
      btnLogin = new Button();
      label3 = new Label();
      txtPassword = new TextBox();
      label2 = new Label();
      txtLoginId = new TextBox();
      chkRememberMe = new CheckBox();
      chkShowPassword = new CheckBox();
      SuspendLayout();
      // 
      // btnLogin
      // 
      btnLogin.Location = new Point(227, 113);
      btnLogin.Name = "btnLogin";
      btnLogin.Size = new Size(101, 39);
      btnLogin.TabIndex = 15;
      btnLogin.Text = "&Login";
      btnLogin.UseVisualStyleBackColor = true;
      btnLogin.Click += btnLogin_Click;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(38, 64);
      label3.Name = "label3";
      label3.Size = new Size(60, 15);
      label3.TabIndex = 14;
      label3.Text = "Password:";
      // 
      // txtPassword
      // 
      txtPassword.Location = new Point(104, 61);
      txtPassword.Name = "txtPassword";
      txtPassword.PasswordChar = '*';
      txtPassword.Size = new Size(224, 23);
      txtPassword.TabIndex = 13;
      txtPassword.Text = "123";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(45, 35);
      label2.Name = "label2";
      label2.Size = new Size(53, 15);
      label2.TabIndex = 12;
      label2.Text = "Login Id:";
      // 
      // txtLoginId
      // 
      txtLoginId.Location = new Point(104, 32);
      txtLoginId.Name = "txtLoginId";
      txtLoginId.Size = new Size(224, 23);
      txtLoginId.TabIndex = 11;
      txtLoginId.Text = "Banderson";
      // 
      // chkRememberMe
      // 
      chkRememberMe.AutoSize = true;
      chkRememberMe.Location = new Point(334, 36);
      chkRememberMe.Name = "chkRememberMe";
      chkRememberMe.Size = new Size(104, 19);
      chkRememberMe.TabIndex = 16;
      chkRememberMe.Text = "Remember me";
      chkRememberMe.UseVisualStyleBackColor = true;
      // 
      // chkShowPassword
      // 
      chkShowPassword.AutoSize = true;
      chkShowPassword.Location = new Point(334, 65);
      chkShowPassword.Name = "chkShowPassword";
      chkShowPassword.Size = new Size(108, 19);
      chkShowPassword.TabIndex = 17;
      chkShowPassword.Text = "Show Password";
      chkShowPassword.UseVisualStyleBackColor = true;
      chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
      // 
      // frmAdministrationUserLogin
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(467, 174);
      Controls.Add(chkShowPassword);
      Controls.Add(chkRememberMe);
      Controls.Add(btnLogin);
      Controls.Add(label3);
      Controls.Add(txtPassword);
      Controls.Add(label2);
      Controls.Add(txtLoginId);
      Name = "frmAdministrationUserLogin";
      Text = "User Login";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button btnLogin;
    private Label label3;
    private TextBox txtPassword;
    private Label label2;
    private TextBox txtLoginId;
    private CheckBox chkRememberMe;
    private CheckBox chkShowPassword;
  }
}