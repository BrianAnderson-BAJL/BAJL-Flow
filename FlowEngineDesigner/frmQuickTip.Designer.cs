namespace FlowEngineDesigner
{
  partial class frmQuickTip
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
      txtQuickTip = new TextBox();
      SuspendLayout();
      // 
      // txtQuickTip
      // 
      txtQuickTip.Location = new Point(1, 1);
      txtQuickTip.Multiline = true;
      txtQuickTip.Name = "txtQuickTip";
      txtQuickTip.ReadOnly = true;
      txtQuickTip.ScrollBars = ScrollBars.Vertical;
      txtQuickTip.Size = new Size(235, 81);
      txtQuickTip.TabIndex = 0;
      txtQuickTip.TabStop = false;
      // 
      // frmQuickTip
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(237, 83);
      Controls.Add(txtQuickTip);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      Name = "frmQuickTip";
      StartPosition = FormStartPosition.Manual;
      Deactivate += frmQuickTip_Deactivate;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox txtQuickTip;
  }
}