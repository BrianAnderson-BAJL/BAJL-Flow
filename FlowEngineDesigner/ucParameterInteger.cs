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
    private cFlowWrapper mFlow;
    private FunctionStep? mStep;

    public ucParameterInteger(PARM_VAR parmVar, FunctionStep? step, cFlowWrapper flow)
    {
      InitializeComponent();
      mParmVar = parmVar;
      mFlow = flow;
      mStep = step;

      txtKey.Text = mParmVar.Parm.Name;
      txtDataType.Text = mParmVar.Parm.DataType.ToString();

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
      nudValue.Value = val;
    }

    private void ucParameterInteger_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateValues()
    {
      if (chkVariable.Checked == false)
      {
        mParmVar.SetVariableLiteral((long)nudValue.Value);
      }
      else
      {
        mParmVar.SetVarRef(txtVariableName.Text);
      }
    }

    private void nudValue_ValueChanged(object sender, EventArgs e)
    {

    }

    private void chkVariable_CheckedChanged(object sender, EventArgs e)
    {
      txtVariableName.Visible = chkVariable.Checked;
    }

    private void btnSelectVariable_Click(object sender, EventArgs e)
    {
      frmVariableSelection f = new frmVariableSelection(mParmVar, mStep, mFlow);
      if (f.ShowDialog() == DialogResult.OK)
      {
        mParmVar.GetValue(out string val);
        txtVariableName.Text = val;
        chkVariable.Checked = true;
      }

    }
  }
}
