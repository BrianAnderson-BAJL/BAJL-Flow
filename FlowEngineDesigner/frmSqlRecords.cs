using FlowEngineCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowEngineDesigner
{
  public partial class frmSqlRecords : Form
  {
    private Variable RecordSet;
    public frmSqlRecords(Variable recordset)
    {
      InitializeComponent();
      RecordSet = recordset;
    }

    private void frmSqlRecords_Load(object sender, EventArgs e)
    {
      lvRecords.Columns.Clear();
      if (RecordSet.Count == 0)
      {
        ColumnHeader ch = new ColumnHeader();
        ch.Width = 180;
        ch.Text = "No Records returned";
        lvRecords.Columns.Add(ch);
        return;
      }
      
      for (int row = 0; row < RecordSet.Count; row++)
      {
        Variable record = RecordSet[row];
        if (lvRecords.Columns.Count == 0)
          PopulateColumns(record);

        if (record.Count == 0)
          continue;
        Variable field = record[0];
        ListViewItem lvi = lvRecords.Items.Add(field.GetValueAsString());

        for (int col = 1; col < record.Count; col++)
        {
          lvi.SubItems.Add(record[col].GetValueAsString());
        }
      }
    }

    private void PopulateColumns(Variable record)
    {
      for (int col = 0; col < record.Count; col++)
      {
        Variable field = record[col];
        ColumnHeader ch = new ColumnHeader();
        if (field.DataType == DATA_TYPE.String)
          ch.Width = 180;
        ch.Text = field.Name;
        lvRecords.Columns.Add(ch);
      }

    }
  }
}
