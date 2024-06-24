using FlowEngineCore;
using FlowEngineCore.Interfaces;
using FlowEngineCore.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace FlowEngineDesigner
{
  public partial class frmSqlEditor : Form
  {
    private string FileName = "";
    private bool Parsing = false;
    private TextBox TxtSql;
    private static readonly string[] Delimiters = { ",", " ", "(", ")", "\n", "\t", "=", "<=", ">=", "<", ">", "." };
    private bool DoubleClickedNode = false;
    private bool Saved = true;
    private IDatabase? Db = null;
    private FunctionStep Step;
    private ComboBox mComboBoxParameterDataType;
    private TextBox mTextBoxParameterValue;
    const int LIMIT_RECORDS = 100;
    public frmSqlEditor(TextBox txtSql, FunctionStep step)
    {
      InitializeComponent();
      TxtSql = txtSql;
      rtbSql.Text = TxtSql.Text;
      ParseSql();
      rtbSql.SelectionStart = rtbSql.Text.Length;
      Saved = true;
      Step = step;
      lblErrorDescription.Text = "";
      mTextBoxParameterValue = new TextBox();
      mTextBoxParameterValue.TextChanged += mTextBoxParameterValue_TextChanged;
      mComboBoxParameterDataType = new System.Windows.Forms.ComboBox();
      mComboBoxParameterDataType.Items.Add("Integer");
      mComboBoxParameterDataType.Items.Add("String");
      mComboBoxParameterDataType.Items.Add("Boolean");
      mComboBoxParameterDataType.Items.Add("Decimal");
      mComboBoxParameterDataType.DropDownStyle = ComboBoxStyle.DropDownList;
      mComboBoxParameterDataType.SelectedIndexChanged += mComboBox_SelectedIndexChanged;

      if (Step.Name == "Database.Execute")
      {
        chkSeeResultRecords.Checked = false;
        chkSeeResultRecords.Enabled = false;
      }
    }

    private void mTextBoxParameterValue_TextChanged(object? sender, EventArgs e)
    {
      if (mTextBoxParameterValue.Tag is Tuple<int, int> cellPosition)
      {
        int rowIndex = cellPosition.Item1;
        int colIndex = cellPosition.Item2;

        lvParams.Items[rowIndex].SubItems[colIndex].Text = mTextBoxParameterValue.Text;
      }
    }

    private void mComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
      if (mComboBoxParameterDataType.Tag is Tuple<int, int> cellPosition)
      {
        int rowIndex = cellPosition.Item1;
        int colIndex = cellPosition.Item2;

        // Update the ListView item with the selected ComboBox value
        lvParams.Items[rowIndex].SubItems[colIndex].Text = mComboBoxParameterDataType.SelectedItem.ToString();
      }
    }

    private void rtbSql_TextChanged(object sender, EventArgs e)
    {
      ParseSql();
      Saved = false;
      lblTestSqlResults.Text = "";
    }

    private void ParseSql()
    {
      if (Parsing == true) //We don't want to overflow the stack...
        return;

      Parsing = true;
      int start = rtbSql.SelectionStart;
      SqlParser parser = new SqlParser();
      parser.ParseSql(rtbSql.Text);
      string val = parser.GetRtfText();
      rtbSql.Rtf = val;
      rtbSql.SelectionStart = start;
      Parsing = false;

      List<string> parms = parser.GetListOf(SqlParser.ParsedUnit.UNIT_TYPE.Parameter);
      lvParams.Items.Clear();
      for (int x = 0; x < parms.Count; x++)
      {
        ListViewItem lvi = lvParams.Items.Add(parms[x]);
        lvi.SubItems.Add("String");
        lvi.SubItems.Add("");
      }
    }

    private void frmSqlEditor_Load(object sender, EventArgs e)
    {
      btnTestSql.Enabled = false;
      lblTestSqlResults.Visible = false;

      if (FlowEngineCore.PluginManager.GlobalPluginValues.ContainsKey("db") == true)
        Db = FlowEngineCore.PluginManager.GlobalPluginValues["db"] as IDatabase;
      if (Db is null)
        return;

      btnTestSql.Enabled = true;
      lblTestSqlResults.Visible = true;
      Task.Run(() => PopulateDatabase(Db));
    }

    private void PopulateDatabase(IDatabase db)
    {
      List<string> tableNames = db.GetTables();
      tableNames.Sort();
      List<string> fieldNames = new List<string>();
      for (int x = 0; x < tableNames.Count; x++)
      {
        fieldNames = db.GetFields(tableNames[x]);
      }


      this.Invoke((MethodInvoker)delegate
      {
        for (int x = 0; x < tableNames.Count; x++)
        {
          TreeNode tn = tvDatabase.Nodes.Add(tableNames[x]);
          for (int y = 0; y < fieldNames.Count; y++)
          {
            tn.Nodes.Add(fieldNames[y]);
          }
        }
      });

    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      openFileDialog1.Filter = "SQL file (*.sql)|*.sql|All files (*.*)|*.*";
      openFileDialog1.Multiselect = true;
      saveFileDialog1.FileName = "";
      openFileDialog1.DefaultExt = "sql";
      DialogResult dr = openFileDialog1.ShowDialog();
      if (dr == DialogResult.OK)
      {
        rtbSql.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8);
      }

    }


    private void SaveAs()
    {
      saveFileDialog1.Filter = "SQL file (*.sql)|*.sql|All files (*.*)|*.*";
      saveFileDialog1.DefaultExt = "sql";
      saveFileDialog1.FileName = FileName;
      saveFileDialog1.OverwritePrompt = true;
      DialogResult dr = saveFileDialog1.ShowDialog();
      if (dr == DialogResult.OK)
      {
        File.WriteAllText(saveFileDialog1.FileName, rtbSql.Text, Encoding.UTF8);
        Saved = true;
      }
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveAs();
    }

    private void frmSqlEditor_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (Saved == false)
      {
        if (MessageBox.Show("You have unsaved changes. Do you want to save your work?", "Cancel closing?", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
        {
          e.Cancel = true;
        }
      }
    }

    private void samplesToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void rtbSql_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      int curserPos = rtbSql.SelectionStart;
      string wholeText = rtbSql.Text;

      string selectedText = rtbSql.SelectedText;

      if (selectedText.Length > 0)
      {
        int endSelect = FindIndexOfDelimterForward(ref wholeText, curserPos); //Later in the string
        int startSelect = FindIndexOfDelimterBackward(ref wholeText, curserPos); //Earlier in the string
        rtbSql.SelectionStart = startSelect;
        rtbSql.SelectionLength = endSelect - startSelect;
      }
    }

    private int FindIndexOfDelimterForward(ref string sql, int startPos)
    {
      int minIndex = int.MaxValue;
      for (int x = 0; x < Delimiters.Length; x++)
      {
        int index = sql.IndexOf(Delimiters[x], startPos);
        if (index > -1 && index < minIndex)
        {
          minIndex = index;
        }
      }
      if (minIndex < int.MaxValue)
      {
        return minIndex;
      }
      else
      {
        return sql.Length;
      }
    }
    private int FindIndexOfDelimterBackward(ref string sql, int maxPosition)
    {
      if (maxPosition == 0)
        return maxPosition;

      int minIndex = maxPosition;
      int startIndex = maxPosition;
      do
      {
        startIndex--;
        for (int x = 0; x < Delimiters.Length; x++)
        {
          int index = sql.IndexOf(Delimiters[x], startIndex);
          if (index > -1 && index < minIndex)
          {
            return index + 1; //Found a delimiter, but we want the letter after it, or before it depending on your point of view since we are going backwards.
          }
        }
      } while (startIndex > 0);
      return 0; //Hit the beginning of the text
    }

    private void innerJoinToolStripMenuItem_Click(object sender, EventArgs e)
    {
      rtbSql.SelectedText = "INNER JOIN Table_B_Name ON Table_A_Name.Id = Table_B_Name.Id";
      ParseSql();
    }

    private void leftJoinToolStripMenuItem_Click(object sender, EventArgs e)
    {
      rtbSql.SelectedText = "LEFT JOIN Table_B_Name ON Table_A_Name.id = Table_B_Name.id";
      ParseSql();
    }

    private void rightJoinToolStripMenuItem_Click(object sender, EventArgs e)
    {
      rtbSql.SelectedText = "RIGHT JOIN Table_B_Name ON Table_A_Name.id = Table_B_Name.id";
      ParseSql();
    }

    private void fullJoinFullOuterJoinToolStripMenuItem_Click(object sender, EventArgs e)
    {
      rtbSql.SelectedText = "FULL JOIN Table_B_Name ON Table_A_Name.id = Table_B_Name.id";
      ParseSql();

    }

    private void simpleToolStripMenuItem_Click(object sender, EventArgs e)
    {
      rtbSql.SelectedText = "SELECT LoginId, StatusId, Active\r\nFROM Users\r\nWHERE Active = @Param1";
      ParseSql();

    }

    private void groupByHavingToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //\\'28\\'2a\\'29\\'20\\'41\\'53\\'20\\'4e\\'75\\'6d
      rtbSql.SelectedText = "SELECT Users.StatusId, Count(*) AS Num \r\nFROM Users \r\nWHERE Active = @ParamActive AND MONTH(CreatedDateTime) = @ParamMonthNumber \r\nGROUP BY Users.StatusId \r\nHAVING Num > 0 \r\nORDER BY Num DESC";
      ParseSql();
    }

    private void joinToLookUpToolStripMenuItem_Click(object sender, EventArgs e)
    {
      rtbSql.SelectedText = "SELECT Users.LoginId, Users.StatusId, luUserStatus.Descriptoin\r\nFROM Users LEFT JOIN luUserStatus ON Users.StatusId = luUserStatus.Id";
      ParseSql();
    }

    private void showDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
      showDatabaseToolStripMenuItem.Checked = !showDatabaseToolStripMenuItem.Checked;

      if (showDatabaseToolStripMenuItem.Checked == true)
      {
        scSql.Panel2Collapsed = false;
        scSql.Panel2.Show();
      }
      else
      {
        scSql.Panel2Collapsed = true;
        scSql.Panel2.Hide();
      }
    }

    private void tvDatabase_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      rtbSql.SelectedText = e.Node.Text;
    }

    private void tvDatabase_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Clicks > 1)
      {
        DoubleClickedNode = true;
      }
    }

    private void tvDatabase_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
    {
      if (DoubleClickedNode == true && e.Action == TreeViewAction.Collapse)
      {
        e.Cancel = true;
        DoubleClickedNode = false;
      }
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      TxtSql.Text = "";
      TxtSql.Lines = rtbSql.Lines;
      Saved = true;
      this.Close();
    }

    private void leftJoinMultipleTablesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      rtbSql.SelectedText = "SELECT Users.LoginId, UserDevices.DeviceToken, UserSessions.SessionToken \r\nFROM ((Users LEFT JOIN UserSessions ON(Users.UserId = UserSessions.UserId)) \r\nLEFT JOIN UserDevices ON (Users.UserId = UserDevices.UserId))";
      ParseSql();
    }

    private void btnTestSql_Click(object sender, EventArgs e)
    {
      if (Db is null)
        return;

      Variable[] vars = new Variable[lvParams.Items.Count];
      string lastParam = "";
      try
      {
        for (int x = 0; x < lvParams.Items.Count; x++)
        {
          ListViewItem lvi = lvParams.Items[x];
          lastParam = lvi.SubItems[0].Text;
          Variable v = new Variable(lvi.SubItems[0].Text);
          DATA_TYPE dataType = Enum.Parse<DATA_TYPE>(lvi.SubItems[1].Text);
          v.DataType = dataType;
          if (dataType == DATA_TYPE.Integer)
            v.Value = Convert.ToInt64(lvi.SubItems[2].Text);
          else if (dataType == DATA_TYPE.Decimal)
            v.Value = Convert.ToDecimal(lvi.SubItems[2].Text);
          else if (dataType == DATA_TYPE.Boolean)
            v.Value = 1;
          else
            v.Value = lvi.SubItems[2].Text;
          vars[x] = v;
        }
      }
      catch (Exception ex)
      {
        lblTestSqlResults.Text = "FAILURE";
        lblTestSqlResults.ForeColor = System.Drawing.Color.Red;
        lblErrorDescription.Text = $"{lastParam} - {ex.Message}";
        return;
      }

      if (Step.Name == "Database.Select" || Step.Name == "Database.Select Single")
      {
        try
        {
          string sql = rtbSql.Text;
          sql = sql.Trim();
          if (sql.Length > 0)
          {
            if (sql[sql.Length - 1] == ';')
              sql = sql.Substring(0, sql.Length - 1) + " LIMIT " + LIMIT_RECORDS + ";";
            else
              sql = sql + " LIMIT " + LIMIT_RECORDS;
          }
          Variable recordset = Db.Select(sql, vars);
          lblTestSqlResults.Text = "SUCCESS";
          lblTestSqlResults.ForeColor = System.Drawing.Color.Green;
          lblErrorDescription.Text = "";
          if (chkSeeResultRecords.Checked == true)
          {
            frmSqlRecords f = new frmSqlRecords(recordset);
            f.Show();
          }
        }
        catch (Exception ex)
        {
          lblTestSqlResults.Text = "FAILURE";
          lblTestSqlResults.ForeColor = System.Drawing.Color.Red;
          if (ex.InnerException is not null)
            lblErrorDescription.Text = ex.InnerException.Message;
          else
            lblErrorDescription.Text = ex.Message;
        }
      }
      else
      {
        object conn = Db.TransactionBegin();
        try
        {
          Db.Execute(conn, rtbSql.Text, vars);
          lblTestSqlResults.Text = "SUCCESS";
          lblTestSqlResults.ForeColor = System.Drawing.Color.Green;
          lblErrorDescription.Text = "";
        }
        catch (Exception ex)
        {
          lblTestSqlResults.Text = "FAILURE";
          lblTestSqlResults.ForeColor = System.Drawing.Color.Red;
          if (ex.InnerException is not null)
            lblErrorDescription.Text = ex.InnerException.Message;
          else
            lblErrorDescription.Text = ex.Message;
        }
        Db.TransactionRollback(conn);
      }
    }

    private void lvParams_MouseClick(object sender, MouseEventArgs e)
    {
     
    }

    private void lvParams_MouseDown(object sender, MouseEventArgs e)
    {
      var hitTest = lvParams.HitTest(e.Location);
      if (hitTest.Item != null && hitTest.SubItem != null)
      {
        int rowIndex = hitTest.Item.Index;
        int colIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
        Rectangle rect = hitTest.SubItem.Bounds;
        if (colIndex == 1) // Data Type combobox
        {
          lvParams.Controls.Remove(mTextBoxParameterValue);
          mComboBoxParameterDataType.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
          mComboBoxParameterDataType.Tag = new Tuple<int, int>(rowIndex, colIndex);
          lvParams.Controls.Add(mComboBoxParameterDataType);
          mComboBoxParameterDataType.BringToFront();
          
          Global.ComboBoxSetIndex(mComboBoxParameterDataType, hitTest.SubItem.Text);
          return;
        }
        else if (colIndex == 2) // Value textbox
        {
          lvParams.Controls.Remove(mComboBoxParameterDataType);
          mTextBoxParameterValue.Text = hitTest.SubItem.Text;
          mTextBoxParameterValue.SetBounds(rect.X + 6, rect.Y + 2, rect.Width - 6, rect.Height - 2); //Cells seem to have a little padding
          mTextBoxParameterValue.Visible = true;
          mTextBoxParameterValue.BorderStyle = BorderStyle.None;
          mTextBoxParameterValue.Tag = new Tuple<int, int>(rowIndex, colIndex);
          lvParams.Controls.Add(mTextBoxParameterValue);
          mTextBoxParameterValue.BringToFront();
          this.ActiveControl = mTextBoxParameterValue;
          //Ugly hack to get the control to activate, doesn't seem to activate otherwise when a control is newly added, don't know why
          Task.Delay(100).ContinueWith(_ =>
          {
            this.Invoke(new Action(() =>
            {
              mTextBoxParameterValue.Focus();
            }));
          });
          return;
        }
      }

      //If we fell through then we didn't add a edit control
      lvParams.Controls.Remove(mComboBoxParameterDataType);
      lvParams.Controls.Remove(mTextBoxParameterValue);
    }
  }
}
