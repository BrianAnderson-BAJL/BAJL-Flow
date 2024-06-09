using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowEngineCore
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
    public Variable RespNames = new Variable("", "");
    public PARM_VARS ParmVars = new PARM_VARS();
    public string DebugTraceXml = "";
    public FunctionValidator? Validator = null;
    

    public FunctionStep(Flow flow, int id, string pluginName, string functionName, Vector2 pos, string linkStr)
    {
      Function? f = null;
      try
      {
        f = PluginManager.FindFunctionByName(pluginName, functionName);
      } 
      catch 
      {
        Global.WriteToConsoleDebug($"Failed to FindFunctionByName({pluginName}, {functionName})", LOG_TYPE.ERR);
      }
      if (f is null)
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
      if (f is null)
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
        if (s is not null && resps.OutputType == LinkOutputs[x].Output.Type)
        {
          nextSteps.Add(s);
        }
      }
      return nextSteps;
    }


    public void LinkAdd(Flow f, Output o, Input i)
    {
      LinkOutputs.Add(new Link(f.GetNextId(), o, i));
      
    }


    /// <summary>
    /// Execute the actual step.
    /// 
    /// I don't like how 'busy' this function is. It does too much slowing down the execution of the entire flow.  Needs to be refactored at some point. But for now it works (Nov 18, 2023 - How long will it take me to improve it!)
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <returns></returns>
    public virtual RESP Execute(FlowEngineCore.Flow flow)
    {
      Variable var = new Variable(Flow.VAR_NAME_PREVIOUS_STEP);
      Variable[] vars = new Variable[ParmVars.Count];

      if (ParmVars.Count < Function.Parms.CountOfRequiredParms()) //ParmVars could have more parameters than Function.Parms if one of them is multiple
      {
        resps = RESP.SetError(1, "Not enough parameters to execute step");
        goto GotoResults;
      }
      for (int x = 0; x < ParmVars.Count; x++)
      {
        PARM_VAR pv = ParmVars[x];
        if (pv.Parm.ResolveVariables == PARM.PARM_RESOLVE_VARIABLES.Yes) //Most functions will resolve the variables, but some want the raw variable names (VariableDelete, ...)
        {
          pv.GetValue(out Variable? pvVar, flow);
          if (pvVar is null)
          {
            resps = RESP.SetError(2, $"Could not resolve variable [{pv.VariableName}]");
            goto GotoResults;
          }
          if (pv.Parm.NameChangeable == true) //Database.Select you can set the parameter names for the SQL parameters, these values will be used in the SQL statement
          {
            pvVar = pvVar.CloneWithNewName(pv.ParmName);
          }
          vars[x] = pvVar;
        }
        else
        {
          vars[x] = new Variable($"varName{x}", pv.VariableName);
        }
      }

      try
      {
        resps = Function.Execute(flow, vars);
      }
      catch (Exception ex)
      {
        resps = RESP.SetError(0, Global.FullExceptionMessage(ex));
      }

    GotoResults:
      if (this.Name == "FlowCore.Trace") //Don't overwrite the previous step variables with the trace
        return resps;

      if (resps.Success == false) //If there was an error, lets create the error number & description to be used in the flow, while the resp object contains the same info, it isn't accessible in the flow
      {
      }
      var.SubVariableAdd(new Variable("ErrorNumber", resps.ErrorNumber));
      var.SubVariableAdd(new Variable("ErrorDescription", resps.ErrorDescription));
      var.SubVariableAdd(new Variable("resp", resps));
      var.SubVariableAdd(new Variable("step", this));

      // If this is a development server then add the steps parameters for the trace information
      if (Options.GetSettings.SettingGetAsString("ServerType") == Options.SERVER_TYPE.Development.ToString())
      {
        Variable parameters = new Variable("parms");
        for (int x = 0; x < vars.Length; x++)
        {
          parameters.SubVariableAdd(vars[x]);
        }
        var.SubVariables.Add(parameters);
      }
      
      flow.VariableAdd(Flow.VAR_NAME_PREVIOUS_STEP, var);  //Previous step variable always contains the last steps values
      if (this.SaveResponseVariable == true && resps.Variable is not null)
      {
        resps.Variable.Name = this.RespNames.Name;
        flow.VariableAdd(this.RespNames.Name, resps.Variable);  //If the flow author wants/needs the response later, they can store it in a new flow variable
      }

      //Execute the Validator to double check that the response is correct, Validators can turn a SUCCESS to ERROR or Change and ERROR to SUCCESS.
      if (this.Validator is not null)
      {
        this.Validator.Validate(ref resps);
      }
      return resps;
    }


    public FunctionStep Clone(Flow flow)
    {
      FunctionStep f = new FunctionStep(flow, this.Id, Name, Position);
      f.Id = this.Id;
      f.Name = this.Name;
      f.Function = this.Function;
      f.RespNames.Name = this.RespNames.Name;
      f.SaveResponseVariable = this.SaveResponseVariable;
      f.LinkOutputs = LinkOutputs; //Don't need to clone the LinkOutputs there isn't any volitile data in them
      f.Validator = this.Validator;
      f.ParmVars = this.ParmVars.Clone();
      return f;
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
