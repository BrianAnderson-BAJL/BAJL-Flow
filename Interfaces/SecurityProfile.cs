using Core.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class SecurityProfile
  {
    public enum SECURITY_ACCESS_LEVEL
    {
      None = 0,
      Readonly = 1,
      Edit = 2,
      Full = 3,
    }

    public enum SECURITY_AREA
    {
      Users,
      SecurityProfiles,
      Flows,
    }


    public string Name = "";

    public SECURITY_ACCESS_LEVEL AdministrationUsers = SECURITY_ACCESS_LEVEL.None;
    public SECURITY_ACCESS_LEVEL AdministrationSecurityProfiles = SECURITY_ACCESS_LEVEL.None;
    public SECURITY_ACCESS_LEVEL AdministrationFlows = SECURITY_ACCESS_LEVEL.None;


    public bool HasAccessServer(Packet.PACKET_TYPE packetType)
    {
      bool rc = false;
      switch (packetType)
      {
        case Packet.PACKET_TYPE._Unknown:
          rc = false; //Always false
          break;
        case Packet.PACKET_TYPE.UserLogin:
        case Packet.PACKET_TYPE.UserLogout:
          rc = true; //Always true
          break;

        case Packet.PACKET_TYPE.UsersGet:
          rc = AdministrationUsers >= SECURITY_ACCESS_LEVEL.Readonly; break;
        case Packet.PACKET_TYPE.UserEdit:
          rc = AdministrationUsers >= SECURITY_ACCESS_LEVEL.Edit; break;
        case Packet.PACKET_TYPE.UserAdd:
        case Packet.PACKET_TYPE.UserDelete:
          rc = AdministrationUsers >= SECURITY_ACCESS_LEVEL.Full; break;

        case Packet.PACKET_TYPE.SecurityProfilesGet:
          rc = AdministrationSecurityProfiles >= SECURITY_ACCESS_LEVEL.Readonly; break;
        case Packet.PACKET_TYPE.SecurityProfileEdit:
          rc = AdministrationSecurityProfiles >= SECURITY_ACCESS_LEVEL.Edit; break;
        case Packet.PACKET_TYPE.SecurityProfileAdd:
        case Packet.PACKET_TYPE.SecurityProfileDelete:
          rc = AdministrationSecurityProfiles >= SECURITY_ACCESS_LEVEL.Full; break;
        default:
          rc = false;
          break;
      }

      return rc;
    }

    public SECURITY_ACCESS_LEVEL AccessLevelClient(SECURITY_AREA securityArea)
    {
      if (securityArea == SECURITY_AREA.Users)
        return AdministrationUsers;
      if (securityArea == SECURITY_AREA.SecurityProfiles)
        return AdministrationSecurityProfiles;
      if (securityArea == SECURITY_AREA.Flows)
        return AdministrationFlows;

      return SECURITY_ACCESS_LEVEL.None;
    }
  }
}
