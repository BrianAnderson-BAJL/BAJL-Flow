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
      btnSelectVariable = new Button();
      txtKey = new TextBox();
      nudValue = new NumericUpDown();
      txtVariableName = new TextBox();
      cmbDataType = new ComboBox();
      ((System.ComponentModel.ISupportInitialize)nudValue).BeginInit();
      SuspendLayout();
      // 
      // btnSelectVariable
      // 
      btnSelectVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSelectVariable.Location = new Point(571, 2);
      btnSelectVariable.Name = "btnSelectVariable";
      btnSelectVariable.Size = new Size(31, 23);
      btnSelectVariable.TabIndex = 13;
      btnSelectVariable.Text = "...";
      btnSelectVariable.UseVisualStyleBackColor = true;
      btnSelectVariable.Click += btnSelectVariable_Click;
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
      nudValue.Size = new Size(243, 23);
      nudValue.TabIndex = 15;
      nudValue.ValueChanged += nudValue_ValueChanged;
      // 
      // txtVariableName
      // 
      txtVariableName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      txtVariableName.Location = new Point(321, 2);
      txtVariableName.Name = "txtVariableName";
      txtVariableName.Size = new Size(245, 23);
      txtVariableName.TabIndex = 16;
      txtVariableName.Visible = false;
      // 
      // cmbDataType
      // 
      cmbDataType.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbDataType.FormattingEnabled = true;
      cmbDataType.Items.AddRange(new object[] { "String", "Variable" });
      cmbDataType.Location = new Point(233, 3);
      cmbDataType.Name = "cmbDataType";
      cmbDataType.Size = new Size(83, 23);
      cmbDataType.TabIndex = 22;
      // 
      // ucParameterInteger
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(cmbDataType);
      Controls.Add(txtVariableName);
      Controls.Add(nudValue);
      Controls.Add(btnSelectVariable);
      Controls.Add(txtKey);
      Name = "ucParameterInteger";
      Size = new Size(609, 29);
      Load += ucParameterInteger_Load;
      ((System.ComponentModel.ISupportInitialize)nudValue).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Button btnSelectVariable;
    private TextBox txtKey;
    private NumericUpDown nudValue;
    private TextBox txtVariableName;
    private ComboBox cmbDataType;
  }
}
