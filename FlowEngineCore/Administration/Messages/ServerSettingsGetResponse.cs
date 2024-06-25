using FlowEngineCore.Administration;
using FlowEngineCore.Administration.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class ServerSettingsGetResponse : BaseResponse
  {
    public string Xml = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public ServerSettingsGetResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.Xml);
    }
    public ServerSettingsGetResponse(int packetId, BaseResponse.RESPONSE_CODE response, string xml) : base(packetId, response, Packet.PACKET_TYPE.ServerSettingsGetResponse)
    {
      this.Xml = xml;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Xml);
      return packet;
    }

  }
}
