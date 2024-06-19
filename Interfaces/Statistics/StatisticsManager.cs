using FlowEngineCore.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Statistics
{
  public enum STATISTICS_UPDATE_FREQUENCY
  {
    SuperFast = 1, // Every 1 second
    Fast = 5,      // Every 5 seconds
    Medium = 15,   // Every 15 seconds
    Slow = 30,     // Every 30 seconds
  }
  public class StatisticsManager
  {
    private const int TimePeriodMinutes = 15;
    private const int OldStatisticsToKeep = 100;
    private static Queue<Statistic> OldStatistics = new Queue<Statistic>();
    private static Statistic mCurrentStat = new Statistic();
    private static Thread? mThread;
    private static int mActiveFlowCount = 0;
    private static bool mKeepRunning = true;
    private static object mCriticalSectionStats = new object();
    private static object mCriticalSectionUsers = new object();
    private static List<StatisticsUser> mRegisteredUsers = new List<StatisticsUser>();

    public static void RequestStarted(FlowRequest request)
    {
      lock (mCriticalSectionStats)
      {
        mCurrentStat.RequestStarted(request);
        mActiveFlowCount++;
      }
    }

    public static void RequestFinished(FlowRequest request)
    {
      lock (mCriticalSectionStats)
      {
        mCurrentStat.RequestFinished(request);
        mActiveFlowCount--;
      }
    }

    public static void Start()
    {
      mKeepRunning = true;
      mThread = new Thread(StatsThread);
      mThread.Start();
    }

    public static void Stop()
    {
      mKeepRunning = false;
    }

    public static void UserRegister(FlowEngineCore.Administration.TcpClientBase client, STATISTICS_UPDATE_FREQUENCY frequency)
    {
      UserDeregister(client); //Make sure the client isn't already registered, we don't want to send duplicate data all the time
      StatisticsUser user = new StatisticsUser(client, frequency);
      lock (mCriticalSectionUsers)
      {
        mRegisteredUsers.Add(user);
      }
    }

    public static void UserDeregister(FlowEngineCore.Administration.TcpClientBase client)
    {
      lock (mCriticalSectionUsers)
      {
        for (int x = 0; x < mRegisteredUsers.Count; x++)
        {
          StatisticsUser user = mRegisteredUsers[x];
          if (user.SameClient(client) == true)
          {
            mRegisteredUsers.RemoveAt(x);
            break;
          }
        }
      }
    }

    private static Packet CreateStatisticsPacket(StatisticsUser user)
    {
      Packet packet = new Packet(Packet.PACKET_TYPE.StatisticsData);
      lock (mCriticalSectionStats)
      {
        int currentSecond = DateTime.UtcNow.Second;
        packet.AddData(currentSecond);

        //Add flows started per second (this stat is only available for the current stats)
        int secondsOfData = 1 + (int)user.Frequency; //Always send one more seconds worth, sometimes it skips a second in the data, fucks up the chart.
        packet.AddData(secondsOfData);
        int startIndex = currentSecond - secondsOfData;
        int firstBatchStartIndex = -1;
        if (startIndex < 0)
        {
          firstBatchStartIndex = 60 + startIndex;
          startIndex = 0;
        }
        if (firstBatchStartIndex >= 0)
        {
          for (int x = firstBatchStartIndex; x < 60; x++)
          {
            packet.AddData(mCurrentStat.mFlowsStartedPerSecond[x]);
          }
        }
        for (int x = startIndex; x < currentSecond; x++)
        {
          packet.AddData(mCurrentStat.mFlowsStartedPerSecond[x]);
        }

        mCurrentStat.AddToPacket(ref packet);

        //packet.AddData(user.OldDataRequested); //So we can tell if there is old data included, we only need to send the old data on the first packet
        //if (user.OldDataRequested == true)
        //{
          //user.OldDataRequested = false;
          packet.AddData(OldStatistics.Count);
          for (int x = 0; x < OldStatistics.Count; x++)
          {
            OldStatistics.ElementAt(x).AddToPacket(ref packet);
          }
        //}
      }
      return packet;
    }

    private static void StatsThread()
    {
      int previousMinute = DateTime.UtcNow.Minute;
      while (mKeepRunning == true)
      {
        lock (mCriticalSectionStats)
        {
          if (previousMinute != DateTime.UtcNow.Minute)
          {
            mCurrentStat.NextMinute();
            previousMinute = DateTime.UtcNow.Minute;
          }
          TimeSpan ts = DateTime.UtcNow - mCurrentStat.StartTime;
          if (ts.TotalMinutes > TimePeriodMinutes)
          {
            mCurrentStat.End();
            OldStatistics.Enqueue(mCurrentStat);
            if (OldStatistics.Count > OldStatisticsToKeep)
            {
              OldStatistics.Dequeue();
            }
            mCurrentStat = new Statistic(mCurrentStat);
          }
        }
        lock (mCriticalSectionUsers)
        {
          Packet? packet = null;
          for (int x = 0; x < mRegisteredUsers.Count; x++)
          {
            StatisticsUser user = mRegisteredUsers[x];
            if (user.TimeToSend() == true)
            {
              if (packet is null)
                packet = CreateStatisticsPacket(user);  //If we are sending data to multiple users (actually not very likely) then only create a single packet
              user.Send(packet);
            }
          }
        }
        Thread.Sleep(100);
      }
    }
  }
}
