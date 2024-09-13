namespace FlowEngineDesigner
{
  partial class frmServerConnect
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
      btnConnect = new Button();
      nudPort = new NumericUpDown();
      label1 = new Label();
      label2 = new Label();
      chkShowPassword = new CheckBox();
      chkRememberMe = new CheckBox();
      label3 = new Label();
      txtPassword = new TextBox();
      label4 = new Label();
      txtLoginId = new TextBox();
      chkRememberPassword = new CheckBox();
      cmbProfile = new ComboBox();
      chkAutoConnect = new CheckBox();
      chkDebugAlways = new CheckBox();
      label5 = new Label();
      txtServerPrivateKey = new TextBox();
      label6 = new Label();
      txtUrl = new TextBox();
      ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
      SuspendLayout();
      // 
      // btnConnect
      // 
      btnConnect.Location = new Point(211, 253);
      btnConnect.Name = "btnConnect";
      btnConnect.Size = new Size(92, 35);
      btnConnect.TabIndex = 0;
      btnConnect.Text = "&Connect";
      btnConnect.UseVisualStyleBackColor = true;
      btnConnect.Click += btnConnect_Click;
      // 
      // nudPort
      // 
      nudPort.Location = new Point(211, 66);
      nudPort.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
      nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
      nudPort.Name = "nudPort";
      nudPort.Size = new Size(131, 23);
      nudPort.TabIndex = 2;
      nudPort.Value = new decimal(new int[] { 7000, 0, 0, 0 });
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(152, 40);
      label1.Name = "label1";
      label1.Size = new Size(46, 15);
      label1.TabIndex = 3;
      label1.Text = "Url / IP:";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(173, 68);
      label2.Name = "label2";
      label2.Size = new Size(32, 15);
      label2.TabIndex = 4;
      label2.Text = "Port:";
      // 
      // chkShowPassword
      // 
      chkShowPassword.AutoSize = true;
      chkShowPassword.Location = new Point(441, 144);
      chkShowPassword.Name = "chkShowPassword";
      chkShowPassword.Size = new Size(108, 19);
      chkShowPassword.TabIndex = 23;
      chkShowPassword.Text = "Show Password";
      chkShowPassword.UseVisualStyleBackColor = true;
      chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
      // 
      // chkRememberMe
      // 
      chkRememberMe.AutoSize = true;
      chkRememberMe.Location = new Point(441, 94);
      chkRememberMe.Name = "chkRememberMe";
      chkRememberMe.Size = new Size(117, 19);
      chkRememberMe.TabIndex = 22;
      chkRememberMe.Text = "Remember Login";
      chkRememberMe.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(145, 127);
      label3.Name = "label3";
      label3.Size = new Size(60, 15);
      label3.TabIndex = 21;
      label3.Text = "Password:";
      // 
      // txtPassword
      // 
      txtPassword.Location = new Point(211, 124);
      txtPassword.Name = "txtPassword";
      txtPassword.PasswordChar = '*';
      txtPassword.Size = new Size(224, 23);
      txtPassword.TabIndex = 20;
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(152, 98);
      label4.Name = "label4";
      label4.Size = new Size(53, 15);
      label4.TabIndex = 19;
      label4.Text = "Login Id:";
      // 
      // txtLoginId
      // 
      txtLoginId.Location = new Point(211, 95);
      txtLoginId.Name = "txtLoginId";
      txtLoginId.Size = new Size(224, 23);
      txtLoginId.TabIndex = 18;
      // 
      // chkRememberPassword
      // 
      chkRememberPassword.AutoSize = true;
      chkRememberPassword.Location = new Point(441, 119);
      chkRememberPassword.Name = "chkRememberPassword";
      chkRememberPassword.Size = new Size(137, 19);
      chkRememberPassword.TabIndex = 24;
      chkRememberPassword.Text = "Remember password";
      chkRememberPassword.UseVisualStyleBackColor = true;
      // 
      // cmbProfile
      // 
      cmbProfile.FormattingEnabled = true;
      cmbProfile.Location = new Point(211, 7);
      cmbProfile.Name = "cmbProfile";
      cmbProfile.Size = new Size(224, 23);
      cmbProfile.TabIndex = 25;
      cmbProfile.SelectedIndexChanged += cmbProfile_SelectedIndexChanged;
      // 
      // chkAutoConnect
      // 
      chkAutoConnect.AutoSize = true;
      chkAutoConnect.Location = new Point(211, 204);
      chkAutoConnect.Name = "chkAutoConnect";
      chkAutoConnect.Size = new Size(100, 19);
      chkAutoConnect.TabIndex = 26;
      chkAutoConnect.Text = "Auto Connect";
      chkAutoConnect.UseVisualStyleBackColor = true;
      // 
      // chkDebugAlways
      // 
      chkDebugAlways.AutoSize = true;
      chkDebugAlways.Location = new Point(211, 224);
      chkDebugAlways.Name = "chkDebugAlways";
      chkDebugAlways.Size = new Size(101, 19);
      chkDebugAlways.TabIndex = 27;
      chkDebugAlways.Text = "Debug Always";
      chkDebugAlways.UseVisualStyleBackColor = true;
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.Location = new Point(103, 178);
      label5.Name = "label5";
      label5.Size = new Size(102, 15);
      label5.TabIndex = 29;
      label5.Text = "Server Private key:";
      // 
      // txtServerPrivateKey
      // 
      txtServerPrivateKey.Location = new Point(211, 175);
      txtServerPrivateKey.Name = "txtServerPrivateKey";
      txtServerPrivateKey.Size = new Size(313, 23);
      txtServerPrivateKey.TabIndex = 28;
      // 
      // label6
      // 
      label6.AutoSize = true;
      label6.Location = new Point(128, 10);
      label6.Name = "label6";
      label6.Size = new Size(77, 15);
      label6.TabIndex = 30;
      label6.Text = "Profile name:";
      // 
      // txtUrl
      // 
      txtUrl.Location = new Point(211, 37);
      txtUrl.Name = "txtUrl";
      txtUrl.Size = new Size(224, 23);
      txtUrl.TabIndex = 31;
      // 
      // frmServerConnect
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(581, 312);
      Controls.Add(txtUrl);
      Controls.Add(label6);
      Controls.Add(label5);
      Controls.Add(txtServerPrivateKey);
      Controls.Add(chkDebugAlways);
      Controls.Add(chkAutoConnect);
      Controls.Add(cmbProfile);
      Controls.Add(chkRememberPassword);
      Controls.Add(chkShowPassword);
      Controls.Add(chkRememberMe);
      Controls.Add(label3);
      Controls.Add(txtPassword);
      Controls.Add(label4);
      Controls.Add(txtLoginId);
      Controls.Add(label2);
      Controls.Add(label1);
      Controls.Add(nudPort);
      Controls.Add(btnConnect);
      Name = "frmServerConnect";
      Text = "Connect to Flow Engine server";
      Load += frmServerConnect_Load;
      ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button btnConnect;
    private NumericUpDown nudPort;
    private Label label1;
    private Label label2;
    private CheckBox chkShowPassword;
    private CheckBox chkRememberMe;
    private Label label3;
    private TextBox txtPassword;
    private Label label4;
    private TextBox txtLoginId;
    private CheckBox chkRememberPassword;
    private ComboBox cmbProfile;
    private CheckBox chkAutoConnect;
    private CheckBox chkDebugAlways;
    private Label label5;
    private TextBox txtServerPrivateKey;
    private Label label6;
    private TextBox txtUrl;
  }
}