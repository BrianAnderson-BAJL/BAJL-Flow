namespace FlowEngineDesigner
{
  partial class frmAdministrationUsers
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
      lstUsers = new ListView();
      chLoginId = new ColumnHeader();
      chNameFirst = new ColumnHeader();
      chNameSur = new ColumnHeader();
      chSecurityProfile = new ColumnHeader();
      btnAdd = new Button();
      btnEditView = new Button();
      btnDelete = new Button();
      SuspendLayout();
      // 
      // lstUsers
      // 
      lstUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lstUsers.Columns.AddRange(new ColumnHeader[] { chLoginId, chNameFirst, chNameSur, chSecurityProfile });
      lstUsers.FullRowSelect = true;
      lstUsers.GridLines = true;
      lstUsers.Location = new Point(12, 12);
      lstUsers.Name = "lstUsers";
      lstUsers.Size = new Size(640, 309);
      lstUsers.TabIndex = 0;
      lstUsers.UseCompatibleStateImageBehavior = false;
      lstUsers.View = View.Details;
      lstUsers.MouseDoubleClick += lstUsers_MouseDoubleClick;
      // 
      // chLoginId
      // 
      chLoginId.Text = "Login Id";
      chLoginId.Width = 120;
      // 
      // chNameFirst
      // 
      chNameFirst.Text = "First Name";
      chNameFirst.Width = 180;
      // 
      // chNameSur
      // 
      chNameSur.Text = "Surname";
      chNameSur.Width = 180;
      // 
      // chSecurityProfile
      // 
      chSecurityProfile.Text = "Security Profile";
      chSecurityProfile.Width = 120;
      // 
      // btnAdd
      // 
      btnAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnAdd.Location = new Point(397, 327);
      btnAdd.Name = "btnAdd";
      btnAdd.Size = new Size(81, 36);
      btnAdd.TabIndex = 1;
      btnAdd.Text = "Add";
      btnAdd.UseVisualStyleBackColor = true;
      btnAdd.Click += btnAdd_Click;
      // 
      // btnEditView
      // 
      btnEditView.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnEditView.Location = new Point(484, 327);
      btnEditView.Name = "btnEditView";
      btnEditView.Size = new Size(81, 36);
      btnEditView.TabIndex = 2;
      btnEditView.Text = "Edit";
      btnEditView.UseVisualStyleBackColor = true;
      btnEditView.Click += btnEditView_Click;
      // 
      // btnDelete
      // 
      btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnDelete.Location = new Point(571, 327);
      btnDelete.Name = "btnDelete";
      btnDelete.Size = new Size(81, 36);
      btnDelete.TabIndex = 3;
      btnDelete.Text = "Delete";
      btnDelete.UseVisualStyleBackColor = true;
      btnDelete.Click += btnDelete_Click;
      // 
      // frmAdministrationUsers
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(664, 375);
      Controls.Add(btnDelete);
      Controls.Add(btnEditView);
      Controls.Add(btnAdd);
      Controls.Add(lstUsers);
      Name = "frmAdministrationUsers";
      Text = "Administration - Users";
      Activated += frmAdministrationUsers_Activated;
      Load += frmAdministrationUsers_Load;
      ResumeLayout(false);
    }

    #endregion

    private ListView lstUsers;
    private ColumnHeader chLoginId;
    private ColumnHeader chNameFirst;
    private ColumnHeader chNameSur;
    private Button btnAdd;
    private Button btnEditView;
    private Button btnDelete;
    private ColumnHeader chSecurityProfile;
  }
}