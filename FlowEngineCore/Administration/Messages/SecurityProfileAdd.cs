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
    public SecurityProfile.SECURITY_ACCESS_SIMPLE Statistics;
    public SecurityProfile.SECURITY_ACCESS_SIMPLE Templates;
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public SecurityProfileAdd(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.Name);
      packet.GetData(out int val);
      AdministrationUsers = (SecurityProfile.SECURITY_ACCESS_LEVEL)val;
      packet.GetData(out val);
      AdministrationSecurityProfiles = (SecurityProfile.SECURITY_ACCESS_LEVEL)val;
      packet.GetData(out val);
      AdministrationFlows = (SecurityProfile.SECURITY_ACCESS_LEVEL)val;
      packet.GetData(out val);
      Statistics = (SecurityProfile.SECURITY_ACCESS_SIMPLE)val;
      packet.GetData(out val);
      Templates = (SecurityProfile.SECURITY_ACCESS_SIMPLE)val;
    }
    public SecurityProfileAdd(string serverKey, string sessionKey, string name, SecurityProfile.SECURITY_ACCESS_LEVEL administrationUsers, SecurityProfile.SECURITY_ACCESS_LEVEL administrationSecurityProfiles, SecurityProfile.SECURITY_ACCESS_LEVEL administrationFlows, SecurityProfile.SECURITY_ACCESS_SIMPLE statistics, SecurityProfile.SECURITY_ACCESS_SIMPLE templates) : base(serverKey, sessionKey, Packet.PACKET_TYPE.SecurityProfileAdd)
    {
      this.Name = name;
      this.AdministrationUsers = administrationUsers;
      this.AdministrationSecurityProfiles = administrationSecurityProfiles;
      this.AdministrationFlows = administrationFlows;
      this.Statistics = statistics;
      this.Templates = templates;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Name);
      packet.AddData(this.AdministrationUsers);
      packet.AddData(this.AdministrationSecurityProfiles);
      packet.AddData(this.AdministrationFlows);
      packet.AddData(this.Statistics);
      packet.AddData(this.Templates);
      return packet;
    }


  }
}
