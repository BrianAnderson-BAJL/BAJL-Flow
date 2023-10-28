using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class Trace : BaseMessage
  {
    public int PreviousStepId = 0;
    public string PreviousStepName = "";
    public string ResponseXml = "";
    public long ExecutionTicks = 0;

    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public Trace(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.PreviousStepId);
      packet.GetData(out this.PreviousStepName);
      packet.GetData(out this.ResponseXml);
      packet.GetData(out this.ExecutionTicks);
    }
    public Trace(int previousStepId, string previousStepName, string responseXml, long executionTicks) : base("Hello", "Key", Packet.PACKET_TYPE.Trace)
    {
      this.PreviousStepId = previousStepId;
      this.PreviousStepName = previousStepName;
      this.ResponseXml = responseXml;
      this.ExecutionTicks = executionTicks;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.PreviousStepId);
      packet.AddData(this.PreviousStepName);
      packet.AddData(this.ResponseXml);
      packet.AddData(this.ExecutionTicks);
      return packet;
    }


  }
}
