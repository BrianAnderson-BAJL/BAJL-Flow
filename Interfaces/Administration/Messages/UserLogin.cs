using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UserLogin : BaseMessage
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
    public UserLogin(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.LoginId);
      packet.GetData(out this.Password);
    }
    public UserLogin(string serverKey, string sessionKey, string loginName, string password) : base(serverKey, sessionKey, Packet.PACKET_TYPE.UserLogin)
    {
      this.LoginId = loginName;
      this.Password = password;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.LoginId);
      packet.AddData(this.Password);
      return packet;
    }


  }
}
