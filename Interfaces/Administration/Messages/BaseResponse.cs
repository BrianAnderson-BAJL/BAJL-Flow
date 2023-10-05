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
      Duplicate,
    }
    public RESPONSE_CODE ResponseCode = RESPONSE_CODE.Success;
    public Packet.PACKET_TYPE PacketType = Packet.PACKET_TYPE._Unknown;
    public int PacketId;
    public BaseResponse(int packetId, Packet.PACKET_TYPE packetType)
    {
      PacketType = packetType;
      PacketId = packetId;
    }
    public BaseResponse(int packetId, RECORD_RESULT results, Packet.PACKET_TYPE packetType)
    {
      if (results == RECORD_RESULT.Success)
        ResponseCode = RESPONSE_CODE.Success;
      else if (results == RECORD_RESULT.Error)
        ResponseCode = RESPONSE_CODE.Duplicate;
      else
        ResponseCode = RESPONSE_CODE.Error;

      PacketType = packetType;
      PacketId = packetId;
    }
    public BaseResponse(int packetId, RESPONSE_CODE responseCode, Packet.PACKET_TYPE packetType)
    {
      ResponseCode = responseCode;
      PacketType = packetType;
      PacketId = packetId;
    }

    public BaseResponse(Core.Administration.Packet packet)
    {
      packet.GetData(out this.ResponseCode);
    }

    public virtual Core.Administration.Packet GetPacket()
    {
      Packet packet = new Packet(PacketType, PacketId);
      packet.AddData((int)ResponseCode);
      return packet;
    }
  }
}
