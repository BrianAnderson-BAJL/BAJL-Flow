using FlowEngineCore;
using Microsoft.VisualBasic.Devices;
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
  public partial class ucParameterString : ucParameter
  {
    const int DT_STRING = 0;
    const int DT_VARIABLE = 1;

    public ucParameterString(PARM_VAR parmVar, FunctionStep? step, cFlowWrapper flow)
    {
      InitializeComponent();
      mParmVar = parmVar;
      mFlow = flow;
      mStep = step;
      txtKey.Text = mParmVar.Parm.Name;
      if (mParmVar.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Literal)
      {
        cmbDataType.SelectedIndex = DT_STRING;
      }
      else
      {
        cmbDataType.SelectedIndex = DT_VARIABLE;
      }


      string val = "";
      if (parmVar.Parm.DataType == DATA_TYPE.Object)
      {
        val = parmVar.VariableName;
        cmbDataType.SelectedIndex = DT_VARIABLE;
      }
      else
      {
        mParmVar.GetValue(out val);
      }
      txtValue.Text = val;

    }


    public override void UpdateValues()
    {
      if (cmbDataType.SelectedIndex == DT_VARIABLE)
      {
        mParmVar.SetVarRef(txtValue.Text);
      }
      else
      {
        mParmVar.SetVariableLiteral(txtValue.Text);
      }
    }

    private void ucParameterString_Load(object sender, EventArgs e)
    {

    }

    private void btnSelectVariable_Click(object sender, EventArgs e)
    {
      frmVariableSelection f = new frmVariableSelection(mParmVar, mStep, mFlow);
      f.SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
      if (f.ShowDialog() == DialogResult.OK)
      {
        mParmVar.GetValue(out string val);
        txtValue.Text = val;
        cmbDataType.SelectedIndex = DT_VARIABLE;
      }
    }


    private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cmbDataType.SelectedIndex == DT_VARIABLE)
      {
        mParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Variable;
      }
      else
      {
        mParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Literal;
      }

    }

    private void txtValue_TextChanged(object sender, EventArgs e)
    {
      if (mStep is null)
        return;

      Variable? var = mFlow.FindVariableDesigner(txtValue.Text, mStep);
      if (var is null)
        txtValue.ForeColor = Color.Red;
      else
        txtValue.ForeColor = Color.Black;

    }
  }
}
