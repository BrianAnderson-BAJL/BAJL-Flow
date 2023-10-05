using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class SecurityProfilesGetResponse : BaseResponse
  {
    public List<SecurityProfile> Profiles;
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public SecurityProfilesGetResponse(Core.Administration.Packet packet) : base(packet)
    {
      int Count;
      packet.GetData(out Count);
      Profiles = new List<SecurityProfile>(Count);
      for (int x = 0; x < Count; x++)
      {
        string name;
        int adminUsers;
        int adminSecurityProfile;
        int adminFlows;
        packet.GetData(out name);
        packet.GetData(out adminUsers);
        packet.GetData(out adminSecurityProfile);
        packet.GetData(out adminFlows);
        SecurityProfile profile = new SecurityProfile();
        profile.Name = name;
        profile.AdministrationUsers = (SecurityProfile.SECURITY_ACCESS_LEVEL)adminUsers;
        profile.AdministrationSecurityProfiles = (SecurityProfile.SECURITY_ACCESS_LEVEL)adminSecurityProfile;
        profile.AdministrationFlows = (SecurityProfile.SECURITY_ACCESS_LEVEL)adminFlows;
        Profiles.Add(profile);
      }
    }
    public SecurityProfilesGetResponse(int packetId, SecurityProfile[] profiles) : base(packetId, Packet.PACKET_TYPE.SecurityProfilesGetResponse)
    {
      this.Profiles = profiles.ToList();
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Profiles.Count);  //Add the number of users
      for (int x = 0; x < Profiles.Count; x++)
      {
        packet.AddData(Profiles[x].Name);
        packet.AddData(Profiles[x].AdministrationUsers);
        packet.AddData(Profiles[x].AdministrationSecurityProfiles);
        packet.AddData(Profiles[x].AdministrationFlows);
      }
      return packet;
    }

  }
}
