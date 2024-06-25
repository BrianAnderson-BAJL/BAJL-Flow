using FlowEngineCore;
using FlowEngineCore.Administration;
using FlowEngineCore.Administration.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class ServerSettingsEdit : BaseMessage
  {
    public string Xml = "";

    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public ServerSettingsEdit(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.Xml);
    }
    public ServerSettingsEdit(string serverKey, string sessionKey, string xml) : base(serverKey, sessionKey, Packet.PACKET_TYPE.ServerSettingsEdit)
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
