using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class FlowDebugAlways : BaseMessage
  {
    public enum DEBUG_ALWAYS
    {
      No,
      Yes,
    }
    public DEBUG_ALWAYS DebugAlways = DEBUG_ALWAYS.No;
    public FlowDebugAlways(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData<DEBUG_ALWAYS>(out DebugAlways);
    }
    public FlowDebugAlways(string serverKey, string sessionKey, DEBUG_ALWAYS debugAlways) : base(serverKey, sessionKey, Packet.PACKET_TYPE.FlowDebugAlways)
    {
      DebugAlways = debugAlways;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(DebugAlways);
      return packet;
    }
  }
}
