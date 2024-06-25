using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class SecurityProfilesGet : BaseMessage
  {
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public SecurityProfilesGet(FlowEngineCore.Administration.Packet packet) : base(packet)
    {

    }
    public SecurityProfilesGet(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.SecurityProfilesGet)
    {
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      return base.GetPacket();
    }

  }
}
