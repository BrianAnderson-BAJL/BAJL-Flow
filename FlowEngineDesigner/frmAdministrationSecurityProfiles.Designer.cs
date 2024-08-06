namespace FlowEngineDesigner
{
  partial class frmAdministrationSecurityProfiles
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
      btnDelete = new Button();
      btnEditView = new Button();
      btnAdd = new Button();
      lstData = new ListView();
      chName = new ColumnHeader();
      chUsers = new ColumnHeader();
      chSecurityProfiles = new ColumnHeader();
      chFlows = new ColumnHeader();
      chStatistics = new ColumnHeader();
      chTemplates = new ColumnHeader();
      chServerSettings = new ColumnHeader();
      SuspendLayout();
      // 
      // btnDelete
      // 
      btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnDelete.Location = new Point(873, 408);
      btnDelete.Name = "btnDelete";
      btnDelete.Size = new Size(81, 36);
      btnDelete.TabIndex = 7;
      btnDelete.Text = "Delete";
      btnDelete.UseVisualStyleBackColor = true;
      btnDelete.Click += btnDelete_Click;
      // 
      // btnEditView
      // 
      btnEditView.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnEditView.Location = new Point(786, 408);
      btnEditView.Name = "btnEditView";
      btnEditView.Size = new Size(81, 36);
      btnEditView.TabIndex = 6;
      btnEditView.Text = "Edit";
      btnEditView.UseVisualStyleBackColor = true;
      btnEditView.Click += btnEditView_Click;
      // 
      // btnAdd
      // 
      btnAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnAdd.Location = new Point(699, 408);
      btnAdd.Name = "btnAdd";
      btnAdd.Size = new Size(81, 36);
      btnAdd.TabIndex = 5;
      btnAdd.Text = "Add";
      btnAdd.UseVisualStyleBackColor = true;
      btnAdd.Click += btnAdd_Click;
      // 
      // lstData
      // 
      lstData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lstData.Columns.AddRange(new ColumnHeader[] { chName, chUsers, chSecurityProfiles, chFlows, chServerSettings, chStatistics, chTemplates });
      lstData.FullRowSelect = true;
      lstData.GridLines = true;
      lstData.Location = new Point(12, 12);
      lstData.Name = "lstData";
      lstData.Size = new Size(942, 390);
      lstData.TabIndex = 4;
      lstData.UseCompatibleStateImageBehavior = false;
      lstData.View = View.Details;
      lstData.MouseDoubleClick += lstData_MouseDoubleClick;
      // 
      // chName
      // 
      chName.Text = "Name";
      chName.Width = 120;
      // 
      // chUsers
      // 
      chUsers.Text = "Users";
      chUsers.Width = 120;
      // 
      // chSecurityProfiles
      // 
      chSecurityProfiles.Text = "Security Profiles";
      chSecurityProfiles.Width = 120;
      // 
      // chFlows
      // 
      chFlows.Text = "Flows";
      chFlows.Width = 120;
      // 
      // chStatistics
      // 
      chStatistics.Text = "Statistics";
      chStatistics.Width = 120;
      // 
      // chTemplates
      // 
      chTemplates.Text = "Templates";
      chTemplates.Width = 120;
      // 
      // chServerSettings
      // 
      chServerSettings.Text = "Server Settings";
      chServerSettings.Width = 120;
      // 
      // frmAdministrationSecurityProfiles
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(966, 456);
      Controls.Add(btnDelete);
      Controls.Add(btnEditView);
      Controls.Add(btnAdd);
      Controls.Add(lstData);
      Name = "frmAdministrationSecurityProfiles";
      Text = "Administration - Security Profiles";
      Activated += frmAdministrationSecurityProfiles_Activated;
      Load += frmAdministrationSecurityProfiles_Load;
      ResumeLayout(false);
    }

    #endregion

    private Button btnDelete;
    private Button btnEditView;
    private Button btnAdd;
    private ListView lstData;
    private ColumnHeader chName;
    private ColumnHeader chUsers;
    private ColumnHeader chFlows;
    private ColumnHeader chSecurityProfiles;
    private ColumnHeader chStatistics;
    private ColumnHeader chTemplates;
    private ColumnHeader chServerSettings;
  }
}