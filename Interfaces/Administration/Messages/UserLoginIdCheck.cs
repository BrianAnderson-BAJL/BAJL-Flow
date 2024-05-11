using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UserLoginIdCheck : BaseMessage
  {
    public string LoginId = "";


    public UserLoginIdCheck(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.LoginId);
    }

    public UserLoginIdCheck(string serverKey, string sessionKey, string loginId) : base(serverKey, sessionKey, Packet.PACKET_TYPE.UserLoginIdCheck)
    {
      this.LoginId = loginId;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.LoginId);
      return packet;
    }
  }
}
