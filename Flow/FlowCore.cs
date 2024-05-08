using Core;
using Core.Interfaces;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace FlowCore
{
  public class FlowCore : Core.Plugin
  {

    /// <summary>
    /// Kind of weird that I store a static instance of itself, but the flow engine only creates one instance of each plugin, and this plugin needs to know about the instance when the FlowRun step is called
    /// </summary>
    public static FlowCore? Plugin = null;
    private const string EVENT = "Event";
    private const string EVENT_START = "FlowEngine Start";
    private const string EVENT_STOP = "FlowEngine Stop";
    private const string EVENT_MANUAL = "Manual";

    private const string OPTION_SUCCESS = "Success";
    private const string OPTION_ERROR = "Error";

    private const string PARM_FLOW_NAME = "Flow name";
    public override void Init()
    {
      base.Init();
      FlowCore.Plugin = this;

      Function function = new Function("Start", this, Start);
      function.OutputClear();
      function.OutputAdd("Start");
      function.Input = null;
      Functions.Add(function);

      function = new Function("Trace", this, Trace);
      function.Parms.Add("Previous step results", DATA_TYPE.Object);
      function.Input = new Input("Input", new System.Numerics.Vector2(10, 50));
      function.OutputClear();
      function.OutputAddSuccess();
      Functions.Add(function);


      function = new Function("Flow Run", this, FlowRun);
      function.Parms.Add(PARM_FLOW_NAME, DATA_TYPE.String);
      function.Parms.Add("data", DATA_TYPE.Various);
      Functions.Add(function);

      function = new Function("Flow Run Async", this, FlowRunAsync);
      function.Parms.Add(PARM_FLOW_NAME, DATA_TYPE.String);
      function.Parms.Add("data", DATA_TYPE.Various);
      Functions.Add(function);

      function = new Function("Flow Return", this, FlowReturn);
      function.OutputClear();
      function.OutputAdd("Continue");
      PARM pddl = new PARM("Return", STRING_SUB_TYPE.DropDownList, PARM.PARM_REQUIRED.Yes);
      pddl.OptionAdd(OPTION_SUCCESS);
      pddl.OptionAdd(OPTION_ERROR);
      function.Parms.Add(pddl);
      function.Parms.Add("Variable", DATA_TYPE.Various);
      Functions.Add(function);

      Functions.Add(new Function("If", this, If));

      function = new Function("Variables Exists", this, VariablesExists);
      function.UpdateErrorOutputLabel("Var Missing");
      
      function.Parms.Add("Variable Name", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes, PARM.PARM_ALLOW_MULTIPLE.Multiple, PARM.PARM_RESOLVE_VARIABLES.No);
      Functions.Add(function);

      function = new Function("Variables Delete", this, VariablesDelete);
      function.Parms.Add("Variable Name to delete", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes, PARM.PARM_ALLOW_MULTIPLE.Multiple, PARM.PARM_RESOLVE_VARIABLES.No); //We want raw variable names in this function so we can delete the actual variable objects
      Functions.Add(function);

      function = new Function("Contains", this, VariableContains);
      function.Parms.Add("Source string to check", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      PARM parm =function.Parms.Add("Value to seek", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      parm.ValidatorAdd(PARM.PARM_VALIDATION.StringMinLength, 1);
      function.Parms.Add("Case sensitive?", DATA_TYPE.Boolean, PARM.PARM_REQUIRED.Yes);
      Functions.Add(function);

      function = new Function("Split", this, VariableSplit);
      function.Parms.Add("Source string to be split", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      parm = function.Parms.Add("Value to split with", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      parm.ValidatorAdd(PARM.PARM_VALIDATION.StringMinLength, 1);
      Functions.Add(function);

      function = new Function("Sleep", this, Sleep);
      parm = function.Parms.Add("Time in ms", DATA_TYPE.Integer);
      parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 0);
      function.OutputClear();
      function.OutputAdd("Complete");
      Functions.Add(function);

      //MethodInfo[] m = typeof(System.Threading.Thread).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
      //f = new Function("System.Threading.Thread.SpinWait", this);
      //System.Threading.Thread.SpinWait

      function = new Function("Stop", this, Stop);
      function.Input = new Input("Input", new System.Numerics.Vector2(10, 50));
      function.OutputClear();
      Functions.Add(function);
      
      Function fun = new Function("Switch", this, Switch) { OutputsModifiable = true, }; //This is a switch the flow programmer needs to be able to modify the number of outputs (case val = 36, case val = 0, case default, ...)
      Functions.Add(fun);

      SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      SettingAdd(new Setting("", "Designer", "BorderColor", Color.Green));
      SettingAdd(new Setting("", "Designer", "FontColor", Color.White));

      pddl = new PARM(EVENT, STRING_SUB_TYPE.DropDownList, PARM.PARM_REQUIRED.Yes);
      pddl.OptionAdd(EVENT_START);
      pddl.OptionAdd(EVENT_STOP);
      pddl.OptionAdd(EVENT_MANUAL);
      FlowStartCommands.Add(pddl);
      FlowStartCommands.Add(PARM_FLOW_NAME, DATA_TYPE.String);

      //SAMPLE VARIABLES FOR DESIGNER
      {
        Variable root = new Variable(Flow.VAR_NAME_FLOW_START, DATA_FORMAT_SUB_VARIABLES.Block);
        Variable data = new Variable(Flow.VAR_DATA, "");
        data.SubVariableAdd(new Variable("YOUR_SAMPLE_DATA", "GOES_HERE"));
        root.SubVariableAdd(data);
        SampleStartData = root;
        //Variable v = new Variable(Flow.VAR_NAME_FLOW_START);
        //Variable request = new Variable(Flow.VAR_REQUEST);
        //request.Add(new Variable(Flow.VAR_DATA));
        //v.Add(request);
        //SampleVariables.Add(Flow.VAR_NAME_FLOW_START, v);
      }
      //SAMPLE VARIABLES FOR DESIGNER
    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);
      List<Flow> flows = FindFlows(EVENT_START);
      for (int i = 0; i < flows.Count; i++)
      {
        FlowEngine.StartFlow(new FlowRequest(null, this, flows[i]));
      }


    }

    public override void StopPlugin()
    {
      List<Flow> flows = FindFlows(EVENT_STOP);
      for (int i = 0; i < flows.Count; i++)
      {
        FlowEngine.StartFlow(new FlowRequest(null, this, flows[i]));
      }

      base.StopPlugin();
    }

    public override void Dispose()
    {
      base.Dispose();
    }

    private List<Flow> FindFlows(string eventName)
    {
      List<Flow> result = new List<Flow>();
      if (FlowStartCommands.Count == 0)
        return result;
      lock (mFlowsCriticalSection)
      {
        for (int x = 0; x < Flows.Count; x++)
        {
          Flow flow = Flows[x];
          
          for (int y = 0; y < flow.StartCommands.Count; y++)
          {
            PARM_VAR pv = flow.StartCommands[y];
            if (pv.Parm.Name == EVENT)
            {
              pv.GetValue(out string value);
              if (value == eventName)
              {
                result.Add(flow);
                break; //Get out of the startCommands for loop, there is only one event per flow
              }
            }
          }
        }
      }
      return result;
    }

    private Flow? FindFlowByName(string flowName)
    {
      lock (mFlowsCriticalSection)
      {
        for (int x = 0; x < Flows.Count; x++)
        {
          Flow flow = Flows[x];
          PARM_VAR? parmEvent = flow.StartCommands.FindByParmName(EVENT);
          PARM_VAR? parmFlowName = flow.StartCommands.FindByParmName(PARM_FLOW_NAME);
          if (parmEvent is not null && parmFlowName is not null)
          {
            parmEvent.GetValue(out string eventName);
            parmFlowName.GetValue(out string flowName2);
            if (eventName == "Manual" && flowName2.Equals(flowName, StringComparison.InvariantCultureIgnoreCase) == true)
            {
              return flow;
            }
          }
        }
      }
      return null;
    }

    private RESP Start(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.Start", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }
    private RESP Stop(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.Stop", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }

    private RESP Trace(Core.Flow flow, Variable[] vars)
    {
      Variable varPreviousStepResp = vars[0];
      Variable? varResp = varPreviousStepResp.SubVariableFindByName("resp") as Variable;
      if (varResp is null)
        return RESP.SetSuccess();
      Variable? varStep = varPreviousStepResp.SubVariableFindByName("step") as Variable;
      if (varStep is null)
        return RESP.SetSuccess();
      Variable? varParms = varPreviousStepResp.SubVariableFindByName("parms") as Variable;
      if (varParms is null)
        return RESP.SetSuccess();

      RESP? resp = varResp.Value as RESP;
      if (resp is null)
        return RESP.SetSuccess();
      FunctionStep? step = varStep.Value as FunctionStep;
      if (step is null)
        return RESP.SetSuccess();

      long elapsedTicks = flow.DebugStepTime.End().Ticks;
      flow.DebugStepTime = new TimeElapsed();

      flow.SendFlowDebugTraceStep(resp, step, varParms, elapsedTicks);
      mLog?.Write($"Previous Step [{step.Name}] Success [{resp.Success}], error number [{resp.ErrorNumber}], error message [{resp.ErrorDescription}]", LOG_TYPE.DBG);

      return RESP.SetSuccess();
    }

    private RESP VariableSplit(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableSplit", LOG_TYPE.DBG);
      vars[0].GetValue(out string source);
      vars[1].GetValue(out string splitOn);

      if (source is null)
        return RESP.SetError(1, "Source is null");
      if (splitOn is null)
        return RESP.SetError(1, "Split on value is null");


      string[] splitStr = source.Split(splitOn, StringSplitOptions.RemoveEmptyEntries);
      Variable var = new Variable();
      for (int x = 0; x < splitStr.Length; x++)
      {
        var.SubVariableAdd(new Variable(x.ToString(), splitStr[x]));
      }
      return RESP.SetSuccess(var);
    }


    private RESP VariableContains(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableContains", LOG_TYPE.DBG);
      vars[0].GetValue(out string source);
      vars[1].GetValue(out string seekVal);
      vars[2].GetValue(out bool caseSensitive);

      if (source is null)
        return RESP.SetError(1, "Source is null");
      if (seekVal is null)
        return RESP.SetError(1, "Seek value is null");

      if (caseSensitive == false)
      {
        source = source.ToLower();
        seekVal = seekVal.ToLower();
      }

      if (source.Contains(seekVal) == true)
        return RESP.SetSuccess();
      else
        return RESP.SetError(1, "Seek value is not contained in Source value");
    }

    private RESP VariablesExists(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableExists", LOG_TYPE.DBG);

      for (int x = 0; x < vars.Length; x++) 
      {
        Variable? v = flow.FindVariable(vars[x].Value);
        if (v is null)
          return RESP.SetError(1, $"Variable missing [{vars[x].Value}]");
      }
      return RESP.SetSuccess();
    }

    private RESP VariablesDelete(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariablesDelete", LOG_TYPE.DBG);

      for (int x = 0; x < vars.Length; x++)
      {
        vars[x].GetValue(out string varName);
        mLog?.Write($"Deleting variable [{varName}]", LOG_TYPE.DBG);
        flow.DeleteVariable(varName);
      }
      //If we got into this function, then all the variables exist (FunctionStep.Execute validates all the variables before calling the function), just return success. Easist function ever!

      return RESP.SetSuccess();
    }

    private RESP Sleep(Core.Flow flow, Variable[] vars)  
    {
      mLog?.Write("FlowCore.Sleep", LOG_TYPE.DBG);
      if (vars.Length == 0)
        return RESP.SetSuccess();

      vars[0].GetValue(out long val);

      Thread.Sleep((int)val);
        
      return RESP.SetSuccess();
    }

    /// <summary>
    ///       
    /// </summary>
    /// <param name="vars[0]">Flow name to start</param>
    /// <param name="vars[1]">Variables to pass to new flow</param>
    /// <returns></returns>
    private RESP FlowRun(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.FlowRun", LOG_TYPE.DBG);
      RESP? resp = null;
      vars[0].GetValue(out string flowName);
      Variable? var = vars[1];
      if (flowName == "")
        return RESP.SetError(10, "No flow name specified to start");

      mLog?.Write($"FlowCore.FlowRun - flowName [{flowName}]", LOG_TYPE.DBG);
      Flow? flowToRun = FlowCore.Plugin!.FindFlowByName(flowName);
      if (flowToRun is null)
        return RESP.SetError(0, $"Could not find flow to run [{flowName}]");

      Variable varBase = new Variable("flow_start", 0L);
      Variable varData = new Variable("data", 0L);
      varData.SubVariableAdd(var);
      varBase.SubVariableAdd(varData);
      Flow clonedFlow = FlowEngine.StartFlowSameThread(new FlowRequest(varBase, FlowCore.Plugin, flowToRun));
      resp = clonedFlow.Resp;

      if (resp is null) //If the flow didn't return a response, we will assume it failed. Flow Author must specify SUCCESS!
      {
        resp = RESP.SetError(0, "Unknown failure, flow didn't return a response");
      }


      return resp;
    }


    /// <summary>
    ///       
    /// </summary>
    /// <param name="vars[0]">Flow name to start</param>
    /// <param name="vars[1]">Variables to pass to new flow</param>
    /// <returns></returns>
    public RESP FlowRunAsync(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.FlowRunAsync", LOG_TYPE.DBG);
      RESP? resp = null;
      vars[0].GetValue(out string flowName); //Just found that you can declare the variable in the out statement, I love it!!
      Variable? var = vars[1];

      if (flowName == "")
        return RESP.SetError(10, "No flow name specified to start");

      mLog?.Write("FlowCore.FlowRun - flowName = " + flowName, LOG_TYPE.DBG);
      Flow? flowToRun = FlowCore.Plugin!.FindFlowByName(flowName);
      if (flowToRun is null)
        return RESP.SetError(0, $"Could not find flow to run [{flowName}]");


      FlowEngine.StartFlow(new FlowRequest(var, FlowCore.Plugin, flowToRun));
      resp = RESP.SetSuccess(); //This flow is running asynchronously so just return success

      return resp;
    }

    public RESP FlowReturn(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("Flow.FlowReturn", LOG_TYPE.DBG);
      if (vars.Length < 2)
      {
        return RESP.SetError(0, "");
      }
      vars[0].GetValue(out string success);
      Variable varVariable = vars[1];

      if (success == OPTION_SUCCESS)
      {
        mLog?.Write("Flow.FlowReturn - SUCCESS", LOG_TYPE.DBG);
        flow.Resp = RESP.SetSuccess(varVariable);
      }
      else
      {
        mLog?.Write("Flow.FlowReturn - ERROR", LOG_TYPE.DBG);
        flow.Resp = RESP.SetError(1, "Flow returned Error");
      }


      return flow.Resp;
    }


    public RESP If(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.If", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }

    public RESP Switch(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.Switch", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }
  }
}
