using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UsersGet : BaseMessage
  {
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public UsersGet(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      
    }
    public UsersGet(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.UsersGet)
    {
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      return base.GetPacket(); 
    }

  }
}
