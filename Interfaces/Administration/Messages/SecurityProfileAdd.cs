using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class SecurityProfileAdd : BaseMessage
  {
    public string Name = "";
    public SecurityProfile.SECURITY_ACCESS_LEVEL AdministrationUsers;
    public SecurityProfile.SECURITY_ACCESS_LEVEL AdministrationSecurityProfiles;
    public SecurityProfile.SECURITY_ACCESS_LEVEL AdministrationFlows;
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public SecurityProfileAdd(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      int val;
      packet.GetData(out this.Name);
      packet.GetData(out val);
      AdministrationUsers = (SecurityProfile.SECURITY_ACCESS_LEVEL)val;
      packet.GetData(out val);
      AdministrationSecurityProfiles = (SecurityProfile.SECURITY_ACCESS_LEVEL)val;
      packet.GetData(out val);
      AdministrationFlows = (SecurityProfile.SECURITY_ACCESS_LEVEL)val;
    }
    public SecurityProfileAdd(string serverKey, string sessionKey, string name, SecurityProfile.SECURITY_ACCESS_LEVEL administrationUsers, SecurityProfile.SECURITY_ACCESS_LEVEL administrationSecurityProfiles, SecurityProfile.SECURITY_ACCESS_LEVEL administrationFlows) : base(serverKey, sessionKey, Packet.PACKET_TYPE.SecurityProfileAdd)
    {
      this.Name = name;
      this.AdministrationUsers = administrationUsers;
      this.AdministrationSecurityProfiles = administrationSecurityProfiles;
      this.AdministrationFlows = administrationFlows;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Name);
      packet.AddData(this.AdministrationUsers);
      packet.AddData(this.AdministrationSecurityProfiles);
      packet.AddData(this.AdministrationFlows);
      return packet;
    }


  }
}
