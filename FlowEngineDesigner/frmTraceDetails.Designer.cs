namespace FlowEngineDesigner
{
  partial class frmTraceDetails
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
      txtXml = new TextBox();
      label1 = new Label();
      txtFileName = new TextBox();
      label2 = new Label();
      lblSuccess = new Label();
      txtError = new TextBox();
      txtStepName = new TextBox();
      label3 = new Label();
      btnShowFlow = new Button();
      btnHighlightStep = new Button();
      lvVariables = new ListView();
      chName = new ColumnHeader();
      chValue = new ColumnHeader();
      contextMenuStrip1 = new ContextMenuStrip(components);
      copyToolStripMenuItem = new ToolStripMenuItem();
      tvVariables = new TreeView();
      contextMenuStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // txtXml
      // 
      txtXml.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      txtXml.Location = new Point(12, 395);
      txtXml.Multiline = true;
      txtXml.Name = "txtXml";
      txtXml.ScrollBars = ScrollBars.Vertical;
      txtXml.Size = new Size(776, 127);
      txtXml.TabIndex = 0;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(27, 24);
      label1.Name = "label1";
      label1.Size = new Size(61, 15);
      label1.TabIndex = 1;
      label1.Text = "File name:";
      // 
      // txtFileName
      // 
      txtFileName.Location = new Point(94, 21);
      txtFileName.Name = "txtFileName";
      txtFileName.ReadOnly = true;
      txtFileName.Size = new Size(284, 23);
      txtFileName.TabIndex = 2;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(23, 87);
      label2.Name = "label2";
      label2.Size = new Size(65, 15);
      label2.TabIndex = 3;
      label2.Text = "Step result:";
      // 
      // lblSuccess
      // 
      lblSuccess.AutoSize = true;
      lblSuccess.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
      lblSuccess.Location = new Point(95, 81);
      lblSuccess.Name = "lblSuccess";
      lblSuccess.Size = new Size(57, 21);
      lblSuccess.TabIndex = 4;
      lblSuccess.Text = "label3";
      // 
      // txtError
      // 
      txtError.Location = new Point(187, 79);
      txtError.Name = "txtError";
      txtError.ReadOnly = true;
      txtError.Size = new Size(489, 23);
      txtError.TabIndex = 5;
      // 
      // txtStepName
      // 
      txtStepName.Location = new Point(94, 50);
      txtStepName.Name = "txtStepName";
      txtStepName.ReadOnly = true;
      txtStepName.Size = new Size(284, 23);
      txtStepName.TabIndex = 7;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(55, 54);
      label3.Name = "label3";
      label3.Size = new Size(33, 15);
      label3.TabIndex = 6;
      label3.Text = "Step:";
      // 
      // btnShowFlow
      // 
      btnShowFlow.Location = new Point(384, 21);
      btnShowFlow.Name = "btnShowFlow";
      btnShowFlow.Size = new Size(83, 23);
      btnShowFlow.TabIndex = 8;
      btnShowFlow.Text = "Show Flow";
      btnShowFlow.UseVisualStyleBackColor = true;
      btnShowFlow.Click += btnShowFlow_Click;
      // 
      // btnHighlightStep
      // 
      btnHighlightStep.Location = new Point(384, 50);
      btnHighlightStep.Name = "btnHighlightStep";
      btnHighlightStep.Size = new Size(102, 23);
      btnHighlightStep.TabIndex = 9;
      btnHighlightStep.Text = "Hightlight Step";
      btnHighlightStep.UseVisualStyleBackColor = true;
      btnHighlightStep.Click += btnHighlightStep_Click;
      // 
      // lvVariables
      // 
      lvVariables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lvVariables.Columns.AddRange(new ColumnHeader[] { chName, chValue });
      lvVariables.FullRowSelect = true;
      lvVariables.GridLines = true;
      lvVariables.Location = new Point(267, 108);
      lvVariables.MultiSelect = false;
      lvVariables.Name = "lvVariables";
      lvVariables.Size = new Size(520, 187);
      lvVariables.TabIndex = 10;
      lvVariables.UseCompatibleStateImageBehavior = false;
      lvVariables.View = View.Details;
      lvVariables.MouseDoubleClick += lvVariables_MouseDoubleClick;
      lvVariables.MouseDown += lvParameters_MouseDown;
      // 
      // chName
      // 
      chName.Text = "Name";
      chName.Width = 160;
      // 
      // chValue
      // 
      chValue.Text = "Value";
      chValue.Width = 320;
      // 
      // contextMenuStrip1
      // 
      contextMenuStrip1.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem });
      contextMenuStrip1.Name = "contextMenuStrip1";
      contextMenuStrip1.Size = new Size(103, 26);
      // 
      // copyToolStripMenuItem
      // 
      copyToolStripMenuItem.Name = "copyToolStripMenuItem";
      copyToolStripMenuItem.Size = new Size(102, 22);
      copyToolStripMenuItem.Text = "Copy";
      copyToolStripMenuItem.Click += copyToolStripMenuItem_Click_1;
      // 
      // tvVariables
      // 
      tvVariables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      tvVariables.HideSelection = false;
      tvVariables.Location = new Point(27, 108);
      tvVariables.Name = "tvVariables";
      tvVariables.Size = new Size(241, 187);
      tvVariables.TabIndex = 11;
      tvVariables.AfterSelect += tvVariables_AfterSelect;
      tvVariables.NodeMouseClick += tvVariables_NodeMouseClick;
      // 
      // frmTraceDetails
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 534);
      Controls.Add(tvVariables);
      Controls.Add(lvVariables);
      Controls.Add(btnHighlightStep);
      Controls.Add(btnShowFlow);
      Controls.Add(txtStepName);
      Controls.Add(label3);
      Controls.Add(txtError);
      Controls.Add(lblSuccess);
      Controls.Add(label2);
      Controls.Add(txtFileName);
      Controls.Add(label1);
      Controls.Add(txtXml);
      Name = "frmTraceDetails";
      Text = "frmTraceDetails";
      Load += frmTraceDetails_Load;
      contextMenuStrip1.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    public TextBox txtXml;
    private Label label1;
    private TextBox txtFileName;
    private Label label2;
    private Label lblSuccess;
    private TextBox txtError;
    private TextBox txtStepName;
    private Label label3;
    private Button btnShowFlow;
    private Button btnHighlightStep;
    private ListView lvVariables;
    private ColumnHeader chName;
    private ColumnHeader chValue;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem copyToolStripMenuItem;
    private TreeView tvVariables;
  }
}