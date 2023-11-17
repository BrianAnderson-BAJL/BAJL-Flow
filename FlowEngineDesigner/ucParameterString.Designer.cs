namespace FlowEngineDesigner
{
  partial class ucParameterString
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
      btnSelectVariable = new Button();
      txtValue = new TextBox();
      txtKey = new TextBox();
      cmbDataType = new ComboBox();
      SuspendLayout();
      // 
      // btnSelectVariable
      // 
      btnSelectVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSelectVariable.Location = new Point(582, 3);
      btnSelectVariable.Name = "btnSelectVariable";
      btnSelectVariable.Size = new Size(31, 23);
      btnSelectVariable.TabIndex = 13;
      btnSelectVariable.Text = "...";
      btnSelectVariable.UseVisualStyleBackColor = true;
      btnSelectVariable.Click += btnSelectVariable_Click;
      // 
      // txtValue
      // 
      txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      txtValue.Location = new Point(320, 3);
      txtValue.Name = "txtValue";
      txtValue.Size = new Size(256, 23);
      txtValue.TabIndex = 12;
      // 
      // txtKey
      // 
      txtKey.Location = new Point(2, 3);
      txtKey.Name = "txtKey";
      txtKey.ReadOnly = true;
      txtKey.Size = new Size(223, 23);
      txtKey.TabIndex = 10;
      txtKey.TabStop = false;
      // 
      // cmbDataType
      // 
      cmbDataType.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbDataType.FormattingEnabled = true;
      cmbDataType.Items.AddRange(new object[] { "String", "Variable" });
      cmbDataType.Location = new Point(231, 3);
      cmbDataType.Name = "cmbDataType";
      cmbDataType.Size = new Size(83, 23);
      cmbDataType.TabIndex = 21;
      cmbDataType.SelectedIndexChanged += cmbDataType_SelectedIndexChanged;
      // 
      // ucParameterString
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(cmbDataType);
      Controls.Add(btnSelectVariable);
      Controls.Add(txtValue);
      Controls.Add(txtKey);
      Name = "ucParameterString";
      Size = new Size(616, 29);
      Load += ucParameterString_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Button btnSelectVariable;
    public TextBox txtValue;
    public TextBox txtKey;
    private ComboBox cmbDataType;
  }
}
