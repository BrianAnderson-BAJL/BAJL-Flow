namespace FlowEngineDesigner
{
  partial class ucParameterInteger
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
      chkVariable = new CheckBox();
      txtKey = new TextBox();
      nudValue = new NumericUpDown();
      txtVariableName = new TextBox();
      ((System.ComponentModel.ISupportInitialize)nudValue).BeginInit();
      SuspendLayout();
      // 
      // txtDataType
      // 
      txtDataType.Location = new Point(232, 3);
      txtDataType.Name = "txtDataType";
      txtDataType.ReadOnly = true;
      txtDataType.Size = new Size(83, 23);
      txtDataType.TabIndex = 14;
      txtDataType.Text = "Integer";
      // 
      // btnSelectVariable
      // 
      btnSelectVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSelectVariable.Location = new Point(493, 2);
      btnSelectVariable.Name = "btnSelectVariable";
      btnSelectVariable.Size = new Size(31, 23);
      btnSelectVariable.TabIndex = 13;
      btnSelectVariable.Text = "...";
      btnSelectVariable.UseVisualStyleBackColor = true;
      btnSelectVariable.Click += btnSelectVariable_Click;
      // 
      // chkVariable
      // 
      chkVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      chkVariable.AutoSize = true;
      chkVariable.Location = new Point(530, 5);
      chkVariable.Name = "chkVariable";
      chkVariable.Size = new Size(67, 19);
      chkVariable.TabIndex = 11;
      chkVariable.Text = "Variable";
      chkVariable.UseVisualStyleBackColor = true;
      chkVariable.CheckedChanged += chkVariable_CheckedChanged;
      // 
      // txtKey
      // 
      txtKey.Location = new Point(3, 3);
      txtKey.Name = "txtKey";
      txtKey.ReadOnly = true;
      txtKey.Size = new Size(223, 23);
      txtKey.TabIndex = 10;
      txtKey.TabStop = false;
      // 
      // nudValue
      // 
      nudValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      nudValue.Location = new Point(321, 2);
      nudValue.Name = "nudValue";
      nudValue.Size = new Size(166, 23);
      nudValue.TabIndex = 15;
      nudValue.ValueChanged += nudValue_ValueChanged;
      // 
      // txtVariableName
      // 
      txtVariableName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      txtVariableName.Location = new Point(321, 2);
      txtVariableName.Name = "txtVariableName";
      txtVariableName.Size = new Size(166, 23);
      txtVariableName.TabIndex = 16;
      txtVariableName.Visible = false;
      // 
      // ucParameterInteger
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(txtVariableName);
      Controls.Add(nudValue);
      Controls.Add(txtDataType);
      Controls.Add(btnSelectVariable);
      Controls.Add(chkVariable);
      Controls.Add(txtKey);
      Name = "ucParameterInteger";
      Size = new Size(609, 29);
      Load += ucParameterInteger_Load;
      ((System.ComponentModel.ISupportInitialize)nudValue).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox txtDataType;
    private Button btnSelectVariable;
    private CheckBox chkVariable;
    private TextBox txtKey;
    private NumericUpDown nudValue;
    private TextBox txtVariableName;
  }
}
