using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class AboutResponse : BaseResponse
  {
    public string Version;
    public TimeSpan UpTime;
    public AboutResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out Version);
      packet.GetData(out UpTime);
    }

    public AboutResponse(int packetId, string version, TimeSpan upTime) : base(packetId, Packet.PACKET_TYPE.AboutResponse)
    {
      Version = version;
      UpTime = upTime;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(Version);
      packet.AddData(UpTime);
      return packet;
    }
  }
  }
