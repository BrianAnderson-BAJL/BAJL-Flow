using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class FlowRequest
  {
    public enum START_TYPE
    {
      Now,
      WaitForEvent,
    }
    public enum CLONE_FLOW
    {
      CloneFlow,
      DoNotCloneFlow,
    }

    public Variable? Var;
    public Plugin? PluginStartedFrom;
    public Flow FlowToStart;
    public START_TYPE StartType = START_TYPE.WaitForEvent;
    public CLONE_FLOW CloneFlow = CLONE_FLOW.CloneFlow;

    public FlowRequest(Variable? var, Plugin? startedFrom, Flow flowToStart, START_TYPE startType = START_TYPE.WaitForEvent, CLONE_FLOW cloneFlow = CLONE_FLOW.CloneFlow)
    {
      Var = var;
      PluginStartedFrom = startedFrom;
      FlowToStart = flowToStart;
      StartType = startType;
      CloneFlow = cloneFlow;
    }
  }
}
