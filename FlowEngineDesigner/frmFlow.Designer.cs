﻿namespace FlowEngineDesigner
{
  partial class frmFlow
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFlow));
      pictureBox1 = new PictureBox();
      menuStrip1 = new MenuStrip();
      fileToolStripMenuItem = new ToolStripMenuItem();
      openToolStripMenuItem = new ToolStripMenuItem();
      toolStripMenuItem1 = new ToolStripSeparator();
      saveToolStripMenuItem = new ToolStripMenuItem();
      saveAsToolStripMenuItem = new ToolStripMenuItem();
      toolStripMenuItem2 = new ToolStripSeparator();
      closeToolStripMenuItem = new ToolStripMenuItem();
      flowToolStripMenuItem = new ToolStripMenuItem();
      propertiesToolStripMenuItem = new ToolStripMenuItem();
      saveFileDialog1 = new SaveFileDialog();
      openFileDialog1 = new OpenFileDialog();
      toolStrip1 = new ToolStrip();
      tsbPlay = new ToolStripButton();
      statusStrip1 = new StatusStrip();
      tslZoom = new ToolStripStatusLabel();
      toolStripDropDownButton1 = new ToolStripDropDownButton();
      tsbZoomOut = new ToolStripDropDownButton();
      toolStripSeparator1 = new ToolStripSeparator();
      tsbComment = new ToolStripButton();
      ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
      menuStrip1.SuspendLayout();
      toolStrip1.SuspendLayout();
      statusStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // pictureBox1
      // 
      pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      pictureBox1.BackColor = Color.White;
      pictureBox1.Location = new Point(2, 52);
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Size = new Size(797, 385);
      pictureBox1.TabIndex = 0;
      pictureBox1.TabStop = false;
      pictureBox1.Click += pictureBox1_Click;
      pictureBox1.DragDrop += pictureBox1_DragDrop;
      pictureBox1.Paint += pictureBox1_Paint;
      pictureBox1.DoubleClick += pictureBox1_DoubleClick;
      pictureBox1.MouseDown += pictureBox1_MouseDown;
      pictureBox1.MouseEnter += pictureBox1_MouseEnter;
      pictureBox1.MouseMove += pictureBox1_MouseMove;
      pictureBox1.MouseUp += pictureBox1_MouseUp;
      // 
      // menuStrip1
      // 
      menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, flowToolStripMenuItem });
      menuStrip1.Location = new Point(0, 0);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.Size = new Size(800, 24);
      menuStrip1.TabIndex = 1;
      menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, toolStripMenuItem1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripMenuItem2, closeToolStripMenuItem });
      fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      fileToolStripMenuItem.Size = new Size(37, 20);
      fileToolStripMenuItem.Text = "&File";
      // 
      // openToolStripMenuItem
      // 
      openToolStripMenuItem.Name = "openToolStripMenuItem";
      openToolStripMenuItem.Size = new Size(180, 22);
      openToolStripMenuItem.Text = "&Open";
      openToolStripMenuItem.Click += openToolStripMenuItem_Click;
      // 
      // toolStripMenuItem1
      // 
      toolStripMenuItem1.Name = "toolStripMenuItem1";
      toolStripMenuItem1.Size = new Size(177, 6);
      // 
      // saveToolStripMenuItem
      // 
      saveToolStripMenuItem.Name = "saveToolStripMenuItem";
      saveToolStripMenuItem.Size = new Size(180, 22);
      saveToolStripMenuItem.Text = "&Save";
      saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
      // 
      // saveAsToolStripMenuItem
      // 
      saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
      saveAsToolStripMenuItem.Size = new Size(180, 22);
      saveAsToolStripMenuItem.Text = "Save &As";
      saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
      // 
      // toolStripMenuItem2
      // 
      toolStripMenuItem2.Name = "toolStripMenuItem2";
      toolStripMenuItem2.Size = new Size(177, 6);
      // 
      // closeToolStripMenuItem
      // 
      closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      closeToolStripMenuItem.Size = new Size(180, 22);
      closeToolStripMenuItem.Text = "&Close";
      closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
      // 
      // flowToolStripMenuItem
      // 
      flowToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { propertiesToolStripMenuItem });
      flowToolStripMenuItem.Name = "flowToolStripMenuItem";
      flowToolStripMenuItem.Size = new Size(44, 20);
      flowToolStripMenuItem.Text = "Flow";
      // 
      // propertiesToolStripMenuItem
      // 
      propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
      propertiesToolStripMenuItem.Size = new Size(136, 22);
      propertiesToolStripMenuItem.Text = "Properties...";
      propertiesToolStripMenuItem.Click += propertiesToolStripMenuItem_Click;
      // 
      // openFileDialog1
      // 
      openFileDialog1.FileName = "openFileDialog1";
      // 
      // toolStrip1
      // 
      toolStrip1.Items.AddRange(new ToolStripItem[] { tsbPlay, toolStripSeparator1, tsbComment });
      toolStrip1.Location = new Point(0, 24);
      toolStrip1.Name = "toolStrip1";
      toolStrip1.Size = new Size(800, 25);
      toolStrip1.TabIndex = 2;
      toolStrip1.Text = "toolStrip1";
      toolStrip1.MouseEnter += toolStrip1_MouseEnter;
      // 
      // tsbPlay
      // 
      tsbPlay.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbPlay.Image = (Image)resources.GetObject("tsbPlay.Image");
      tsbPlay.ImageTransparentColor = Color.Magenta;
      tsbPlay.Name = "tsbPlay";
      tsbPlay.Size = new Size(23, 22);
      tsbPlay.Text = "Play";
      tsbPlay.Click += tsbPlay_Click;
      // 
      // statusStrip1
      // 
      statusStrip1.Items.AddRange(new ToolStripItem[] { tslZoom, toolStripDropDownButton1, tsbZoomOut });
      statusStrip1.Location = new Point(0, 440);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new Size(800, 22);
      statusStrip1.TabIndex = 3;
      statusStrip1.Text = "statusStrip1";
      // 
      // tslZoom
      // 
      tslZoom.Name = "tslZoom";
      tslZoom.Size = new Size(35, 17);
      tslZoom.Text = "100%";
      tslZoom.Click += tslZoom_Click;
      // 
      // toolStripDropDownButton1
      // 
      toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
      toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
      toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
      toolStripDropDownButton1.Name = "toolStripDropDownButton1";
      toolStripDropDownButton1.ShowDropDownArrow = false;
      toolStripDropDownButton1.Size = new Size(19, 20);
      toolStripDropDownButton1.Text = "+";
      toolStripDropDownButton1.ToolTipText = "Zoom in";
      toolStripDropDownButton1.Click += toolStripDropDownButton1_Click;
      // 
      // tsbZoomOut
      // 
      tsbZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Text;
      tsbZoomOut.Image = (Image)resources.GetObject("tsbZoomOut.Image");
      tsbZoomOut.ImageTransparentColor = Color.Magenta;
      tsbZoomOut.Name = "tsbZoomOut";
      tsbZoomOut.ShowDropDownArrow = false;
      tsbZoomOut.Size = new Size(16, 20);
      tsbZoomOut.Text = "-";
      tsbZoomOut.ToolTipText = "Zoom out";
      tsbZoomOut.Click += tsbZoomOut_Click;
      // 
      // toolStripSeparator1
      // 
      toolStripSeparator1.Name = "toolStripSeparator1";
      toolStripSeparator1.Size = new Size(6, 25);
      // 
      // tsbComment
      // 
      tsbComment.DisplayStyle = ToolStripItemDisplayStyle.Image;
      tsbComment.Image = (Image)resources.GetObject("tsbComment.Image");
      tsbComment.ImageTransparentColor = Color.Magenta;
      tsbComment.Name = "tsbComment";
      tsbComment.Size = new Size(23, 22);
      tsbComment.Text = "Comment";
      tsbComment.Click += TsbComment_Click;
      // 
      // frmFlow
      // 
      AllowDrop = true;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 462);
      Controls.Add(statusStrip1);
      Controls.Add(toolStrip1);
      Controls.Add(pictureBox1);
      Controls.Add(menuStrip1);
      MainMenuStrip = menuStrip1;
      Name = "frmFlow";
      Text = "Flow - [New]";
      Load += frmFlow_Load;
      DragDrop += frmFlow_DragDrop;
      DragEnter += frmFlow_DragEnter;
      KeyDown += frmFlow_KeyDown;
      KeyPress += frmFlow_KeyPress;
      KeyUp += frmFlow_KeyUp;
      MouseEnter += frmFlow_MouseEnter;
      Resize += frmFlow_Resize;
      ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      toolStrip1.ResumeLayout(false);
      toolStrip1.PerformLayout();
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }


    #endregion

    public PictureBox pictureBox1;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private ToolStripMenuItem closeToolStripMenuItem;
    private SaveFileDialog saveFileDialog1;
    private OpenFileDialog openFileDialog1;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem1;
    private ToolStripSeparator toolStripMenuItem2;
    private ToolStrip toolStrip1;
    private ToolStripButton tsbPlay;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel tslZoom;
    private ToolStripDropDownButton toolStripDropDownButton1;
    private ToolStripDropDownButton tsbZoomOut;
    private ToolStripMenuItem flowToolStripMenuItem;
    private ToolStripMenuItem propertiesToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton tsbComment;
  }
}