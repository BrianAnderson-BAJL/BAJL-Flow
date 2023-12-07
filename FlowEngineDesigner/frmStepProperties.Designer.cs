namespace FlowEngineDesigner
{
  partial class frmStepProperties
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
      label1 = new Label();
      txtType = new TextBox();
      btnOk = new Button();
      gbParameters = new GroupBox();
      gbPanel = new Panel();
      btnDeleteParameter = new Button();
      btnAddParameter = new Button();
      chkSaveResponse = new CheckBox();
      label2 = new Label();
      txtSaveResponseName = new TextBox();
      lblValidator = new Label();
      cmbValidators = new ComboBox();
      gbParameters.SuspendLayout();
      gbPanel.SuspendLayout();
      SuspendLayout();
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(158, 33);
      label1.Name = "label1";
      label1.Size = new Size(34, 15);
      label1.TabIndex = 0;
      label1.Text = "Type:";
      // 
      // txtType
      // 
      txtType.Location = new Point(198, 30);
      txtType.Name = "txtType";
      txtType.ReadOnly = true;
      txtType.Size = new Size(280, 23);
      txtType.TabIndex = 1;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(857, 362);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(91, 42);
      btnOk.TabIndex = 8;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // gbParameters
      // 
      gbParameters.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      gbParameters.Controls.Add(gbPanel);
      gbParameters.Location = new Point(18, 108);
      gbParameters.Name = "gbParameters";
      gbParameters.Size = new Size(930, 242);
      gbParameters.TabIndex = 9;
      gbParameters.TabStop = false;
      gbParameters.Text = "Parameters";
      // 
      // gbPanel
      // 
      gbPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      gbPanel.AutoScroll = true;
      gbPanel.Controls.Add(btnDeleteParameter);
      gbPanel.Controls.Add(btnAddParameter);
      gbPanel.Location = new Point(6, 22);
      gbPanel.Name = "gbPanel";
      gbPanel.Size = new Size(918, 214);
      gbPanel.TabIndex = 4;
      gbPanel.Paint += gbPanel_Paint;
      // 
      // btnDeleteParameter
      // 
      btnDeleteParameter.Location = new Point(31, 32);
      btnDeleteParameter.Name = "btnDeleteParameter";
      btnDeleteParameter.Size = new Size(26, 23);
      btnDeleteParameter.TabIndex = 5;
      btnDeleteParameter.Text = "X";
      btnDeleteParameter.UseVisualStyleBackColor = true;
      btnDeleteParameter.Visible = false;
      btnDeleteParameter.Click += btnDeleteParameter_Click;
      // 
      // btnAddParameter
      // 
      btnAddParameter.Location = new Point(38, 61);
      btnAddParameter.Name = "btnAddParameter";
      btnAddParameter.Size = new Size(116, 28);
      btnAddParameter.TabIndex = 4;
      btnAddParameter.Text = "Add parameter";
      btnAddParameter.UseVisualStyleBackColor = true;
      btnAddParameter.Click += btnAddParameter_Click;
      // 
      // chkSaveResponse
      // 
      chkSaveResponse.AutoSize = true;
      chkSaveResponse.Location = new Point(484, 68);
      chkSaveResponse.Name = "chkSaveResponse";
      chkSaveResponse.Size = new Size(147, 19);
      chkSaveResponse.TabIndex = 10;
      chkSaveResponse.Text = "Save Response variable";
      chkSaveResponse.UseVisualStyleBackColor = true;
      chkSaveResponse.CheckedChanged += chkSaveResponse_CheckedChanged;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(31, 65);
      label2.Name = "label2";
      label2.Size = new Size(161, 15);
      label2.TabIndex = 11;
      label2.Text = "Save response variable name:";
      // 
      // txtSaveResponseName
      // 
      txtSaveResponseName.Enabled = false;
      txtSaveResponseName.Location = new Point(198, 64);
      txtSaveResponseName.Name = "txtSaveResponseName";
      txtSaveResponseName.Size = new Size(280, 23);
      txtSaveResponseName.TabIndex = 12;
      txtSaveResponseName.TextChanged += txtSaveResponseName_TextChanged;
      // 
      // lblValidator
      // 
      lblValidator.AutoSize = true;
      lblValidator.Location = new Point(685, 12);
      lblValidator.Name = "lblValidator";
      lblValidator.Size = new Size(56, 15);
      lblValidator.TabIndex = 13;
      lblValidator.Text = "Validator:";
      // 
      // cmbValidators
      // 
      cmbValidators.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbValidators.Enabled = false;
      cmbValidators.FormattingEnabled = true;
      cmbValidators.Location = new Point(685, 30);
      cmbValidators.Name = "cmbValidators";
      cmbValidators.Size = new Size(263, 23);
      cmbValidators.TabIndex = 14;
      cmbValidators.SelectedIndexChanged += cmbValidators_SelectedIndexChanged;
      // 
      // frmStepProperties
      // 
      AcceptButton = btnOk;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(960, 416);
      Controls.Add(cmbValidators);
      Controls.Add(lblValidator);
      Controls.Add(txtSaveResponseName);
      Controls.Add(label2);
      Controls.Add(chkSaveResponse);
      Controls.Add(gbParameters);
      Controls.Add(btnOk);
      Controls.Add(txtType);
      Controls.Add(label1);
      Name = "frmStepProperties";
      Text = "Step properties";
      Load += frmStepProperties_Load;
      gbParameters.ResumeLayout(false);
      gbPanel.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox txtType;
    private Button btnOk;
    private GroupBox gbParameters;
    private CheckBox chkSaveResponse;
    private Label label2;
    private TextBox txtSaveResponseName;
    private Label lblValidator;
    private ComboBox cmbValidators;
    private Panel gbPanel;
    private Button btnDeleteParameter;
    private Button btnAddParameter;
  }
}