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
      optionsToolStripMenuItem = new ToolStripMenuItem();
      serverToolStripMenuItem = new ToolStripMenuItem();
      connectToolStripMenuItem = new ToolStripMenuItem();
      disconnectToolStripMenuItem = new ToolStripMenuItem();
      connectRecentToolStripMenuItem = new ToolStripMenuItem();
      statisticsToolStripMenuItem = new ToolStripMenuItem();
      settingsToolStripMenuItem1 = new ToolStripMenuItem();
      administrationToolStripMenuItem = new ToolStripMenuItem();
      usersToolStripMenuItem1 = new ToolStripMenuItem();
      securityProfilesToolStripMenuItem = new ToolStripMenuItem();
      changePasswordToolStripMenuItem = new ToolStripMenuItem();
      flowOpenToolStripMenuItem = new ToolStripMenuItem();
      debugAlwaysToolStripMenuItem = new ToolStripMenuItem();
      helpToolStripMenuItem = new ToolStripMenuItem();
      aboutToolStripMenuItem = new ToolStripMenuItem();
      statusStrip1 = new StatusStrip();
      tsServer = new ToolStripStatusLabel();
      toolStripStatusLabel1 = new ToolStripStatusLabel();
      tsLoggedInAs = new ToolStripStatusLabel();
      openFileDialog1 = new OpenFileDialog();
      menuStrip1.SuspendLayout();
      statusStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // menuStrip1
      // 
      menuStrip1.Items.AddRange(new ToolStripItem[] { tsmFile, viewToolStripMenuItem, settingsToolStripMenuItem, serverToolStripMenuItem, helpToolStripMenuItem });
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
      openToolStripMenuItem.Click += openToolStripMenuItem_Click;
      // 
      // exitToolStripMenuItem
      // 
      exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      exitToolStripMenuItem.Size = new Size(103, 22);
      exitToolStripMenuItem.Text = "E&xit";
      exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
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
      settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { optionsToolStripMenuItem });
      settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
      settingsToolStripMenuItem.Size = new Size(61, 20);
      settingsToolStripMenuItem.Text = "S&ettings";
      // 
      // optionsToolStripMenuItem
      // 
      optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      optionsToolStripMenuItem.Size = new Size(116, 22);
      optionsToolStripMenuItem.Text = "&Options";
      optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
      // 
      // serverToolStripMenuItem
      // 
      serverToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { connectToolStripMenuItem, disconnectToolStripMenuItem, connectRecentToolStripMenuItem, statisticsToolStripMenuItem, settingsToolStripMenuItem1, administrationToolStripMenuItem });
      serverToolStripMenuItem.Name = "serverToolStripMenuItem";
      serverToolStripMenuItem.Size = new Size(51, 20);
      serverToolStripMenuItem.Text = "&Server";
      serverToolStripMenuItem.DropDownOpening += serverToolStripMenuItem_DropDownOpening;
      serverToolStripMenuItem.Click += serverToolStripMenuItem_Click;
      // 
      // connectToolStripMenuItem
      // 
      connectToolStripMenuItem.Name = "connectToolStripMenuItem";
      connectToolStripMenuItem.Size = new Size(180, 22);
      connectToolStripMenuItem.Text = "&Connect";
      connectToolStripMenuItem.Click += connectToolStripMenuItem_Click;
      // 
      // disconnectToolStripMenuItem
      // 
      disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
      disconnectToolStripMenuItem.Size = new Size(180, 22);
      disconnectToolStripMenuItem.Text = "Disconnect";
      disconnectToolStripMenuItem.Click += disconnectToolStripMenuItem_Click;
      // 
      // connectRecentToolStripMenuItem
      // 
      connectRecentToolStripMenuItem.Name = "connectRecentToolStripMenuItem";
      connectRecentToolStripMenuItem.Size = new Size(180, 22);
      connectRecentToolStripMenuItem.Text = "Connect &Recent...";
      connectRecentToolStripMenuItem.Click += connectRecentToolStripMenuItem_Click;
      // 
      // statisticsToolStripMenuItem
      // 
      statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
      statisticsToolStripMenuItem.Size = new Size(180, 22);
      statisticsToolStripMenuItem.Text = "S&tatistics";
      statisticsToolStripMenuItem.Click += statisticsToolStripMenuItem_Click;
      // 
      // settingsToolStripMenuItem1
      // 
      settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
      settingsToolStripMenuItem1.Size = new Size(180, 22);
      settingsToolStripMenuItem1.Text = "&Settings";
      settingsToolStripMenuItem1.Click += settingsToolStripMenuItem1_Click;
      // 
      // administrationToolStripMenuItem
      // 
      administrationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { usersToolStripMenuItem1, securityProfilesToolStripMenuItem, changePasswordToolStripMenuItem, flowOpenToolStripMenuItem, debugAlwaysToolStripMenuItem });
      administrationToolStripMenuItem.Name = "administrationToolStripMenuItem";
      administrationToolStripMenuItem.Size = new Size(180, 22);
      administrationToolStripMenuItem.Text = "&Administration";
      // 
      // usersToolStripMenuItem1
      // 
      usersToolStripMenuItem1.Name = "usersToolStripMenuItem1";
      usersToolStripMenuItem1.Size = new Size(168, 22);
      usersToolStripMenuItem1.Text = "&Users...";
      usersToolStripMenuItem1.Click += usersToolStripMenuItem1_Click;
      // 
      // securityProfilesToolStripMenuItem
      // 
      securityProfilesToolStripMenuItem.Name = "securityProfilesToolStripMenuItem";
      securityProfilesToolStripMenuItem.Size = new Size(168, 22);
      securityProfilesToolStripMenuItem.Text = "Security Profiles...";
      securityProfilesToolStripMenuItem.Click += securityProfilesToolStripMenuItem_Click;
      // 
      // changePasswordToolStripMenuItem
      // 
      changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
      changePasswordToolStripMenuItem.Size = new Size(168, 22);
      changePasswordToolStripMenuItem.Text = "Change &Password";
      changePasswordToolStripMenuItem.Click += changePasswordToolStripMenuItem_Click;
      // 
      // flowOpenToolStripMenuItem
      // 
      flowOpenToolStripMenuItem.Name = "flowOpenToolStripMenuItem";
      flowOpenToolStripMenuItem.Size = new Size(168, 22);
      flowOpenToolStripMenuItem.Text = "Flow Open...";
      flowOpenToolStripMenuItem.Click += flowOpenToolStripMenuItem_Click;
      // 
      // debugAlwaysToolStripMenuItem
      // 
      debugAlwaysToolStripMenuItem.Name = "debugAlwaysToolStripMenuItem";
      debugAlwaysToolStripMenuItem.Size = new Size(168, 22);
      debugAlwaysToolStripMenuItem.Text = "Debug Always";
      debugAlwaysToolStripMenuItem.CheckStateChanged += debugAlwaysToolStripMenuItem_CheckStateChanged;
      debugAlwaysToolStripMenuItem.Click += debugAlwaysToolStripMenuItem_Click;
      // 
      // helpToolStripMenuItem
      // 
      helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
      helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      helpToolStripMenuItem.Size = new Size(44, 20);
      helpToolStripMenuItem.Text = "&Help";
      // 
      // aboutToolStripMenuItem
      // 
      aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      aboutToolStripMenuItem.Size = new Size(107, 22);
      aboutToolStripMenuItem.Text = "&About";
      aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
      // 
      // statusStrip1
      // 
      statusStrip1.Items.AddRange(new ToolStripItem[] { tsServer, toolStripStatusLabel1, tsLoggedInAs });
      statusStrip1.Location = new Point(0, 458);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new Size(947, 22);
      statusStrip1.TabIndex = 1;
      statusStrip1.Text = "statusStrip1";
      // 
      // tsServer
      // 
      tsServer.Name = "tsServer";
      tsServer.Size = new Size(79, 17);
      tsServer.Text = "Disconnected";
      // 
      // toolStripStatusLabel1
      // 
      toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      toolStripStatusLabel1.Size = new Size(13, 17);
      toolStripStatusLabel1.Text = "  ";
      // 
      // tsLoggedInAs
      // 
      tsLoggedInAs.DoubleClickEnabled = true;
      tsLoggedInAs.Name = "tsLoggedInAs";
      tsLoggedInAs.Size = new Size(101, 17);
      tsLoggedInAs.Text = "[NOT LOGGED IN]";
      tsLoggedInAs.DoubleClick += tsLoggedInAs_DoubleClick;
      // 
      // openFileDialog1
      // 
      openFileDialog1.FileName = "openFileDialog1";
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
      Activated += frmMain_Activated;
      FormClosing += frmMain_FormClosing;
      Load += frmMain_Load;
      ResizeEnd += frmMain_ResizeEnd;
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem tsmFile;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem toolboxToolStripMenuItem;
    private ToolStripMenuItem newToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem layoutToolStripMenuItem;
    private ToolStripMenuItem layout1ToolStripMenuItem;
    private ToolStripMenuItem settingsToolStripMenuItem;
    private ToolStripMenuItem optionsToolStripMenuItem;
    private ToolStripMenuItem tracerToolStripMenuItem;
    private ToolStripMenuItem serverToolStripMenuItem;
    private ToolStripMenuItem connectToolStripMenuItem;
    private ToolStripMenuItem connectRecentToolStripMenuItem;
    private ToolStripMenuItem administrationToolStripMenuItem;
    private ToolStripMenuItem usersToolStripMenuItem1;
    private ToolStripMenuItem changePasswordToolStripMenuItem;
    private ToolStripMenuItem securityProfilesToolStripMenuItem;
    private ToolStripMenuItem disconnectToolStripMenuItem;
    public StatusStrip statusStrip1;
    public ToolStripStatusLabel tsServer;
    private ToolStripStatusLabel toolStripStatusLabel1;
    public ToolStripStatusLabel tsLoggedInAs;
    private ToolStripMenuItem flowOpenToolStripMenuItem;
    private OpenFileDialog openFileDialog1;
    public ToolStripMenuItem debugAlwaysToolStripMenuItem;
    private ToolStripMenuItem settingsToolStripMenuItem1;
    private ToolStripMenuItem statisticsToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem aboutToolStripMenuItem;
  }
}