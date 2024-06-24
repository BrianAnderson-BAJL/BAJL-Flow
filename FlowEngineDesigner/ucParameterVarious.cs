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
    //String
    //Integer
    //Decimal
    //Boolean
    //Variable
    //NONE

    private int DT_STRING = 0;
    private int DT_INTEGER = 1;
    private int DT_DECIMAL = 2;
    private int DT_BOOLEAN = 3;
    private int DT_VARIABLE = 4;
    private int DT_NONE = 5;
    private int mParameterIndex = 0;
    public ucParameterVarious(PARM_VAR parmVar, FunctionStep step, cFlowWrapper flow, int parameterIndex = 0) : base(parmVar, step, flow)
    {
      InitializeComponent();
      mParameterIndex = parameterIndex;
      txtKey.Text = mParmVar.ParmName;
      txtDataType.Text = mParmVar.Parm.DataType.ToString();

      if (parmVar.Parm.DataType == DATA_TYPE.String)
      {
        cmbDataType.Items.Clear();
        cmbDataType.Items.Add("String");
        cmbDataType.Items.Add("Variable");
        DT_VARIABLE = 1;
        DT_DECIMAL = -1;
        DT_BOOLEAN = -1;
        DT_INTEGER = -1;
        DT_NONE = -1;
      }
      if (parmVar.Parm.DataType == DATA_TYPE.Integer)
      {
        cmbDataType.Items.Clear();
        cmbDataType.Items.Add("Integer");
        cmbDataType.Items.Add("Variable");
        DT_INTEGER = 0;
        DT_VARIABLE = 1;
        DT_STRING = -1;
        DT_DECIMAL = -1;
        DT_BOOLEAN = -1;
        DT_NONE = -1;
      }
      if (parmVar.Parm.DataType == DATA_TYPE.Boolean)
      {
        cmbDataType.Items.Clear();
        cmbDataType.Items.Add("Boolean");
        cmbDataType.Items.Add("Variable");
        DT_BOOLEAN = 0;
        DT_VARIABLE = 1;
        DT_STRING = -1;
        DT_INTEGER = -1;
        DT_DECIMAL = -1;
        DT_NONE = -1;
      }
      if (parmVar.Parm.DataType == DATA_TYPE.Decimal)
      {
        cmbDataType.Items.Clear();
        cmbDataType.Items.Add("Decimal");
        cmbDataType.Items.Add("Variable");
        DT_DECIMAL = 0;
        DT_VARIABLE = 1;
        DT_STRING = -1;
        DT_INTEGER = -1;
        DT_BOOLEAN = -1;
        DT_NONE = -1;
      }
      if (parmVar.Parm.DataType == DATA_TYPE.Object)
      {
        cmbDataType.Items.Clear();
        cmbDataType.Items.Add("Variable");
        DT_VARIABLE = 0;
        DT_STRING = -1;
        DT_INTEGER = -1;
        DT_BOOLEAN = -1;
        DT_NONE = -1;
        DT_DECIMAL = -1;
        mParmVar.ParmLiteralOrVariable = PARM_VAR.PARM_L_OR_V.Variable; //It's an object, it needs to be a variable
      }

      if (mParmVar.ParmLiteralOrVariable == PARM_VAR.PARM_L_OR_V.Literal)
      {
        if (mParmVar.Var.DataType == DATA_TYPE.String || mParmVar.Var.DataType == DATA_TYPE.Various)
        {
          cmbDataType.SelectedIndex = DT_STRING;
          mParmVar.GetValue(out string val);
          txtValue.Enabled = true;
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
          mParmVar.GetValue(out bool val);
          chkValue.Checked = val;
        }
        else if (mParmVar.Var.DataType == DATA_TYPE._None)
        {
          cmbDataType.SelectedIndex = DT_NONE;
          txtValue.Text = "";
          txtValue.Enabled = false;
        }
      }
      else
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
        mParmVar.Var.DataType = DATA_TYPE.Various;

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
      else if (cmbDataType.SelectedIndex == DT_BOOLEAN)
      {
        mParmVar.SetVariableLiteral(chkValue.Checked);
      }
      else if (cmbDataType.SelectedIndex == DT_NONE)
      {
        mParmVar.SetDataTypeToNone();
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
        txtValue.Enabled = true;
        txtValue.Visible = true;
        nudNumber.Visible = false;
        chkValue.Visible = false;
        ValidateVariable(); //Not a variable, but a string, but this will change the text color back to black if it was highlighted in red
      }
      else if (cmbDataType.SelectedIndex == DT_INTEGER)
      {
        txtValue.Visible = false;
        nudNumber.Visible = true;
        chkValue.Visible = false;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMax, out decimal validatorVal);
        nudNumber.Maximum = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMin, out validatorVal);
        nudNumber.Minimum = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberIncrement, out validatorVal);
        nudNumber.Increment = validatorVal;

      }
      else if (cmbDataType.SelectedIndex == DT_DECIMAL)
      {
        txtValue.Visible = false;
        nudNumber.Visible = true;
        chkValue.Visible = false;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMax, out decimal validatorVal);
        nudNumber.Maximum = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberMin, out validatorVal);
        nudNumber.Minimum = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberDecimalPlaces, out validatorVal, 5); //defaults to 5 decimal places for decimal
        nudNumber.DecimalPlaces = (int)validatorVal;
        //mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberDefaultValue, out validatorVal);
        //nudNumber.Value = validatorVal;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.NumberIncrement, out validatorVal);
        nudNumber.Increment = validatorVal;

      }
      else if (cmbDataType.SelectedIndex == DT_BOOLEAN)
      {
        txtValue.Visible = false;
        nudNumber.Visible = false;
        chkValue.Visible = true;
        mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.BooleanDefaultValue, out bool validatorVal);
        chkValue.Checked = validatorVal;
      }
      else if (cmbDataType.SelectedIndex == DT_VARIABLE)
      {
        txtValue.Enabled = true;
        txtValue.Visible = true;
        nudNumber.Visible = false;
        chkValue.Visible = false;
        ValidateVariable();
      }
      else if (cmbDataType.SelectedIndex == DT_NONE)
      {
        txtValue.Visible = true;
        txtValue.Text = "";
        txtValue.Enabled = false;
        mParmVar.SetDataTypeToNone();
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
      ValidateVariable();
    }

    private void ValidateVariable()
    {
      if (mStep is null)
        return;
      if (cmbDataType.SelectedIndex != DT_VARIABLE)
      {
        txtValue.ForeColor = Color.Black;
        return;
      }
      Variable? var = mFlow.FindVariableDesigner(txtValue.Text, mStep);
      if (var is null)
        txtValue.ForeColor = Color.Red;
      else
        txtValue.ForeColor = Color.Black;
    }
  }
}
