namespace FlowEngineDesigner
{
  partial class frmSettings
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
      components = new System.ComponentModel.Container();
      lstSettings = new ListView();
      chKey = new ColumnHeader();
      chDataType = new ColumnHeader();
      chValue = new ColumnHeader();
      btnOk = new Button();
      toolTip1 = new ToolTip(components);
      cmbServerSettings = new ComboBox();
      SuspendLayout();
      // 
      // lstSettings
      // 
      lstSettings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      lstSettings.Columns.AddRange(new ColumnHeader[] { chKey, chDataType, chValue });
      lstSettings.FullRowSelect = true;
      lstSettings.GridLines = true;
      lstSettings.Location = new Point(2, 39);
      lstSettings.Name = "lstSettings";
      lstSettings.Size = new Size(642, 404);
      lstSettings.TabIndex = 0;
      lstSettings.UseCompatibleStateImageBehavior = false;
      lstSettings.View = View.Details;
      lstSettings.SelectedIndexChanged += lstSettings_SelectedIndexChanged;
      lstSettings.MouseDoubleClick += lstSettings_MouseDoubleClick;
      lstSettings.MouseHover += lstSettings_MouseHover;
      // 
      // chKey
      // 
      chKey.Text = "Key";
      chKey.Width = 240;
      // 
      // chDataType
      // 
      chDataType.Text = "Data type";
      chDataType.Width = 120;
      // 
      // chValue
      // 
      chValue.Text = "Value";
      chValue.Width = 240;
      // 
      // btnOk
      // 
      btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      btnOk.Location = new Point(549, 449);
      btnOk.Name = "btnOk";
      btnOk.Size = new Size(86, 33);
      btnOk.TabIndex = 1;
      btnOk.Text = "Ok";
      btnOk.UseVisualStyleBackColor = true;
      btnOk.Click += btnOk_Click;
      // 
      // cmbServerSettings
      // 
      cmbServerSettings.FormattingEnabled = true;
      cmbServerSettings.Location = new Point(2, 10);
      cmbServerSettings.Name = "cmbServerSettings";
      cmbServerSettings.Size = new Size(249, 23);
      cmbServerSettings.TabIndex = 2;
      // 
      // frmSettings
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(647, 494);
      Controls.Add(cmbServerSettings);
      Controls.Add(btnOk);
      Controls.Add(lstSettings);
      Name = "frmSettings";
      Text = "Plugin Settings";
      Load += frmSettings_Load;
      ResumeLayout(false);
    }

    #endregion

    private ListView lstSettings;
    private ColumnHeader chKey;
    private ColumnHeader chValue;
    private Button btnOk;
    private ColumnHeader chDataType;
    private ToolTip toolTip1;
    private ComboBox cmbServerSettings;
  }
}