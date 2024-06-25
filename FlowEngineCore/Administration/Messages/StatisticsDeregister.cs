using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class StatisticsDeregister : BaseMessage
  {

    public StatisticsDeregister(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
    }

    public StatisticsDeregister(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.StatisticsDeregister)
    {
      
    }


  }
}
