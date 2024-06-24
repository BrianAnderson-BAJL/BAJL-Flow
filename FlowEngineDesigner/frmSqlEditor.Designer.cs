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
      splitContainer1 = new SplitContainer();
      scSql = new SplitContainer();
      rtbSql = new RichTextBox();
      label1 = new Label();
      tvDatabase = new TreeView();
      lvParams = new ListView();
      chParamName = new ColumnHeader();
      chDataType = new ColumnHeader();
      chValue = new ColumnHeader();
      btnOk = new Button();
      lblErrorDescription = new Label();
      lblTestSqlResults = new Label();
      btnTestSql = new Button();
      chkSeeResultRecords = new CheckBox();
      menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
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
      menuStrip1.Size = new Size(1482, 24);
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
      // splitContainer1
      // 
      splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      splitContainer1.Location = new Point(0, 27);
      splitContainer1.Name = "splitContainer1";
      splitContainer1.Orientation = Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(scSql);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(chkSeeResultRecords);
      splitContainer1.Panel2.Controls.Add(lvParams);
      splitContainer1.Panel2.Controls.Add(btnOk);
      splitContainer1.Panel2.Controls.Add(lblErrorDescription);
      splitContainer1.Panel2.Controls.Add(lblTestSqlResults);
      splitContainer1.Panel2.Controls.Add(btnTestSql);
      splitContainer1.Size = new Size(1482, 695);
      splitContainer1.SplitterDistance = 558;
      splitContainer1.TabIndex = 7;
      // 
      // scSql
      // 
      scSql.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      scSql.Location = new Point(3, 3);
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
      scSql.Size = new Size(1579, 552);
      scSql.SplitterDistance = 1206;
      scSql.TabIndex = 3;
      // 
      // rtbSql
      // 
      rtbSql.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      rtbSql.Font = new Font("Courier New", 12F, FontStyle.Regular, GraphicsUnit.Point);
      rtbSql.Location = new Point(9, 3);
      rtbSql.Name = "rtbSql";
      rtbSql.Size = new Size(1194, 546);
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
      tvDatabase.Size = new Size(263, 522);
      tvDatabase.TabIndex = 0;
      tvDatabase.BeforeCollapse += tvDatabase_BeforeCollapse;
      tvDatabase.NodeMouseDoubleClick += tvDatabase_NodeMouseDoubleClick;
      tvDatabase.MouseDown += tvDatabase_MouseDown;
      // 
      // lvParams
      // 
      lvParams.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      lvParams.Columns.AddRange(new ColumnHeader[] { chParamName, chDataType, chValue });
      lvParams.GridLines = true;
      lvParams.Location = new Point(12, 3);
      lvParams.Name = "lvParams";
      lvParams.Size = new Size(312, 122);
      lvParams.TabIndex = 11;
      lvParams.UseCompatibleStateImageBehavior = false;
      lvParams.View = View.Details;
      lvParams.MouseClick += lvParams_MouseClick;
      lvParams.MouseDown += lvParams_MouseDown;
      // 
      // chParamName
      // 
      chParamName.Text = "Param Name";
      chParamName.Width = 100;
      // 
      // chDataType
      // 
      chDataType.Text = "Data Type";
      chDataType.Width = 80;
      // 
      // chValue
      // 
      chValue.Text = "Test Value";
      chValue.Width = 80;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(1396, 92);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(83, 36);
      btnOk.TabIndex = 10;
      btnOk.Text = "Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // lblErrorDescription
      // 
      lblErrorDescription.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      lblErrorDescription.AutoSize = true;
      lblErrorDescription.Location = new Point(470, 94);
      lblErrorDescription.Name = "lblErrorDescription";
      lblErrorDescription.Size = new Size(17, 15);
      lblErrorDescription.TabIndex = 9;
      lblErrorDescription.Text = "**";
      // 
      // lblTestSqlResults
      // 
      lblTestSqlResults.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      lblTestSqlResults.AutoSize = true;
      lblTestSqlResults.Location = new Point(409, 94);
      lblTestSqlResults.Name = "lblTestSqlResults";
      lblTestSqlResults.Size = new Size(55, 15);
      lblTestSqlResults.TabIndex = 8;
      lblTestSqlResults.Text = "SUCCESS";
      // 
      // btnTestSql
      // 
      btnTestSql.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      btnTestSql.Location = new Point(330, 85);
      btnTestSql.Name = "btnTestSql";
      btnTestSql.Size = new Size(73, 33);
      btnTestSql.TabIndex = 7;
      btnTestSql.Text = "Test SQL";
      btnTestSql.UseVisualStyleBackColor = true;
      btnTestSql.Click += btnTestSql_Click;
      // 
      // chkSeeResultRecords
      // 
      chkSeeResultRecords.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      chkSeeResultRecords.AutoSize = true;
      chkSeeResultRecords.Location = new Point(334, 54);
      chkSeeResultRecords.Name = "chkSeeResultRecords";
      chkSeeResultRecords.Size = new Size(96, 19);
      chkSeeResultRecords.TabIndex = 12;
      chkSeeResultRecords.Text = "View Records";
      chkSeeResultRecords.UseVisualStyleBackColor = true;
      // 
      // frmSqlEditor
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1482, 719);
      Controls.Add(splitContainer1);
      Controls.Add(menuStrip1);
      MainMenuStrip = menuStrip1;
      Name = "frmSqlEditor";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "SQL Editor";
      FormClosing += frmSqlEditor_FormClosing;
      Load += frmSqlEditor_Load;
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
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
    private ToolStripMenuItem leftJoinMultipleTablesToolStripMenuItem;
    private ToolStripMenuItem testingToolStripMenuItem;
    private SplitContainer splitContainer1;
    private SplitContainer scSql;
    private RichTextBox rtbSql;
    private Label label1;
    private TreeView tvDatabase;
    private Label lblErrorDescription;
    private Label lblTestSqlResults;
    private Button btnTestSql;
    private Button btnOk;
    private ListView lvParams;
    private ColumnHeader chParamName;
    private ColumnHeader chDataType;
    private ColumnHeader chValue;
    private CheckBox chkSeeResultRecords;
  }
}