using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Core.PARM;

namespace Core
{
  public class PARMS: List<PARM>
  {
    public Core.Flow? Flow;
    public PARM? FindParmByName(string name)
    {
      for (int x = 0; x < this.Count; x++)
      {
        PARM p = this[x];
        if (p.Name == name)
          return p;
      }
      return null;
    }

    public PARMS Clone()
    {
      PARMS parms = new PARMS();
      for (int x = 0; x < this.Count; x++)
      {
        PARM p = this[x];
        parms.Add(p.Clone());
      }
      return parms;
    }

    public void Add(string name, string defaultValue, PARM.PARM_REQUIRED required = PARM.PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE allowMultiple = PARM_ALLOW_MULTIPLE.Single)
    {
        this.Add(new PARM_Various(name, required, defaultValue, allowMultiple));
    }

    public void Add(string name, DATA_TYPE dataType, PARM.PARM_REQUIRED required = PARM.PARM_REQUIRED.Yes)
    {
      this.Add(new PARM_Various(name, dataType, required));
    }

    public void Add(string name, long defaultValue, PARM.PARM_REQUIRED required = PARM.PARM_REQUIRED.Yes)
    {
      this.Add(new PARM_Integer(name, required, defaultValue));
    }

    public object? ResolveObjectValue(string name)
    {
      object? o = null;
      PARM_Various? parm = FindParmByName(name) as PARM_Various;
      if (parm is not null)
      {
        o = Flow!.FindVariableObjectValue(parm.Value);
      }
      return o;
    }

    /// <summary>
    /// Find and retrieve the string value either stored literally in the parm, or find the string value stored in the variable
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string? ResolveStringValue(string name)
    {
      string? s = null;
      PARM_Various? parm = FindParmByName(name) as PARM_Various;
      if (parm is not null)
      {
        if (parm.ParmLiteral == PARM.PARM_L_OR_V.Literal)
        {
          s = parm.Value;
        }
        else
        {
          s = Flow!.FindVariableStringValue(parm.Value);
        }
      }
      return s;
    }

    public Variable? ResolveVariable(string name)
    {
      PARM_Various? parm = FindParmByName(name) as PARM_Various;
      if (parm is not null)
      {
        return Flow!.FindVariable(parm.Value);
      }
      return null;
    }

    public string? ResolveDropDownListValue(string name)
    {
      string? s = null;
      PARM_DropDownList? parm = FindParmByName(name) as PARM_DropDownList;
      if (parm is not null)
      {
        s = parm.Value;
      }
      return s;
    }
  }
}
