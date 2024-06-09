using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Statistics
{
  internal class Statistic
  {
    private class StatisticsTime
    {
      public TimeSpan MaxTime;
      public TimeSpan Mintime;
      public TimeSpan AvgTime;
      public long CountOfTimes;
      public long TotalTicks;

      public void UpdateTimes(TimeSpan time)
      {
        CountOfTimes++;
        TotalTicks += time.Ticks;
        if (MaxTime.Ticks < time.Ticks)
          MaxTime = time;
        if (Mintime.Ticks > time.Ticks)
          Mintime = time;
        AvgTime =  TimeSpan.FromTicks(TotalTicks / CountOfTimes);
      }
    }

    private DateTime mStartTime;
    private DateTime mEndTime;
    private Dictionary<string, long> mPluginRequestsStarted = new(); //How many flows has the plugin started  <string:plugin name, long:Count of flows started>
    private Dictionary<string, long> mFlowsRequestsStarted = new();  //How many times each flows has started  <string:flow name, long:Count of times this flow was started>
    private Dictionary<int, DateTime> mActiveFlows = new();          //The start time of each active flow.    <int:Managed Thread Id, DateTime:UTC time of thread start>
    private Dictionary<string, StatisticsTime> mFlowTime = new();    //Execution time of flows.               <string:flow name, StatisticsTime:Max,Min,Avg time of flow execution>

    public DateTime StartTime
    {
      get { return mStartTime; }
    }

    public Statistic() 
    {
      mStartTime = DateTime.UtcNow;
    }

    public void End()
    {
      mEndTime = DateTime.UtcNow;
    }

    public void RequestStarted(FlowRequest request)
    {
      if (request.PluginStartedFrom is null)
        return;

      if (mPluginRequestsStarted.ContainsKey(request.PluginStartedFrom.Name) == false)
      {
        mPluginRequestsStarted.Add(request.PluginStartedFrom.Name, 1);
      }
      else
      {
        mPluginRequestsStarted[request.PluginStartedFrom.Name]++;
      }

      if (mFlowsRequestsStarted.ContainsKey(request.FlowToStart.FileName) == false)
      {
        mFlowsRequestsStarted.Add(request.FlowToStart.FileName, 1);
      }
      else
      {
        mFlowsRequestsStarted[request.FlowToStart.FileName]++;
      }
      mActiveFlows.Add(request.ManagedThreadId, DateTime.UtcNow);
    }

    public void RequestFinished(FlowRequest request)
    {
      if (mActiveFlows.ContainsKey(request.ManagedThreadId) == false) //Any flows that are active across the 15 minute boundry are just lost for now.  
        return; //TODO: Copy still active flows to new Statistic

      StatisticsTime st;
      if (mFlowTime.ContainsKey(request.FlowToStart.FileName) == true)
        st = mFlowTime[request.FlowToStart.FileName];
      else
      {
        st = new StatisticsTime();
        mFlowTime.Add(request.FlowToStart.FileName, st);
      }

      DateTime startTime = mActiveFlows[request.ManagedThreadId];
      TimeSpan ts = DateTime.UtcNow - startTime;
      st.UpdateTimes(ts);

      mActiveFlows.Remove(request.ManagedThreadId);
    }

  }
}
