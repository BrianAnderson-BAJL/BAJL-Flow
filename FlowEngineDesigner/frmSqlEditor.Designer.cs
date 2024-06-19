namespace FlowEngineDesigner
{
  partial class frmSqlEditor
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
      menuStrip1 = new MenuStrip();
      fileToolStripMenuItem = new ToolStripMenuItem();
      openToolStripMenuItem = new ToolStripMenuItem();
      saveAsToolStripMenuItem = new ToolStripMenuItem();
      samplesToolStripMenuItem = new ToolStripMenuItem();
      selectToolStripMenuItem = new ToolStripMenuItem();
      simpleToolStripMenuItem = new ToolStripMenuItem();
      groupByHavingToolStripMenuItem = new ToolStripMenuItem();
      joinToLookUpToolStripMenuItem = new ToolStripMenuItem();
      leftJoinMultipleTablesToolStripMenuItem = new ToolStripMenuItem();
      joinToolStripMenuItem = new ToolStripMenuItem();
      innerJoinToolStripMenuItem = new ToolStripMenuItem();
      leftJoinToolStripMenuItem = new ToolStripMenuItem();
      rightJoinToolStripMenuItem = new ToolStripMenuItem();
      fullJoinFullOuterJoinToolStripMenuItem = new ToolStripMenuItem();
      viewToolStripMenuItem = new ToolStripMenuItem();
      showDatabaseToolStripMenuItem = new ToolStripMenuItem();
      testingToolStripMenuItem = new ToolStripMenuItem();
      openFileDialog1 = new OpenFileDialog();
      saveFileDialog1 = new SaveFileDialog();
      scSql = new SplitContainer();
      rtbSql = new RichTextBox();
      label1 = new Label();
      tvDatabase = new TreeView();
      btnOk = new Button();
      menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)scSql).BeginInit();
      scSql.Panel1.SuspendLayout();
      scSql.Panel2.SuspendLayout();
      scSql.SuspendLayout();
      SuspendLayout();
      // 
      // menuStrip1
      // 
      menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, samplesToolStripMenuItem, viewToolStripMenuItem });
      menuStrip1.Location = new Point(0, 0);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.Size = new Size(1221, 24);
      menuStrip1.TabIndex = 1;
      menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveAsToolStripMenuItem });
      fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      fileToolStripMenuItem.Size = new Size(37, 20);
      fileToolStripMenuItem.Text = "&File";
      // 
      // openToolStripMenuItem
      // 
      openToolStripMenuItem.Name = "openToolStripMenuItem";
      openToolStripMenuItem.Size = new Size(114, 22);
      openToolStripMenuItem.Text = "&Open";
      openToolStripMenuItem.Click += openToolStripMenuItem_Click;
      // 
      // saveAsToolStripMenuItem
      // 
      saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
      saveAsToolStripMenuItem.Size = new Size(114, 22);
      saveAsToolStripMenuItem.Text = "Save &As";
      saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
      // 
      // samplesToolStripMenuItem
      // 
      samplesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { selectToolStripMenuItem, joinToolStripMenuItem });
      samplesToolStripMenuItem.Name = "samplesToolStripMenuItem";
      samplesToolStripMenuItem.Size = new Size(95, 20);
      samplesToolStripMenuItem.Text = "&Insert Samples";
      samplesToolStripMenuItem.Click += samplesToolStripMenuItem_Click;
      // 
      // selectToolStripMenuItem
      // 
      selectToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { simpleToolStripMenuItem, groupByHavingToolStripMenuItem, joinToLookUpToolStripMenuItem, leftJoinMultipleTablesToolStripMenuItem });
      selectToolStripMenuItem.Name = "selectToolStripMenuItem";
      selectToolStripMenuItem.Size = new Size(105, 22);
      selectToolStripMenuItem.Text = "&Select";
      // 
      // simpleToolStripMenuItem
      // 
      simpleToolStripMenuItem.Name = "simpleToolStripMenuItem";
      simpleToolStripMenuItem.Size = new Size(199, 22);
      simpleToolStripMenuItem.Text = "&Simple";
      simpleToolStripMenuItem.Click += simpleToolStripMenuItem_Click;
      // 
      // groupByHavingToolStripMenuItem
      // 
      groupByHavingToolStripMenuItem.Name = "groupByHavingToolStripMenuItem";
      groupByHavingToolStripMenuItem.Size = new Size(199, 22);
      groupByHavingToolStripMenuItem.Text = "&Group by / Having";
      groupByHavingToolStripMenuItem.Click += groupByHavingToolStripMenuItem_Click;
      // 
      // joinToLookUpToolStripMenuItem
      // 
      joinToLookUpToolStripMenuItem.Name = "joinToLookUpToolStripMenuItem";
      joinToLookUpToolStripMenuItem.Size = new Size(199, 22);
      joinToLookUpToolStripMenuItem.Text = "&Join to Look up";
      joinToLookUpToolStripMenuItem.Click += joinToLookUpToolStripMenuItem_Click;
      // 
      // leftJoinMultipleTablesToolStripMenuItem
      // 
      leftJoinMultipleTablesToolStripMenuItem.Name = "leftJoinMultipleTablesToolStripMenuItem";
      leftJoinMultipleTablesToolStripMenuItem.Size = new Size(199, 22);
      leftJoinMultipleTablesToolStripMenuItem.Text = "&Left Join multiple tables";
      leftJoinMultipleTablesToolStripMenuItem.Click += leftJoinMultipleTablesToolStripMenuItem_Click;
      // 
      // joinToolStripMenuItem
      // 
      joinToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { innerJoinToolStripMenuItem, leftJoinToolStripMenuItem, rightJoinToolStripMenuItem, fullJoinFullOuterJoinToolStripMenuItem });
      joinToolStripMenuItem.Name = "joinToolStripMenuItem";
      joinToolStripMenuItem.Size = new Size(105, 22);
      joinToolStripMenuItem.Text = "&Join";
      // 
      // innerJoinToolStripMenuItem
      // 
      innerJoinToolStripMenuItem.Name = "innerJoinToolStripMenuItem";
      innerJoinToolStripMenuItem.Size = new Size(204, 22);
      innerJoinToolStripMenuItem.Text = "Inner Join";
      innerJoinToolStripMenuItem.Click += innerJoinToolStripMenuItem_Click;
      // 
      // leftJoinToolStripMenuItem
      // 
      leftJoinToolStripMenuItem.Name = "leftJoinToolStripMenuItem";
      leftJoinToolStripMenuItem.Size = new Size(204, 22);
      leftJoinToolStripMenuItem.Text = "Left Join";
      leftJoinToolStripMenuItem.Click += leftJoinToolStripMenuItem_Click;
      // 
      // rightJoinToolStripMenuItem
      // 
      rightJoinToolStripMenuItem.Name = "rightJoinToolStripMenuItem";
      rightJoinToolStripMenuItem.Size = new Size(204, 22);
      rightJoinToolStripMenuItem.Text = "Right Join";
      rightJoinToolStripMenuItem.Click += rightJoinToolStripMenuItem_Click;
      // 
      // fullJoinFullOuterJoinToolStripMenuItem
      // 
      fullJoinFullOuterJoinToolStripMenuItem.Name = "fullJoinFullOuterJoinToolStripMenuItem";
      fullJoinFullOuterJoinToolStripMenuItem.Size = new Size(204, 22);
      fullJoinFullOuterJoinToolStripMenuItem.Text = "Full Join / Full Outer Join";
      fullJoinFullOuterJoinToolStripMenuItem.Click += fullJoinFullOuterJoinToolStripMenuItem_Click;
      // 
      // viewToolStripMenuItem
      // 
      viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showDatabaseToolStripMenuItem, testingToolStripMenuItem });
      viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      viewToolStripMenuItem.Size = new Size(44, 20);
      viewToolStripMenuItem.Text = "&View";
      // 
      // showDatabaseToolStripMenuItem
      // 
      showDatabaseToolStripMenuItem.Checked = true;
      showDatabaseToolStripMenuItem.CheckState = CheckState.Checked;
      showDatabaseToolStripMenuItem.Name = "showDatabaseToolStripMenuItem";
      showDatabaseToolStripMenuItem.Size = new Size(154, 22);
      showDatabaseToolStripMenuItem.Text = "&Show Database";
      showDatabaseToolStripMenuItem.Click += showDatabaseToolStripMenuItem_Click;
      // 
      // testingToolStripMenuItem
      // 
      testingToolStripMenuItem.Name = "testingToolStripMenuItem";
      testingToolStripMenuItem.Size = new Size(154, 22);
      testingToolStripMenuItem.Text = "&Testing";
      // 
      // openFileDialog1
      // 
      openFileDialog1.FileName = "openFileDialog1";
      // 
      // scSql
      // 
      scSql.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      scSql.Location = new Point(0, 27);
      scSql.Name = "scSql";
      // 
      // scSql.Panel1
      // 
      scSql.Panel1.Controls.Add(rtbSql);
      // 
      // scSql.Panel2
      // 
      scSql.Panel2.Controls.Add(label1);
      scSql.Panel2.Controls.Add(tvDatabase);
      scSql.Size = new Size(1221, 596);
      scSql.SplitterDistance = 928;
      scSql.TabIndex = 2;
      // 
      // rtbSql
      // 
      rtbSql.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      rtbSql.Font = new Font("Courier New", 12F, FontStyle.Regular, GraphicsUnit.Point);
      rtbSql.Location = new Point(0, 0);
      rtbSql.Name = "rtbSql";
      rtbSql.Size = new Size(925, 593);
      rtbSql.TabIndex = 1;
      rtbSql.Text = "";
      rtbSql.TextChanged += rtbSql_TextChanged;
      rtbSql.MouseDoubleClick += rtbSql_MouseDoubleClick;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(3, 9);
      label1.Name = "label1";
      label1.Size = new Size(55, 15);
      label1.TabIndex = 1;
      label1.Text = "Database";
      // 
      // tvDatabase
      // 
      tvDatabase.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tvDatabase.Location = new Point(3, 27);
      tvDatabase.Name = "tvDatabase";
      tvDatabase.Size = new Size(283, 566);
      tvDatabase.TabIndex = 0;
      tvDatabase.BeforeCollapse += tvDatabase_BeforeCollapse;
      tvDatabase.NodeMouseDoubleClick += tvDatabase_NodeMouseDoubleClick;
      tvDatabase.MouseDown += tvDatabase_MouseDown;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(1135, 629);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(83, 36);
      btnOk.TabIndex = 3;
      btnOk.Text = "Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // frmSqlEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1221, 672);
      Controls.Add(btnOk);
      Controls.Add(scSql);
      Controls.Add(menuStrip1);
      MainMenuStrip = menuStrip1;
      Name = "frmSqlEditor";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "SQL Editor";
      FormClosing += frmSqlEditor_FormClosing;
      Load += frmSqlEditor_Load;
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      scSql.Panel1.ResumeLayout(false);
      scSql.Panel2.ResumeLayout(false);
      scSql.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)scSql).EndInit();
      scSql.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private OpenFileDialog openFileDialog1;
    private SaveFileDialog saveFileDialog1;
    private ToolStripMenuItem samplesToolStripMenuItem;
    private ToolStripMenuItem joinToolStripMenuItem;
    private ToolStripMenuItem innerJoinToolStripMenuItem;
    private ToolStripMenuItem leftJoinToolStripMenuItem;
    private ToolStripMenuItem rightJoinToolStripMenuItem;
    private ToolStripMenuItem fullJoinFullOuterJoinToolStripMenuItem;
    private ToolStripMenuItem selectToolStripMenuItem;
    private ToolStripMenuItem simpleToolStripMenuItem;
    private ToolStripMenuItem groupByHavingToolStripMenuItem;
    private ToolStripMenuItem joinToLookUpToolStripMenuItem;
    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem showDatabaseToolStripMenuItem;
    private SplitContainer scSql;
    private RichTextBox rtbSql;
    private Label label1;
    private TreeView tvDatabase;
    private Button btnOk;
    private ToolStripMenuItem leftJoinMultipleTablesToolStripMenuItem;
    private ToolStripMenuItem testingToolStripMenuItem;
  }
}