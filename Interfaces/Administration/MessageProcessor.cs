﻿using Core.Administration.Messages;
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
      Processors.Add(Packet.PACKET_TYPE.UserChangePassword, ProcessUserChangePassword);
      Processors.Add(Packet.PACKET_TYPE.SecurityProfilesGet, ProcessSecurityProfilesGet);
      Processors.Add(Packet.PACKET_TYPE.SecurityProfileAdd, ProcessSecurityProfileAdd);
      Processors.Add(Packet.PACKET_TYPE.SecurityProfileEdit, ProcessSecurityProfileEdit);
      Processors.Add(Packet.PACKET_TYPE.SecurityProfileDelete, ProcessSecurityProfileDelete);
      Processors.Add(Packet.PACKET_TYPE.FlowsGet, ProcessFlowsGet);

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
        SendGenericError(packet, client, RESPONSE_CODE.Error);
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
        if (userLogin is null)
        {
          return false;
        }
        user = UserManager.FindByLoginId(userLogin.LoginId);

        if (user is null)
        {
          Global.Write("SECURITY ERROR!  User Login, User not found [{0}]", userLogin.LoginId);
          SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
          return false;
        }
        if (SecureHasherV1.Verify(userLogin.Password, user.passwordHash) == true)
        {
          user.SessionKey = SecureHasherV1.Hash(Guid.NewGuid().ToString()); //User has successfully logged in, lets generate a new session key
          user.SessionKeyExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(Options.AdministrationUserSessionKeyTimeoutInMinutes);
          return true;
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
      if (user.SessionKeyExpiration < DateTime.UtcNow)
      {
        user.SessionKey = "";
        user.SessionKeyExpiration = DateTime.MinValue;
        Global.Write("SECURITY ERROR!  Session Key has expired");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.SessionInvalid);
        return false;
      }

      SecurityProfile? sp = SecurityProfileManager.FindByName(user.SecurityProfile);
      if (sp is null)
      {
        Global.Write("SECURITY ERROR!  Missing Security profile for user");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
        return false; //No security profile defined for this user, they don't have access
      }
      if (sp.HasAccessServer(packet.PacketType) == false)
      {
        Global.Write("SECURITY ERROR!  User's Security profile does not allow this action");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
        return false; //User's security profile doesn't allow them to perform this function
      }
      
      //If we got through all the possible errors, the user has passed the security check
      return true;
    }


    private static void SendGenericError(Packet packet, TcpClientBase client, BaseResponse.RESPONSE_CODE responseCode)
    {
      Packet.PACKET_TYPE type = packet.PacketType + 1; //Response messages are allways one bigger in the enum
      Global.Write("SendGenericError - response code [0], response PACKET_TYPE id [{1}]", responseCode.ToString(), type.ToString());
      BaseResponse response = new BaseResponse(packet.PacketId, type);
      response.ResponseCode = responseCode;
      client.Send(response.GetPacket());
    }

    private static void ProcessUserAdd(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      UserAdd? userAdd = new UserAdd(packet);
      RECORD_RESULT results = UserManager.Add(userAdd);
      Global.Write("ProcessUserAdd - Record result [{0}], login id [{1}]", results.ToString(), userAdd.LoginId);
      BaseResponse response = new BaseResponse(packet.PacketId, results, Packet.PACKET_TYPE.UserAddResponse);
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
        response = new BaseResponse(packet.PacketId, BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserEditResponse);
      }
      else
      {
        if (userEdit is not null)
          Global.Write("ProcessUserAdd - response code [1], login id [{0}]", userEdit.LoginId);
        response = new BaseResponse(packet.PacketId, BaseResponse.RESPONSE_CODE.Error, Packet.PACKET_TYPE.UserEditResponse);
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
        response = new BaseResponse(packet.PacketId, BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserDeleteResponse);
      }
      else
      {
        response = new BaseResponse(packet.PacketId, BaseResponse.RESPONSE_CODE.Error, Packet.PACKET_TYPE.UserDeleteResponse);
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
          UserLoginResponse response = new UserLoginResponse(packet.PacketId, user.LoginId, user.SecurityProfile, user.NameFirst, user.NameSur, user.SessionKey, user.SessionKeyExpiration, user.NeedToChangePassword);
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
        RECORD_RESULT result = UserManager.CheckLoginIdInUse(loginCheck.LoginId, out SuggestedLoginId);
        BaseResponse.RESPONSE_CODE responseCode = BaseResponse.RESPONSE_CODE.Error;
        if (result == RECORD_RESULT.Success)
        {
          responseCode = BaseResponse.RESPONSE_CODE.Success;
        }
        else if (result == RECORD_RESULT.Duplicate)
        {
          responseCode = BaseResponse.RESPONSE_CODE.Duplicate;
        }
        Global.Write("ProcessUserLoginIdCheck - response code [{0}], suggested login id [{1}]", responseCode.ToString(), SuggestedLoginId);
        UserLoginIdCheckResponse response = new UserLoginIdCheckResponse(packet.PacketId, responseCode, SuggestedLoginId);
        client.Send(response.GetPacket());
      }

    }

    private static void ProcessUsersGet(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      UsersGetResponse response = new UsersGetResponse(packet.PacketId, UserManager.GetUsers.ToArray());
      client.Send(response.GetPacket());
    }

    private static void ProcessUserChangePassword(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      //If we got here, the user successfully logged in the SecurityValid function
      UserChangePassword? data = new UserChangePassword(packet);
      if (data is not null)
      {
        User? user = UserManager.FindByLoginId(data.LoginId);
        if (user is not null)
        {
          user.passwordHash = SecureHasherV1.Hash(data.NewPassword);
          Global.Write("ProcessUserChangePassword - response code [0], login id [{0}]", user.LoginId);
          BaseResponse response = new BaseResponse(packet.PacketId, BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserChangePasswordResponse);
          client.Send(response.GetPacket());
          UserManager.FileWrite();
        }
      }
    }


    private static void ProcessSecurityProfilesGet(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      SecurityProfilesGetResponse response = new SecurityProfilesGetResponse(packet.PacketId, SecurityProfileManager.GetProfiles.ToArray());
      client.Send(response.GetPacket());
    }

    private static void ProcessSecurityProfileAdd(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      SecurityProfileAdd data = new SecurityProfileAdd(packet);
      RECORD_RESULT result = SecurityProfileManager.Add(data);
      BaseResponse response = new BaseResponse(packet.PacketId, result, Packet.PACKET_TYPE.SecurityProfileAddResponse);
      client.Send(response.GetPacket());
    }

    private static void ProcessSecurityProfileEdit(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      SecurityProfileEdit data = new SecurityProfileEdit(packet);
      RECORD_RESULT result = SecurityProfileManager.Edit(data);
      BaseResponse response = new BaseResponse(packet.PacketId, result, Packet.PACKET_TYPE.SecurityProfileEditResponse);
      client.Send(response.GetPacket());
    }

    private static void ProcessSecurityProfileDelete(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      SecurityProfileDelete data = new SecurityProfileDelete(packet);
      RECORD_RESULT result = SecurityProfileManager.Delete(data);
      BaseResponse response = new BaseResponse(packet.PacketId, result, Packet.PACKET_TYPE.SecurityProfileDeleteResponse);
      client.Send(response.GetPacket());
    }
    private static void ProcessFlowsGet(Core.Administration.Packet packet, Core.Administration.TcpClientBase client)
    {
      FlowsGet data = new FlowsGet(packet);
      string path = Options.GetFullPath(Options.FlowPath);
      string relativePath = "";
      Xml xml = new Xml();
      xml.WriteMemoryNew();
      xml.WriteTagStart("Directories");
      WriteSubDirectory(path, relativePath, xml, 0);
      xml.WriteTagEnd("Directories");
      string flowsXml = xml.ReadMemory();
      FlowsGetResponse response = new FlowsGetResponse(packet.PacketId, flowsXml);
      client.Send(response.GetPacket());
    }

    private static void WriteSubDirectory(string path, string relativePath, Xml xml, int depth)
    {
      xml.WriteTagStart("Directory" + depth.ToString()); //Adding depth to the Directory tag to make my XML parsing easier, cheap hack! //TODO: Fix the cheesy/cheap XML hack with a real XML parser
      xml.WriteTagAndContents("Path", relativePath);
      string[] files = Directory.GetFiles(path, "*.flow");
      for (int y = 0; y < files.Length; y++)
      {
        Flow flow = new Flow();
        flow.XmlRead(files[y], Flow.READ_TIL.TilSteps);
        xml.WriteTagStart("File");
        xml.WriteTagAndContents("FileName", Path.GetFileName(files[y]));
        xml.WriteTagAndContents("ModifiedDateTime", flow.ModifiedLastDateTime);
        if (flow.StartPlugin is not null)
          xml.WriteTagAndContents("PluginStarting", flow.StartPlugin.Name);
        xml.WriteTagAndContents("StartCommands", flow.FormatStartCommands());
        xml.WriteTagEnd("File");
      }

      if (Options.FlowPathAllowSubDirectories == true)
      {
        string[] dirs = Directory.GetDirectories(path);
        for (int x = 0; x < dirs.Length; x++)
        {
          string strippedPath = Global.StripOff(path, dirs[x]);
          WriteSubDirectory(path + "/" + strippedPath, relativePath + "/" + strippedPath, xml, depth + 1);
        }
      }
      xml.WriteTagEnd("Directory" + depth.ToString());
    }

  }
}
