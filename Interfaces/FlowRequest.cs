using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class FlowRequest
  {
    public enum START_TYPE
    {
      Now,           //When debugging a flow you might want to use the debug data sent with the flow rather than waiting for an event
      WaitForEvent,  //Sometimes when debugging a flow you want to test with the actual event
    }
    public enum CLONE_FLOW
    {
      CloneFlow,      //Normal execution of flows is done on a clone/copy of the flow
      DoNotCloneFlow, //This is used when debugging a flow, the designer sends the flow over and is executed only once, no need to clone it, if the user wants to run it again, they send the flow again.
    }

    public Variable? Var;
    public Plugin? PluginStartedFrom;
    public Flow FlowToStart;
    public string? OverrideThreadName;
    public int ManagedThreadId;
    public START_TYPE StartType = START_TYPE.WaitForEvent;
    public CLONE_FLOW CloneFlow = CLONE_FLOW.CloneFlow;

    public FlowRequest(Variable? var, Plugin? startedFrom, Flow flowToStart, START_TYPE startType = START_TYPE.WaitForEvent, CLONE_FLOW cloneFlow = CLONE_FLOW.CloneFlow, string? overrideThreadName = null)
    {
      Var = var;
      PluginStartedFrom = startedFrom;
      FlowToStart = flowToStart;
      StartType = startType;
      CloneFlow = cloneFlow;
      OverrideThreadName = overrideThreadName;
    }
  }
}
