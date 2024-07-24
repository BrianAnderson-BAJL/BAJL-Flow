using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class TemplatesGetResponse : BaseResponse
  {
    public string TemplatesXml = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public TemplatesGetResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.TemplatesXml);
    }
    public TemplatesGetResponse(int packetId, string flowsXml) : base(packetId, Packet.PACKET_TYPE.TemplatesGetResponse)
    {
      this.TemplatesXml = flowsXml;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.TemplatesXml);
      return packet;
    }

  }
}
