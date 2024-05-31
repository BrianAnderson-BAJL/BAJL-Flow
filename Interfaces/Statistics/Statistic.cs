using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Statistics
{
  internal class Statistic
  {
    private DateTime StartTime;
    private DateTime EndTime;
    private Dictionary<string, long> PluginRequestsStarted = new();
    private object CriticalSection = new object();

    public void RequestStarted(FlowRequest request)
    {
      //lock (CriticalSection)
      //{
      //  if (PluginRequestsStarted.ContainsKey(pluginName) == false)
      //  {
      //    PluginRequestsStarted.Add(pluginName, 1);
      //  }
      //  else
      //  {
      //    PluginRequestsStarted[pluginName]++;
      //  }
      //}
    }

  }
}
