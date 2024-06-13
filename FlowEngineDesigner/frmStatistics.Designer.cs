namespace FlowEngineDesigner
{
  partial class frmStatistics
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
      statusStrip1 = new StatusStrip();
      tslStatus = new ToolStripStatusLabel();
      label1 = new Label();
      cmbFrequency = new ComboBox();
      label2 = new Label();
      splitContainer1 = new SplitContainer();
      splitContainer2 = new SplitContainer();
      plotFlowsPie = new ScottPlot.WinForms.FormsPlot();
      plotFlowsPerSecond = new ScottPlot.WinForms.FormsPlot();
      lvFlows = new ListView();
      chFlowName = new ColumnHeader();
      chMin = new ColumnHeader();
      chAvg = new ColumnHeader();
      chMax = new ColumnHeader();
      cmbTimePeriod = new ComboBox();
      chCount = new ColumnHeader();
      statusStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
      splitContainer2.Panel1.SuspendLayout();
      splitContainer2.Panel2.SuspendLayout();
      splitContainer2.SuspendLayout();
      SuspendLayout();
      // 
      // statusStrip1
      // 
      statusStrip1.Items.AddRange(new ToolStripItem[] { tslStatus });
      statusStrip1.Location = new Point(0, 621);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new Size(1117, 22);
      statusStrip1.TabIndex = 1;
      statusStrip1.Text = "statusStrip1";
      // 
      // tslStatus
      // 
      tslStatus.Name = "tslStatus";
      tslStatus.Size = new Size(118, 17);
      tslStatus.Text = "toolStripStatusLabel1";
      tslStatus.Click += tslStatus_Click;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(372, 9);
      label1.Name = "label1";
      label1.Size = new Size(73, 15);
      label1.TabIndex = 2;
      label1.Text = "Time period:";
      // 
      // cmbFrequency
      // 
      cmbFrequency.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbFrequency.FormattingEnabled = true;
      cmbFrequency.Location = new Point(123, 6);
      cmbFrequency.Name = "cmbFrequency";
      cmbFrequency.Size = new Size(167, 23);
      cmbFrequency.TabIndex = 4;
      cmbFrequency.SelectedIndexChanged += cmbFrequency_SelectedIndexChanged;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(13, 9);
      label2.Name = "label2";
      label2.Size = new Size(104, 15);
      label2.TabIndex = 5;
      label2.Text = "Update frequency:";
      // 
      // splitContainer1
      // 
      splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      splitContainer1.BorderStyle = BorderStyle.FixedSingle;
      splitContainer1.Location = new Point(12, 43);
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(splitContainer2);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(lvFlows);
      splitContainer1.Size = new Size(1093, 575);
      splitContainer1.SplitterDistance = 527;
      splitContainer1.TabIndex = 6;
      // 
      // splitContainer2
      // 
      splitContainer2.BorderStyle = BorderStyle.FixedSingle;
      splitContainer2.Dock = DockStyle.Fill;
      splitContainer2.Location = new Point(0, 0);
      splitContainer2.Name = "splitContainer2";
      splitContainer2.Orientation = Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      splitContainer2.Panel1.Controls.Add(plotFlowsPie);
      // 
      // splitContainer2.Panel2
      // 
      splitContainer2.Panel2.Controls.Add(plotFlowsPerSecond);
      splitContainer2.Size = new Size(527, 575);
      splitContainer2.SplitterDistance = 296;
      splitContainer2.TabIndex = 0;
      // 
      // plotFlowsPie
      // 
      plotFlowsPie.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      plotFlowsPie.DisplayScale = 1F;
      plotFlowsPie.Location = new Point(3, 3);
      plotFlowsPie.Name = "plotFlowsPie";
      plotFlowsPie.Size = new Size(519, 288);
      plotFlowsPie.TabIndex = 0;
      // 
      // plotFlowsPerSecond
      // 
      plotFlowsPerSecond.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      plotFlowsPerSecond.DisplayScale = 1F;
      plotFlowsPerSecond.Location = new Point(3, 6);
      plotFlowsPerSecond.Name = "plotFlowsPerSecond";
      plotFlowsPerSecond.Size = new Size(519, 267);
      plotFlowsPerSecond.TabIndex = 1;
      // 
      // lvFlows
      // 
      lvFlows.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lvFlows.Columns.AddRange(new ColumnHeader[] { chFlowName, chCount, chMin, chAvg, chMax });
      lvFlows.FullRowSelect = true;
      lvFlows.GridLines = true;
      lvFlows.Location = new Point(3, 3);
      lvFlows.MultiSelect = false;
      lvFlows.Name = "lvFlows";
      lvFlows.Size = new Size(554, 567);
      lvFlows.TabIndex = 0;
      lvFlows.UseCompatibleStateImageBehavior = false;
      lvFlows.View = View.Details;
      // 
      // chFlowName
      // 
      chFlowName.Text = "Flow Name";
      chFlowName.Width = 180;
      // 
      // chMin
      // 
      chMin.Text = "Min";
      chMin.Width = 80;
      // 
      // chAvg
      // 
      chAvg.Text = "Avg";
      chAvg.Width = 80;
      // 
      // chMax
      // 
      chMax.Text = "Max";
      chMax.Width = 80;
      // 
      // cmbTimePeriod
      // 
      cmbTimePeriod.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbTimePeriod.FormattingEnabled = true;
      cmbTimePeriod.Location = new Point(456, 6);
      cmbTimePeriod.Name = "cmbTimePeriod";
      cmbTimePeriod.Size = new Size(263, 23);
      cmbTimePeriod.TabIndex = 7;
      cmbTimePeriod.SelectedIndexChanged += cmbTimePeriod_SelectedIndexChanged;
      // 
      // chCount
      // 
      chCount.Text = "Count";
      chCount.Width = 80;
      // 
      // frmStatistics
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1117, 643);
      Controls.Add(cmbTimePeriod);
      Controls.Add(splitContainer1);
      Controls.Add(label2);
      Controls.Add(cmbFrequency);
      Controls.Add(label1);
      Controls.Add(statusStrip1);
      Name = "frmStatistics";
      Text = "Statistics";
      FormClosing += frmStatistics_FormClosing;
      Load += frmStatistics_Load;
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      splitContainer2.Panel1.ResumeLayout(false);
      splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
      splitContainer2.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel tslStatus;
    private Label label1;
    private ComboBox cmbFrequency;
    private Label label2;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private ScottPlot.WinForms.FormsPlot plotFlowsPie;
    private ScottPlot.WinForms.FormsPlot plotFlowsPerSecond;
    private ListView lvFlows;
    private ColumnHeader chFlowName;
    private ColumnHeader chMin;
    private ColumnHeader chAvg;
    private ColumnHeader chMax;
    private ComboBox cmbTimePeriod;
    private ColumnHeader chCount;
  }
}