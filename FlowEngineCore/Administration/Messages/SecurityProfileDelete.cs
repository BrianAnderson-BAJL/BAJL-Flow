﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class SecurityProfileDelete : BaseMessage
  {
    public string Name = "";

    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public SecurityProfileDelete(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.Name);
    }
    public SecurityProfileDelete(string serverKey, string sessionKey, string name) : base(serverKey, sessionKey, Packet.PACKET_TYPE.SecurityProfileDelete)
    {
      this.Name = name;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Name);
      return packet;
    }


  }
}
