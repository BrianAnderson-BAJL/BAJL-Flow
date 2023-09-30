using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class BaseMessage
  {
    public string ServerKey = "";
    public string SessionKey = "";

    public Core.Administration.Packet.PACKET_TYPE PacketType;

    protected BaseMessage(Packet.PACKET_TYPE packetType)
    {
      PacketType = packetType;
    }

    protected BaseMessage(string serverKey, string sessionKey, Packet.PACKET_TYPE packetType)
    {
      PacketType = packetType;
      ServerKey = serverKey;
      SessionKey = sessionKey;
    }

    public BaseMessage(Core.Administration.Packet packet)
    {
      PacketType = packet.PacketType;
      packet.GetData(out this.ServerKey);
      packet.GetData(out this.SessionKey);
    }

    public virtual Core.Administration.Packet GetPacket()
    {
      Packet p = new Packet(PacketType);
      p.AddData(ServerKey);
      p.AddData(SessionKey);
      return p;
    }

  }
}
