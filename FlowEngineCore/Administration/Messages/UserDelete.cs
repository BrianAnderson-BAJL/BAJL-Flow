using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UserDelete : BaseMessage
  {
    public string LoginId = "";
    public UserDelete(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.LoginId);
    }
    public UserDelete(string serverKey, string sessionKey, string loginId) : base(serverKey, sessionKey, Packet.PACKET_TYPE.UserDelete)
    {
      LoginId = loginId;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(LoginId);
      return packet;
    }


  }
}
