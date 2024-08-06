using FlowEngineCore.Administration.Messages;
using FlowEngineCore.Interfaces;
using FlowEngineCore.Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static FlowEngineCore.Administration.Messages.BaseResponse;

namespace FlowEngineCore.Administration
{
  internal class MessageProcessor
  {
    public delegate void ProcessMessageDelegate(Packet message, TcpClientBase client);
    private static Dictionary<Packet.PACKET_TYPE, ProcessMessageDelegate> Processors = new(128);
    private static ILog? mLog = null;

    private static string CacheAdministrationPrivateKey = "";
    private static int CacheAdministrationUserMaxLoginAttempts = 0;
    private static int CacheAdministrationUserLockOutMinutes = 0;
    private static int CacheAdministrationUserSessionKeyTimeoutInMinutes = 0;

    public static void Init(Dictionary<string, object> GlobalPluginValues)
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
      Processors.Add(Packet.PACKET_TYPE.FlowSave, ProcessFlowSave);
      Processors.Add(Packet.PACKET_TYPE.FlowOpen, ProcessFlowOpen);
      Processors.Add(Packet.PACKET_TYPE.FlowDebug, ProcessFlowDebug);
      Processors.Add(Packet.PACKET_TYPE.CloseConnection, ProcessCloseConnection);
      Processors.Add(Packet.PACKET_TYPE.FlowDebugAlways, ProcessFlowDebugAlways);
      Processors.Add(Packet.PACKET_TYPE.ServerSettingsGet, ProcessServerSettingsGet);
      Processors.Add(Packet.PACKET_TYPE.ServerSettingsEdit, ProcessServerSettingsEdit);
      Processors.Add(Packet.PACKET_TYPE.StatisticsRegister, ProcessStatisticsRegister);
      Processors.Add(Packet.PACKET_TYPE.StatisticsDeregister, ProcessStatisticsDeregister);
      Processors.Add(Packet.PACKET_TYPE.TemplatesGet, ProcessTemplatesGet);


      if (GlobalPluginValues.ContainsKey("log") == true)
      {
        mLog = GlobalPluginValues["log"] as ILog;
      }

      CacheAdministrationPrivateKey = Options.GetSettings.SettingGetAsString("PrivateKey");
      CacheAdministrationUserMaxLoginAttempts = Options.GetSettings.SettingGetAsInt("UserMaxLoginAttempts");
      CacheAdministrationUserLockOutMinutes = Options.GetSettings.SettingGetAsInt("UserLockOutMinutes");
      CacheAdministrationUserSessionKeyTimeoutInMinutes = Options.GetSettings.SettingGetAsInt("UserSessionKeyTimeoutInMinutes");
    }

