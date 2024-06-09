using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Statistics
{
  internal class StatisticsManager
  {
    private static Queue<Statistic> OldStatistics = new Queue<Statistic>();
    private static Statistic mCurrentStat = new Statistic();
    private static Thread? mThread;
    private static int mActiveThreadCount = 0;
    private static bool mKeepRunning = true;
    private static object mCriticalSection = new object();

    public static void RequestStarted(FlowRequest request)
    {
      lock (mCriticalSection)
      {
        mCurrentStat.RequestStarted(request);
        mActiveThreadCount++;
      }
    }

    public static void RequestFinished(FlowRequest request)
    {
      lock (mCriticalSection)
      {
        mCurrentStat.RequestFinished(request);
        mActiveThreadCount--;
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

    private static void StatsThread()
    {

      while (mKeepRunning == true)
      {
        lock (mCriticalSection)
        {
          TimeSpan ts = DateTime.UtcNow - mCurrentStat.StartTime;
          if (ts.TotalMinutes > 15)
          {
            mCurrentStat.End();
            OldStatistics.Enqueue(mCurrentStat);
            if (OldStatistics.Count > 100)
            {
              OldStatistics.Dequeue();
            }
            mCurrentStat = new Statistic();
          }
        }
        Thread.Sleep(500);
      }
    }
  }
}
