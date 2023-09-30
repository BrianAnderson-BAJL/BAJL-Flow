namespace FlowEngineDesigner
{
  partial class frmVariableSelection
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
      tvVariables = new TreeView();
      btnOk = new Button();
      bntCancel = new Button();
      SuspendLayout();
      // 
      // tvVariables
      // 
      tvVariables.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      tvVariables.Location = new Point(12, 12);
      tvVariables.Name = "tvVariables";
      tvVariables.Size = new Size(296, 426);
      tvVariables.TabIndex = 0;
      tvVariables.AfterSelect += tvVariables_AfterSelect;
      tvVariables.NodeMouseDoubleClick += tvVariables_NodeMouseDoubleClick;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(414, 394);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(79, 44);
      btnOk.TabIndex = 1;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // bntCancel
      // 
      bntCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      bntCancel.Location = new Point(314, 394);
      bntCancel.Name = "bntCancel";
      bntCancel.Size = new Size(79, 44);
      bntCancel.TabIndex = 2;
      bntCancel.Text = "&Cancel";
      bntCancel.UseVisualStyleBackColor = true;
      bntCancel.Click += bntCancel_Click;
      // 
      // frmVariableSelection
      // 
      AcceptButton = btnOk;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      CancelButton = bntCancel;
      ClientSize = new Size(505, 450);
      Controls.Add(bntCancel);
      Controls.Add(btnOk);
      Controls.Add(tvVariables);
      Name = "frmVariableSelection";
      Text = "VariableSelection";
      Load += VariableSelection_Load;
      ResumeLayout(false);
    }

    #endregion

    private TreeView tvVariables;
    private Button btnOk;
    private Button bntCancel;
  }
}