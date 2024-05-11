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
  public partial class ucParameterInteger : ucParameter
  {
    const int DT_INTEGER = 0;
    const int DT_VARIABLE = 1;


    public ucParameterInteger(PARM_VAR parmVar, FunctionStep? step, cFlowWrapper flow)
    {
      InitializeComponent();
      mParmVar = parmVar;
      mFlow = flow;
      mStep = step;

      txtKey.Text = mParmVar.Parm.Name;
      

      parmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMax, out decimal validatorVal);
      nudValue.Maximum = validatorVal;
      parmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMin, out validatorVal);
      nudValue.Minimum = validatorVal;
      parmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberDecimalPlaces, out validatorVal);
      nudValue.DecimalPlaces = (int)validatorVal;
      parmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberDefaultValue, out validatorVal);
      nudValue.Value = validatorVal;
      parmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberIncrement, out validatorVal);
      nudValue.Increment = validatorVal;

      mParmVar.GetValue(out long val);
      if (val < nudValue.Minimum)
        val = (long)nudValue.Minimum;
      if (val > nudValue.Maximum)
        val = (long)nudValue.Maximum;

      if (mParmVar.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Literal)
      {
        cmbDataType.SelectedIndex = DT_INTEGER;
      }
      else
      {
        cmbDataType.SelectedIndex = DT_VARIABLE;
      }

      nudValue.Value = val;
    }

    private void ucParameterInteger_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateValues()
    {
      if (cmbDataType.SelectedIndex == DT_VARIABLE)
      {
        mParmVar.SetVarRef(txtVariableName.Text);
      }
      else
      {
        mParmVar.SetVariableLiteral((long)nudValue.Value);
      }
    }

    private void nudValue_ValueChanged(object sender, EventArgs e)
    {

    }


    private void btnSelectVariable_Click(object sender, EventArgs e)
    {
      frmVariableSelection f = new frmVariableSelection(mParmVar, mStep, mFlow);
      f.SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
      if (f.ShowDialog() == DialogResult.OK)
      {
        mParmVar.GetValue(out string val);
        txtVariableName.Text = val;
        mParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Variable;
      }

    }
  }
}
