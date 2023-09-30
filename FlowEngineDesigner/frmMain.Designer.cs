namespace FlowEngineDesigner
{
  partial class frmMain
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
      tsmFile = new ToolStripMenuItem();
      newToolStripMenuItem = new ToolStripMenuItem();
      openToolStripMenuItem = new ToolStripMenuItem();
      exitToolStripMenuItem = new ToolStripMenuItem();
      viewToolStripMenuItem = new ToolStripMenuItem();
      toolboxToolStripMenuItem = new ToolStripMenuItem();
      tracerToolStripMenuItem = new ToolStripMenuItem();
      layoutToolStripMenuItem = new ToolStripMenuItem();
      layout1ToolStripMenuItem = new ToolStripMenuItem();
      settingsToolStripMenuItem = new ToolStripMenuItem();
      usersToolStripMenuItem = new ToolStripMenuItem();
      optionsToolStripMenuItem = new ToolStripMenuItem();
      setFocusOnMouseEnterToolStripMenuItem = new ToolStripMenuItem();
      highlightStepsOnExecutionToolStripMenuItem = new ToolStripMenuItem();
      serverToolStripMenuItem = new ToolStripMenuItem();
      connectToolStripMenuItem = new ToolStripMenuItem();
      connectRecentToolStripMenuItem = new ToolStripMenuItem();
      administrationToolStripMenuItem = new ToolStripMenuItem();
      usersToolStripMenuItem1 = new ToolStripMenuItem();
      statusStrip1 = new StatusStrip();
      menuStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // menuStrip1
      // 
      menuStrip1.Items.AddRange(new ToolStripItem[] { tsmFile, viewToolStripMenuItem, settingsToolStripMenuItem, serverToolStripMenuItem });
      menuStrip1.Location = new Point(0, 0);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.Size = new Size(947, 24);
      menuStrip1.TabIndex = 0;
      menuStrip1.Text = "menuStrip1";
      // 
      // tsmFile
      // 
      tsmFile.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, exitToolStripMenuItem });
      tsmFile.Name = "tsmFile";
      tsmFile.Size = new Size(37, 20);
      tsmFile.Text = "&File";
      // 
      // newToolStripMenuItem
      // 
      newToolStripMenuItem.Name = "newToolStripMenuItem";
      newToolStripMenuItem.Size = new Size(103, 22);
      newToolStripMenuItem.Text = "&New";
      newToolStripMenuItem.Click += newToolStripMenuItem_Click;
      // 
      // openToolStripMenuItem
      // 
      openToolStripMenuItem.Name = "openToolStripMenuItem";
      openToolStripMenuItem.Size = new Size(103, 22);
      openToolStripMenuItem.Text = "&Open";
      // 
      // exitToolStripMenuItem
      // 
      exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      exitToolStripMenuItem.Size = new Size(103, 22);
      exitToolStripMenuItem.Text = "E&xit";
      // 
      // viewToolStripMenuItem
      // 
      viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolboxToolStripMenuItem, tracerToolStripMenuItem, layoutToolStripMenuItem });
      viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      viewToolStripMenuItem.Size = new Size(44, 20);
      viewToolStripMenuItem.Text = "&View";
      // 
      // toolboxToolStripMenuItem
      // 
      toolboxToolStripMenuItem.Name = "toolboxToolStripMenuItem";
      toolboxToolStripMenuItem.Size = new Size(116, 22);
      toolboxToolStripMenuItem.Text = "&Toolbox";
      toolboxToolStripMenuItem.Click += toolboxToolStripMenuItem_Click;
      // 
      // tracerToolStripMenuItem
      // 
      tracerToolStripMenuItem.Name = "tracerToolStripMenuItem";
      tracerToolStripMenuItem.Size = new Size(116, 22);
      tracerToolStripMenuItem.Text = "Tracer";
      tracerToolStripMenuItem.Click += tracerToolStripMenuItem_Click;
      // 
      // layoutToolStripMenuItem
      // 
      layoutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { layout1ToolStripMenuItem });
      layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
      layoutToolStripMenuItem.Size = new Size(116, 22);
      layoutToolStripMenuItem.Text = "Layout";
      // 
      // layout1ToolStripMenuItem
      // 
      layout1ToolStripMenuItem.Name = "layout1ToolStripMenuItem";
      layout1ToolStripMenuItem.Size = new Size(119, 22);
      layout1ToolStripMenuItem.Text = "Layout 1";
      layout1ToolStripMenuItem.Click += layout1ToolStripMenuItem_Click;
      // 
      // settingsToolStripMenuItem
      // 
      settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { usersToolStripMenuItem, optionsToolStripMenuItem, setFocusOnMouseEnterToolStripMenuItem, highlightStepsOnExecutionToolStripMenuItem });
      settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
      settingsToolStripMenuItem.Size = new Size(61, 20);
      settingsToolStripMenuItem.Text = "S&ettings";
      // 
      // usersToolStripMenuItem
      // 
      usersToolStripMenuItem.Name = "usersToolStripMenuItem";
      usersToolStripMenuItem.Size = new Size(226, 22);
      usersToolStripMenuItem.Text = "&Users";
      // 
      // optionsToolStripMenuItem
      // 
      optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      optionsToolStripMenuItem.Size = new Size(226, 22);
      optionsToolStripMenuItem.Text = "&Options";
      // 
      // setFocusOnMouseEnterToolStripMenuItem
      // 
      setFocusOnMouseEnterToolStripMenuItem.Checked = true;
      setFocusOnMouseEnterToolStripMenuItem.CheckState = CheckState.Checked;
      setFocusOnMouseEnterToolStripMenuItem.Name = "setFocusOnMouseEnterToolStripMenuItem";
      setFocusOnMouseEnterToolStripMenuItem.Size = new Size(226, 22);
      setFocusOnMouseEnterToolStripMenuItem.Text = "Set Focus on Mouse enter";
      setFocusOnMouseEnterToolStripMenuItem.Click += setFocusOnMouseEnterToolStripMenuItem_Click;
      // 
      // highlightStepsOnExecutionToolStripMenuItem
      // 
      highlightStepsOnExecutionToolStripMenuItem.Name = "highlightStepsOnExecutionToolStripMenuItem";
      highlightStepsOnExecutionToolStripMenuItem.Size = new Size(226, 22);
      highlightStepsOnExecutionToolStripMenuItem.Text = "Highlight steps on execution";
      highlightStepsOnExecutionToolStripMenuItem.Click += highlightStepsOnExecutionToolStripMenuItem_Click;
      // 
      // serverToolStripMenuItem
      // 
      serverToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { connectToolStripMenuItem, connectRecentToolStripMenuItem, administrationToolStripMenuItem });
      serverToolStripMenuItem.Name = "serverToolStripMenuItem";
      serverToolStripMenuItem.Size = new Size(51, 20);
      serverToolStripMenuItem.Text = "&Server";
      // 
      // connectToolStripMenuItem
      // 
      connectToolStripMenuItem.Name = "connectToolStripMenuItem";
      connectToolStripMenuItem.Size = new Size(180, 22);
      connectToolStripMenuItem.Text = "&Connect";
      connectToolStripMenuItem.Click += connectToolStripMenuItem_Click;
      // 
      // connectRecentToolStripMenuItem
      // 
      connectRecentToolStripMenuItem.Name = "connectRecentToolStripMenuItem";
      connectRecentToolStripMenuItem.Size = new Size(180, 22);
      connectRecentToolStripMenuItem.Text = "Connect &Recent...";
      // 
      // administrationToolStripMenuItem
      // 
      administrationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { usersToolStripMenuItem1 });
      administrationToolStripMenuItem.Name = "administrationToolStripMenuItem";
      administrationToolStripMenuItem.Size = new Size(180, 22);
      administrationToolStripMenuItem.Text = "&Administration";
      // 
      // usersToolStripMenuItem1
      // 
      usersToolStripMenuItem1.Name = "usersToolStripMenuItem1";
      usersToolStripMenuItem1.Size = new Size(180, 22);
      usersToolStripMenuItem1.Text = "&Users...";
      usersToolStripMenuItem1.Click += usersToolStripMenuItem1_Click;
      // 
      // statusStrip1
      // 
      statusStrip1.Location = new Point(0, 458);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new Size(947, 22);
      statusStrip1.TabIndex = 1;
      statusStrip1.Text = "statusStrip1";
      // 
      // frmMain
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(947, 480);
      Controls.Add(statusStrip1);
      Controls.Add(menuStrip1);
      MainMenuStrip = menuStrip1;
      Name = "frmMain";
      Text = "BAJL, LLC - Flow Designer";
      FormClosing += frmMain_FormClosing;
      Load += frmMain_Load;
      ResizeEnd += frmMain_ResizeEnd;
      MouseEnter += frmMain_MouseEnter;
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem tsmFile;
    private StatusStrip statusStrip1;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem toolboxToolStripMenuItem;
    private ToolStripMenuItem newToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem layoutToolStripMenuItem;
    private ToolStripMenuItem layout1ToolStripMenuItem;
    private ToolStripMenuItem settingsToolStripMenuItem;
    private ToolStripMenuItem usersToolStripMenuItem;
    private ToolStripMenuItem optionsToolStripMenuItem;
    private ToolStripMenuItem tracerToolStripMenuItem;
    private ToolStripMenuItem setFocusOnMouseEnterToolStripMenuItem;
    private ToolStripMenuItem highlightStepsOnExecutionToolStripMenuItem;
    private ToolStripMenuItem serverToolStripMenuItem;
    private ToolStripMenuItem connectToolStripMenuItem;
    private ToolStripMenuItem connectRecentToolStripMenuItem;
    private ToolStripMenuItem administrationToolStripMenuItem;
    private ToolStripMenuItem usersToolStripMenuItem1;
  }
}