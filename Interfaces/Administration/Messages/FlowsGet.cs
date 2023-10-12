using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class FlowsGet : BaseMessage
  {
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowsGet(Core.Administration.Packet packet) : base(packet)
    {
    }
    public FlowsGet(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.FlowsGet)
    {
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      return packet;
    }


  }
}
