using Core;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
//using static Core.PARM2;

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

      Function f = new Function("Start", this, Start);
      f.OutputClear();
      f.OutputAdd("Start", new Vector2(160, 50));
      f.Input = null;
      
      Functions.Add(f);
      f = new Function("FlowRun", this, FlowRun);
      f.Parms.Add(PARM_FLOW_NAME, DATA_TYPE.String);
      f.Parms.Add("Variable", DATA_TYPE.Block);
      Functions.Add(f);

      f = new Function("FlowRunAsync", this, FlowRunAsync);
      f.Parms.Add(PARM_FLOW_NAME, DATA_TYPE.String);
      f.Parms.Add("Variable", DATA_TYPE.Block);
      Functions.Add(f);

      f = new Function("FlowReturn", this, FlowReturn);
      PARM2 pddl = new PARM2("Return", DATA_TYPE.DropDownList, PARM2.PARM_REQUIRED.Yes);
      pddl.OptionAdd(OPTION_SUCCESS);
      pddl.OptionAdd(OPTION_ERROR);
      f.Parms.Add(pddl);
      f.Parms.Add("Variable", DATA_TYPE.Block);

      f.Parms.Add("Variable", DATA_TYPE.Block);
      Functions.Add(f);
      Functions.Add(new Function("If", this, If));

      f = new Function("VariablesExists", this, VariablesExists);
      f.Outputs[1].Label = "Var Missing";
      f.Parms.Add("Variable Name", DATA_TYPE.String, PARM2.PARM_REQUIRED.Yes, PARM2.PARM_ALLOW_MULTIPLE.Multiple);
      Functions.Add(f);

      f = new Function("Sleep", this, Sleep);
      f.Parms.Add("Time in ms", DATA_TYPE.Integer);
      f.OutputClear();
      f.OutputAdd("Complete", Output.SUCCESS_POS);
      Functions.Add(f);

      //MethodInfo[] m = typeof(System.Threading.Thread).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
      //f = new Function("System.Threading.Thread.SpinWait", this);
      //System.Threading.Thread.SpinWait

      f = new Function("Stop", this, Stop);

      f.Input = new Input("Input", new System.Numerics.Vector2(10, 50));
      f.OutputClear();
      Functions.Add(f);
      
      Function fun = new Function("Switch", this, Switch) { OutputsModifiable = true, }; //This is a switch the flow programmer needs to be able to modify the number of outputs (case val = 36, case val = 0, case default, ...)
      Functions.Add(fun);

      SettingAddIfMissing(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      SettingAddIfMissing(new Setting("", "Designer", "BorderColor", Color.Green));
      SettingAddIfMissing(new Setting("", "Designer", "FontColor", Color.White));

      pddl = new PARM2(EVENT, DATA_TYPE.DropDownList, PARM2.PARM_REQUIRED.Yes);
      pddl.OptionAdd(EVENT_START);
      pddl.OptionAdd(EVENT_STOP);
      pddl.OptionAdd(EVENT_MANUAL);
      FlowStartCommands.Add(pddl);
      FlowStartCommands.Add(PARM_FLOW_NAME, DATA_TYPE.String);

      //SAMPLE VARIABLES FOR DESIGNER
      {
        Variable v = new Variable(Flow.VAR_NAME_FLOW_START);
        Variable request = new Variable(Flow.VAR_REQUEST);
        request.Add(new Variable(Flow.VAR_DATA));
        v.Add(request);
        SampleVariables.Add(Flow.VAR_NAME_FLOW_START, v);
      }
      //SAMPLE VARIABLES FOR DESIGNER
    }

    public override void StartPlugin()
    {
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

    private static RESP Start(Variable[] vars)
    {
      Global.Write("Flow.Start");
      return RESP.SetSuccess();
    }
    private static RESP Stop(Variable[] vars)
    {
      Global.Write("Flow.Stop");
      return RESP.SetSuccess();
    }

    private static RESP VariablesExists(Variable[] vars)
    {
      Global.Write("Flow.VariableExists");

      //If we got into this function, then all the variables exist (FunctionStep.Execute validates all the variables before calling the function), just return success. Easist function ever!

      return RESP.SetSuccess();
    }

    private static RESP Sleep(Variable[] vars)  
    {
      Global.Write("Flow.Sleep");
      if (vars.Length == 0)
        return RESP.SetSuccess();

      VariableInteger? i = vars[0] as VariableInteger;
      if (i is null)
        return RESP.SetSuccess();

      Thread.Sleep((int)i.Value);
        
      return RESP.SetSuccess();
    }

    /// <summary>
    ///       
    /// </summary>
    /// <param name="vars[0]">Flow name to start</param>
    /// <param name="vars[1]">Variables to pass to new flow</param>
    /// <returns></returns>
    private static RESP FlowRun(Variable[] vars)
    {
      Global.Write("Flow.FlowRun");
      RESP? resp = null;
      vars[0].GetValue(out string flowName);
      Variable? var = vars[1];
      if (flowName == "")
        return RESP.SetError(10, "No flow name specified to start");

      Global.Write("Flow.FlowRun - flowName = " + flowName);
      Flow? flow = FlowCore.Plugin!.FindFlowByName(flowName);
      if (flow is null)
        return RESP.SetError(0, String.Format("Could not find flow to run [{0}]", flowName));

      Flow clonedFlow = FlowEngine.StartFlowSameThread(new FlowRequest(var, FlowCore.Plugin, flow));
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
    public static RESP FlowRunAsync(Variable[] vars)
    {
      Global.Write("Flow.FlowRunAsync");
      RESP? resp = null;
      vars[0].GetValue(out string flowName); //Just found that you can declare the variable in the out statement, I love it!!
      Variable? var = vars[1];

      if (flowName == "")
        return RESP.SetError(10, "No flow name specified to start");

      Global.Write("Flow.FlowRun - flowName = " + flowName);
      Flow? flow = FlowCore.Plugin!.FindFlowByName(flowName);
      if (flow is null)
        return RESP.SetError(0, String.Format("Could not find flow to run [{0}]", flowName));


      FlowEngine.StartFlow(new FlowRequest(var, FlowCore.Plugin, flow));
      resp = RESP.SetSuccess(); //This flow is running asynchronously so just return success

      return resp;
    }

    public static RESP FlowReturn(Variable[] vars)
    {
      //Global.Write("Flow.FlowReturn");
      //Variable? var = Parms.ResolveVariable("Variable");
      //string? success = Parms.ResolveDropDownListValue("Return");
      //if (success == OPTION_SUCCESS)
      //{
      //  Global.Write("Flow.FlowReturn - SUCCESS");
      //  Parms.Flow!.Resp = RESP.SetSuccess(var);
      //}
      //else
      //{
      //  Global.Write("Flow.FlowReturn - ERROR");
      //  Parms.Flow!.Resp = RESP.SetError(1, "Flow returned Error");
      //}


      return RESP.SetSuccess();
    }


    public static RESP If(Variable[] vars)
    {
      Global.Write("Flow.If");
      return RESP.SetSuccess();
    }

    public static RESP Switch(Variable[] vars)
    {
      Global.Write("Flow.Switch");
      return RESP.SetSuccess();
    }
  }
}
