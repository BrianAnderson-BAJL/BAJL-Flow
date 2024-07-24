namespace FlowEngineDesigner
{
  partial class ucParameterStringTemplate
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
      btnFile = new Button();
      txtDataType = new TextBox();
      btnSelectVariable = new Button();
      txtValue = new TextBox();
      txtKey = new TextBox();
      SuspendLayout();
      // 
      // btnFile
      // 
      btnFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnFile.Location = new Point(684, 3);
      btnFile.Name = "btnFile";
      btnFile.Size = new Size(40, 23);
      btnFile.TabIndex = 26;
      btnFile.Text = "File";
      btnFile.UseVisualStyleBackColor = true;
      btnFile.Click += btnFile_Click;
      // 
      // txtDataType
      // 
      txtDataType.Location = new Point(231, 3);
      txtDataType.Name = "txtDataType";
      txtDataType.ReadOnly = true;
      txtDataType.Size = new Size(83, 23);
      txtDataType.TabIndex = 25;
      txtDataType.Text = "Integer";
      // 
      // btnSelectVariable
      // 
      btnSelectVariable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      btnSelectVariable.Location = new Point(647, 3);
      btnSelectVariable.Name = "btnSelectVariable";
      btnSelectVariable.Size = new Size(31, 23);
      btnSelectVariable.TabIndex = 24;
      btnSelectVariable.Text = "...";
      btnSelectVariable.UseVisualStyleBackColor = true;
      btnSelectVariable.Click += btnSelectVariable_Click;
      // 
      // txtValue
      // 
      txtValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      txtValue.Location = new Point(320, 3);
      txtValue.Multiline = true;
      txtValue.Name = "txtValue";
      txtValue.ScrollBars = ScrollBars.Vertical;
      txtValue.Size = new Size(321, 23);
      txtValue.TabIndex = 23;
      // 
      // txtKey
      // 
      txtKey.Location = new Point(2, 3);
      txtKey.Name = "txtKey";
      txtKey.ReadOnly = true;
      txtKey.Size = new Size(223, 23);
      txtKey.TabIndex = 21;
      txtKey.TabStop = false;
      // 
      // ucParameterStringTemplate
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      Controls.Add(btnFile);
      Controls.Add(txtDataType);
      Controls.Add(btnSelectVariable);
      Controls.Add(txtValue);
      Controls.Add(txtKey);
      Name = "ucParameterStringTemplate";
      Size = new Size(727, 32);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button btnFile;
    public TextBox txtDataType;
    private Button btnSelectVariable;
    public TextBox txtValue;
    public TextBox txtKey;
  }
}
