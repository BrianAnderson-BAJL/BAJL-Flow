using Core.Administration.Messages;
using Core.Administration.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Core.Administration.Messages.BaseResponse;

namespace Core.Administration
{
  internal class MessageProcessor
  {
    public delegate void ProcessMessageDelegate(Packet message, TcpClientBase client);
    private static Dictionary<Packet.PACKET_TYPE, ProcessMessageDelegate> Processors = new Dictionary<Packet.PACKET_TYPE, ProcessMessageDelegate>(128);


    public static void Init()
    {
      Processors.Add(Packet.PACKET_TYPE.UserAdd, ProcessUserAdd);
      Processors.Add(Packet.PACKET_TYPE.UserEdit, ProcessUserEdit);
      Processors.Add(Packet.PACKET_TYPE.UserDelete, ProcessUserDelete);
      Processors.Add(Packet.PACKET_TYPE.UserLogin, ProcessUserLogin);
      Processors.Add(Packet.PACKET_TYPE.UserLoginIdCheck, ProcessUserLoginIdCheck);
      Processors.Add(Packet.PACKET_TYPE.UsersGet, ProcessUsersGet);
    }

    public static void ProcessMessage(Packet packet, TcpClientBase client)
    {

      if (SecurityValid(packet, client) == false)
        return;

      ProcessMessageDelegate? processor;
      if (Processors.TryGetValue(packet.PacketType, out processor) == true)
      {
        try
        {
          processor(packet, client);
        }
        catch 
        {
          SendGenericError(packet, client, RESPONSE_CODE.Error);
        }
      }
      else
      {
        Global.Write("MessageProcessor - Can't process message, unknown packet type [{0}]", packet.PacketType.ToString());
      }
     
    }

    private static bool SecurityValid(Packet packet, TcpClientBase client)
    {
      BaseMessage message = new BaseMessage(packet);
      packet.ResetReadPosition();
      //Does the private key match?
      if (message.ServerKey != Options.AdministrationPrivateKey)
      {
        Global.Write("SECURITY ERROR!  Missing Server Key");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
        return false;
      }

      //If there are no users a UserAdd message is allowed without a valid user SessionKey (can't check user session keys if there are no users)
      if (message.PacketType == Packet.PACKET_TYPE.UserAdd && UserManager.UserCount == 0)
      {
        return true;
      }

      //Is it a login and the user password hash matches, if yes, create a new session key and timeout
      User? user = null;
      if (message.PacketType == Packet.PACKET_TYPE.UserLogin)
      {
        UserLogin? userLogin = new UserLogin(packet);
        packet.ResetReadPosition();
        if (userLogin is not null)
        {
          user = UserManager.FindByLoginId(userLogin.LoginId);

          if (user is null)
          {
            Global.Write("SECURITY ERROR!  User Login, User not found [{0}]", userLogin.LoginId);
            SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
            return false;
          }
          else
          {
            if (SecureHasherV1.Verify(userLogin.Password, user.passwordHash) == true)
            {
              user.SessionKey = SecureHasherV1.Hash(Guid.NewGuid().ToString()); //User has successfully logged in, lets generate a new session key
              user.SessionKeyExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(Options.AdministrationUserSessionKeyTimeoutInMinutes);
              return true;
            }
          }
        }
      }

      //Normal user message, just check the session key
      user = UserManager.FindBySessionKey(message.SessionKey);
      if (user is null)
      {
        Global.Write("SECURITY ERROR!  Missing Session Key");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.SessionInvalid);
        return false;
      }
      else
      {
        if (user.SessionKeyExpiration < DateTime.UtcNow)
        {
          user.SessionKey = "";
          user.SessionKeyExpiration = DateTime.MinValue;
          Global.Write("SECURITY ERROR!  Session Key has expired");
          SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.SessionInvalid);
          return false;
        }
        else
        {
        }
      }
      
