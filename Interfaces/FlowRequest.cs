using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class FlowRequest
  {
    public Variable? Var;
    public Plugin PluginStartedFrom;
    public Flow FlowToStart;

    public FlowRequest(Variable? var, Plugin startedFrom, Flow flowToStart)
    {
      Var = var;
      PluginStartedFrom = startedFrom;
      FlowToStart = flowToStart;
    }
  }
}
