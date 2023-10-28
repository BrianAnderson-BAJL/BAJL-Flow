using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class FlowDebug : FlowSave
  {
    public string SampleDataFormat;
    public string SampleData;
    public FlowRequest.START_TYPE StartType;
    
    public FlowDebug(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out SampleDataFormat);
      packet.GetData(out SampleData);
      packet.GetData(out int tmpStartType);
      StartType = (FlowRequest.START_TYPE)tmpStartType;
    }
    public FlowDebug(string serverKey, string sessionKey, string fileName, FlowRequest.START_TYPE startType, string flowXml, string sampleDataFormat, string sampleData) : base(serverKey, sessionKey, fileName, false, flowXml)
    {
      this.SampleDataFormat = sampleDataFormat;
      this.SampleData = sampleData;
      this.StartType = startType;
      this.PacketType = Packet.PACKET_TYPE.FlowDebug;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.SampleDataFormat);
      packet.AddData(this.SampleData);
      packet.AddData(this.StartType);
      return packet;
    }
  }
}
