﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class BaseResponse
  {
    public enum RESPONSE_CODE
    {
      Success = 200,
      BadRequest = 400,
      AccessDenied = 401,
      Error = 500,
      UserSessionExpired = 600,
      UserSessionInvalid,
      UserInvalid,
      UserLockedout,

      Duplicate,
      NoStartPluginDefined,
      DebugOnlyAllowedInDevelopmentServer,
    }
    public RESPONSE_CODE ResponseCode = RESPONSE_CODE.Success;
    public Packet.PACKET_TYPE PacketType = Packet.PACKET_TYPE._Unknown;
    public int PacketId;

    public BaseResponse(int packetId, Packet.PACKET_TYPE packetType)
    {
      PacketType = packetType;
      PacketId = packetId;
    }

    /// <summary>
    /// The constructor is for creating the response to send to the client, it is a helper constructor to allow translating from the RECORD_RESULT type being returned by some singleton managers.
    /// </summary>
    /// <param name="packetId">The unique packet identifier sent in the original request</param>
    /// <param name="results">Was it a Success, Error, or Duplicate</param>
    /// <param name="packetType">What kind of packet response is this</param>
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

    /// <summary>
    /// When a client receives a response this constructor is for parsing the ResponseCode and packet values
    /// </summary>
    /// <param name="packet"></param>
    public BaseResponse(FlowEngineCore.Administration.Packet packet)
    {
      packet.GetData<RESPONSE_CODE>(out this.ResponseCode);
    }

    public virtual FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = new(PacketType, PacketId);
      packet.AddData((int)ResponseCode);
      return packet;
    }
  }
}
