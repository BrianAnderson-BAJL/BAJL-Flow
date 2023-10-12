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
      splitContainer1 = new SplitContainer();
      tvDirectories = new TreeView();
      lvFiles = new ListView();
      chFileName = new ColumnHeader();
      chModified = new ColumnHeader();
      chStartPlugin = new ColumnHeader();
      chStartCommands = new ColumnHeader();
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
      splitContainer1.Size = new Size(961, 588);
      splitContainer1.SplitterDistance = 245;
      splitContainer1.TabIndex = 2;
      // 
      // tvDirectories
      // 
      tvDirectories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tvDirectories.Location = new Point(3, 3);
      tvDirectories.Name = "tvDirectories";
      tvDirectories.Size = new Size(239, 582);
      tvDirectories.TabIndex = 0;
      tvDirectories.AfterSelect += tvDirectories_AfterSelect;
      // 
      // lvFiles
      // 
      lvFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lvFiles.Columns.AddRange(new ColumnHeader[] { chFileName, chModified, chStartPlugin, chStartCommands });
      lvFiles.Location = new Point(3, 3);
      lvFiles.Name = "lvFiles";
      lvFiles.Size = new Size(706, 582);
      lvFiles.TabIndex = 0;
      lvFiles.UseCompatibleStateImageBehavior = false;
      lvFiles.View = View.Details;
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
      // frmAdministrationFile
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(985, 694);
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
    }

    #endregion

    private SplitContainer splitContainer1;
    private TreeView tvDirectories;
    private ListView lvFiles;
    private ColumnHeader chFileName;
    private ColumnHeader chModified;
    private ColumnHeader chStartPlugin;
    private ColumnHeader chStartCommands;
  }
}