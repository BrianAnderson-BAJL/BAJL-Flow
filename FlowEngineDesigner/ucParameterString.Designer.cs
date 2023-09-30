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
      txtDataType = new TextBox();
      btnSelectVariable = new Button();
      txtValue = new TextBox();
      chkVariable = new CheckBox();
      txtKey = new TextBox();
      SuspendLayout();
      // 
      // txtDataType
      // 
      txtDataType.Location = new Point(231, 3);
      txtDataType.Name = "txtDataType";
      txtDataType.ReadOnly = true;
      txtDataType.Size = new Size(83, 23);
      txtDataType.TabIndex = 14;
      txtDataType.Text = "Integer";
      // 
      // btnSelectVariable
      // 
      btnSelectVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSelectVariable.Location = new Point(492, 2);
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
      txtValue.Size = new Size(166, 23);
      txtValue.TabIndex = 12;
      // 
      // chkVariable
      // 
      chkVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      chkVariable.AutoSize = true;
      chkVariable.Location = new Point(529, 5);
      chkVariable.Name = "chkVariable";
      chkVariable.Size = new Size(67, 19);
      chkVariable.TabIndex = 11;
      chkVariable.Text = "Variable";
      chkVariable.UseVisualStyleBackColor = true;
      chkVariable.CheckedChanged += chkVariable_CheckedChanged;
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
      // ucParameterString
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(txtDataType);
      Controls.Add(btnSelectVariable);
      Controls.Add(txtValue);
      Controls.Add(chkVariable);
      Controls.Add(txtKey);
      Name = "ucParameterString";
      Size = new Size(609, 29);
      Load += ucParameterString_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    public TextBox txtDataType;
    private Button btnSelectVariable;
    public TextBox txtValue;
    public CheckBox chkVariable;
    public TextBox txtKey;
  }
}
