using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class SecurityProfileEdit : SecurityProfileAdd
  {
    public string OldName = "";

    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public SecurityProfileEdit(FlowEngineCore.Administration.Packet packet) : base(packet)
  {
      packet.GetData(out this.OldName);
    }

    /// <summary>
    /// This class inherits from SecurityProfileAdd, so you won't see all the security profile fiels here.
    /// </summary>
    public SecurityProfileEdit(string serverKey, string sessionKey, string oldName, string name, SecurityProfile.SECURITY_ACCESS_LEVEL administrationUsers, SecurityProfile.SECURITY_ACCESS_LEVEL administrationSecurityProfiles, SecurityProfile.SECURITY_ACCESS_LEVEL administrationFlows, SecurityProfile.SECURITY_ACCESS_LEVEL serverSettings, SecurityProfile.SECURITY_ACCESS_SIMPLE statistics, SecurityProfile.SECURITY_ACCESS_SIMPLE templates) : base(serverKey, sessionKey, name, administrationUsers, administrationSecurityProfiles, administrationFlows, serverSettings, statistics, templates)
  {
      this.PacketType = Packet.PACKET_TYPE.SecurityProfileEdit;
      this.OldName = oldName;
  }

  public override FlowEngineCore.Administration.Packet GetPacket()
  {
      Packet packet = base.GetPacket();
      packet.AddData(OldName);
      return packet;

    }


  }
}
