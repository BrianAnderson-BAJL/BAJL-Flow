using FlowEngineCore;
using FlowEngineCore.Administration;
using FlowEngineCore.Administration.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlowEngineDesigner.cEventManager;

namespace FlowEngineDesigner
{
  internal class cServer
  {

    /// <summary>
    /// At some point I will need to create a thread to check for a response timeout in the responseHandlers dictionary
    /// </summary>
    struct CALLBACK_INFO
    {
      public ResponseDelegate Callback;
      public ResponseDelegate? Callback_AlsoNotify;
      public TimeElapsed ElapsedTime;

      public CALLBACK_INFO(ResponseDelegate callback, ResponseDelegate? callback_also = null)
      {
        Callback = callback;
        Callback_AlsoNotify = callback_also;
        ElapsedTime = new TimeElapsed();
      }
    }
    public delegate void ResponseDelegate(FlowEngineCore.Administration.EventArgsPacket e);
    private static FlowEngineCore.Administration.TcpTlsClient? Client;
    private static Dictionary<int, CALLBACK_INFO> responseHandlers = new Dictionary<int, CALLBACK_INFO>(8);
    private static object CriticalSection = new object();
    public static FlowEngineCore.User UserLoggedIn = new User();
    public static List<FlowEngineCore.SecurityProfile>? SecurityProfiles;

    public static List<cServerInformation> Servers = new List<cServerInformation>();

    public static cServerInformation Info = new cServerInformation();

    public static bool Connect(cServerInformation server)
    {
      bool wasLastConnected = server.WasLastConnected;
      ClearWasLastConnected();
      server.WasLastConnected = true;
      if (wasLastConnected == false)
        SaveProfiles(); //Wasn't connected to this server before, lets resave the profiles.

      TimeElapsed te = new TimeElapsed(); //Starts a timer
      Info = server;
      lock (CriticalSection)
      {
        if (Client is not null && Client.Connected == true) //Already connected, just return true, they need to call Disconnect to create a new connection first.
          return true;

        Client = FlowEngineCore.Administration.TcpTlsClient.Connect(server.Url, server.Port);
        if (Client is null)
        {
          cEventManager.RaiseEventTracer(SENDER.FlowEngineServer, $"FAILED to Connect to [{server.Url}], [{server.Port}]", cEventManager.TRACER_TYPE.Error, te.HowLong().Ticks);
          Global.FormMain!.tsServer.Text = "Disconected";
          Global.FormMain!.tsServer.ForeColor = Color.Red;
          Global.FormMain!.tsLoggedInAs.Text = "[NOT LOGGED IN]";
          return false;
        }
        else
        {
          Client.NewPacket += Client_NewPacket;
          Client.ConnectionClosed += Client_ConnectionClosed;
          cEventManager.RaiseEventTracer(SENDER.FlowEngineServer, $"Connected to [{server.Url}], [{server.Port}]", cEventManager.TRACER_TYPE.Information, te.HowLong().Ticks);
          Global.FormMain!.tsServer.Text = $"Connected to [{server.Url}]";
          Global.FormMain!.tsServer.ForeColor = Color.Green;
          return true;
        }

      }
    }

    

    private static void ClearWasLastConnected()
    {
      for (int x = 0; x < Servers.Count; x++)
      {
        Servers[x].WasLastConnected = false;
      }
    }

    public static void SaveProfiles()
    {
      Xml xml = new Xml();
      string path = cOptions.GetFullPath(".");
      xml.WriteFileNew(path + "/server_profiles.xml");
      xml.WriteTagStart("Servers");
      for (int x = 0; x < Servers.Count; x++)
      {
        Servers[x].WriteXml(xml);
      }
      xml.WriteTagEnd("Servers");
      xml.WriteFileClose();
    }

    public static void LoadProfiles()
    {
      Xml xml = new Xml();
      string path = cOptions.GetFullPath(".");
      string xmlStr = xml.FileRead(path + "/server_profiles.xml");

      xmlStr = Xml.GetXMLChunk(ref xmlStr, "Servers");
      string tempXml = Xml.GetXMLChunk(ref xmlStr, "Server");
      while (tempXml != "")
      {
        cServerInformation  server = new cServerInformation();
        server.ReadXml(tempXml);
        Servers.Add(server);
        if (server.WasLastConnected == true)
          Info = server;
        tempXml = Xml.GetXMLChunk(ref xmlStr, "Server");
      }
    }

    public static bool IsConnectedToServer
    {
      get
      {
        if (Client is null)
          return false;
        return Client.Connected;
      }
    }

    public static void GetServerSettings(ResponseDelegate? callback = null)
    {
      if (UserLoggedIn is null)
        return;

      ServerSettingsGet ssg = new ServerSettingsGet(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey);
      cServer.SendAndResponse(ssg.GetPacket(), Callback_GetServerSettings, callback);
    }

    private static void Callback_GetServerSettings(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() != BaseResponse.RESPONSE_CODE.Success)
        return;

      ServerSettingsGetResponse ssgr = new ServerSettingsGetResponse(e.Packet);
      string xmlData = ssgr.Xml;
      Options.LoadSettingsFromXml(xmlData);
    }

    public static void RefreshSecurityProfiles(ResponseDelegate? callback = null)
    {
      if (UserLoggedIn is null)
        return;

      SecurityProfilesGet spg = new SecurityProfilesGet(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey);
      cServer.SendAndResponse(spg.GetPacket(), Callback_SecurityProfile, callback);
    }

