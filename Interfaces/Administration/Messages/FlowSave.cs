using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  /// <summary>
  /// Save a flow onto the server with the option to put the flow live now.
  /// </summary>
  public class FlowSave : BaseMessage
  {
    public string FileName = "";
    public bool FlowGoLive = false;
    public string FlowData = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowSave(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.FileName);

      packet.GetData(out this.FlowData);
    }
    public FlowSave(string serverKey, string sessionKey, string fileName, bool flowGoLive, string flowData) : base(serverKey, sessionKey, Packet.PACKET_TYPE.FlowSave)
    {
      this.FileName = fileName;
      this.FlowGoLive = flowGoLive;
      this.FlowData = flowData;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.FileName);
      packet.AddData(this.FlowGoLive);
      packet.AddData(this.FlowData);
      return packet;
    }


  }
}
