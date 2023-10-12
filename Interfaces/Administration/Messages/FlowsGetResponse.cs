﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class FlowsGetResponse : BaseResponse
  {
    public string FlowsXml = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowsGetResponse(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.FlowsXml);
    }
    public FlowsGetResponse(int packetId, string flowsXml) : base(packetId, Packet.PACKET_TYPE.FlowsGetResponse)
    {
      this.FlowsXml = flowsXml;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.FlowsXml);
      return packet;
    }

  }
}