    public static void ProcessMessage(Packet packet, TcpClientBase client)
    {

      if (SecurityValid(packet, client) == false)
        return;

      if (Processors.TryGetValue(packet.PacketType, out ProcessMessageDelegate? processor) == true)
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
        mLog?.Write($"MessageProcessor - Can't process message, unknown packet type [{packet.PacketType}]");
        SendGenericError(packet, client, RESPONSE_CODE.Error);
      }

    }

    private static bool SecurityValid(Packet packet, TcpClientBase client)
    {
      BaseMessage message = new(packet);
      packet.ResetReadPosition();
      //Does the private key match?
      if (message.ServerKey != CacheAdministrationPrivateKey)
      {
        mLog?.Write("SECURITY ERROR!  Server Key mismatch");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
        return false;
      }

      //If there are no users a UserAdd message is allowed without a valid user SessionKey (can't check user session keys if there are no users)
      if (message.PacketType == Packet.PACKET_TYPE.UserAdd && UserManager.UserCount == 0)
      {
        return true;
      }

      //Is it a login and the user password hash matches, if yes, create a new session key and timeout
      User? user;
      if (message.PacketType == Packet.PACKET_TYPE.UserLogin)
      {
        UserLogin? userLogin = new(packet);
        packet.ResetReadPosition();
        if (userLogin is null)
        {
          return false;
        }
        user = UserManager.FindByLoginId(userLogin.LoginId);

        if (user is null)
        {
          mLog?.Write($"SECURITY ERROR!  User Login, User not found [{userLogin.LoginId}]");
          SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.UserInvalid);
          return false;
        }
        if (user.LockOutUntil > DateTime.UtcNow)
        {
          mLog?.Write($"SECURITY ERROR!  User Login, User is locked out for another [{(user.LockOutUntil - DateTime.UtcNow).TotalMinutes}] minutes");
          SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.UserLockedout);
          return false;
        }
        if (SecureHasherV1.Verify(userLogin.Password, user.passwordHash) == false)
        {
          user.LoginAttempts++;
          mLog?.Write($"SECURITY ERROR!  User Login, User password does not match Attempt number [{user.LoginAttempts}]");
          if (user.LoginAttempts >= CacheAdministrationUserMaxLoginAttempts && CacheAdministrationUserMaxLoginAttempts > 0)
          {
            user.LockOutUntil = DateTime.UtcNow.AddMinutes(CacheAdministrationUserLockOutMinutes);
            mLog?.Write($"SECURITY ERROR!  User Login, User login attempts is [{CacheAdministrationUserMaxLoginAttempts}] User is being locked out for [{(user.LockOutUntil - DateTime.UtcNow).TotalMinutes}] minutes");
          }
          return false;
        }
        user.LoginAttempts = 0;
        user.SessionKey = SecureHasherV1.SessionIdCreate(); //User has successfully logged in, lets generate a new session key
        user.SessionKeyExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(CacheAdministrationUserSessionKeyTimeoutInMinutes);
        return true;

      }

      //Normal user message, just check the session key
      user = UserManager.FindBySessionKey(message.SessionKey);
      if (user is null)
      {
        mLog?.Write("SECURITY ERROR!  Missing Session Key");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.UserSessionInvalid);
        return false;
      }
      if (user.SessionKeyExpiration < DateTime.UtcNow)
      {
        user.SessionKey = "";
        user.SessionKeyExpiration = DateTime.MinValue;
        mLog?.Write("SECURITY ERROR!  Session Key has expired");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.UserSessionExpired);
        return false;
      }

      if (user.SecurityProfile is null)
      {
        mLog?.Write("SECURITY ERROR!  Missing Security profile for user");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
        return false; //No security profile defined for this user, they don't have access
      }
      if (user.SecurityProfile.HasAccessServer(packet.PacketType) == false)
      {
        mLog?.Write("SECURITY ERROR!  User's Security profile does not allow this action");
        SendGenericError(packet, client, BaseResponse.RESPONSE_CODE.AccessDenied);
        return false; //User's security profile doesn't allow them to perform this function
      }

      //If we got through all the possible errors, the user has passed the security check

      user.TcpClientConnection = client; //Assign this TCP connection to the user

      return true;
    }


    private static void SendGenericError(Packet packet, TcpClientBase client, BaseResponse.RESPONSE_CODE responseCode)
    {
      Packet.PACKET_TYPE type = packet.PacketType + 1; //Response messages are allways one bigger in the enum
      mLog?.Write($"SendGenericError - response code [{responseCode}], response PACKET_TYPE id [{type}]");
      BaseResponse response = new(packet.PacketId, type);
      response.ResponseCode = responseCode;
      client.Send(response.GetPacket());
    }

    private static void ProcessUserAdd(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      UserAdd? userAdd = new(packet);
      RECORD_RESULT results = UserManager.Add(userAdd);
      mLog?.Write($"ProcessUserAdd - Record result [{results}], login id [{userAdd.LoginId}]");
      BaseResponse response = new(packet.PacketId, results, Packet.PACKET_TYPE.UserAddResponse);
      client.Send(response.GetPacket());
    }

    private static void ProcessUserEdit(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      UserEdit? userEdit = new(packet);
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
        user.SecurityProfile = SecurityProfileManager.FindByName(userEdit.SecurityProfile);
        UserManager.Save();
        response = new BaseResponse(packet.PacketId, BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserEditResponse);
      }
      else
      {
        if (userEdit is not null)
          mLog?.Write($"ProcessUserAdd - response code [1], login id [{userEdit.LoginId}]");
        response = new BaseResponse(packet.PacketId, BaseResponse.RESPONSE_CODE.Error, Packet.PACKET_TYPE.UserEditResponse);
      }
      client.Send(response.GetPacket());

    }

    private static void ProcessUserDelete(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      UserDelete userDelete = new(packet);
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
      mLog?.Write($"ProcessUserDelete - response code [{response.ResponseCode}], login id [{userDelete.LoginId}]");
      client.Send(response.GetPacket());

    }

    private static void ProcessUserLogin(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      //If we got here, the user successfully logged in in the SecurityValid function
      UserLogin? userLogin = new(packet);
      if (userLogin is not null)
      {
        User? user = UserManager.FindByLoginId(userLogin.LoginId);
        if (user is not null)
        {
          mLog?.Write($"ProcessUserLogin - response code [0], login id [{user.LoginId}]");
          UserLoginResponse response = new(packet.PacketId, user.LoginId, user.SecurityProfile.Name, user.NameFirst, user.NameSur, user.SessionKey, user.SessionKeyExpiration, user.NeedToChangePassword);
          client.Send(response.GetPacket());
        }
      }
    }
    private static void ProcessUserLoginIdCheck(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      UserLoginIdCheck? loginCheck = new(packet);
      if (loginCheck is not null)
      {
        RECORD_RESULT result = UserManager.CheckLoginIdInUse(loginCheck.LoginId, out string SuggestedLoginId);
        BaseResponse.RESPONSE_CODE responseCode = BaseResponse.RESPONSE_CODE.Error;
        if (result == RECORD_RESULT.Success)
        {
          responseCode = BaseResponse.RESPONSE_CODE.Success;
        }
        else if (result == RECORD_RESULT.Duplicate)
        {
          responseCode = BaseResponse.RESPONSE_CODE.Duplicate;
        }
        mLog?.Write($"ProcessUserLoginIdCheck - response code [{responseCode}], suggested login id [{SuggestedLoginId}]");
        UserLoginIdCheckResponse response = new(packet.PacketId, responseCode, SuggestedLoginId);
        client.Send(response.GetPacket());
      }

    }

    private static void ProcessUsersGet(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      UsersGetResponse response = new(packet.PacketId, UserManager.GetUsers.ToArray());
      client.Send(response.GetPacket());
    }

    private static void ProcessUserChangePassword(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      //If we got here, the user successfully logged in the SecurityValid function
      UserChangePassword? data = new(packet);
      if (data is not null)
      {
        User? user = UserManager.FindByLoginId(data.LoginId);
        if (user is not null)
        {
          user.passwordHash = SecureHasherV1.Hash(data.NewPassword);
          mLog?.Write($"ProcessUserChangePassword - response code [0], login id [{user.LoginId}]");
          BaseResponse response = new(packet.PacketId, BaseResponse.RESPONSE_CODE.Success, Packet.PACKET_TYPE.UserChangePasswordResponse);
          client.Send(response.GetPacket());
          UserManager.Save();
        }
      }
    }


    private static void ProcessSecurityProfilesGet(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      SecurityProfilesGetResponse response = new(packet.PacketId, SecurityProfileManager.GetProfiles.ToArray());
      client.Send(response.GetPacket());
    }

    private static void ProcessSecurityProfileAdd(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      SecurityProfileAdd data = new(packet);
      RECORD_RESULT result = SecurityProfileManager.Add(data);
      BaseResponse response = new(packet.PacketId, result, Packet.PACKET_TYPE.SecurityProfileAddResponse);
      client.Send(response.GetPacket());
    }

    private static void ProcessSecurityProfileEdit(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      SecurityProfileEdit data = new(packet);
      RECORD_RESULT result = SecurityProfileManager.Edit(data);
      BaseResponse response = new(packet.PacketId, result, Packet.PACKET_TYPE.SecurityProfileEditResponse);
      client.Send(response.GetPacket());
    }

    private static void ProcessSecurityProfileDelete(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      SecurityProfileDelete data = new(packet);
      RECORD_RESULT result = SecurityProfileManager.Delete(data);
      BaseResponse response = new(packet.PacketId, result, Packet.PACKET_TYPE.SecurityProfileDeleteResponse);
      client.Send(response.GetPacket());
    }
    private static void ProcessFlowsGet(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      string path = Options.GetFullPath(Options.GetSettings.SettingGetAsString("FlowPath"));
      string relativePath = "";
      Xml xml = new();
      xml.WriteMemoryNew();
      xml.WriteTagStart("Directories");
      WriteSubDirectory(path, "*.flow", relativePath, xml, 0);
      xml.WriteTagEnd("Directories");
      string flowsXml = xml.ReadMemory();
      FlowsGetResponse response = new(packet.PacketId, flowsXml);
      client.Send(response.GetPacket());
    }

    private static void ProcessTemplatesGet(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      string path = Options.GetFullPath(Options.GetSettings.SettingGetAsString("TemplateBasePath"));
      string relativePath = "";
      Xml xml = new();
      xml.WriteMemoryNew();
      xml.WriteTagStart("Directories");
      WriteSubDirectory(path, "*.*", relativePath, xml, 0);
      xml.WriteTagEnd("Directories");
      string flowsXml = xml.ReadMemory();
      TemplatesGetResponse response = new(packet.PacketId, flowsXml);
      client.Send(response.GetPacket());
    }

    private static void WriteSubDirectory(string path, string fileType, string relativePath, Xml xml, int depth)
    {
      xml.WriteTagStart("Directory" + depth.ToString()); //Adding depth to the Directory tag to make my XML parsing easier, cheap hack! //TODO: Fix the cheesy/cheap XML hack with a real XML parser
      xml.WriteTagAndContents("Path", "/" + Path.GetFileNameWithoutExtension(relativePath)); //Not really getting the file name, but the last directory in the path, we don't want the full path
      
      string[] files = Directory.GetFiles(path, fileType);
      for (int y = 0; y < files.Length; y++)
      {
        Flow flow = new();
        flow.XmlReadFile(files[y], Flow.READ_TIL.TilSteps);
        xml.WriteTagStart("File");
        xml.WriteTagAndContents("FileName", Path.GetFileName(files[y]));
        xml.WriteTagAndContents("ModifiedDateTime", flow.ModifiedLastDateTime);
        if (flow.StartPlugin is not null)
          xml.WriteTagAndContents("PluginStarting", flow.StartPlugin.Name);
        xml.WriteTagAndContents("StartCommands", flow.FormatStartCommands());
        xml.WriteTagEnd("File");
      }

      if (Options.GetSettings.SettingGetAsBoolean("FlowPathAllowSubDirectories") == true)
      {
        string[] dirs = Directory.GetDirectories(path);
        for (int x = 0; x < dirs.Length; x++)
        {
          string strippedPath = Global.StripOff(path, dirs[x]);
          WriteSubDirectory(path + "/" + strippedPath, fileType, relativePath + "/" + strippedPath, xml, depth + 1);
        }
      }
      xml.WriteTagEnd("Directory" + depth.ToString());
    }

    private static void ProcessFlowSave(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      FlowSave data = new(packet);
      
      RESPONSE_CODE responseCode = RESPONSE_CODE.Success;

      for (int x = 0; x < FlowEngineCore.Global.IllegalFileNameCharacters.Length; x++)
      {
        if (data.FileName.Contains(FlowEngineCore.Global.IllegalFileNameCharacters[x]) == true)
        {
          responseCode = RESPONSE_CODE.BadRequest;
          break;
        }
      }
      for (int x = 0; x < FlowEngineCore.Global.IllegalFileNameStartCharacters.Length; x++)
      {
        if (data.FileName.StartsWith(FlowEngineCore.Global.IllegalFileNameStartCharacters[x]) == true)
        {
          responseCode = RESPONSE_CODE.BadRequest;
          break;
        }
      }
      if (data.FileName.Contains(".") == true && data.FileName.ToLower().Contains(".flow") == false)
        responseCode = RESPONSE_CODE.BadRequest;
      if (data.FileName.ToLower().EndsWith(".flow") == false)
        responseCode = RESPONSE_CODE.BadRequest;


      if (responseCode == RESPONSE_CODE.Success)
      {
        try
        {
          string path = Options.GetFullPath(Options.GetSettings.SettingGetAsString("FlowPath"), data.FileName);
          System.IO.File.WriteAllText(path, data.FlowXml);
          if (data.FlowGoLive == true)
          {
            FlowManager.LoadSingleFlow(path);
          }
        }
        catch
        {
          responseCode = RESPONSE_CODE.Error;
        }
      }

      BaseResponse response = new(packet.PacketId, responseCode, Packet.PACKET_TYPE.FlowSaveResponse);
      client.Send(response.GetPacket());
    }


    private static void ProcessFlowOpen(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      FlowOpen data = new(packet);

      RESPONSE_CODE responseCode = RESPONSE_CODE.Success;

      for (int x = 0; x < FlowEngineCore.Global.IllegalFileNameCharacters.Length; x++)
      {
        if (data.FileName.Contains(FlowEngineCore.Global.IllegalFileNameCharacters[x]) == true)
        {
          responseCode = RESPONSE_CODE.BadRequest;
          break;
        }
      }
      for (int x = 0; x < FlowEngineCore.Global.IllegalFileNameStartCharacters.Length; x++)
      {
        if (data.FileName.StartsWith(FlowEngineCore.Global.IllegalFileNameStartCharacters[x]) == true)
        {
          responseCode = RESPONSE_CODE.BadRequest;
          break;
        }
      }
      if (data.FileName.Contains(".") == true && data.FileName.ToLower().Contains(".flow") == false)
      {
        responseCode = RESPONSE_CODE.BadRequest;
      }
      if (data.FileName.ToLower().EndsWith(".flow") == false)
      {
        responseCode = RESPONSE_CODE.BadRequest;
      }

      string flowXml = "";
      if (responseCode == RESPONSE_CODE.Success)
      {
        try
        {
          string path = Options.GetFullPath(Options.GetSettings.SettingGetAsString("FlowPath"), data.FileName);
          flowXml = System.IO.File.ReadAllText(path);
        }
        catch
        {
          responseCode = RESPONSE_CODE.Error;
        }
      }

      FlowOpenResponse response = new(packet.PacketId, responseCode, data.FileName, flowXml);
      client.Send(response.GetPacket());
    }


    private static void ProcessFlowDebug(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      FlowDebug data = new(packet);
      string xmlData = data.FlowXml;

      if (Options.GetSettings.SettingGetAsString("ServerType") != Options.SERVER_TYPE.Development.ToString())
      {
        BaseResponse response = new(packet.PacketId, RESPONSE_CODE.DebugOnlyAllowedInDevelopmentServer, Packet.PACKET_TYPE.FlowDebugResponse);
        client.Send(response.GetPacket());
        return;
      }

      try
      {
        Flow flow = new();
        User? user = UserManager.FindByTcpConnection(client);
        if (user is null)
          return;
        flow.XmlRead(ref xmlData);
        flow.FileName = Guid.NewGuid().ToString(); //Assign a random guid as the name, we will use this to map this flow back to the debugger when the flow completes
        DebuggerManager.Add(user, packet.PacketId, flow.FileName);
        if (flow.StartPlugin is null && data.StartType == FlowRequest.START_TYPE.WaitForEvent)
        {
          BaseResponse response = new(packet.PacketId, RESPONSE_CODE.NoStartPluginDefined, Packet.PACKET_TYPE.FlowDebugResponse);
          client.Send(response.GetPacket());
          return;
        }

        if (data.StartType == FlowRequest.START_TYPE.Now)
        {
          FlowRequest fr = new(null, flow.StartPlugin, flow, FlowRequest.START_TYPE.Now, FlowRequest.CLONE_FLOW.DoNotCloneFlow);
          FlowEngine.StartFlow(fr);
        }
        else if (flow.StartPlugin is not null && data.StartType == FlowRequest.START_TYPE.WaitForEvent)
        {
          flow.StartPlugin.FlowAdd(flow);
        }
      }
      catch (Exception ex)
      {
        mLog?.Write("Flow Debug Execution error", ex, LOG_TYPE.ERR);
      }
    }


    private static void ProcessFlowDebugAlways(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      FlowDebugAlways data = new FlowDebugAlways(packet);

      //TODO: Change this, I hate it, comparing strings...
      if (Options.GetSettings.SettingGetAsString("ServerType") != Options.SERVER_TYPE.Development.ToString()) 
      {
        FlowDebugAlwaysResponse response1 = new(packet.PacketId, RESPONSE_CODE.DebugOnlyAllowedInDevelopmentServer, data.DebugAlways);
        client.Send(response1.GetPacket());
        return;
      }

      if (data.DebugAlways == FlowDebugAlways.DEBUG_ALWAYS.Yes)
      {
        User? user = UserManager.FindByTcpConnection(client);
        if (user is null)
          return;
        DebuggerManager.Add(user);
      }
      else
      {
        User? user = UserManager.FindByTcpConnection(client);
        if (user is null)
          return;
        DebuggerManager.RemoveByUser(user);
      }
      FlowDebugAlwaysResponse response2 = new(packet.PacketId, RESPONSE_CODE.Success, data.DebugAlways);
      client.Send(response2.GetPacket());

    }
    private static void ProcessCloseConnection(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
#if QUIT_CONNECTION_CLOSED
      FlowEngine.Instance.Stop(); //Stop the flow engine running after a disconnect for development purposes.
#endif
    }

    private static void ProcessServerSettingsGet(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      string path = Options.GetFullPath(Options.SettingsPath);
      string xml = System.IO.File.ReadAllText(path);
      ServerSettingsGetResponse response = new(packet.PacketId, RESPONSE_CODE.Success, xml);
      client.Send(response.GetPacket());
    }


    private static void ProcessServerSettingsEdit(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      ServerSettingsEdit data = new ServerSettingsEdit(packet);
      string path = Options.GetFullPath(Options.SettingsPath);
      System.IO.File.WriteAllText(path, data.Xml);
      Options.LoadSettingsFromXml(data.Xml);
      ServerSettingsEditResponse response = new ServerSettingsEditResponse(packet.PacketId, RESPONSE_CODE.Success);
      client.Send(response.GetPacket());
    }

    private static void ProcessStatisticsRegister(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      StatisticsRegister data = new StatisticsRegister(packet);
      StatisticsManager.UserRegister(client, data.Frequency);
      BaseResponse response = new(packet.PacketId, RESPONSE_CODE.Success, Packet.PACKET_TYPE.StatisticsRegisterResponse);
      client.Send(response.GetPacket());
    }
    private static void ProcessStatisticsDeregister(FlowEngineCore.Administration.Packet packet, FlowEngineCore.Administration.TcpClientBase client)
    {
      StatisticsManager.UserDeregister(client);
      BaseResponse response = new(packet.PacketId, RESPONSE_CODE.Success, Packet.PACKET_TYPE.StatisticsDeregisterResponse);
      client.Send(response.GetPacket());
    }
  }
}
