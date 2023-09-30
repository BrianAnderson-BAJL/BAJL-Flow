using Core.Administration.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Packets
{
  public class UsersGet : BaseMessage
  {
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public UsersGet(Core.Administration.Packet packet) : base(packet)
    {
      
    }
    public UsersGet(string serverKey, string sessionKey) : base(serverKey, sessionKey, Packet.PACKET_TYPE.UsersGet)
    {
    }

    public override Core.Administration.Packet GetPacket()
    {
      return base.GetPacket(); 
    }

  }
}
