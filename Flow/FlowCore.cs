﻿using FlowEngineCore;
using FlowEngineCore.Interfaces;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static FlowEngineCore.PARM;

namespace FlowCore
{
  public class FlowCore : FlowEngineCore.Plugin
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

    private const string IF_COMPARE_LESS_THAN = "< (less than)";
    private const string IF_COMPARE_LESS_EQUAL = "<= (less or equal)";
    private const string IF_COMPARE_EQUAL = "= (equal)";
    private const string IF_COMPARE_GREATER_EQUAL = ">= (greater or equal)";
    private const string IF_COMPARE_GREATER_THAN = "> (greater than)";
    private const string IF_COMPARE_NOT_EQUAL = "!= (not equal)";
    private const int IF_OUTPUT_INDEX_FALSE = 1;

    private const int ERROR_FLOW_NAME_INVALID = (int)STEP_ERROR_NUMBERS.FlowCoreErrorMin;
    private const int ERROR_FLOW_RESP_MISSING = (int)STEP_ERROR_NUMBERS.FlowCoreErrorMin + 1;


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
      function.OutputAddSuccess("Continue");
      Functions.Add(function);

      function = new Function("Clean Error Info", this, CleanErrorDescription);
      function.Parms.Add("Error Info", DATA_TYPE.String);
      function.DefaultSaveResponseVariable = true;
      function.RespNames.Name = "ErrorInfo";
      function.RespNames.SubVariableAdd(new Variable("ErrorNumber", 0L));
      function.RespNames.SubVariableAdd(new Variable("ErrorDescription", ""));
      function.OutputClear();
      function.OutputAddSuccess(); //We don't want an error response, it is a pass through value
      Functions.Add(function);

      function = new Function("Create Error Info", this, CreateErrorInfo);
      function.Parms.Add("ErrorNumber", DATA_TYPE.Integer);
      function.Parms.Add("ErrorDescription", DATA_TYPE.String);
      function.DefaultSaveResponseVariable = true;
      function.RespNames.Name = "ErrorInfo";
      function.OutputClear();
      function.OutputAddSuccess(); //We don't want an error response, it is a pass through value
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
      pddl = new PARM("Error Number", DATA_TYPE.Integer, PARM_REQUIRED.No);
      pddl.ValidatorAdd(PARM_VALIDATION.NumberMin, (decimal)STEP_ERROR_NUMBERS.CustomErrorMin);
      pddl.ValidatorAdd(PARM_VALIDATION.NumberMax, (decimal)STEP_ERROR_NUMBERS.CustomErrorMax);
      pddl.ValidatorAdd(PARM_VALIDATION.NumberDefaultValue, (decimal)STEP_ERROR_NUMBERS.CustomErrorMin + 10);
      function.Parms.Add(pddl);
      Functions.Add(function);

      function = new Function("If", this, If);
      function.Parms.Add(new PARM("Variable 1", DATA_TYPE.Various));
      pddl = new PARM("Comparer", STRING_SUB_TYPE.DropDownList, PARM.PARM_REQUIRED.Yes);
      pddl.OptionAdd(IF_COMPARE_LESS_THAN);
      pddl.OptionAdd(IF_COMPARE_LESS_EQUAL);
      pddl.OptionAdd(IF_COMPARE_EQUAL);
      pddl.OptionAdd(IF_COMPARE_GREATER_EQUAL);
      pddl.OptionAdd(IF_COMPARE_GREATER_THAN);
      pddl.OptionAdd(IF_COMPARE_NOT_EQUAL);
      function.Parms.Add(pddl);
      function.Parms.Add(new PARM("Variable 2", DATA_TYPE.Various));
      function.OutputClear();
      function.OutputAddSuccess("true");
      function.OutputAdd("false");
      Functions.Add(function);



      function = new Function("Sleep", this, Sleep);
      PARM parm = function.Parms.Add("Time in ms", DATA_TYPE.Integer);
      parm.ValidatorAdd(PARM.PARM_VALIDATION.NumberMin, 0);
      function.OutputClear();
      function.OutputAdd("Continue");
      Functions.Add(function);

