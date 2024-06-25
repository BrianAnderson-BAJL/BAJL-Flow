using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class FlowsGet : BaseMessage
  {
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowsGet(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
    }
    public FlowsGet(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.FlowsGet)
    {
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      return packet;
    }


  }
}
