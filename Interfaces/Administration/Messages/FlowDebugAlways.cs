using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class FlowDebugAlways : BaseMessage
  {

    public FlowDebugAlways(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
    }
    public FlowDebugAlways(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.FlowDebugAlways)
    {
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      return packet;
    }
  }
}
