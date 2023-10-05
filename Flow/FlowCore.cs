using Core;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Core.PARM;

namespace FlowCore
{
  public class FlowCore : Core.Plugin
  {
    /// <summary>
    /// Kind of weird that I store a static instance of itself, but the flow engine only creates one instance of each plugin, and this plugin needs to know about the instance when the FlowRun step is called
    /// </summary>
    public static FlowCore? Plugin = null; 
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
      f.Parms.Add(new PARM_Various("Variable", DATA_TYPE.Block));
      Functions.Add(f);

      f = new Function("FlowRunAsync", this, FlowRunAsync);
      f.Parms.Add(PARM_FLOW_NAME, DATA_TYPE.String);
      f.Parms.Add(new PARM_Various("Variable", DATA_TYPE.Block));
      Functions.Add(f);

      f = new Function("FlowReturn", this, FlowReturn);
      PARM_DropDownList pddl = new PARM_DropDownList("Return", PARM_REQUIRED.Yes, OPTION_ERROR);
      pddl.OptionAdd(OPTION_SUCCESS);
      pddl.OptionAdd(OPTION_ERROR);
      f.Parms.Add(pddl);
      f.Parms.Add(new PARM_Various("Variable", DATA_TYPE.Block));
      Functions.Add(f);
      Functions.Add(new Function("If", this, If));

      f = new Function("VariablesExists", this, VariablesExists);
      f.Outputs[1].Label = "Var Missing";
      f.Parms.Add("Variable Name", "", PARM.PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE.Multiple);
      Functions.Add(f);

      f = new Function("Sleep", this, Sleep);
      f.Parms.Add(new PARM_Integer("Time in ms"));
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

      pddl = new PARM_DropDownList("Event", PARM.PARM_REQUIRED.Yes, EVENT_START);
      pddl.OptionAdd(EVENT_START);
      pddl.OptionAdd(EVENT_STOP);
      pddl.OptionAdd(EVENT_MANUAL);
      FlowStartCommands.Add(pddl);
      FlowStartCommands.Add(PARM_FLOW_NAME, "");

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

      for (int x = 0; x < Flows.Count; x++)
      {
        Flow f = Flows[x];
        PARM? p = f.StartCommands.FindParmByName("Event");
        if (p != null)
        {
          if (p.GetValue().ToString() == eventName)
          {
            result.Add(f);
          }
        }
      }
      return result;
    }

    private Flow? FindFlowByName(string flowName)
    {
      Flow? result = null;
      for (int x = 0; x < Flows.Count; x++)
      {
        Flow f = Flows[x];
        string? eventName = f.StartCommands.ResolveDropDownListValue("Event");
        string? name = f.StartCommands.ResolveStringValue(PARM_FLOW_NAME);
        if (name is not null && eventName == "Manual" && name.ToLower() == flowName.ToLower())
        {
          result = f;
        }
      }
      return result;
    }

    private static RESP Start(PARMS Parms)
    {
      Global.Write("Flow.Start");
      return RESP.SetSuccess();
    }
    private static RESP Stop(PARMS Parms)
    {
      Global.Write("Flow.Stop");
      return RESP.SetSuccess();
    }

    private static RESP VariablesExists(PARMS Parms)
    {
      Global.Write("Flow.VariableExists");
      for (int x = 0; x < Parms.Count; x++)
      {
        PARM_Various? pv = Parms[x] as PARM_Various;
        if (pv is not null)
        {
          Variable? var = Parms.Flow!.FindVariable(pv.Value);
          if (var is null)
          {
            return RESP.SetError(1, String.Format("Variable [{0}] does not exist", pv.Value));
          }
        }
      }

      return RESP.SetSuccess();
    }

    private static RESP Sleep(PARMS Parms)
    {
      Global.Write("Flow.Sleep");
      if (Parms.Count != 1)
      {
        return RESP.SetSuccess();
      }

      PARM_Integer? p = Parms[0] as PARM_Integer;
      if (p != null)
      {
        Thread.Sleep((int)p.Value);
        
      }
      return RESP.SetSuccess();
    }

    private static RESP FlowRun(PARMS Parms)
    {
      Global.Write("Flow.FlowRun");
      RESP? resp = null;
      Variable? var = Parms.ResolveVariable("Variable");
      string? flowName = Parms.ResolveStringValue(PARM_FLOW_NAME);
      if (flowName is not null && flowName != "")
      {
        Global.Write("Flow.FlowRun - flowName = " + flowName);
        Flow? flow = FlowCore.Plugin!.FindFlowByName(flowName);
        if (flow is not null)
        {
          Flow clonedFlow = FlowEngine.StartFlowSameThread(new FlowRequest(var, FlowCore.Plugin, flow));
          resp = clonedFlow.Resp;
        }
      }
      else
      {
        resp = RESP.SetError(0, String.Format("Could not find flow to run [{0}]", flowName));
      }

      if (resp is null) //If the flow didn't return a response, we will assume it failed. Flow Author must specify SUCCESS!
      {
        resp = RESP.SetError(0, "Unknown failure, flow didn't return a response");
      }


      return resp;
    }

    public static RESP FlowRunAsync(PARMS Parms)
    {
      Global.Write("Flow.FlowRunAsync");
      RESP? resp = null;
      Variable? var = Parms.ResolveVariable("Variable");
      string? flowName = Parms.ResolveStringValue(PARM_FLOW_NAME);
      if (flowName is not null && flowName != "")
      {
        Global.Write("Flow.FlowRun - flowName = " + flowName);
        Flow? flow = FlowCore.Plugin!.FindFlowByName(flowName);
        if (flow is not null)
        {
          FlowEngine.StartFlow(new FlowRequest(var, FlowCore.Plugin, flow));
          resp = RESP.SetSuccess();
        }
        else
        {
          resp = RESP.SetError(0, String.Format("Could not find flow to run [{0}]", flowName));
        }
      }
      else
      {
        resp = RESP.SetError(0, String.Format("Could not find flow to run [{0}]", flowName));
      }

      return resp;
    }

    public static RESP FlowReturn(PARMS Parms)
    {
      Global.Write("Flow.FlowReturn");
      Variable? var = Parms.ResolveVariable("Variable");
      string? success = Parms.ResolveDropDownListValue("Return");
      if (success == OPTION_SUCCESS)
      {
        Global.Write("Flow.FlowReturn - SUCCESS");
        Parms.Flow!.Resp = RESP.SetSuccess(var);
      }
      else
      {
        Global.Write("Flow.FlowReturn - ERROR");
        Parms.Flow!.Resp = RESP.SetError(1, "Flow returned Error");
      }


      return RESP.SetSuccess();
    }


    public static RESP If(PARMS Parms)
    {
      Global.Write("Flow.If");
      return RESP.SetSuccess();
    }

    public static RESP Switch(PARMS Parms)
    {
      Global.Write("Flow.Switch");
      return RESP.SetSuccess();
    }
  }
}
