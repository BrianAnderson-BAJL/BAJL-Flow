using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class FlowDebugResponse : BaseResponse
  {
    public string FileName = "";
    public long Ticks = 0;
    public string FlowXml = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowDebugResponse(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.FileName);
      packet.GetData(out this.Ticks);
      packet.GetData(out this.FlowXml);
    }
    public FlowDebugResponse(int packetId, BaseResponse.RESPONSE_CODE response, string fileName, long ticks, string flowXml) : base(packetId, response, Packet.PACKET_TYPE.FlowDebugResponse)
    {
      this.FileName = fileName;
      this.Ticks = ticks;
      this.FlowXml = flowXml;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.FileName);
      packet.AddData(this.Ticks);
      packet.AddData(this.FlowXml);
      return packet;
    }

  }
}
