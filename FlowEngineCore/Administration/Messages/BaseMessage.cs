﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class BaseMessage
  {
    public string ServerKey = "";
    public string SessionKey = "";

    public FlowEngineCore.Administration.Packet.PACKET_TYPE PacketType;

    public BaseMessage(Packet.PACKET_TYPE packetType)
    {
      PacketType = packetType;
    }

    public BaseMessage(string serverKey, string sessionKey, Packet.PACKET_TYPE packetType)
    {
      PacketType = packetType;
      ServerKey = serverKey;
      SessionKey = sessionKey;
    }

    public BaseMessage(FlowEngineCore.Administration.Packet packet)
    {
      PacketType = packet.PacketType;
      packet.GetData(out this.ServerKey);
      packet.GetData(out this.SessionKey);
    }

    public virtual FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet p = new(PacketType);
      p.AddData(ServerKey);
      p.AddData(SessionKey);
      return p;
    }

  }
}
