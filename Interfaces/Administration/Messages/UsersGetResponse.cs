using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class UsersGetResponse : BaseResponse
  {
    public List<User> Users;
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public UsersGetResponse(Core.Administration.Packet packet) : base(packet)
    {
      int Count;
      packet.GetData(out Count);
      Users = new List<User>(Count);
      for (int x = 0; x < Count; x++)
      {
        string loginId;
        string nameFirst;
        string nameSur;
        string securityProfile;
        packet.GetData(out loginId);
        packet.GetData(out nameFirst);
        packet.GetData(out nameSur);
        packet.GetData(out securityProfile);
        User user = new User();
        user.LoginId = loginId;
        user.NameFirst = nameFirst;
        user.NameSur = nameSur;
        user.SecurityProfile = securityProfile;
        Users.Add(user);
      }
    }
    public UsersGetResponse(int packetId, User[] users) : base(packetId, Packet.PACKET_TYPE.UsersGetResponse)
    {
      this.Users = users.ToList();
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.Users.Count);  //Add the number of users
      for (int x = 0; x < Users.Count; x++)
      {
        packet.AddData(Users[x].LoginId);
        packet.AddData(Users[x].NameFirst);
        packet.AddData(Users[x].NameSur);
        packet.AddData(Users[x].SecurityProfile);
      }
      return packet;
    }

  }
}
