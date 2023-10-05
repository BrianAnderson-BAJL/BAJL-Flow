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
      txtDomainName = new TextBox();
      nudPort = new NumericUpDown();
      label1 = new Label();
      label2 = new Label();
      chkShowPassword = new CheckBox();
      chkRememberMe = new CheckBox();
      label3 = new Label();
      txtPassword = new TextBox();
      label4 = new Label();
      txtLoginId = new TextBox();
      ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
      SuspendLayout();
      // 
      // btnConnect
      // 
      btnConnect.Location = new Point(211, 155);
      btnConnect.Name = "btnConnect";
      btnConnect.Size = new Size(92, 35);
      btnConnect.TabIndex = 0;
      btnConnect.Text = "&Connect";
      btnConnect.UseVisualStyleBackColor = true;
      btnConnect.Click += btnConnect_Click;
      // 
      // txtDomainName
      // 
      txtDomainName.Location = new Point(211, 12);
      txtDomainName.Name = "txtDomainName";
      txtDomainName.Size = new Size(221, 23);
      txtDomainName.TabIndex = 1;
      txtDomainName.Text = "flowengine.bajlllc.com";
      // 
      // nudPort
      // 
      nudPort.Location = new Point(211, 41);
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
      label1.Location = new Point(95, 15);
      label1.Name = "label1";
      label1.Size = new Size(112, 15);
      label1.TabIndex = 3;
      label1.Text = "Domain name or IP:";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(173, 43);
      label2.Name = "label2";
      label2.Size = new Size(32, 15);
      label2.TabIndex = 4;
      label2.Text = "Port:";
      // 
      // chkShowPassword
      // 
      chkShowPassword.AutoSize = true;
      chkShowPassword.Location = new Point(443, 130);
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
      chkRememberMe.Location = new Point(443, 101);
      chkRememberMe.Name = "chkRememberMe";
      chkRememberMe.Size = new Size(104, 19);
      chkRememberMe.TabIndex = 22;
      chkRememberMe.Text = "Remember me";
      chkRememberMe.UseVisualStyleBackColor = true;
      chkRememberMe.CheckedChanged += chkRememberMe_CheckedChanged;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(147, 129);
      label3.Name = "label3";
      label3.Size = new Size(60, 15);
      label3.TabIndex = 21;
      label3.Text = "Password:";
      label3.Click += label3_Click;
      // 
      // txtPassword
      // 
      txtPassword.Location = new Point(213, 126);
      txtPassword.Name = "txtPassword";
      txtPassword.PasswordChar = '*';
      txtPassword.Size = new Size(224, 23);
      txtPassword.TabIndex = 20;
      txtPassword.Text = "123";
      txtPassword.TextChanged += txtPassword_TextChanged;
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(154, 100);
      label4.Name = "label4";
      label4.Size = new Size(53, 15);
      label4.TabIndex = 19;
      label4.Text = "Login Id:";
      label4.Click += label4_Click;
      // 
      // txtLoginId
      // 
      txtLoginId.Location = new Point(213, 97);
      txtLoginId.Name = "txtLoginId";
      txtLoginId.Size = new Size(224, 23);
      txtLoginId.TabIndex = 18;
      txtLoginId.Text = "Jleung";
      txtLoginId.TextChanged += txtLoginId_TextChanged;
      // 
      // frmServerConnect
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(581, 229);
      Controls.Add(chkShowPassword);
      Controls.Add(chkRememberMe);
      Controls.Add(label3);
      Controls.Add(txtPassword);
      Controls.Add(label4);
      Controls.Add(txtLoginId);
      Controls.Add(label2);
      Controls.Add(label1);
      Controls.Add(nudPort);
      Controls.Add(txtDomainName);
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
    private TextBox txtDomainName;
    private NumericUpDown nudPort;
    private Label label1;
    private Label label2;
    private CheckBox chkShowPassword;
    private CheckBox chkRememberMe;
    private Label label3;
    private TextBox txtPassword;
    private Label label4;
    private TextBox txtLoginId;
  }
}