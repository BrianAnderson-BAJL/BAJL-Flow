using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UsersGetResponse : BaseResponse
  {
    public List<User> Users;
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public UsersGetResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out int Count);
      Users = new List<User>(Count);
      for (int x = 0; x < Count; x++)
      {
        packet.GetData(out string loginId);
        packet.GetData(out string nameFirst);
        packet.GetData(out string nameSur);
        packet.GetData(out string securityProfile);
        User user = new();
        user.LoginId = loginId;
        user.NameFirst = nameFirst;
        user.NameSur = nameSur;
        user.SecurityProfileNameTemp = securityProfile;
        Users.Add(user);
      }
    }
    public UsersGetResponse(int packetId, User[] users) : base(packetId, Packet.PACKET_TYPE.UsersGetResponse)
    {
      this.Users = users.ToList();
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Users.Count);  //Add the number of users
      for (int x = 0; x < Users.Count; x++)
      {
        packet.AddData(Users[x].LoginId);
        packet.AddData(Users[x].NameFirst);
        packet.AddData(Users[x].NameSur);
        packet.AddData(Users[x].SecurityProfile.Name);
      }
      return packet;
    }

  }
}
