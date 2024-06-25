using FlowEngineCore.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Statistics
{
  public class Statistic
  {
    public class StatisticsTime
    {
      public TimeSpan MaxTime = TimeSpan.MinValue;
      public TimeSpan MinTime = TimeSpan.MaxValue;
      public TimeSpan AvgTime;
      
      public long CountOfTimes;
      private long TotalTicks;

      public void UpdateTimes(TimeSpan time)
      {
        CountOfTimes++;
        TotalTicks += time.Ticks;
        if (MaxTime.Ticks < time.Ticks || MaxTime == TimeSpan.MinValue)
          MaxTime = time;
        if (MinTime.Ticks > time.Ticks || MinTime == TimeSpan.MaxValue)
          MinTime = time;
        AvgTime =  TimeSpan.FromTicks(TotalTicks / CountOfTimes);
      }
    }

    public DateTime mStartTime;                                     //The start time of this 15 minute statistic block
    public DateTime mEndTime;                                       //The end time of this 15 minute statistic block
    public List<int> mFlowsStartedPerSecond = new(60);              //Number of flows started each second
    public List<int> mFlowsStartedPerMinute = new(15);              //Number of flows started each minute
    public Dictionary<string, long> mPluginRequestsStarted = new(); //How many flows has the plugin started  <string:plugin name, long:Count of flows started>
    public Dictionary<string, long> mFlowsRequestsStarted = new();  //How many times each flows has started  <string:flow name, long:Count of times this flow was started>
    public Dictionary<int, DateTime> mActiveFlows = new();          //The start time of each active flow.    <int:Managed Thread Id, DateTime:UTC time of thread start>
    public Dictionary<string, StatisticsTime> mFlowTime = new();    //Execution time of flows.               <string:flow name, StatisticsTime:Max,Min,Avg time of flow execution>

    public DateTime StartTime
    {
      get { return mStartTime; }
    }

    public override string ToString()
    {
      return mStartTime.ToLocalTime().ToString() + " - " + mEndTime.ToLocalTime().ToString();
    }

    public Statistic(Statistic? oldStatistics = null) 
    {
      mStartTime = DateTime.UtcNow;
      mEndTime = DateTime.MaxValue;
      for (int x = 0; x < 60; x++)
      {
        mFlowsStartedPerSecond.Add(0);
      }

      if (oldStatistics is null)
        return;

      foreach (var val in oldStatistics.mActiveFlows)
      {
        this.mActiveFlows.Add(val.Key, val.Value);
      }
    }

    public Statistic(Packet packet)
    {
      packet.GetData(out mStartTime);
      packet.GetData(out mEndTime);

      //Get flows started per minute
      packet.GetData(out int flowStartedPerMinuteCount);
      for (int x = 0; x < flowStartedPerMinuteCount; x++)
      {
        packet.GetData(out int flowsPerMinute);
        this.mFlowsStartedPerMinute.Add(flowsPerMinute);
      }

      //Get how many flows each plugin has started
      packet.GetData(out int pluginRequestsStartedCount);
      for (int x = 0; x < pluginRequestsStartedCount; x++)
      {
        packet.GetData(out string pluginName);
        packet.GetData(out long val);
        this.mPluginRequestsStarted.Add(pluginName, val);
      }

      //Get how many times each flow has been started
      packet.GetData(out int flowRequestsStartedCount);
      for (int x = 0; x < flowRequestsStartedCount; x++)
      {
        packet.GetData(out string flowName);
        packet.GetData(out long val);
        this.mFlowsRequestsStarted.Add(flowName, val);
      }

      //Get the speed of each flow
      packet.GetData(out int flowTimeCount);
      for (int x = 0; x < flowTimeCount; x++)
      {
        packet.GetData(out string flowName);
        packet.GetData(out long count);
        packet.GetData(out TimeSpan maxTime);
        packet.GetData(out TimeSpan minTime);
        packet.GetData(out TimeSpan avgTime);
        StatisticsTime stat = new();
        stat.CountOfTimes = count;
        stat.MaxTime = maxTime;
        stat.MinTime = minTime;
        stat.AvgTime = avgTime;
        this.mFlowTime.Add(flowName, stat);
      }
    }

    public void AddToPacket(ref Packet packet)
    {
      packet.AddData(this.mStartTime);
      packet.AddData(this.mEndTime);

      //Add flows started per minute
      packet.AddData(this.mFlowsStartedPerMinute.Count);
      for (int x = 0; x < this.mFlowsStartedPerMinute.Count; x++)
      {
        packet.AddData(this.mFlowsStartedPerMinute[x]);
      }

      //Add how many flows each plugin has started
      packet.AddData(this.mPluginRequestsStarted.Count);
      foreach (var val in this.mPluginRequestsStarted)
      {
        packet.AddData(val.Key);
        packet.AddData(val.Value);
      }

      //Add how many times each flow has been started
      packet.AddData(this.mFlowsRequestsStarted.Count);
      foreach (var val in this.mFlowsRequestsStarted)
      {
        packet.AddData(val.Key);
        packet.AddData(val.Value);
      }

      //Add speed of each flow
      packet.AddData(this.mFlowTime.Count);
      foreach (var val in this.mFlowTime)
      {
        packet.AddData(val.Key);
        packet.AddData(val.Value.CountOfTimes);
        packet.AddData(val.Value.MaxTime.Ticks);
        packet.AddData(val.Value.MinTime.Ticks);
        packet.AddData(val.Value.AvgTime.Ticks);
      }
    }

    public void End()
    {
      mEndTime = DateTime.UtcNow;
    }

    public void NextMinute()
    {
      int total = 0;
      for (int x = 0; x < 60; x++)
      {
        total += mFlowsStartedPerSecond[x];
        mFlowsStartedPerSecond[x] = 0;
      }
      mFlowsStartedPerMinute.Add(total);
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

      if (mFlowsRequestsStarted.ContainsKey(request.FlowToStart.FileNameRelative) == false)
      {
        mFlowsRequestsStarted.Add(request.FlowToStart.FileNameRelative, 1);
      }
      else
      {
        mFlowsRequestsStarted[request.FlowToStart.FileNameRelative]++;
      }
      mFlowsStartedPerSecond[DateTime.UtcNow.Second]++;
      mActiveFlows.Add(request.ManagedThreadId, DateTime.UtcNow);
    }

    public void RequestFinished(FlowRequest request)
    {
      if (mActiveFlows.ContainsKey(request.ManagedThreadId) == false) //Any flows that are active across the 15 minute boundry are just lost for now.  
        return;

      StatisticsTime st;
      if (mFlowTime.ContainsKey(request.FlowToStart.FileNameRelative) == true)
        st = mFlowTime[request.FlowToStart.FileNameRelative];
      else
      {
        st = new StatisticsTime();
        mFlowTime.Add(request.FlowToStart.FileNameRelative, st);
      }

      DateTime startTime = mActiveFlows[request.ManagedThreadId];
      TimeSpan ts = DateTime.UtcNow - startTime;
      st.UpdateTimes(ts);

      mActiveFlows.Remove(request.ManagedThreadId);
    }

  }
}
