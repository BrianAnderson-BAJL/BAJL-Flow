namespace FlowEngineDesigner
{
  partial class frmAbout
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
      tableLayoutPanel = new TableLayoutPanel();
      logoPictureBox = new PictureBox();
      labelProductName = new Label();
      labelVersion = new Label();
      labelCopyright = new Label();
      labelCompanyName = new Label();
      textBoxDescription = new TextBox();
      listView1 = new ListView();
      chPlugin = new ColumnHeader();
      chVersion = new ColumnHeader();
      okButton = new Button();
      statusStrip1 = new StatusStrip();
      tableLayoutPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
      SuspendLayout();
      // 
      // tableLayoutPanel
      // 
      tableLayoutPanel.ColumnCount = 2;
      tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
      tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
      tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
      tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
      tableLayoutPanel.Controls.Add(labelVersion, 1, 1);
      tableLayoutPanel.Controls.Add(labelCopyright, 1, 2);
      tableLayoutPanel.Controls.Add(labelCompanyName, 1, 3);
      tableLayoutPanel.Controls.Add(textBoxDescription, 1, 4);
      tableLayoutPanel.Controls.Add(listView1, 1, 5);
      tableLayoutPanel.Controls.Add(okButton, 1, 6);
      tableLayoutPanel.Dock = DockStyle.Fill;
      tableLayoutPanel.Location = new Point(10, 10);
      tableLayoutPanel.Margin = new Padding(4, 3, 4, 3);
      tableLayoutPanel.Name = "tableLayoutPanel";
      tableLayoutPanel.RowCount = 7;
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 6.296296F));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 6.111111F));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 6.851852F));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 6.111111F));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 17.1903877F));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 57.4861374F));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
      tableLayoutPanel.Size = new Size(654, 606);
      tableLayoutPanel.TabIndex = 0;
      // 
      // logoPictureBox
      // 
      logoPictureBox.Dock = DockStyle.Fill;
      logoPictureBox.Image = (Image)resources.GetObject("logoPictureBox.Image");
      logoPictureBox.Location = new Point(4, 3);
      logoPictureBox.Margin = new Padding(4, 3, 4, 3);
      logoPictureBox.Name = "logoPictureBox";
      tableLayoutPanel.SetRowSpan(logoPictureBox, 6);
      logoPictureBox.Size = new Size(207, 567);
      logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
      logoPictureBox.TabIndex = 12;
      logoPictureBox.TabStop = false;
      // 
      // labelProductName
      // 
      labelProductName.Dock = DockStyle.Fill;
      labelProductName.Location = new Point(222, 0);
      labelProductName.Margin = new Padding(7, 0, 4, 0);
      labelProductName.MaximumSize = new Size(0, 20);
      labelProductName.Name = "labelProductName";
      labelProductName.Size = new Size(428, 20);
      labelProductName.TabIndex = 19;
      labelProductName.Text = "Product Name";
      labelProductName.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelVersion
      // 
      labelVersion.Dock = DockStyle.Fill;
      labelVersion.Location = new Point(222, 36);
      labelVersion.Margin = new Padding(7, 0, 4, 0);
      labelVersion.MaximumSize = new Size(0, 20);
      labelVersion.Name = "labelVersion";
      labelVersion.Size = new Size(428, 20);
      labelVersion.TabIndex = 0;
      labelVersion.Text = "Version";
      labelVersion.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelCopyright
      // 
      labelCopyright.Dock = DockStyle.Fill;
      labelCopyright.Location = new Point(222, 71);
      labelCopyright.Margin = new Padding(7, 0, 4, 0);
      labelCopyright.MaximumSize = new Size(0, 20);
      labelCopyright.Name = "labelCopyright";
      labelCopyright.Size = new Size(428, 20);
      labelCopyright.TabIndex = 21;
      labelCopyright.Text = "Copyright";
      labelCopyright.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelCompanyName
      // 
      labelCompanyName.Dock = DockStyle.Fill;
      labelCompanyName.Location = new Point(222, 110);
      labelCompanyName.Margin = new Padding(7, 0, 4, 0);
      labelCompanyName.MaximumSize = new Size(0, 20);
      labelCompanyName.Name = "labelCompanyName";
      labelCompanyName.Size = new Size(428, 20);
      labelCompanyName.TabIndex = 22;
      labelCompanyName.Text = "Company Name";
      labelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // textBoxDescription
      // 
      textBoxDescription.Dock = DockStyle.Fill;
      textBoxDescription.Location = new Point(222, 148);
      textBoxDescription.Margin = new Padding(7, 3, 4, 3);
      textBoxDescription.Multiline = true;
      textBoxDescription.Name = "textBoxDescription";
      textBoxDescription.ReadOnly = true;
      textBoxDescription.ScrollBars = ScrollBars.Both;
      textBoxDescription.Size = new Size(428, 92);
      textBoxDescription.TabIndex = 23;
      textBoxDescription.TabStop = false;
      textBoxDescription.Text = "Description";
      // 
      // listView1
      // 
      listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      listView1.Columns.AddRange(new ColumnHeader[] { chPlugin, chVersion });
      listView1.GridLines = true;
      listView1.Location = new Point(218, 246);
      listView1.Name = "listView1";
      listView1.Size = new Size(433, 324);
      listView1.TabIndex = 25;
      listView1.UseCompatibleStateImageBehavior = false;
      listView1.View = View.Details;
      // 
      // chPlugin
      // 
      chPlugin.Text = "Plugin";
      chPlugin.Width = 120;
      // 
      // chVersion
      // 
      chVersion.Text = "Version";
      chVersion.Width = 120;
      // 
      // okButton
      // 
      okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      okButton.DialogResult = DialogResult.Cancel;
      okButton.Location = new Point(562, 576);
      okButton.Margin = new Padding(4, 3, 4, 3);
      okButton.Name = "okButton";
      okButton.Size = new Size(88, 27);
      okButton.TabIndex = 24;
      okButton.Text = "&OK";
      // 
      // statusStrip1
      // 
      statusStrip1.Location = new Point(10, 616);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Size = new Size(654, 22);
      statusStrip1.TabIndex = 1;
      statusStrip1.Text = "statusStrip1";
      // 
      // frmAbout
      // 
      AcceptButton = okButton;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(674, 648);
      Controls.Add(tableLayoutPanel);
      Controls.Add(statusStrip1);
      FormBorderStyle = FormBorderStyle.SizableToolWindow;
      Margin = new Padding(4, 3, 4, 3);
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "frmAbout";
      Padding = new Padding(10);
      ShowIcon = false;
      ShowInTaskbar = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "About BAJL Flow Engine";
      Load += frmAbout_Load;
      tableLayoutPanel.ResumeLayout(false);
      tableLayoutPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    private System.Windows.Forms.PictureBox logoPictureBox;
    private System.Windows.Forms.Label labelProductName;
    private System.Windows.Forms.Label labelVersion;
    private System.Windows.Forms.Label labelCopyright;
    private System.Windows.Forms.Label labelCompanyName;
    private System.Windows.Forms.TextBox textBoxDescription;
    private System.Windows.Forms.Button okButton;
    private ListView listView1;
    private ColumnHeader chPlugin;
    private ColumnHeader chVersion;
    private StatusStrip statusStrip1;
  }
}
