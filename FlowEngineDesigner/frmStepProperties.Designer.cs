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
      btnAddParameter = new Button();
      chkSaveResponse = new CheckBox();
      label2 = new Label();
      txtSaveResponseName = new TextBox();
      gbParameters.SuspendLayout();
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
      btnOk.Location = new Point(820, 356);
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
      gbParameters.Controls.Add(btnAddParameter);
      gbParameters.Location = new Point(18, 108);
      gbParameters.Name = "gbParameters";
      gbParameters.Size = new Size(930, 191);
      gbParameters.TabIndex = 9;
      gbParameters.TabStop = false;
      gbParameters.Text = "Parameters";
      // 
      // btnAddParameter
      // 
      btnAddParameter.Location = new Point(13, 83);
      btnAddParameter.Name = "btnAddParameter";
      btnAddParameter.Size = new Size(116, 28);
      btnAddParameter.TabIndex = 0;
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
      // frmStepProperties
      // 
      AcceptButton = btnOk;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(960, 416);
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
    private Button btnAddParameter;
  }
}