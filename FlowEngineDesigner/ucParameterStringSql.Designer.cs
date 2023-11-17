namespace FlowEngineDesigner
{
  partial class ucParameterStringSql
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
      btnSql = new Button();
      SuspendLayout();
      // 
      // txtDataType
      // 
      txtDataType.Location = new Point(234, 3);
      txtDataType.Name = "txtDataType";
      txtDataType.ReadOnly = true;
      txtDataType.Size = new Size(83, 23);
      txtDataType.TabIndex = 19;
      txtDataType.Text = "Integer";
      // 
      // btnSelectVariable
      // 
      btnSelectVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSelectVariable.Location = new Point(456, 2);
      btnSelectVariable.Name = "btnSelectVariable";
      btnSelectVariable.Size = new Size(31, 23);
      btnSelectVariable.TabIndex = 18;
      btnSelectVariable.Text = "...";
      btnSelectVariable.UseVisualStyleBackColor = true;
      btnSelectVariable.Click += btnSelectVariable_Click;
      // 
      // txtValue
      // 
      txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      txtValue.Location = new Point(323, 3);
      txtValue.Multiline = true;
      txtValue.Name = "txtValue";
      txtValue.ScrollBars = ScrollBars.Vertical;
      txtValue.Size = new Size(126, 23);
      txtValue.TabIndex = 17;
      // 
      // chkVariable
      // 
      chkVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      chkVariable.AutoSize = true;
      chkVariable.Location = new Point(539, 5);
      chkVariable.Name = "chkVariable";
      chkVariable.Size = new Size(67, 19);
      chkVariable.TabIndex = 16;
      chkVariable.Text = "Variable";
      chkVariable.UseVisualStyleBackColor = true;
      chkVariable.CheckedChanged += chkVariable_CheckedChanged;
      // 
      // txtKey
      // 
      txtKey.Location = new Point(5, 3);
      txtKey.Name = "txtKey";
      txtKey.ReadOnly = true;
      txtKey.Size = new Size(223, 23);
      txtKey.TabIndex = 15;
      txtKey.TabStop = false;
      // 
      // btnSql
      // 
      btnSql.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSql.Location = new Point(493, 2);
      btnSql.Name = "btnSql";
      btnSql.Size = new Size(40, 23);
      btnSql.TabIndex = 20;
      btnSql.Text = "SQL";
      btnSql.UseVisualStyleBackColor = true;
      btnSql.Click += btnSql_Click;
      // 
      // ucParameterStringSql
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(btnSql);
      Controls.Add(txtDataType);
      Controls.Add(btnSelectVariable);
      Controls.Add(txtValue);
      Controls.Add(chkVariable);
      Controls.Add(txtKey);
      Name = "ucParameterStringSql";
      Size = new Size(609, 29);
      Load += ucParameterStringSql_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    public TextBox txtDataType;
    private Button btnSelectVariable;
    public TextBox txtValue;
    public CheckBox chkVariable;
    public TextBox txtKey;
    private Button btnSql;
  }
}
