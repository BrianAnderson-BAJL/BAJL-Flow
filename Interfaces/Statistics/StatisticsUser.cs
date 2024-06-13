using FlowEngineCore.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Statistics
{
  internal class StatisticsUser
  {
    private FlowEngineCore.Administration.TcpClientBase mClient;
    private STATISTICS_UPDATE_FREQUENCY mFrequency;
    private DateTime mLastSend = DateTime.MinValue;
    public bool OldDataRequested = true;

    public STATISTICS_UPDATE_FREQUENCY Frequency
    {
      get { return mFrequency; }
    }

    internal StatisticsUser(FlowEngineCore.Administration.TcpClientBase client, STATISTICS_UPDATE_FREQUENCY frequency)
    {
      mClient = client;
      mFrequency = frequency;
    }

    internal bool SameClient(FlowEngineCore.Administration.TcpClientBase client)
    {
      return client == mClient;
    }

    internal bool TimeToSend()
    {
      TimeSpan ts = DateTime.UtcNow - mLastSend;
      if (ts.TotalSeconds > (int)mFrequency)
      {
        return true;
      }
      return false;
    }

    internal void Send(Packet packet)
    {
      mClient.Send(packet);
      mLastSend = DateTime.UtcNow;
    }
  }
}
