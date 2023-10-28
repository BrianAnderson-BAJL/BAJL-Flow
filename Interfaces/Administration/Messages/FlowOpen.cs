using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class FlowOpen : BaseMessage
  {
    public string FileName = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public FlowOpen(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.FileName);
    }
    public FlowOpen(string serverKey, string sessionKey, string fileName) : base(serverKey, sessionKey, Packet.PACKET_TYPE.FlowOpen)
    {
      this.FileName = fileName;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.FileName);
      return packet;
    }


  }
}