      //MethodInfo[] m = typeof(System.Threading.Thread).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
      //f = new Function("System.Threading.Thread.SpinWait", this);
      //System.Threading.Thread.SpinWait

      function = new Function("Stop", this, Stop);
      function.Input = new Input("Input", new System.Numerics.Vector2(10, 50));
      function.OutputClear();
      Functions.Add(function);
      
      //Function fun = new Function("Switch", this, Switch) { OutputsModifiable = true, }; //This is a switch the flow programmer needs to be able to modify the number of outputs (case val = 36, case val = 0, case default, ...)
      //Functions.Add(fun);

      function = new Function("Null Step", this, NullStep);
      function.OutputClear();
      function.OutputAddSuccess("Continue");
      Functions.Add(function);

      function = new Function("Error Start", this, ErrorStart);
      function.OutputClear();
      function.Input = null; //Just like the 'Start' step, no inputs
      function.OutputAddSuccess("Error Start");
      function.RespNames.Name = "errorInfo";
      function.DefaultSaveResponseVariable = true;
      Functions.Add(function);

      mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Green));
      mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.White));

      pddl = new PARM(EVENT, STRING_SUB_TYPE.DropDownList, PARM.PARM_REQUIRED.Yes);
      pddl.OptionAdd(EVENT_START);
      pddl.OptionAdd(EVENT_STOP);
      pddl.OptionAdd(EVENT_MANUAL);
      FlowStartCommands.Add(pddl);
      FlowStartCommands.Add(PARM_FLOW_NAME, DATA_TYPE.String);

      //SAMPLE VARIABLES FOR DESIGNER
      {
        Variable root = new Variable(Flow.VAR_NAME_FLOW_START);
        Variable data = new Variable(Flow.VAR_DATA);
        data.SubVariableAdd(new Variable("YOUR_SAMPLE_DATA", "GOES_HERE"));
        root.SubVariableAdd(data);
        SampleStartData = root;
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

    private RESP Start(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.Start", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }
    private RESP Stop(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.Stop", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }
    private RESP ErrorStart(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.ErrorStart", LOG_TYPE.DBG);

      Variable? stepWithError = flow.FindVariable(Flow.VAR_NAME_PREVIOUS_STEP);
      if (stepWithError is null)
        return RESP.SetSuccess();

      Variable var = new Variable("errorInfo");
      for (int x = 0; x < stepWithError.Count; x++)
      {
        var.SubVariableAdd(stepWithError[x].Clone());
      }
      return RESP.SetSuccess(var);
    }

    private RESP Trace(FlowEngineCore.Flow flow, Variable[] vars)
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
      Dictionary<string, string> tmpDupeCheck = new Dictionary<string, string>();
      for (int x = 0; x < varParms.Count; x++)
      {
        if (tmpDupeCheck.ContainsKey(varParms[x].Name) == true)
          varParms[x] = varParms[x].CloneWithNewName(varParms[x].Name + $"_{x}");
        else
          tmpDupeCheck.Add(varParms[x].Name, "");
      }

      RESP? resp = varResp.Value as RESP;
      if (resp is null)
        return RESP.SetSuccess();
      FunctionStep? step = varStep.Value as FunctionStep;
      if (step is null)
        return RESP.SetSuccess();

      long elapsedTicks = flow.DebugStepTime.HowLong().Ticks;
      flow.DebugStepTime = new TimeElapsed();

      flow.SendFlowDebugTraceStep(resp, step, varParms, elapsedTicks);
      mLog?.Write($"Previous Step [{step.Name}] Success [{resp.Success}], error number [{resp.ErrorNumber}], error message [{resp.ErrorDescription}]", LOG_TYPE.DBG);

      return RESP.SetSuccess();
    }


    //TODO: Add more functions...
    //TODO: VariableCopy      - Copy an existing variable to a new variable, will do a deep copy of all sub variables.


    /// <summary>
    /// Have the flow/thread sleep for some milliseconds, mostly I used this as my test step, it was the first step I ever created.
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    private RESP Sleep(FlowEngineCore.Flow flow, Variable[] vars)  
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
    private RESP FlowRun(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.FlowRun", LOG_TYPE.DBG);
      RESP? resp = null;
      vars[0].GetValue(out string flowName);
      Variable? var = vars[1];
      if (flowName == "")
        return RESP.SetError(ERROR_FLOW_NAME_INVALID, "No flow name specified to start");

      mLog?.Write($"FlowCore.FlowRun - flowName [{flowName}]", LOG_TYPE.DBG);
      Flow? flowToRun = FlowCore.Plugin!.FindFlowByName(flowName);
      if (flowToRun is null)
        return RESP.SetError(ERROR_FLOW_NAME_INVALID, $"Could not find flow to run [{flowName}]");

      Variable varBase = new Variable("flow_start", 0L);
      Variable varData = new Variable("data", 0L);
      varData.SubVariableAdd(var);
      varBase.SubVariableAdd(varData);
      Flow clonedFlow = FlowEngine.StartFlowSameThread(new FlowRequest(varBase, FlowCore.Plugin, flowToRun));
      resp = clonedFlow.Resp;

      if (resp is null) //If the flow didn't return a response, we will assume it failed. Flow Author must specify SUCCESS!
      {
        resp = RESP.SetError(ERROR_FLOW_RESP_MISSING, "Unknown failure, flow didn't return a response");
      }


      return resp;
    }


    /// <summary>
    ///       
    /// </summary>
    /// <param name="vars[0]">Flow name to start</param>
    /// <param name="vars[1]">Variables to pass to new flow</param>
    /// <returns></returns>
    public RESP FlowRunAsync(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.FlowRunAsync", LOG_TYPE.DBG);
      RESP? resp = null;
      vars[0].GetValue(out string flowName); //Just found that you can declare the variable in the out statement, I love it!!
      Variable? var = vars[1];

      if (flowName == "")
        return RESP.SetError(ERROR_FLOW_NAME_INVALID, "No flow name specified to start");

      mLog?.Write("FlowCore.FlowRun - flowName = " + flowName, LOG_TYPE.DBG);
      Flow? flowToRun = FlowCore.Plugin!.FindFlowByName(flowName);
      if (flowToRun is null)
        return RESP.SetError(ERROR_FLOW_NAME_INVALID, $"Could not find flow to run [{flowName}]");


      FlowEngine.StartFlow(new FlowRequest(var, FlowCore.Plugin, flowToRun));
      resp = RESP.SetSuccess(); //This flow is running asynchronously so just return success

      return resp;
    }

    /// <summary>
    /// Have a manually started flow return a specific variable.
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP FlowReturn(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Flow.FlowReturn", LOG_TYPE.DBG);

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

    /// <summary>
    /// If statement. Check if a variable is equal to, less than, greater than, and such
    /// This is a rather ugly function, but it might not be used much, with steps returning success/error responses flows mostly make decisions based on that.
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP If(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.If", LOG_TYPE.DBG);

      Variable var1 = vars[0];
      string compareStyle = vars[1].GetValueAsString();
      Variable var2 = vars[2];

      //Check if numeric comparing
      if ((var1.DataType == DATA_TYPE.Integer || var1.DataType == DATA_TYPE.Decimal) && (var2.DataType == DATA_TYPE.Integer || var2.DataType == DATA_TYPE.Decimal))
      {
        decimal val1 = (decimal)var1.Value;
        decimal val2 = (decimal)var2.Value;
        if (compareStyle == IF_COMPARE_EQUAL)
        {
          if (val1 == val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_NOT_EQUAL)
        {
          if (val1 != val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_LESS_EQUAL)
        {
          if (val1 <= val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_LESS_THAN)
        {
          if (val1 < val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_GREATER_EQUAL)
        {
          if (val1 >= val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_GREATER_THAN)
        {
          if (val1 > val2)
            return RESP.SetSuccess();
        }
      }
      else if (var1.DataType == DATA_TYPE.String && var2.DataType == DATA_TYPE.String)
      {
        string val1 = var1.GetValueAsString();
        string val2 = var2.GetValueAsString();
        if (compareStyle == IF_COMPARE_EQUAL)
        {
          if (val1 == val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_NOT_EQUAL)
        {
          if (val1 != val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_LESS_EQUAL)
        {
          if (val1.CompareTo(val2) <= 0)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_LESS_THAN)
        {
          if (val1.CompareTo(val2) < 0)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_GREATER_EQUAL)
        {
          if (val1.CompareTo(val2) >= 0)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_GREATER_THAN)
        {
          if (val1.CompareTo(val2) > 0)
            return RESP.SetSuccess();
        }
      }
      else if (var1.DataType == DATA_TYPE.Boolean && var2.DataType == DATA_TYPE.Boolean)
      {
        bool val1 = (bool)var1.Value;
        bool val2 = (bool)var2.Value;
        if (compareStyle == IF_COMPARE_EQUAL)
        {
          if (val1 == val2)
            return RESP.SetSuccess();
        }
        else if (compareStyle == IF_COMPARE_NOT_EQUAL)
        {
          if (val1 != val2)
            return RESP.SetSuccess();
        }
        else
          return RESP.SetError(1, "Boolean compares can only be equal or not equal (<,<=,>,>= not allowed)");
      }
      else if (var1.DataType == DATA_TYPE.String && var2.DataType != DATA_TYPE.String)
        return RESP.SetError(1, "Can not compare String type to non string type");
      else if (var1.DataType != DATA_TYPE.String && var2.DataType == DATA_TYPE.String)
        return RESP.SetError(1, "Can not compare String type to non string type");
      else if (var1.DataType == DATA_TYPE.Object || var2.DataType == DATA_TYPE.Object)
        return RESP.SetError(1, "Can not compare Object types");
      else if (var1.DataType == DATA_TYPE.Boolean && var2.DataType != DATA_TYPE.Boolean)
        return RESP.SetError(1, "Boolean can not be compred to non Boolean");
      else if (var1.DataType == DATA_TYPE._None || var2.DataType == DATA_TYPE._None)
        return RESP.SetError(1, "Can't compare to data type NONE");
      else if (var1.DataType == DATA_TYPE.Various || var2.DataType == DATA_TYPE.Various)
        return RESP.SetError(1, "Can't compare to data type Various");

      //If we get to the end then the result is false, not an error, not success
      return RESP.SetError(1, "", IF_OUTPUT_INDEX_FALSE);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP Switch(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.Switch", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }


    /// <summary>
    /// Clean up the previous_step_resp and ErrorDescription for end user consumption, we want to remove any remnants of the flow engine.
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP CleanErrorDescription(FlowEngineCore.Flow flow, Variable[] vars)
    {
      if (vars.Length == 0 || vars[0].Value is null)
        return RESP.SetSuccess(new Variable());

      Variable var = vars[0];
      if (var.Count > 0)
      {
        if (var.Name == Flow.VAR_NAME_PREVIOUS_STEP || var.Name == "errorInfo")
        {
          var.SubVariableDeleteByName("resp");
          var.SubVariableDeleteByName("step");
          var.SubVariableDeleteByName("parms");
          Variable? errDesc = var.SubVariableFindByName("ErrorDescription");
          if (errDesc is not null)
          {
            string val = errDesc.GetValueAsString();
            val = val.Replace("flow_start.data.", "");
            errDesc.Value = val;
          }
        }
        return RESP.SetSuccess(var);
      }
      else
      {
        string val = vars[0].GetValueAsString();
        val = val.Replace("flow_start.data.", "");
        return RESP.SetSuccess(new Variable("", val));
      }
    }

    /// <summary>
    /// A blank step I added just to test the speed of a flow with zero actual work performed. I just left it in here...
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP NullStep(Flow flow, Variable[] vars)
    {
      return RESP.SetSuccess();
    }


    public RESP CreateErrorInfo(Flow flow, Variable[] vars)
    {
      Variable newVar = new Variable("ErrorInfo");

      newVar.SubVariableAdd(new Variable(vars[0])); //0 = Error Number
      newVar.SubVariableAdd(new Variable(vars[1])); //1 = Error Description


      return RESP.SetSuccess(newVar);
    }


  }
}
