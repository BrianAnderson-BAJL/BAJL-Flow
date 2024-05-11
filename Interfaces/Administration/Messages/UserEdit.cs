using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UserEdit : UserAdd
  {
    public string OldLoginId = "";
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public UserEdit(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out this.OldLoginId);
    }
    public UserEdit(string serverKey, string sessionKey, string oldLoginId, string loginName, string password, string firstName, string surName, string securityProfile) : base(serverKey, sessionKey, loginName, password, firstName, surName, securityProfile)
    {
      PacketType = Packet.PACKET_TYPE.UserEdit;
      OldLoginId = oldLoginId;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(OldLoginId);
      return packet;
    }


  }
}
