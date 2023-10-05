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
      label1.Location = new Point(74, 120);
      label1.Name = "label1";
      label1.Size = new Size(40, 15);
      label1.TabIndex = 27;
      label1.Text = "Flows:";
      // 
      // cmbFlows
      // 
      cmbFlows.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbFlows.FormattingEnabled = true;
      cmbFlows.Location = new Point(130, 120);
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
      // frmAdministrationSecurityProfileProperties
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(570, 287);
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
  }
}