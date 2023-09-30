namespace FlowEngineDesigner
{
  partial class frmAdministrationUserProperties
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
      label2 = new Label();
      txtLoginId = new TextBox();
      label3 = new Label();
      txtPassword = new TextBox();
      label4 = new Label();
      txtNameFirst = new TextBox();
      label5 = new Label();
      txtNameSur = new TextBox();
      btnAction = new Button();
      cmbSecurityProfile = new ComboBox();
      label6 = new Label();
      lblDuplicate = new Label();
      SuspendLayout();
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(61, 57);
      label2.Name = "label2";
      label2.Size = new Size(53, 15);
      label2.TabIndex = 3;
      label2.Text = "Login Id:";
      label2.Click += label2_Click;
      // 
      // txtLoginId
      // 
      txtLoginId.Location = new Point(120, 54);
      txtLoginId.Name = "txtLoginId";
      txtLoginId.Size = new Size(224, 23);
      txtLoginId.TabIndex = 0;
      txtLoginId.Text = "Banderson";
      txtLoginId.TextChanged += txtLoginId_TextChanged;
      txtLoginId.Enter += txtLoginId_Enter;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(54, 86);
      label3.Name = "label3";
      label3.Size = new Size(60, 15);
      label3.TabIndex = 5;
      label3.Text = "Password:";
      label3.Click += label3_Click;
      // 
      // txtPassword
      // 
      txtPassword.Location = new Point(120, 83);
      txtPassword.Name = "txtPassword";
      txtPassword.PasswordChar = '*';
      txtPassword.Size = new Size(224, 23);
      txtPassword.TabIndex = 1;
      txtPassword.Text = "123";
      txtPassword.TextChanged += txtPassword_TextChanged;
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(47, 115);
      label4.Name = "label4";
      label4.Size = new Size(67, 15);
      label4.TabIndex = 7;
      label4.Text = "First Name:";
      // 
      // txtNameFirst
      // 
      txtNameFirst.Location = new Point(120, 112);
      txtNameFirst.Name = "txtNameFirst";
      txtNameFirst.Size = new Size(224, 23);
      txtNameFirst.TabIndex = 2;
      txtNameFirst.Text = "Brian";
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.Location = new Point(57, 144);
      label5.Name = "label5";
      label5.Size = new Size(57, 15);
      label5.TabIndex = 9;
      label5.Text = "Surname:";
      // 
      // txtNameSur
      // 
      txtNameSur.Location = new Point(120, 141);
      txtNameSur.Name = "txtNameSur";
      txtNameSur.Size = new Size(224, 23);
      txtNameSur.TabIndex = 3;
      txtNameSur.Text = "Anderson";
      // 
      // btnAction
      // 
      btnAction.Location = new Point(426, 241);
      btnAction.Name = "btnAction";
      btnAction.Size = new Size(101, 39);
      btnAction.TabIndex = 5;
      btnAction.Text = "[Action]";
      btnAction.UseVisualStyleBackColor = true;
      btnAction.Click += btnAction_Click;
      // 
      // cmbSecurityProfile
      // 
      cmbSecurityProfile.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbSecurityProfile.FormattingEnabled = true;
      cmbSecurityProfile.Location = new Point(119, 171);
      cmbSecurityProfile.Name = "cmbSecurityProfile";
      cmbSecurityProfile.Size = new Size(225, 23);
      cmbSecurityProfile.TabIndex = 4;
      // 
      // label6
      // 
      label6.AutoSize = true;
      label6.Location = new Point(25, 174);
      label6.Name = "label6";
      label6.Size = new Size(89, 15);
      label6.TabIndex = 12;
      label6.Text = "Security Profile:";
      // 
      // lblDuplicate
      // 
      lblDuplicate.AutoSize = true;
      lblDuplicate.Location = new Point(354, 57);
      lblDuplicate.Name = "lblDuplicate";
      lblDuplicate.Size = new Size(57, 15);
      lblDuplicate.TabIndex = 13;
      lblDuplicate.Text = "Duplicate";
      lblDuplicate.Click += lblDuplicate_Click;
      // 
      // frmAdministrationUserProperties
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(539, 292);
      Controls.Add(lblDuplicate);
      Controls.Add(label6);
      Controls.Add(cmbSecurityProfile);
      Controls.Add(btnAction);
      Controls.Add(label5);
      Controls.Add(txtNameSur);
      Controls.Add(label4);
      Controls.Add(txtNameFirst);
      Controls.Add(label3);
      Controls.Add(txtPassword);
      Controls.Add(label2);
      Controls.Add(txtLoginId);
      Name = "frmAdministrationUserProperties";
      Text = "frmAdministrationUserProperties";
      Load += frmAdministrationUserProperties_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Label label2;
    private TextBox txtLoginId;
    private Label label3;
    private TextBox txtPassword;
    private Label label4;
    private TextBox txtNameFirst;
    private Label label5;
    private TextBox txtNameSur;
    private Button btnAction;
    private ComboBox cmbSecurityProfile;
    private Label label6;
    private Label lblDuplicate;
  }
}