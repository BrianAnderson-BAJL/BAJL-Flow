namespace FlowEngineDesigner
{
  partial class frmTracer
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTracer));
      lvTracer = new ListView();
      chItem = new ColumnHeader();
      chData = new ColumnHeader();
      chTime = new ColumnHeader();
      toolStrip1 = new ToolStrip();
      tsbClear = new ToolStripButton();
      toolStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // lvTracer
      // 
      lvTracer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lvTracer.Columns.AddRange(new ColumnHeader[] { chItem, chData, chTime });
      lvTracer.FullRowSelect = true;
      lvTracer.GridLines = true;
      lvTracer.Location = new Point(0, 28);
      lvTracer.Name = "lvTracer";
      lvTracer.Size = new Size(938, 422);
      lvTracer.TabIndex = 0;
      lvTracer.UseCompatibleStateImageBehavior = false;
      lvTracer.View = View.Details;
      // 
      // chItem
      // 
      chItem.Text = "Item";
      chItem.Width = 180;
      // 
      // chData
      // 
      chData.Text = "Data";
      chData.Width = 600;
      // 
      // chTime
      // 
      chTime.Text = "Time";
      chTime.Width = 120;
      // 
      // toolStrip1
      // 
      toolStrip1.Items.AddRange(new ToolStripItem[] { tsbClear });
      toolStrip1.Location = new Point(0, 0);
      toolStrip1.Name = "toolStrip1";
      toolStrip1.Size = new Size(938, 25);
      toolStrip1.TabIndex = 1;
      toolStrip1.Text = "toolStrip1";
      // 
      // tsbClear
      // 
      tsbClear.DisplayStyle = ToolStripItemDisplayStyle.Text;
      tsbClear.Image = (Image)resources.GetObject("tsbClear.Image");
      tsbClear.ImageTransparentColor = Color.Magenta;
      tsbClear.Name = "tsbClear";
      tsbClear.Size = new Size(38, 22);
      tsbClear.Text = "Clear";
      tsbClear.ToolTipText = "Clear";
      tsbClear.Click += tsbClear_Click;
      // 
      // frmTracer
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(938, 450);
      Controls.Add(toolStrip1);
      Controls.Add(lvTracer);
      Name = "frmTracer";
      Text = "Tracer";
      Load += frmTracer_Load;
      toolStrip1.ResumeLayout(false);
      toolStrip1.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private ListView lvTracer;
    private ColumnHeader chItem;
    private ColumnHeader chData;
    private ToolStrip toolStrip1;
    private ToolStripButton tsbClear;
    private ColumnHeader chTime;
  }
}