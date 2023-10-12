using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core
{
  public class FunctionStep : FlowBase
  {
    public string Name = "";
    public Function Function;
    public RESP resps = new RESP();
    //public PARMS parms = new PARMS();
    public List<Link> LinkOutputs = new List<Link>(2);
    public string LinkStr = "";
    public Dictionary<string, object> ExtraValues = new Dictionary<string, object>(); //This is only for the designer, in case they want to store some extra stuff with the step (step image data)
    public bool SaveResponseVariable = false;
    public Variable RespNames = new Variable();
    public PARM_VARS ParmVars = new PARM_VARS();

    public FunctionStep(Flow flow, int id, string pluginName, string functionName, Vector2 pos, string linkStr)
    {
      Function? f = PluginManager.FindFunctionByName(pluginName, functionName);
      if (f == null)
      {
        throw new ArgumentException("[" + pluginName + "] plugin and [" + functionName + "] function not found");
      }
      Name = pluginName + "." + functionName;
      Function = f;
      Id = id;
      Position = pos; //This is called from the loading of the XML, no need to offset again
      LinkStr = linkStr;
      SaveResponseVariable = Function.DefaultSaveResponseVariable;
      RespNames = Function.RespNames.Clone();
    }

    public FunctionStep(Flow flow, int id, string name, Vector2 pos)
    {
      Function? f = PluginManager.FindFunctionByName(name);
      if (f == null)
      {
        throw new ArgumentException("[" + name + "] plugin.function combination not found");
      }
      Function = f;
      Name = name;
      Id = id;
      Position = pos;
      SaveResponseVariable = Function.DefaultSaveResponseVariable;
      RespNames = Function.RespNames.Clone();
    }
    public List<FunctionStep> GetNextStepsBasedOnResp()
    {
      List<FunctionStep> nextSteps = new List<FunctionStep>(8);
      for (int x = 0; x < LinkOutputs.Count; x++)
      {
        FunctionStep? s = LinkOutputs[x].Input.Step;
        if (s != null && resps.OutputIndex == LinkOutputs[x].Output.OutputIndex)
        {
          nextSteps.Add(s);
        }
      }
      return nextSteps;
    }


    public void LinkAdd(Flow f, Output o, Input i)
    {
      LinkOutputs.Add(new Link(f.getNextId(), o, i));
      
    }

    public virtual RESP Execute(Core.Flow flow)
    {
      if (ParmVars.Count < Function.Parms.Count) //ParmVars could have more parameters than Function.Parms if one of them is multiple
        return RESP.SetError(1, "Not enough parameters to execute step");

      Variable[] vars = new Variable[ParmVars.Count];
      for (int x = 0; x < ParmVars.Count; x++)
      {
        PARM_VAR pv = ParmVars[x];
        pv.GetValue(out Variable? var, flow);
        if (var is null)
          return RESP.SetError(2, String.Format("Could not resolve variable [{0}]", pv.VariableName));
        vars[x] = var;
      }

     resps = Function.Execute(vars);
      
      return resps;
    }


    public FunctionStep Clone(Flow flow)
    {
      FunctionStep f = new FunctionStep(flow, this.Id, Name, Position);
      f.Name = Name;
      f.Function = Function;
      f.RespNames.Name = RespNames.Name;
      f.SaveResponseVariable = SaveResponseVariable;
      f.LinkOutputs = this.LinkOutputs; //We don't need a deep copy of the links, they aren't modified
      f.ParmVars = this.ParmVars.Clone();
      return f;
    }
  }
}
