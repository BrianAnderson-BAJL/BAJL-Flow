namespace FlowEngineDesigner
{
  partial class frmSqlRecords
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
      lvRecords = new ListView();
      SuspendLayout();
      // 
      // lvRecords
      // 
      lvRecords.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lvRecords.FullRowSelect = true;
      lvRecords.GridLines = true;
      lvRecords.Location = new Point(-1, 6);
      lvRecords.MultiSelect = false;
      lvRecords.Name = "lvRecords";
      lvRecords.ShowGroups = false;
      lvRecords.Size = new Size(800, 443);
      lvRecords.TabIndex = 0;
      lvRecords.UseCompatibleStateImageBehavior = false;
      lvRecords.View = View.Details;
      // 
      // frmSqlRecords
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 450);
      Controls.Add(lvRecords);
      Name = "frmSqlRecords";
      Text = "Records";
      Load += frmSqlRecords_Load;
      ResumeLayout(false);
    }

    #endregion

    private ListView lvRecords;
  }
}