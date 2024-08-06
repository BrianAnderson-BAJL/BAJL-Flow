namespace FlowEngineDesigner
{
  partial class frmAdministrationSecurityProfileProperties
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
      lblDuplicate = new Label();
      label6 = new Label();
      cmbUsers = new ComboBox();
      btnAction = new Button();
      label2 = new Label();
      txtName = new TextBox();
      label1 = new Label();
      cmbFlows = new ComboBox();
      label3 = new Label();
      cmbSecurityProfiles = new ComboBox();
      label4 = new Label();
      cmbStatistics = new ComboBox();
      label5 = new Label();
      cmbTemplates = new ComboBox();
      label7 = new Label();
      cmbServerSettings = new ComboBox();
      SuspendLayout();
      // 
      // lblDuplicate
      // 
      lblDuplicate.AutoSize = true;
      lblDuplicate.Location = new Point(365, 36);
      lblDuplicate.Name = "lblDuplicate";
      lblDuplicate.Size = new Size(57, 15);
      lblDuplicate.TabIndex = 25;
      lblDuplicate.Text = "Duplicate";
      // 
      // label6
      // 
      label6.AutoSize = true;
      label6.Location = new Point(76, 65);
      label6.Name = "label6";
      label6.Size = new Size(38, 15);
      label6.TabIndex = 24;
      label6.Text = "Users:";
      // 
      // cmbUsers
      // 
      cmbUsers.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbUsers.FormattingEnabled = true;
      cmbUsers.Location = new Point(131, 62);
      cmbUsers.Name = "cmbUsers";
      cmbUsers.Size = new Size(225, 23);
      cmbUsers.TabIndex = 19;
      // 
      // btnAction
      // 
      btnAction.Location = new Point(437, 220);
      btnAction.Name = "btnAction";
      btnAction.Size = new Size(101, 39);
      btnAction.TabIndex = 20;
      btnAction.Text = "[Action]";
      btnAction.UseVisualStyleBackColor = true;
      btnAction.Click += btnAction_Click;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(72, 36);
      label2.Name = "label2";
      label2.Size = new Size(42, 15);
      label2.TabIndex = 18;
      label2.Text = "Name:";
      // 
      // txtName
      // 
      txtName.Location = new Point(131, 33);
      txtName.Name = "txtName";
      txtName.Size = new Size(224, 23);
      txtName.TabIndex = 14;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(74, 121);
      label1.Name = "label1";
      label1.Size = new Size(40, 15);
      label1.TabIndex = 27;
      label1.Text = "Flows:";
      // 
      // cmbFlows
      // 
      cmbFlows.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbFlows.FormattingEnabled = true;
      cmbFlows.Location = new Point(130, 121);
      cmbFlows.Name = "cmbFlows";
      cmbFlows.Size = new Size(225, 23);
      cmbFlows.TabIndex = 26;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(20, 94);
      label3.Name = "label3";
      label3.Size = new Size(94, 15);
      label3.TabIndex = 29;
      label3.Text = "Security profiles:";
      // 
      // cmbSecurityProfiles
      // 
      cmbSecurityProfiles.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbSecurityProfiles.FormattingEnabled = true;
      cmbSecurityProfiles.Location = new Point(130, 91);
      cmbSecurityProfiles.Name = "cmbSecurityProfiles";
      cmbSecurityProfiles.Size = new Size(225, 23);
      cmbSecurityProfiles.TabIndex = 28;
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(58, 181);
      label4.Name = "label4";
      label4.Size = new Size(56, 15);
      label4.TabIndex = 31;
      label4.Text = "Statistics:";
      // 
      // cmbStatistics
      // 
      cmbStatistics.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbStatistics.FormattingEnabled = true;
      cmbStatistics.Location = new Point(131, 178);
      cmbStatistics.Name = "cmbStatistics";
      cmbStatistics.Size = new Size(225, 23);
      cmbStatistics.TabIndex = 30;
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.Location = new Point(51, 210);
      label5.Name = "label5";
      label5.Size = new Size(63, 15);
      label5.TabIndex = 33;
      label5.Text = "Templates:";
      // 
      // cmbTemplates
      // 
      cmbTemplates.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbTemplates.FormattingEnabled = true;
      cmbTemplates.Location = new Point(131, 207);
      cmbTemplates.Name = "cmbTemplates";
      cmbTemplates.Size = new Size(225, 23);
      cmbTemplates.TabIndex = 32;
      // 
      // label7
      // 
      label7.AutoSize = true;
      label7.Location = new Point(27, 150);
      label7.Name = "label7";
      label7.Size = new Size(87, 15);
      label7.TabIndex = 35;
      label7.Text = "Server Settings:";
      // 
      // cmbServerSettings
      // 
      cmbServerSettings.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbServerSettings.FormattingEnabled = true;
      cmbServerSettings.Location = new Point(130, 150);
      cmbServerSettings.Name = "cmbServerSettings";
      cmbServerSettings.Size = new Size(225, 23);
      cmbServerSettings.TabIndex = 34;
      // 
      // frmAdministrationSecurityProfileProperties
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(570, 287);
      Controls.Add(label7);
      Controls.Add(cmbServerSettings);
      Controls.Add(label5);
      Controls.Add(cmbTemplates);
      Controls.Add(label4);
      Controls.Add(cmbStatistics);
      Controls.Add(label3);
      Controls.Add(cmbSecurityProfiles);
      Controls.Add(label1);
      Controls.Add(cmbFlows);
      Controls.Add(lblDuplicate);
      Controls.Add(label6);
      Controls.Add(cmbUsers);
      Controls.Add(btnAction);
      Controls.Add(label2);
      Controls.Add(txtName);
      Name = "frmAdministrationSecurityProfileProperties";
      Text = "Security profile";
      Load += frmAdministrationSecurityProfileProperties_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label lblDuplicate;
    private Label label6;
    private ComboBox cmbUsers;
    private Button btnAction;
    private Label label2;
    private TextBox txtName;
    private Label label1;
    private ComboBox cmbFlows;
    private Label label3;
    private ComboBox cmbSecurityProfiles;
    private Label label4;
    private ComboBox cmbStatistics;
    private Label label5;
    private ComboBox cmbTemplates;
    private Label label7;
    private ComboBox cmbServerSettings;
  }
}