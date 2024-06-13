using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class StatisticsRegister : BaseMessage
  {
    public Statistics.STATISTICS_UPDATE_FREQUENCY Frequency;

    public StatisticsRegister(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData<Statistics.STATISTICS_UPDATE_FREQUENCY>(out this.Frequency);
    }

    public StatisticsRegister(string serverKey, string sessionKey, Statistics.STATISTICS_UPDATE_FREQUENCY frequency) : base(serverKey, sessionKey, Packet.PACKET_TYPE.StatisticsRegister)
    {
      Frequency = frequency;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(Frequency);
      return packet;
    }

  }
}
