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


    public PARM_VARS ToParmVars()
    {
      PARM_VARS parmVars = new PARM_VARS();
      for (int x = 0; x < this.Count; x++)
      {
        PARM p = this[x];
        parmVars.Add(p);
      }
      return parmVars;
    }

    public PARM Add(string name, DATA_TYPE dataType, PARM.PARM_REQUIRED required = PARM.PARM_REQUIRED.Yes, PARM.PARM_ALLOW_MULTIPLE allowMultiple = PARM.PARM_ALLOW_MULTIPLE.Single, PARM_RESOLVE_VARIABLES resolveVariables = PARM_RESOLVE_VARIABLES.Yes)
    {
      PARM val = new PARM(name, dataType, required, allowMultiple, resolveVariables);
      this.Add(val);
      return val;
    }

    public PARM Add(string name, STRING_SUB_TYPE stringSubType, PARM.PARM_REQUIRED required = PARM.PARM_REQUIRED.Yes, PARM.PARM_ALLOW_MULTIPLE allowMultiple = PARM.PARM_ALLOW_MULTIPLE.Single, PARM_RESOLVE_VARIABLES resolveVariables = PARM_RESOLVE_VARIABLES.Yes)
    {
      PARM val = new PARM(name, stringSubType, required, allowMultiple, resolveVariables);
      this.Add(val);
      return val;
    }

  }
}
