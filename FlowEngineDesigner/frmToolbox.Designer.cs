namespace FlowEngineDesigner
{
  partial class frmToolbox
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
      components = new System.ComponentModel.Container();
      tvToolbox = new TreeView();
      cmsPopup = new ContextMenuStrip(components);
      settingsToolStripMenuItem = new ToolStripMenuItem();
      cmsPopup.SuspendLayout();
      SuspendLayout();
      // 
      // tvToolbox
      // 
      tvToolbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tvToolbox.Location = new Point(8, 9);
      tvToolbox.Name = "tvToolbox";
      tvToolbox.Size = new Size(338, 764);
      tvToolbox.TabIndex = 0;
      tvToolbox.NodeMouseClick += tvToolbox_NodeMouseClick;
      tvToolbox.MouseDown += tvToolbox_MouseDown;
      // 
      // cmsPopup
      // 
      cmsPopup.Items.AddRange(new ToolStripItem[] { settingsToolStripMenuItem });
      cmsPopup.Name = "cmsPopup";
      cmsPopup.Size = new Size(117, 26);
      // 
      // settingsToolStripMenuItem
      // 
      settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
      settingsToolStripMenuItem.Size = new Size(116, 22);
      settingsToolStripMenuItem.Text = "&Settings";
      settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
      // 
      // frmToolbox
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(352, 778);
      Controls.Add(tvToolbox);
      MdiChildrenMinimizedAnchorBottom = false;
      Name = "frmToolbox";
      Text = "Toolbox";
      Load += frmToolbox_Load;
      cmsPopup.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    private TreeView tvToolbox;
    private ContextMenuStrip cmsPopup;
    private ToolStripMenuItem settingsToolStripMenuItem;
  }
}