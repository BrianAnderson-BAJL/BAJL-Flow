using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  /// <summary>
  /// Save a flow onto the server with the option to put the flow live now.
  /// </summary>
  public class FlowSave : BaseMessage
  {
    public string FileName = "";
    public bool FlowGoLive = false;
    public string FlowXml = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowSave(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.FileName);
      packet.GetData(out this.FlowGoLive);
      packet.GetData(out this.FlowXml);
    }
    public FlowSave(string serverKey, string sessionKey, string fileName, bool flowGoLive, string flowXml) : base(serverKey, sessionKey, Packet.PACKET_TYPE.FlowSave)
    {
      this.FileName = fileName;
      this.FlowGoLive = flowGoLive;
      this.FlowXml = flowXml;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.FileName);
      packet.AddData(this.FlowGoLive);
      packet.AddData(this.FlowXml);
      return packet;
    }


  }
}
