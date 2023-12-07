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
  public partial class ucParameterStringSql : ucParameter
  {

    public ucParameterStringSql(PARM_VAR parmVar, FunctionStep? step, cFlowWrapper flow)
    {
      InitializeComponent();
      mParmVar = parmVar;
      mFlow = flow;
      mStep = step;
      txtKey.Text = mParmVar.Parm.Name;
      txtDataType.Text = mParmVar.Parm.DataType.ToString() + "-" + mParmVar.Parm.StringSubType.ToString();
      mParmVar.GetValue(out string val);
      txtValue.Text = val;

      if (mParmVar.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Literal)
      {
        chkVariable.Checked = false;
      }
      else
      {
        chkVariable.Checked = true;
      }
    }


    public override void UpdateValues()
    {
      if (chkVariable.Checked)
      {
        mParmVar.SetVarRef(txtValue.Text);
      }
      else
      {
        mParmVar.SetVariableLiteral(txtValue.Text);
      }
    }
    private void ucParameterStringSql_Load(object sender, EventArgs e)
    {

    }

    private void btnSelectVariable_Click(object sender, EventArgs e)
    {
      frmVariableSelection f = new frmVariableSelection(mParmVar, mStep, mFlow);
      if (f.ShowDialog() == DialogResult.OK)
      {
        mParmVar.GetValue(out string val);
        txtValue.Text = val;
        chkVariable.Checked = true;
      }

    }

    private void chkVariable_CheckedChanged(object sender, EventArgs e)
    {
      if (chkVariable.Checked)
      {
        mParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Variable;
      }
      else
      {
        mParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Literal;
      }

    }

    private void btnSql_Click(object sender, EventArgs e)
    {
      frmSqlEditor f = new frmSqlEditor(txtValue);
      f.Show();
    }
  }
}