      //If we got through all the possible errors, the user has passed the security check
      return true;
    }


    private static void SendGenericError(Packet packet, TcpClientBase client, BaseResponse.RESPONSE_CODE responseCode)
    {
      Packet.PACKET_TYPE type = packet.PacketType + 1; //Response messages are allways one bigger in the enum
      Global.Write("SendGenericError - response code [0], response PACKET_TYPE id [{1}]", responseCode.ToString(), type.ToString());
      BaseResponse response = new BaseResponse(type);
      response.ResponseCode = responseCode;
      client.Send(response.GetPacket());
    }

    private static void ProcessUserAdd(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      UserAdd? userAdd = new UserAdd(packet);
      USER_RESULT results = UserManager.Add(userAdd);
      BaseResponse response;
      if (results == USER_RESULT.Success)
      {
        if (userAdd is not null)
          Global.Write("ProcessUserAdd - response code [0], login id [{0}]", userAdd.LoginId);
        response = new BaseResponse(BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserAddResponse);
      }
      else
      {
        if (userAdd is not null)
          Global.Write("ProcessUserAdd - response code [1], login id [{0}]", userAdd.LoginId);
        response = new BaseResponse(BaseResponse.RESPONSE_CODE.Error, Packet.PACKET_TYPE.UserAddResponse);
      }
      client.Send(response.GetPacket());

    }

    private static void ProcessUserEdit(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      UserEdit? userEdit = new UserEdit(packet);
      User? user = UserManager.FindByLoginId(userEdit.OldLoginId);
      BaseResponse response;
      if (user is not null)
      {
        user.LoginId = userEdit.LoginId;
        if (userEdit.Password != "")
        {
          user.passwordHash = SecureHasherV1.Hash(userEdit.Password);
        }
        user.NameFirst = userEdit.NameFirst;
        user.NameSur = userEdit.NameSur;
        user.SecurityProfile = userEdit.SecurityProfile;
        UserManager.FileWrite();
        response = new BaseResponse(BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserEditResponse);
      }
      else
      {
        if (userEdit is not null)
          Global.Write("ProcessUserAdd - response code [1], login id [{0}]", userEdit.LoginId);
        response = new BaseResponse(BaseResponse.RESPONSE_CODE.Error, Packet.PACKET_TYPE.UserEditResponse);
      }
      client.Send(response.GetPacket());

    }

    private static void ProcessUserDelete(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      UserDelete userDelete = new UserDelete(packet);
      bool resp = UserManager.Delete(userDelete.LoginId);
      BaseResponse response;
      if (resp == true)
      {
        response = new BaseResponse(BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserDeleteResponse);
      }
      else
      {
        response = new BaseResponse(BaseResponse.RESPONSE_CODE.Error, Packet.PACKET_TYPE.UserDeleteResponse);
      }
      Global.Write("ProcessUserDelete - response code [0], login id [{0}]", response.ResponseCode.ToString(), userDelete.LoginId);
      client.Send(response.GetPacket());

    }

    private static void ProcessUserLogin(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      //If we got here, the user successfully logged in in the SecurityValid function
      UserLogin? userLogin = new UserLogin(packet);
      if (userLogin is not null)
      {
        User? user = UserManager.FindByLoginId(userLogin.LoginId);
        if (user is not null)
        {
          Global.Write("ProcessUserLogin - response code [0], login id [{0}]", user.LoginId);
          UserLoginResponse response = new UserLoginResponse(user.LoginId, user.SecurityProfile, user.NameFirst, user.NameSur, user.SessionKey, user.SessionKeyExpiration);
          client.Send(response.GetPacket());
        }
      }
    }
    private static void ProcessUserLoginIdCheck(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      UserLoginIdCheck? loginCheck = new UserLoginIdCheck(packet);
      if (loginCheck is not null)
      {
        string SuggestedLoginId;
        USER_RESULT result = UserManager.CheckLoginIdInUse(loginCheck.LoginId, out SuggestedLoginId);
        BaseResponse.RESPONSE_CODE responseCode = BaseResponse.RESPONSE_CODE.Error;
        if (result == USER_RESULT.Success)
        {
          responseCode = BaseResponse.RESPONSE_CODE.Success;
        }
        else if (result == USER_RESULT.DuplicateLoginId)
        {
          responseCode = BaseResponse.RESPONSE_CODE.LoginIdDuplicate;
        }
        Global.Write("ProcessUserLoginIdCheck - response code [{0}], suggested login id [{1}]", responseCode.ToString(), SuggestedLoginId);
        UserLoginIdCheckResponse response = new UserLoginIdCheckResponse(responseCode, SuggestedLoginId);
        client.Send(response.GetPacket());
      }

    }

    private static void ProcessUsersGet(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      UsersGetResponse response = new UsersGetResponse(UserManager.GetUsers.ToArray());
      client.Send(response.GetPacket());
    }

  }
}
