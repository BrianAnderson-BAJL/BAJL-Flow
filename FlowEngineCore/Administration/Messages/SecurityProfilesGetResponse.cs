using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class SecurityProfilesGetResponse : BaseResponse
  {
    public List<SecurityProfile> Profiles;
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public SecurityProfilesGetResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out int Count);
      Profiles = new List<SecurityProfile>(Count);
      for (int x = 0; x < Count; x++)
      {
        packet.GetData(out string name);
        packet.GetData(out int adminUsers);
        packet.GetData(out int adminSecurityProfile);
        packet.GetData(out int adminFlows);
        packet.GetData(out int statistics);
        packet.GetData(out int templates);
        SecurityProfile profile = new();
        profile.Name = name;
        profile.AdministrationUsers = (SecurityProfile.SECURITY_ACCESS_LEVEL)adminUsers;
        profile.AdministrationSecurityProfiles = (SecurityProfile.SECURITY_ACCESS_LEVEL)adminSecurityProfile;
        profile.AdministrationFlows = (SecurityProfile.SECURITY_ACCESS_LEVEL)adminFlows;
        profile.Statistics = (SecurityProfile.SECURITY_ACCESS_SIMPLE)statistics;
        profile.Templates = (SecurityProfile.SECURITY_ACCESS_SIMPLE)templates;
        Profiles.Add(profile);
      }
    }
    public SecurityProfilesGetResponse(int packetId, SecurityProfile[] profiles) : base(packetId, Packet.PACKET_TYPE.SecurityProfilesGetResponse)
    {
      this.Profiles = profiles.ToList();
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Profiles.Count);  //Add the number of users
      for (int x = 0; x < Profiles.Count; x++)
      {
        packet.AddData(Profiles[x].Name);
        packet.AddData(Profiles[x].AdministrationUsers);
        packet.AddData(Profiles[x].AdministrationSecurityProfiles);
        packet.AddData(Profiles[x].AdministrationFlows);
        packet.AddData(Profiles[x].Statistics);
        packet.AddData(Profiles[x].Templates);
      }
      return packet;
    }

  }
}
