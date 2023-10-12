using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class PARM
  {
    

    public enum PARM_REQUIRED
    {
      Yes,
      No,
    }

    public enum PARM_ALLOW_MULTIPLE
    {
      Single,
      Multiple,
    }

    /// <summary>
    /// Is the parameter a literal or a variable
    /// </summary>
    public enum PARM_L_OR_V
    {
      Literal,
      Variable,
    }

    public string Name = "";
    public string VarName = "";
    public string Tooltip = "";
    public DATA_TYPE DataType = DATA_TYPE.String;
    public PARM_REQUIRED Required = PARM_REQUIRED.Yes;
    public PARM_L_OR_V ParmLiteral = PARM_L_OR_V.Literal;
    public PARM_ALLOW_MULTIPLE AllowMultiple = PARM_ALLOW_MULTIPLE.Single; //If Multiple, this parameter can be duplicated to allow multiple values to be passed in.
    //public Variable? Variable;

    public PARM()
    {
      DataType = DATA_TYPE.String;
      Required = PARM_REQUIRED.Yes;
    }

    public virtual PARM Clone()
    {
      return new PARM();
    }

    public virtual void SetValue(object value)
    {
    }

    public virtual object GetValue()
    {
      return "";
    }

    public static string GetValueAsString(PARM parm)
    {
      PARM_Various? ps = parm as PARM_Various;
      if (ps != null)
      {
        return ps.Value;
      }
      PARM_Integer? pi = parm as PARM_Integer;
      if (ps != null)
      {
        return ps.Value.ToString();
      }
      return "";
    }

    public virtual Variable Resolve()
    {
      throw new NotImplementedException();
    }

  }

}
