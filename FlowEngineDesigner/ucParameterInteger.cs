using Core;
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
  public partial class ucParameterInteger : ucParameter
  {
    private PARM_VAR mParmVar;
    public ucParameterInteger(PARM_VAR parmVar)
    {
      InitializeComponent();
      mParmVar = parmVar;
      txtKey.Text = mParmVar.Parm.Name;
      txtDataType.Text = mParmVar.Parm.DataType.ToString();
      //nudValue.Minimum = mParmVar.Parm.Min;
      //nudValue.Maximum = mParmVar.Parm.Max;
      //nudValue.Increment = mParmVar.Parm.Increment;
      mParmVar.GetValue(out long val);
      nudValue.Value = val;
    }

    private void ucParameterInteger_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateValues()
    {
      mParmVar.SetVariableLiteral((long)nudValue.Value);
    }
  }
}
