using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Core.PARM;

namespace Core
{
  public class PARMS: List<PARM2>
  {
    public Core.Flow? Flow;
    public PARM2? FindParmByName(string name)
    {
      for (int x = 0; x < this.Count; x++)
      {
        PARM2 p = this[x];
        if (p.Name == name)
          return p;
      }
      return null;
    }

    //public PARMS Clone()
    //{
    //  PARMS parms = new PARMS();
    //  for (int x = 0; x < this.Count; x++)
    //  {
    //    PARM2 p = this[x];
    //    parms.Add(p.Clone());
    //  }
    //  return parms;
    //}

    public PARM_VARS ToParmVars()
    {
      PARM_VARS parmVars = new PARM_VARS();
      for (int x = 0; x < this.Count; x++)
      {
        PARM2 p = this[x];
        parmVars.Add(p.Clone());
      }
      return parmVars;
    }

    public void Add(string name, DATA_TYPE dataType, string defaultValue, PARM2.PARM_REQUIRED required = PARM2.PARM_REQUIRED.Yes, PARM2.PARM_ALLOW_MULTIPLE allowMultiple = PARM2.PARM_ALLOW_MULTIPLE.Single)
    {
      this.Add(new PARM2(name, dataType, defaultValue, required, allowMultiple));
    }

    public void Add(string name, DATA_TYPE dataType, PARM2.PARM_REQUIRED required = PARM2.PARM_REQUIRED.Yes, PARM2.PARM_ALLOW_MULTIPLE allowMultiple = PARM2.PARM_ALLOW_MULTIPLE.Single)
    {
        this.Add(new PARM2(name, dataType, required, allowMultiple));
    }

    //public void Resolve(string parmName, out string val)
    //{
    //    string? s = null;
    //    PARM2? parm = FindParmByName(parmName);
    //    if (parm is not null)
    //    {
          
    //    }

    //}

    //public void Add(string name, DATA_TYPE dataType, PARM2.PARM_REQUIRED required = PARM2.PARM_REQUIRED.Yes)
    //{
    //  this.Add(new PARM2(name, dataType, required));
    //}

    //public void Add(string name, DATA_TYPE dataType, PARM2.PARM_REQUIRED required = PARM2.PARM_REQUIRED.Yes)
    //{
    //  this.Add(new PARM_Integer(name, , required));
    //}

    //public object? ResolveObjectValue(string name)
    //{
    //  object? o = null;
    //  PARM_Various? parm = FindParmByName(name) as PARM_Various;
    //  if (parm is not null)
    //  {
    //    o = Flow!.FindVariableObjectValue(parm.Value);
    //  }
    //  return o;
    //}

    /// <summary>
    /// Find and retrieve the string value either stored literally in the parm, or find the string value stored in the variable
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    //public string? ResolveStringValue(string name)
    //{
    //  string? s = null;
    //  PARM_Various? parm = FindParmByName(name) as PARM_Various;
    //  if (parm is not null)
    //  {
    //    if (parm.ParmLiteral == PARM.PARM_L_OR_V.Literal)
    //    {
    //      s = parm.Value;
    //    }
    //    else
    //    {
    //      s = Flow!.FindVariableStringValue(parm.Value);
    //    }
    //  }
    //  return s;
    //}

    //public Variable? ResolveVariable(string name)
    //{
    //  PARM_Various? parm = FindParmByName(name) as PARM_Various;
    //  if (parm is not null)
    //  {
    //    return Flow!.FindVariable(parm.Value);
    //  }
    //  return null;
    //}

    //public string? ResolveDropDownListValue(string name)
    //{
    //  string? s = null;
    //  PARM_DropDownList? parm = FindParmByName(name) as PARM_DropDownList;
    //  if (parm is not null)
    //  {
    //    s = parm.Value;
    //  }
    //  return s;
    //}
  }
}
