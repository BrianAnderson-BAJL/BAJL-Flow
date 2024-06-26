using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class About : BaseMessage
  {
    public About(FlowEngineCore.Administration.Packet packet) : base(packet)
    {

    }
    public About(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.About)
    {
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      return base.GetPacket();
    }

  }
}
