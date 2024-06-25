using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class FlowOpenResponse : BaseResponse
  {
    public string FlowXml = "";
    public string FileName = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowOpenResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.FileName);
      packet.GetData(out this.FlowXml);
    }
    public FlowOpenResponse(int packetId, BaseResponse.RESPONSE_CODE response, string fileName, string flowXml) : base(packetId, response, Packet.PACKET_TYPE.FlowOpenResponse)
    {
      this.FileName = fileName;
      this.FlowXml = flowXml;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.FileName);
      packet.AddData(this.FlowXml);
      return packet;
    }

  }
}
