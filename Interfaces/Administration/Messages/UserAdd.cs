using Core.Administration.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class UserAdd : BaseMessage
  {
    public string LoginId = "";
    public string Password = "";
    public string NameFirst = "";
    public string NameSur = "";
    public string SecurityProfile = "";

    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public UserAdd(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.LoginId);
      packet.GetData(out this.Password);
      packet.GetData(out this.NameFirst);
      packet.GetData(out this.NameSur);
      packet.GetData(out this.SecurityProfile);
    }
    public UserAdd(string serverKey, string sessionKey, string loginName, string password, string firstName, string surName, string securityProfile) : base(serverKey, sessionKey, Packet.PACKET_TYPE.UserAdd)
    {
      this.LoginId = loginName;
      this.Password = password;
      this.NameFirst = firstName;
      this.NameSur = surName;
      this.SecurityProfile = securityProfile;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.LoginId);
      packet.AddData(this.Password);
      packet.AddData(this.NameFirst);
      packet.AddData(this.NameSur);
      packet.AddData(this.SecurityProfile);
      return packet;
    }

    
  }
}
