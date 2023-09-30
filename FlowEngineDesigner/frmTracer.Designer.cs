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
      columnHeader1 = new ColumnHeader();
      columnHeader2 = new ColumnHeader();
      toolStrip1 = new ToolStrip();
      tsbClear = new ToolStripButton();
      toolStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // lvTracer
      // 
      lvTracer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lvTracer.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
      lvTracer.FullRowSelect = true;
      lvTracer.GridLines = true;
      lvTracer.Location = new Point(0, 28);
      lvTracer.Name = "lvTracer";
      lvTracer.Size = new Size(800, 422);
      lvTracer.TabIndex = 0;
      lvTracer.UseCompatibleStateImageBehavior = false;
      lvTracer.View = View.Details;
      // 
      // columnHeader1
      // 
      columnHeader1.Text = "Item";
      columnHeader1.Width = 180;
      // 
      // columnHeader2
      // 
      columnHeader2.Text = "Data";
      columnHeader2.Width = 600;
      // 
      // toolStrip1
      // 
      toolStrip1.Items.AddRange(new ToolStripItem[] { tsbClear });
      toolStrip1.Location = new Point(0, 0);
      toolStrip1.Name = "toolStrip1";
      toolStrip1.Size = new Size(800, 25);
      toolStrip1.TabIndex = 1;
      toolStrip1.Text = "toolStrip1";
      toolStrip1.MouseEnter += toolStrip1_MouseEnter;
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
      ClientSize = new Size(800, 450);
      Controls.Add(toolStrip1);
      Controls.Add(lvTracer);
      Name = "frmTracer";
      Text = "Tracer";
      Load += frmTracer_Load;
      MouseEnter += frmTracer_MouseEnter;
      toolStrip1.ResumeLayout(false);
      toolStrip1.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private ListView lvTracer;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ToolStrip toolStrip1;
    private ToolStripButton tsbClear;
  }
}