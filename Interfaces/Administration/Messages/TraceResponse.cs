using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class TraceResponse : BaseResponse
  {
    public int PreviousStepId = 0;
    public string PreviousStepName = "";
    public string ResponseXml = "";
    public long ExecutionTicks = 0;
    public bool Success = false;
    public TraceResponse(int previousStepId, string previousStepName, string responseXml, long executionTicks, bool success) : base(0, Packet.PACKET_TYPE.Trace)
    {
      this.PreviousStepId = previousStepId;
      this.PreviousStepName = previousStepName;
      this.ResponseXml = responseXml;
      this.ExecutionTicks = executionTicks;
      this.Success = success;
    }

    public TraceResponse(Core.Administration.Packet packet) : base(packet)
    {
      PacketType = packet.PacketType;
      packet.GetData(out this.PreviousStepId);
      packet.GetData(out this.PreviousStepName);
      packet.GetData(out this.ResponseXml);
      packet.GetData(out this.ExecutionTicks);
      packet.GetData(out this.Success);
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.PreviousStepId);
      packet.AddData(this.PreviousStepName);
      packet.AddData(this.ResponseXml);
      packet.AddData(this.ExecutionTicks);
      packet.AddData(this.Success);
      return packet;
    }
  }
}
