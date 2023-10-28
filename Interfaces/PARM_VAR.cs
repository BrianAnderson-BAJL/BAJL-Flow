using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
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
    private Variable Var;
    private VarRef? VarName = null;
    public PARM_L_OR_V ParmLiteralOrVariable; //This value will determine if we use Var or VarName when accessing the data

    private PARM_VAR(PARM parm, Variable var)
    {
      this.Parm = parm;
      this.Var = var;
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
      Var = new Variable();
      ParmLiteralOrVariable = PARM_L_OR_V.Variable;
    }

    public void SetVariableLiteral(string val)
    {
      if (Parm.DataType != DATA_TYPE.String && Parm.DataType != DATA_TYPE.DropDownList)
        throw new ArgumentException("PARM_VAR.SetVariableLiteral(string) -  DataType is not a String");
      Var = new VariableString(Parm.Name, val);
    }

    public void SetVariableLiteral(long val)
    {
      if (Parm.DataType != DATA_TYPE.Integer)
        throw new ArgumentException("PARM_VAR.SetVariableLiteral(long) -  DataType is not an Integer");
      Var = new VariableInteger(Parm.Name, val);
    }

    public void SetVariableLiteral(Variable var)
    {
      Var = var;
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
    /// <exception cref="NotImplementedException"></exception>
    public bool GetValue(out string val, Flow? flow = null)
    {
      val = "";
      if (ParmLiteralOrVariable == PARM_L_OR_V.Literal)
      {
        VariableString? vs = Var as VariableString;
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
        VariableString? vs = flow.FindVariable(VarName.Value) as VariableString;
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
        VariableInteger? vs = Var as VariableInteger;
        if (vs is not null)
          val = vs.Value;
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
        VariableDecimal? vs = Var as VariableDecimal;
        if (vs is not null)
          val = vs.Value;
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
        VariableBoolean? vs = Var as VariableBoolean;
        if (vs is not null)
          val = vs.Value;
      }
      else if (VarName is not null)
      {
        throw new NotImplementedException();
      }
    }

    public PARM_VAR(PARM parm, VarRef value)
    {
      Parm = parm;
      Var = new Variable();
      VarName = value;
      ParmLiteralOrVariable = PARM_L_OR_V.Variable;
    }

    public PARM_VAR(PARM parm, long value)
    {
      Parm = parm;
      Var = new VariableInteger(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
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
      Var = new VariableString(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }

    public PARM_VAR(PARM parm, object value)
    {
      Parm = parm;
      Var = new VariableObject(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }

    public PARM_VAR(PARM parm, decimal value)
    {
      Parm = parm;
      Var = new VariableDecimal(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }

    public PARM_VAR(PARM parm, bool value)
    {
      Parm = parm;
      Var = new VariableBoolean(parm.Name, value);
      VarName = null;
      ParmLiteralOrVariable = PARM_L_OR_V.Literal;
    }

    public PARM_VAR Clone()
    {
      PARM_VAR pv = new PARM_VAR(this.Parm, this.Var.Clone()) ;
      pv.VarName = this.VarName;
      pv.ParmLiteralOrVariable = this.ParmLiteralOrVariable;
      return pv;
    }
  }
}
