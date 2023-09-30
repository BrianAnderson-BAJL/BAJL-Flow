﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class UserLoginIdCheckResponse : BaseResponse
  {
    public string LoginIdSuggestion = "";
    public UserLoginIdCheckResponse(BaseResponse.RESPONSE_CODE code, string loginIdSuggestion) : base(Packet.PACKET_TYPE.UserLoginIdCheckResponse)
    {
      ResponseCode = code;
      LoginIdSuggestion = loginIdSuggestion;
    }

    public UserLoginIdCheckResponse(Core.Administration.Packet packet) : base(packet)
    {
      
      PacketType = packet.PacketType;
      packet.GetData(out this.LoginIdSuggestion);
    }

    public override Core.Administration.Packet GetPacket()
    {
      Packet packet = new Packet(PacketType);
      packet.AddData(ResponseCode);
      packet.AddData(LoginIdSuggestion);
      return packet;
    }
  }
}