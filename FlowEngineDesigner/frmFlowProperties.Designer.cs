namespace FlowEngineDesigner
{
  partial class frmFlowProperties
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
      txtCreatedDateTime = new TextBox();
      txtModifiedLastDateTime = new TextBox();
      label2 = new Label();
      gbStartOptions = new GroupBox();
      btnOk = new Button();
      cmbPlugins = new ComboBox();
      label3 = new Label();
      groupBox2 = new GroupBox();
      label5 = new Label();
      cmbDebugStartType = new ComboBox();
      groupBox1 = new GroupBox();
      rbSampleDataFormatXml = new RadioButton();
      rbSampleDataFormatJson = new RadioButton();
      rbSampleDataFormatNone = new RadioButton();
      txtSampleData = new TextBox();
      label4 = new Label();
      groupBox2.SuspendLayout();
      groupBox1.SuspendLayout();
      SuspendLayout();
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(513, 15);
      label1.Name = "label1";
      label1.Size = new Size(109, 15);
      label1.TabIndex = 0;
      label1.Text = "Created Date/Time:";
      // 
      // txtCreatedDateTime
      // 
      txtCreatedDateTime.Location = new Point(628, 12);
      txtCreatedDateTime.Name = "txtCreatedDateTime";
      txtCreatedDateTime.ReadOnly = true;
      txtCreatedDateTime.Size = new Size(206, 23);
      txtCreatedDateTime.TabIndex = 1;
      txtCreatedDateTime.TabStop = false;
      // 
      // txtModifiedLastDateTime
      // 
      txtModifiedLastDateTime.Location = new Point(628, 41);
      txtModifiedLastDateTime.Name = "txtModifiedLastDateTime";
      txtModifiedLastDateTime.ReadOnly = true;
      txtModifiedLastDateTime.Size = new Size(206, 23);
      txtModifiedLastDateTime.TabIndex = 3;
      txtModifiedLastDateTime.TabStop = false;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(482, 44);
      label2.Name = "label2";
      label2.Size = new Size(140, 15);
      label2.TabIndex = 2;
      label2.Text = "Last Modified Date/Time:";
      // 
      // gbStartOptions
      // 
      gbStartOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      gbStartOptions.Location = new Point(167, 71);
      gbStartOptions.Name = "gbStartOptions";
      gbStartOptions.Size = new Size(667, 183);
      gbStartOptions.TabIndex = 4;
      gbStartOptions.TabStop = false;
      gbStartOptions.Text = "Start up options";
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(736, 577);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(98, 42);
      btnOk.TabIndex = 5;
      btnOk.Text = "&Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // cmbPlugins
      // 
      cmbPlugins.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbPlugins.FormattingEnabled = true;
      cmbPlugins.Location = new Point(167, 42);
      cmbPlugins.Name = "cmbPlugins";
      cmbPlugins.Size = new Size(202, 23);
      cmbPlugins.TabIndex = 6;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(24, 45);
      label3.Name = "label3";
      label3.Size = new Size(141, 15);
      label3.TabIndex = 7;
      label3.Text = "Plugin that will start flow:";
      label3.Click += label3_Click;
      // 
      // groupBox2
      // 
      groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      groupBox2.Controls.Add(label5);
      groupBox2.Controls.Add(cmbDebugStartType);
      groupBox2.Controls.Add(groupBox1);
      groupBox2.Controls.Add(txtSampleData);
      groupBox2.Controls.Add(label4);
      groupBox2.Location = new Point(12, 274);
      groupBox2.Name = "groupBox2";
      groupBox2.Size = new Size(822, 274);
      groupBox2.TabIndex = 14;
      groupBox2.TabStop = false;
      groupBox2.Text = "Debug Info";
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.Location = new Point(91, 24);
      label5.Name = "label5";
      label5.Size = new Size(60, 15);
      label5.TabIndex = 18;
      label5.Text = "Start type:";
      // 
      // cmbDebugStartType
      // 
      cmbDebugStartType.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbDebugStartType.FormattingEnabled = true;
      cmbDebugStartType.Location = new Point(157, 21);
      cmbDebugStartType.Name = "cmbDebugStartType";
      cmbDebugStartType.Size = new Size(202, 23);
      cmbDebugStartType.TabIndex = 14;
      // 
      // groupBox1
      // 
      groupBox1.Controls.Add(rbSampleDataFormatXml);
      groupBox1.Controls.Add(rbSampleDataFormatJson);
      groupBox1.Controls.Add(rbSampleDataFormatNone);
      groupBox1.Location = new Point(24, 81);
      groupBox1.Name = "groupBox1";
      groupBox1.Size = new Size(127, 171);
      groupBox1.TabIndex = 17;
      groupBox1.TabStop = false;
      groupBox1.Text = "Sample data format";
      // 
      // rbSampleDataFormatXml
      // 
      rbSampleDataFormatXml.AutoSize = true;
      rbSampleDataFormatXml.Location = new Point(15, 85);
      rbSampleDataFormatXml.Name = "rbSampleDataFormatXml";
      rbSampleDataFormatXml.Size = new Size(46, 19);
      rbSampleDataFormatXml.TabIndex = 13;
      rbSampleDataFormatXml.Text = "Xml";
      rbSampleDataFormatXml.UseVisualStyleBackColor = true;
      rbSampleDataFormatXml.CheckedChanged += rbSampleDataFormatXml_CheckedChanged;
      // 
      // rbSampleDataFormatJson
      // 
      rbSampleDataFormatJson.AutoSize = true;
      rbSampleDataFormatJson.Location = new Point(15, 60);
      rbSampleDataFormatJson.Name = "rbSampleDataFormatJson";
      rbSampleDataFormatJson.Size = new Size(48, 19);
      rbSampleDataFormatJson.TabIndex = 12;
      rbSampleDataFormatJson.Text = "Json";
      rbSampleDataFormatJson.UseVisualStyleBackColor = true;
      rbSampleDataFormatJson.CheckedChanged += rbSampleDataFormatJson_CheckedChanged;
      // 
      // rbSampleDataFormatNone
      // 
      rbSampleDataFormatNone.AutoSize = true;
      rbSampleDataFormatNone.Checked = true;
      rbSampleDataFormatNone.Location = new Point(15, 35);
      rbSampleDataFormatNone.Name = "rbSampleDataFormatNone";
      rbSampleDataFormatNone.Size = new Size(68, 19);
      rbSampleDataFormatNone.TabIndex = 11;
      rbSampleDataFormatNone.TabStop = true;
      rbSampleDataFormatNone.Text = "No Data";
      rbSampleDataFormatNone.UseVisualStyleBackColor = true;
      rbSampleDataFormatNone.CheckedChanged += rbSampleDataFormatNone_CheckedChanged;
      // 
      // txtSampleData
      // 
      txtSampleData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      txtSampleData.Font = new Font("Courier New", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
      txtSampleData.Location = new Point(157, 50);
      txtSampleData.Multiline = true;
      txtSampleData.Name = "txtSampleData";
      txtSampleData.ReadOnly = true;
      txtSampleData.ScrollBars = ScrollBars.Vertical;
      txtSampleData.Size = new Size(656, 202);
      txtSampleData.TabIndex = 16;
      txtSampleData.TextChanged += txtSampleData_TextChanged;
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(78, 53);
      label4.Name = "label4";
      label4.Size = new Size(73, 15);
      label4.TabIndex = 15;
      label4.Text = "Sample Data";
      // 
      // frmFlowProperties
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(846, 631);
      Controls.Add(groupBox2);
      Controls.Add(label3);
      Controls.Add(cmbPlugins);
      Controls.Add(btnOk);
      Controls.Add(gbStartOptions);
      Controls.Add(txtModifiedLastDateTime);
      Controls.Add(label2);
      Controls.Add(txtCreatedDateTime);
      Controls.Add(label1);
      MaximizeBox = false;
      Name = "frmFlowProperties";
      Text = "Flow Properties";
      Load += frmFlowProperties_Load;
      groupBox2.ResumeLayout(false);
      groupBox2.PerformLayout();
      groupBox1.ResumeLayout(false);
      groupBox1.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label label1;
    private TextBox txtCreatedDateTime;
    private TextBox txtModifiedLastDateTime;
    private Label label2;
    private GroupBox gbStartOptions;
    private Button btnOk;
    private ComboBox cmbPlugins;
    private Label label3;
    private GroupBox groupBox2;
    private Label label5;
    private ComboBox cmbDebugStartType;
    private GroupBox groupBox1;
    private RadioButton rbSampleDataFormatXml;
    private RadioButton rbSampleDataFormatJson;
    private RadioButton rbSampleDataFormatNone;
    private TextBox txtSampleData;
    private Label label4;
  }
}