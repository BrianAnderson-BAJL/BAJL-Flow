using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class FlowDebugAlwaysResponse : BaseResponse
  {
    public FlowDebugAlways.DEBUG_ALWAYS DebugAlways;

    public FlowDebugAlwaysResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData<FlowDebugAlways.DEBUG_ALWAYS>(out DebugAlways);
    }

    public FlowDebugAlwaysResponse(int packetId, BaseResponse.RESPONSE_CODE response, FlowDebugAlways.DEBUG_ALWAYS debugAlways) : base(packetId, response, Packet.PACKET_TYPE.FlowDebugAlwaysResponse)
    {
      DebugAlways = debugAlways;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.DebugAlways);
      return packet;
    }

  }
}
