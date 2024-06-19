using FlowEngineCore.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
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

    public enum SECURITY_ACCESS_SIMPLE
    {
      None,
      Access,
    }

    public enum SECURITY_AREA
    {
      Users,
      SecurityProfiles,
      Flows,
      Statistics,
    }


    public string Name = "";

    public SECURITY_ACCESS_LEVEL AdministrationUsers = SECURITY_ACCESS_LEVEL.None;
    public SECURITY_ACCESS_LEVEL AdministrationSecurityProfiles = SECURITY_ACCESS_LEVEL.None;
    public SECURITY_ACCESS_LEVEL AdministrationFlows = SECURITY_ACCESS_LEVEL.None;
    public SECURITY_ACCESS_LEVEL ServerSettings = SECURITY_ACCESS_LEVEL.Readonly;
    public SECURITY_ACCESS_SIMPLE Statistics = SECURITY_ACCESS_SIMPLE.None;

    /// <summary>
    /// Returns a blank SecurityProfile with no access to anything.
    /// </summary>
    public static SecurityProfile NoAccess
    {
      get { return new SecurityProfile("NO ACCESS"); }
    }

    public SecurityProfile()
    {
    }

    public SecurityProfile(string name)
    {
      Name = name;
    }

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
        case Packet.PACKET_TYPE.CloseConnection:
          rc = true; //Always true
          break;

        case Packet.PACKET_TYPE.UsersGet:
        case Packet.PACKET_TYPE.UserLoginIdCheck:
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

        case Packet.PACKET_TYPE.FlowsGet:
        case Packet.PACKET_TYPE.FlowOpen:
          rc = AdministrationFlows >= SECURITY_ACCESS_LEVEL.Readonly; break;
        case Packet.PACKET_TYPE.FlowSave:
        case Packet.PACKET_TYPE.FlowDebug:
        case Packet.PACKET_TYPE.FlowDebugAlways:
          rc = AdministrationFlows >= SECURITY_ACCESS_LEVEL.Full; break;
        case Packet.PACKET_TYPE.ServerSettingsGet:
          rc = ServerSettings >= SECURITY_ACCESS_LEVEL.Readonly; break;
        case Packet.PACKET_TYPE.ServerSettingsEdit:
          rc = ServerSettings >= SECURITY_ACCESS_LEVEL.Edit; break;
        case Packet.PACKET_TYPE.StatisticsRegister:
        case Packet.PACKET_TYPE.StatisticsDeregister:
          rc = Statistics >= SECURITY_ACCESS_SIMPLE.Access; break;
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
      if (securityArea == SECURITY_AREA.Statistics)
        return ConvertToLevel(Statistics);

      return SECURITY_ACCESS_LEVEL.None;
    }

    private SECURITY_ACCESS_LEVEL ConvertToLevel(SECURITY_ACCESS_SIMPLE access)
    {
      if (access == SECURITY_ACCESS_SIMPLE.Access)
        return SECURITY_ACCESS_LEVEL.Full;
      else
        return SECURITY_ACCESS_LEVEL.None;
    }
  }
}
