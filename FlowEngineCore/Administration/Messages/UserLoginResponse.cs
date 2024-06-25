using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UserLoginResponse : BaseResponse
  {
    public string LoginId = "";
    public string SecurityProfile = "";
    public string NameFirst = "";
    public string NameSur = "";
    public string SessionKey = "";
    public DateTime SessionKeyExpiration = DateTime.MinValue;
    public bool NeedToChangePassword = false;
    public UserLoginResponse(int packetId, string loginId, string securityProfile, string nameFirst, string nameSur, string sessionKey, DateTime sessionKeyExpiration, bool needToChangePassword) : base(packetId, Packet.PACKET_TYPE.UserLoginResponse)
    {
      this.LoginId = loginId;
      this.SecurityProfile = securityProfile;
      this.NameFirst = nameFirst;
      this.NameSur = nameSur;
      this.SessionKey = sessionKey;
      this.SessionKeyExpiration = sessionKeyExpiration;
      NeedToChangePassword = needToChangePassword;
    }

    public UserLoginResponse(FlowEngineCore.Administration.Packet packet) : base(packet) 
    {
      packet.GetData(out LoginId);
      packet.GetData(out SecurityProfile);
      packet.GetData(out NameFirst);
      packet.GetData(out NameSur);
      packet.GetData(out SessionKey);
      packet.GetData(out SessionKeyExpiration);
      packet.GetData(out NeedToChangePassword);
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(LoginId);
      packet.AddData(SecurityProfile);
      packet.AddData(NameFirst);
      packet.AddData(NameSur);
      packet.AddData(SessionKey);
      packet.AddData(SessionKeyExpiration);
      packet.AddData(NeedToChangePassword);
      return packet;
    }
  }
}
