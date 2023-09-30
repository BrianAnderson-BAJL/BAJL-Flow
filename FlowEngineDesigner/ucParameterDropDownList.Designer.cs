namespace FlowEngineDesigner
{
  partial class ucParameterDropDownList
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      txtKey = new TextBox();
      cmbItems = new ComboBox();
      txtDataType = new TextBox();
      SuspendLayout();
      // 
      // txtKey
      // 
      txtKey.Location = new Point(3, 3);
      txtKey.Name = "txtKey";
      txtKey.Size = new Size(221, 23);
      txtKey.TabIndex = 0;
      // 
      // cmbItems
      // 
      cmbItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      cmbItems.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbItems.FormattingEnabled = true;
      cmbItems.Location = new Point(319, 3);
      cmbItems.Name = "cmbItems";
      cmbItems.Size = new Size(166, 23);
      cmbItems.TabIndex = 1;
      // 
      // txtDataType
      // 
      txtDataType.Location = new Point(230, 3);
      txtDataType.Name = "txtDataType";
      txtDataType.ReadOnly = true;
      txtDataType.Size = new Size(83, 23);
      txtDataType.TabIndex = 4;
      // 
      // ucParameterDropDownList
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(txtDataType);
      Controls.Add(cmbItems);
      Controls.Add(txtKey);
      Name = "ucParameterDropDownList";
      Size = new Size(626, 30);
      Load += ucParameterDropDownList_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox txtKey;
    private ComboBox cmbItems;
    private TextBox txtDataType;
  }
}
