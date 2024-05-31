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
  public partial class ucParameterVarious : ucParameter
  {
    const int DT_STRING = 0;
    const int DT_INTEGER = 1;
    const int DT_DECIMAL = 2;
    const int DT_BOOLEAN = 3;
    const int DT_VARIABLE = 4;
    private int mParameterIndex = 0;
    public ucParameterVarious(PARM_VAR parmVar, FunctionStep? step, cFlowWrapper flow, int parameterIndex = 0)
    {
      InitializeComponent();
      mParmVar = parmVar;
      mFlow = flow;
      mStep = step;
      mParameterIndex = parameterIndex;
      txtKey.Text = mParmVar.ParmName;
      txtDataType.Text = mParmVar.Parm.DataType.ToString();
      

      if (mParmVar.Var.DataType == DATA_TYPE.String || mParmVar.Var.DataType == DATA_TYPE.Various)
      {
        cmbDataType.SelectedIndex = DT_STRING;
        mParmVar.GetValue(out string val);
        txtValue.Text = val;
      }
      else if (mParmVar.Var.DataType == DATA_TYPE.Integer)
      {
        cmbDataType.SelectedIndex = DT_INTEGER;
        mParmVar.GetValue(out long val);
        nudNumber.Value = val;
      }
      else if (mParmVar.Var.DataType == DATA_TYPE.Decimal)
      {
        cmbDataType.SelectedIndex = DT_DECIMAL;
        mParmVar.GetValue(out decimal val);
        nudNumber.Value = val;
      }
      else if (mParmVar.Var.DataType == DATA_TYPE.Boolean)
      {
        cmbDataType.SelectedIndex = DT_BOOLEAN;
        //TODO: implement boolean various data type
      }



      if (mParmVar.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Variable)
      {
        cmbDataType.SelectedIndex = DT_VARIABLE;
        txtValue.Text = mParmVar.VariableName;
      }

      if (mParmVar.Parm.NameChangeable == true)
      {
        txtKey.ReadOnly = false;
      }

      if (mParmVar.Parm.Description == "")
      {
        txtKey.Width = txtKey.Width + 20;
        lblDescription.Visible = false;
      }
    }


    public override void UpdateValues()
    {
      if (cmbDataType.SelectedIndex == DT_VARIABLE)
      {
        mParmVar.SetVarRef(txtValue.Text);
      }
      else if (cmbDataType.SelectedIndex == DT_STRING)
      {
        mParmVar.SetVariableLiteral(txtValue.Text);
      }
      else if (cmbDataType.SelectedIndex == DT_INTEGER)
      {
        mParmVar.SetVariableLiteral((long)nudNumber.Value);
      }
      else if (cmbDataType.SelectedIndex == DT_DECIMAL)
      {
        mParmVar.SetVariableLiteral(nudNumber.Value);
      }
      if (mParmVar.Parm.NameChangeable == true)
      {
        mParmVar.ParmName = txtKey.Text;
      }
    }
    private void ucParameterVarious_Load(object sender, EventArgs e)
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


    /// <summary>
    /// String
    /// Integer
    /// Decimal
    /// Boolean
    /// Variable
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cmbDataType.SelectedIndex == DT_STRING)
      {
        txtValue.Visible = true;
        nudNumber.Visible = false;
      }
      else if (cmbDataType.SelectedIndex == DT_INTEGER || cmbDataType.SelectedIndex == DT_DECIMAL)
      {
        txtValue.Visible = false;
        nudNumber.Visible = true;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMax, out decimal validatorVal);
        nudNumber.Maximum = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMin, out validatorVal);
        nudNumber.Minimum = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberDecimalPlaces, out validatorVal);
        nudNumber.DecimalPlaces = (int)validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberDefaultValue, out validatorVal);
        nudNumber.Value = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberIncrement, out validatorVal);
        nudNumber.Increment = validatorVal;

      }
      else if (cmbDataType.SelectedIndex == DT_BOOLEAN)
      {
        txtValue.Visible = false;
        nudNumber.Visible = false;
      }
      else if (cmbDataType.SelectedIndex == DT_VARIABLE)
      {
        txtValue.Visible = true;
        nudNumber.Visible = false;
      }
    }

    private void txtKey_TextChanged(object sender, EventArgs e)
    {
      if (mParmVar.Parm.NameChangeable == true)
      {
        mParmVar.ParmName = txtKey.Text;
      }
    }

    private void lblDescription_MouseClick(object sender, MouseEventArgs e)
    {
      frmQuickTip.ShowQuickTip(mParmVar.Parm.Description);
    }

    private void txtValue_TextChanged(object sender, EventArgs e)
    {
      if (mStep is null)
        return;
      if (cmbDataType.SelectedIndex != DT_VARIABLE)
        return;

        Variable? var = mFlow.FindVariableDesigner(txtValue.Text, mStep);
      if (var is null)
        txtValue.ForeColor = Color.Red;
      else
        txtValue.ForeColor = Color.Black;
    }
  }
}
