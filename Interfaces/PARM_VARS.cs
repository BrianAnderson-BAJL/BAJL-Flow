using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class PARM_VARS
  {
    private List<PARM_VAR> ParmVars = new List<PARM_VAR>();


    public int Count
    {
      get { return ParmVars.Count; }
    }

    public PARM_VAR this[int i]
    {
      get { return ParmVars[i]; }
      set { ParmVars[i] = value; }
    }

    public void Add(PARM_VAR var)
    {
      ParmVars.Add(var);
    }

    public void Add(string name, DATA_TYPE dataType)
    {
      PARM parm = new PARM(name, dataType);
      Add(parm);
    }

    public void Add(PARM parm)
    {
      PARM_VAR? pv;
      if (parm.DataType == DATA_TYPE.String) 
        pv = new PARM_VAR(parm, "");
      else if (parm.DataType == DATA_TYPE.Boolean)
        pv = new PARM_VAR(parm, false);
      else if (parm.DataType == DATA_TYPE.Integer)
        pv = new PARM_VAR(parm, 0);
      else if (parm.DataType == DATA_TYPE.Decimal)
        pv = new PARM_VAR(parm, 0m);
      else if (parm.DataType == DATA_TYPE.Object)
        pv = new PARM_VAR(parm, new object());
      else if (parm.DataType == DATA_TYPE.Various)
        pv = new PARM_VAR(parm);
      else
        throw new Exception("Unknown data type being added to PARM_VARS.Add(PARM)");
      
      ParmVars.Add(pv);
    }

    public void Remove(PARM_VAR var)
    {
      if (var.Parm.AllowMultiple == PARM.PARM_ALLOW_MULTIPLE.Multiple)
      {
        ParmVars.Remove(var);
      }
    }

    public PARM_VAR? FindByParmName(string parmName)
    {
      for (int x = 0; x < ParmVars.Count; x++)
      {
        PARM_VAR pv = ParmVars[x];
        if (pv.Parm.Name == parmName)
          return pv;
      }
      return null;
    }

    public PARM_VARS Clone()
    {
      PARM_VARS pvs = new PARM_VARS();
      for (int x = 0; x < ParmVars.Count; x++)
      {
        pvs.ParmVars.Add(ParmVars[x].Clone());
      }
      return pvs;
    }

  }
}
