using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class UserLoginIdCheckResponse : BaseResponse
  {
    public string LoginIdSuggestion = "";
    public UserLoginIdCheckResponse(int packetId, BaseResponse.RESPONSE_CODE code, string loginIdSuggestion) : base(packetId, Packet.PACKET_TYPE.UserLoginIdCheckResponse)
    {
      ResponseCode = code;
      LoginIdSuggestion = loginIdSuggestion;
    }

    public UserLoginIdCheckResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      PacketType = packet.PacketType;
      packet.GetData(out this.LoginIdSuggestion);
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(LoginIdSuggestion);
      return packet;
    }
  }
}