    private static void Callback_SecurityProfile(FlowEngineCore.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() == BaseResponse.RESPONSE_CODE.Success)
      {
        SecurityProfilesGetResponse response = new SecurityProfilesGetResponse(e.Packet);
        cServer.SecurityProfiles = response.Profiles;
        if (UserLoggedIn is null)
          return;
        UserLoggedIn.SecurityProfile = FindSecurityProfileByName(UserLoggedIn.SecurityProfileNameTemp);
      }
    }

    public static SecurityProfile.SECURITY_ACCESS_LEVEL AccessLevelForUserLoggedIn(SecurityProfile.SECURITY_AREA secArea)
    {
      if (UserLoggedIn is null)
        return SecurityProfile.SECURITY_ACCESS_LEVEL.None;
      if (UserLoggedIn.SecurityProfile is null)
        return SecurityProfile.SECURITY_ACCESS_LEVEL.None;

      return UserLoggedIn.SecurityProfile.AccessLevelClient(secArea);
    }

    public static SecurityProfile FindSecurityProfileByName(string name)
    {
      if (SecurityProfiles is null)
        return SecurityProfile.NoAccess;

      for (int x = 0; x < SecurityProfiles.Count; x++)
      {
        if (name == SecurityProfiles[x].Name)
        {
          return SecurityProfiles[x];
        }
      }

      return SecurityProfile.NoAccess;
    }

    private static void CheckSecurityAccess(FlowEngineCore.SecurityProfile sp)
    {
      if (sp.AdministrationUsers >= FlowEngineCore.SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
      {
        //Global.FormMain.
      }
    }

    private static void Client_ConnectionClosed(object? sender, FlowEngineCore.Administration.EventArgsTcpClient e)
    {
      try
      {
        Global.FormMain!.Invoke((MethodInvoker)delegate
        {
          Global.FormMain!.tsServer.Text = "Disconnected";
          Global.FormMain!.tsServer.ForeColor = Color.Red;
          Global.FormMain!.tsLoggedInAs.Text = "";
        });
      }
      catch { }
    }

    private static void Client_NewPacket(object? sender, FlowEngineCore.Administration.EventArgsPacket e)
    {
      
      CALLBACK_INFO? callbackInfo = null;
      lock (CriticalSection)
      {
        if (responseHandlers.ContainsKey(e.Packet.PacketId) == true)
        {
          callbackInfo = responseHandlers[e.Packet.PacketId];
          responseHandlers.Remove(e.Packet.PacketId);
        }
      }
      if (callbackInfo.HasValue)
      {
        try
        {
          if (e.Packet.PeekResponseCode() == BaseResponse.RESPONSE_CODE.UserSessionExpired || e.Packet.PeekResponseCode() == BaseResponse.RESPONSE_CODE.UserSessionInvalid)
          {
            Global.FormMain!.Invoke((MethodInvoker)delegate
            {
              Disconnect(false);
              frmServerConnect f = new frmServerConnect();
              f.Show();
            });
            return;
          }

          Global.FormMain!.Invoke((MethodInvoker)delegate
          {
            cEventManager.RaiseEventTracer(SENDER.FlowEngineServer, $"Received [{e.Packet.PacketType}], Response code [{e.Packet.PeekResponseCode()}]", e.Packet.PeekResponseCode(), callbackInfo.Value.ElapsedTime.HowLong().Ticks);
            callbackInfo.Value.Callback(e);
            if (callbackInfo.Value.Callback_AlsoNotify is not null)
            {
              e.Packet.ResetReadPosition();
              callbackInfo.Value.Callback_AlsoNotify(e);
            }
          });
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }
      else //Not a response to an outstanding request, could be a Trace message or something else unsolicited.
      {
        Global.FormMain!.Invoke((MethodInvoker)delegate
        {
          ProcessMessage(e.Client, e.Packet);
        });
      }
    }

    private static void ProcessMessage(TcpClientBase client, Packet packet)
    {
      if (packet.PacketType == Packet.PACKET_TYPE.Trace)
      {
        //Trace trace = new Trace(packet);
        cEventManager.RaiseEventTracer(packet);
      }
      else if (packet.PacketType == Packet.PACKET_TYPE.StatisticsData)
      {
        cEventManager.RaiseEventStatistics(packet);
      }

    }

    public static void RequestSingleResponse(FlowEngineCore.Administration.Packet.PACKET_TYPE packetType)
    {
    }

    public static void Disconnect(bool SendCloseConnectionToServer = true)
    {
      lock (CriticalSection)
      {
        if (cServer.UserLoggedIn is not null && SendCloseConnectionToServer == true)
        {
          Send(new BaseMessage(cServer.Info.PrivateKey, cServer.UserLoggedIn.SessionKey, Packet.PACKET_TYPE.CloseConnection).GetPacket());
        }
        if (Client is not null)
          Client.Close();
        cServer.UserLoggedIn = new User();
        cEventManager.RaiseEventTracer(SENDER.FlowEngineServer, "Disconnected");
      }
    }


    private static bool Send(FlowEngineCore.Administration.Packet packet)
    {
      lock (CriticalSection)
      {
        if (Client is null)
          return false;

        return Client.Send(packet);
      }
    }

    public static bool SendAndResponse(FlowEngineCore.Administration.Packet packet, ResponseDelegate callback, ResponseDelegate? callback_also = null)
    {
      if (UserLoggedIn is null) //If the user isn't logged in and this isn't a login request, don't send anything
      {
        if (packet.PacketType != Packet.PACKET_TYPE.UserLogin && packet.PacketType != Packet.PACKET_TYPE.UserAdd)
          return false;
      }
      lock (CriticalSection)
      {
        if (Client is null)
          return false;
        if (responseHandlers.ContainsKey(packet.PacketId) == true)
          return false;

        responseHandlers.Add(packet.PacketId, new CALLBACK_INFO(callback, callback_also)); //Add one to the packet type for the Response packet type
        return Client.Send(packet);
      }
    }
  }
}
