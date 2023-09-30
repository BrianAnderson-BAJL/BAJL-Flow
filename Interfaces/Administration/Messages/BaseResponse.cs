using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration.Messages
{
  public class BaseResponse
  {
    public enum RESPONSE_CODE
    {
      Success = 0,
      Error = 1,
      AccessDenied,
      SessionInvalid,
      LoginIdDuplicate,
    }
    public RESPONSE_CODE ResponseCode = RESPONSE_CODE.Success;
    public Packet.PACKET_TYPE PacketType = Packet.PACKET_TYPE._Unknown;

    public BaseResponse(Packet.PACKET_TYPE packetType)
    {
      PacketType = packetType;
    }
    public BaseResponse(RESPONSE_CODE responseCode, Packet.PACKET_TYPE packetType)
    {
      ResponseCode = responseCode;
      PacketType = packetType;
    }

    public BaseResponse(Core.Administration.Packet packet)
    {
      packet.GetData(out this.ResponseCode);
    }

    public virtual Core.Administration.Packet GetPacket()
    {
      Packet packet = new Packet(PacketType);
      packet.AddData((int)ResponseCode);
      return packet;
    }
  }
}
