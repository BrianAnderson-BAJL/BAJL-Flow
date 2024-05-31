namespace FlowEngineDesigner
{
  partial class ucParameterVarious
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
      txtKey = new TextBox();
      cmbDataType = new ComboBox();
      nudNumber = new NumericUpDown();
      lblDescription = new LinkLabel();
      ((System.ComponentModel.ISupportInitialize)nudNumber).BeginInit();
      SuspendLayout();
      // 
      // txtDataType
      // 
      txtDataType.Location = new Point(232, 3);
      txtDataType.Name = "txtDataType";
      txtDataType.ReadOnly = true;
      txtDataType.Size = new Size(83, 23);
      txtDataType.TabIndex = 19;
      txtDataType.Text = "Integer";
      // 
      // btnSelectVariable
      // 
      btnSelectVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSelectVariable.Location = new Point(570, 2);
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
      txtValue.Location = new Point(321, 3);
      txtValue.Name = "txtValue";
      txtValue.Size = new Size(246, 23);
      txtValue.TabIndex = 17;
      txtValue.Visible = false;
      txtValue.TextChanged += txtValue_TextChanged;
      // 
      // txtKey
      // 
      txtKey.Location = new Point(2, 3);
      txtKey.Name = "txtKey";
      txtKey.ReadOnly = true;
      txtKey.Size = new Size(204, 23);
      txtKey.TabIndex = 15;
      txtKey.TabStop = false;
      txtKey.TextChanged += txtKey_TextChanged;
      // 
      // cmbDataType
      // 
      cmbDataType.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbDataType.FormattingEnabled = true;
      cmbDataType.Items.AddRange(new object[] { "String", "Integer", "Decimal", "Boolean", "Variable" });
      cmbDataType.Location = new Point(232, 3);
      cmbDataType.Name = "cmbDataType";
      cmbDataType.Size = new Size(83, 23);
      cmbDataType.TabIndex = 20;
      cmbDataType.SelectedIndexChanged += cmbDataType_SelectedIndexChanged;
      // 
      // nudNumber
      // 
      nudNumber.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      nudNumber.Location = new Point(321, 3);
      nudNumber.Name = "nudNumber";
      nudNumber.Size = new Size(247, 23);
      nudNumber.TabIndex = 21;
      nudNumber.Visible = false;
      // 
      // lblDescription
      // 
      lblDescription.AutoSize = true;
      lblDescription.Location = new Point(214, 7);
      lblDescription.Name = "lblDescription";
      lblDescription.Size = new Size(18, 15);
      lblDescription.TabIndex = 22;
      lblDescription.TabStop = true;
      lblDescription.Text = " ? ";
      lblDescription.MouseClick += lblDescription_MouseClick;
      // 
      // ucParameterVarious
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(cmbDataType);
      Controls.Add(txtDataType);
      Controls.Add(btnSelectVariable);
      Controls.Add(txtValue);
      Controls.Add(txtKey);
      Controls.Add(nudNumber);
      Controls.Add(lblDescription);
      Name = "ucParameterVarious";
      Size = new Size(609, 29);
      Load += ucParameterVarious_Load;
      ((System.ComponentModel.ISupportInitialize)nudNumber).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    public TextBox txtDataType;
    private Button btnSelectVariable;
    public TextBox txtValue;
    public TextBox txtKey;
    private ComboBox cmbDataType;
    private NumericUpDown nudNumber;
    private LinkLabel lblDescription;
  }
}
