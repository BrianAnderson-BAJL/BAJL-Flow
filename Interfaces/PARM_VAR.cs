using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  /// <summary>
  /// A PARM is how the parameter is defined by the developer
  /// A Variable is what is actually passed to the function/step
  /// A PARM_VAR is the in between stage where you are linking a Variable/Literal to the parameter to be passed to the function/step
  /// The PARM_VAR values are what will be stored in the .flow file
  /// </summary>
  public class PARM_VAR
  {

    /// <summary>
    /// Is the parameter a literal or a variable
    /// </summary>
    public enum PARM_L_OR_V
    {
      Literal,
      Variable,
    }

    public PARM Parm;
    private Variable mVar;
    private VarRef? VarName = null;
    public PARM_L_OR_V ParmLiteralOrVariable; //This value will determine if we use Var or VarName when accessing the data
    public string ParmName;

    public Variable Var
    {
      get { return mVar; }
    }

    private PARM_VAR(PARM parm, Variable var)
    {
      this.Parm = parm;
      this.mVar = var;
      this.ParmName = parm.Name;
    }

    public string VariableName
    {
      get 
      {
        if (VarName is not null)
          return VarName.Value;
        else
          return "";
      }
    }

    public void SetVarRef(string varName)
    {
      VarName = new VarRef(varName);
      mVar = new Variable();
      ParmLiteralOrVariable = PARM_L_OR_V.Variable;
    }

    public void SetDataTypeToNone()
    {
      VarName = null;
      mVar = new Variable();
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }


    public void SetVariableLiteral(string val)
    {
      if (Parm.DataType != DATA_TYPE.String && Parm.StringSubType != STRING_SUB_TYPE.DropDownList && Parm.DataType != DATA_TYPE.Various)
        throw new ArgumentException("PARM_VAR.SetVariableLiteral(string) -  DataType is not a String");
      mVar = new Variable(Parm.Name, val);
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }

    public void SetVariableLiteral(long val)
    {
      if (Parm.DataType != DATA_TYPE.Integer && Parm.DataType != DATA_TYPE.Various)
        throw new ArgumentException("PARM_VAR.SetVariableLiteral(long) -  DataType is not an Integer");
      mVar = new Variable(Parm.Name, val);
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }
    public void SetVariableLiteral(bool val)
    {
      if (Parm.DataType != DATA_TYPE.Boolean && Parm.DataType != DATA_TYPE.Various)
        throw new ArgumentException("PARM_VAR.SetVariableLiteral(long) -  DataType is not a Boolean");
      mVar = new Variable(Parm.Name, val);
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }
    public void SetVariableLiteral(decimal val)
    {
      if (Parm.DataType != DATA_TYPE.Decimal && Parm.DataType != DATA_TYPE.Various)
        throw new ArgumentException("PARM_VAR.SetVariableLiteral(decimal) -  DataType is not an Decimal");
      mVar = new Variable(Parm.Name, val);
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }

    public void SetVariableLiteral(Variable var)
    {
      mVar = var;
    }

    public void GetValue(out Variable? val, Flow? flow = null)
    {
      val = null;
      if (ParmLiteralOrVariable == PARM_L_OR_V.Literal)
      {
        val = Var;
      }
      else if (VarName is not null && flow is not null)
      {
        val = flow.FindVariable(VarName.Value);
      }
    }

    /// <summary>
    /// Only set the flow parameter if you want to resolve the variable
    /// </summary>
    /// <param name="val"></param>
    /// <param name="flow"></param>
    public bool GetValue(out string val, Flow? flow = null)
    {
      val = "";
      if (ParmLiteralOrVariable == PARM_L_OR_V.Literal)
      {
        Variable? vs = Var as Variable;
        if (vs is not null)
        {
          val = vs.Value;
          return true;
        }
      }
      else if (VarName is not null && flow is null)
      {
        val = VarName.Value;
      }
      else if (VarName is not null && flow is not null)
      {
        Variable? vs = flow.FindVariable(VarName.Value);
        if (vs is not null)
        {
          val = vs.Value;
          return true;
        }
      }
      return false;
    }



    public void GetValue(out long val, Flow? flow = null)
    {
      val = 0;
      if (ParmLiteralOrVariable == PARM_L_OR_V.Literal)
      {
        if (Var is not null)
          val = Var.Value;
      }
      else if (VarName is not null)
      {
        throw new NotImplementedException();
      }
    }

    public void GetValue(out decimal val, Flow? flow = null)
    {
      val = 0;
      if (ParmLiteralOrVariable == PARM_L_OR_V.Literal)
      {
        if (Var is not null)
          val = Var.Value;
      }
      else if (VarName is not null)
      {
        throw new NotImplementedException();
      }
    }

    public void GetValue(out bool val, Flow? flow = null)
    {
      val = false;
      if (ParmLiteralOrVariable == PARM_L_OR_V.Literal)
      {
        if (Var is not null)
          val = Var.Value;
      }
      else if (VarName is not null)
      {
        throw new NotImplementedException();
      }
    }

    public PARM_VAR(PARM parm, VarRef value)
    {
      Parm = parm;
      mVar = new Variable();
      VarName = value;
      ParmLiteralOrVariable = PARM_L_OR_V.Variable;
      this.ParmName = parm.Name;
    }

    public PARM_VAR(PARM parm)
    {
      Parm = parm;
      mVar = new Variable();
      if (parm.DataType == DATA_TYPE.Various)
        Var.DataType = DATA_TYPE.Various;

      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
      this.ParmName = parm.Name;
    }

    public PARM_VAR(PARM parm, long value)
    {
      Parm = parm;
      mVar = new Variable(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
      this.ParmName = parm.Name;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parm">The parameter that this value will be assigned to</param>
    /// <param name="value">If this is a literal, then this should be the actual string value. If it is a Variable then it should be fully qualified Variable name (flow_start.json.val1)</param>
    /// <param name="litOrVar"></param>
    public PARM_VAR(PARM parm, string value)
    {
      Parm = parm;
      mVar = new Variable(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
      this.ParmName = parm.Name;
    }

    public PARM_VAR(PARM parm, object value)
    {
      Parm = parm;
      mVar = new Variable(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
      this.ParmName = parm.Name;
    }

    public PARM_VAR(PARM parm, decimal value)
    {
      Parm = parm;
      mVar = new Variable(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
      this.ParmName = parm.Name;
    }

    public PARM_VAR(PARM parm, bool value)
    {
      Parm = parm;
      mVar = new Variable(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
      this.ParmName = parm.Name;
    }

    public PARM_VAR Clone()
    {
      PARM_VAR pv = new PARM_VAR(this.Parm, this.Var.Clone()) ;
      pv.VarName = this.VarName;
      pv.ParmLiteralOrVariable = this.ParmLiteralOrVariable;
      pv.ParmName = this.ParmName;
      
      return pv;
    }

    public override string ToString()
    {
      if (this.ParmLiteralOrVariable == PARM_L_OR_V.Literal)
      {
        return this.Var.GetValueAsString();
      }
      else
      {
        return this.VariableName;
      }
    }
  }
}
