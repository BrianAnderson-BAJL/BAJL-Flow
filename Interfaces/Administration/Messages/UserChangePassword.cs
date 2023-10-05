using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class UserChangePassword : BaseMessage
  {
    public string LoginId = "";
    public string OldPassword = "";
    public string NewPassword = "";

    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public UserChangePassword(Core.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.LoginId);
      packet.GetData(out this.OldPassword);
      packet.GetData(out this.NewPassword);
    }
    public UserChangePassword(string serverKey, string sessionKey, string loginName, string oldPassword, string newPassword) : base(serverKey, sessionKey, Packet.PACKET_TYPE.UserChangePassword)
    {
      this.LoginId = loginName;
      this.OldPassword = oldPassword;
      this.NewPassword = newPassword;
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.LoginId);
      packet.AddData(this.OldPassword);
      packet.AddData(this.NewPassword);
      return packet;
    }


  }
}
