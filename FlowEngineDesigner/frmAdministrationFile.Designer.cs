namespace FlowEngineDesigner
{
  partial class frmAdministrationFile
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
      if (disposing && (components is not null))
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
      components = new System.ComponentModel.Container();
      splitContainer1 = new SplitContainer();
      tvDirectories = new TreeView();
      lvFiles = new ListView();
      chFileName = new ColumnHeader();
      chModified = new ColumnHeader();
      chStartPlugin = new ColumnHeader();
      chStartCommands = new ColumnHeader();
      txtFileName = new TextBox();
      label1 = new Label();
      btnAction = new Button();
      chkDeployLive = new CheckBox();
      toolTip1 = new ToolTip(components);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      SuspendLayout();
      // 
      // splitContainer1
      // 
      splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      splitContainer1.Location = new Point(12, 12);
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(tvDirectories);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(lvFiles);
      splitContainer1.Size = new Size(961, 572);
      splitContainer1.SplitterDistance = 245;
      splitContainer1.TabIndex = 2;
      // 
      // tvDirectories
      // 
      tvDirectories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tvDirectories.HideSelection = false;
      tvDirectories.Location = new Point(3, 3);
      tvDirectories.Name = "tvDirectories";
      tvDirectories.Size = new Size(239, 566);
      tvDirectories.TabIndex = 0;
      tvDirectories.AfterSelect += tvDirectories_AfterSelect;
      // 
      // lvFiles
      // 
      lvFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lvFiles.Columns.AddRange(new ColumnHeader[] { chFileName, chModified, chStartPlugin, chStartCommands });
      lvFiles.Location = new Point(3, 3);
      lvFiles.Name = "lvFiles";
      lvFiles.Size = new Size(706, 566);
      lvFiles.TabIndex = 0;
      lvFiles.UseCompatibleStateImageBehavior = false;
      lvFiles.View = View.Details;
      lvFiles.ItemSelectionChanged += lvFiles_ItemSelectionChanged;
      lvFiles.MouseDoubleClick += lvFiles_MouseDoubleClick;
      // 
      // chFileName
      // 
      chFileName.Text = "File name";
      chFileName.Width = 180;
      // 
      // chModified
      // 
      chModified.Text = "Modified Date/Time";
      chModified.Width = 180;
      // 
      // chStartPlugin
      // 
      chStartPlugin.Text = "Start plugin";
      chStartPlugin.Width = 120;
      // 
      // chStartCommands
      // 
      chStartCommands.Text = "Start commands";
      chStartCommands.Width = 300;
      // 
      // txtFileName
      // 
      txtFileName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      txtFileName.Location = new Point(264, 587);
      txtFileName.Name = "txtFileName";
      txtFileName.Size = new Size(706, 23);
      txtFileName.TabIndex = 3;
      // 
      // label1
      // 
      label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      label1.AutoSize = true;
      label1.Location = new Point(196, 595);
      label1.Name = "label1";
      label1.Size = new Size(58, 15);
      label1.TabIndex = 4;
      label1.Text = "Filename:";
      // 
      // btnAction
      // 
      btnAction.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnAction.Location = new Point(858, 619);
      btnAction.Name = "btnAction";
      btnAction.Size = new Size(112, 47);
      btnAction.TabIndex = 5;
      btnAction.Text = "[ Action ]";
      btnAction.UseVisualStyleBackColor = true;
      btnAction.Click += btnAction_Click;
      // 
      // chkDeployLive
      // 
      chkDeployLive.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      chkDeployLive.AutoSize = true;
      chkDeployLive.Location = new Point(735, 619);
      chkDeployLive.Name = "chkDeployLive";
      chkDeployLive.Size = new Size(105, 19);
      chkDeployLive.TabIndex = 6;
      chkDeployLive.Text = "Make flow Live";
      toolTip1.SetToolTip(chkDeployLive, "Make this flow live to handle events in the flow engine. this will replace any active flows with the same start up parameters.");
      chkDeployLive.UseVisualStyleBackColor = true;
      // 
      // frmAdministrationFile
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(985, 678);
      Controls.Add(chkDeployLive);
      Controls.Add(btnAction);
      Controls.Add(label1);
      Controls.Add(txtFileName);
      Controls.Add(splitContainer1);
      Name = "frmAdministrationFile";
      Text = "Server Files";
      Activated += frmAdministrationFile_Activated;
      Load += frmAdministrationFile_Load;
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private SplitContainer splitContainer1;
    private TreeView tvDirectories;
    private ListView lvFiles;
    private ColumnHeader chFileName;
    private ColumnHeader chModified;
    private ColumnHeader chStartPlugin;
    private ColumnHeader chStartCommands;
    private TextBox txtFileName;
    private Label label1;
    private Button btnAction;
    private CheckBox chkDeployLive;
    private ToolTip toolTip1;
  }
}