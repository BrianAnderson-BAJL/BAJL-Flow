﻿using Core;
using Core.Administration;
using Core.Administration.Messages;
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
    public delegate void ResponseDelegate(Core.Administration.EventArgsPacket e);
    private static Core.Administration.TcpTlsClient? Client;
    private static Dictionary<int, CALLBACK_INFO> responseHandlers = new Dictionary<int, CALLBACK_INFO>(8);
    private static object CriticalSection = new object();
    public static Core.User UserLoggedIn = new User();
    public static List<Core.SecurityProfile>? SecurityProfiles;

    public static bool Connect(string domainName, int port)
    {
      TimeElapsed te = new TimeElapsed(); //Starts a timer
      lock (CriticalSection)
      {
        if (Client is not null && Client.Connected == true) //Already connected, just return true, they need to call Disconnect to create a new connection first.
          return true;

        Client = Core.Administration.TcpTlsClient.Connect(domainName, port);
        if (Client is null)
          return false;
        else
        {
          Client.NewPacket += Client_NewPacket;
          Client.ConnectionClosed += Client_ConnectionClosed;
          cEventManager.RaiseEventTracer(SENDER.FlowEngineServer, $"Connected to [{domainName}], [{port}]", cEventManager.TRACER_TYPE.Information, te.End().Ticks);
          Global.FormMain!.tsServer.Text = "Connected";
          Global.FormMain!.tsServer.ForeColor = Color.Green;
          return true;
        }

      }
    }

    public static bool IsConnectedToServer()
    {
      if (Client is null)
        return false;
      return Client.Connected;
    }

    public static void RefreshSecurityProfiles(ResponseDelegate? callback = null)
    {
      if (UserLoggedIn is null)
        return;

      SecurityProfilesGet spg = new SecurityProfilesGet(cOptions.AdministrationPrivateKey, cServer.UserLoggedIn.SessionKey);
      cServer.SendAndResponse(spg.GetPacket(), Callback_SecurityProfile, callback);
    }

    private static void Callback_SecurityProfile(Core.Administration.EventArgsPacket e)
    {
      if (e.Packet.PeekResponseCode() == BaseResponse.RESPONSE_CODE.Success)
      {
        SecurityProfilesGetResponse response = new SecurityProfilesGetResponse(e.Packet);
        cServer.SecurityProfiles = response.Profiles;
        if (UserLoggedIn is null)
          return;
        
      }
    }

    public static SecurityProfile.SECURITY_ACCESS_LEVEL AccessLevelForUserLoggedIn(SecurityProfile.SECURITY_AREA secArea)
    {
      if (UserLoggedIn is null)
        return SecurityProfile.SECURITY_ACCESS_LEVEL.None;
      SecurityProfile? sp = FindSecurityProfileByName(UserLoggedIn.SecurityProfile);
      if (sp is null)
        return SecurityProfile.SECURITY_ACCESS_LEVEL.None;

      return sp.AccessLevelClient(secArea);
    }

    public static SecurityProfile? FindSecurityProfileByName(string name)
    {
      if (SecurityProfiles is null)
        return null;

      for (int x = 0; x < SecurityProfiles.Count; x++)
      {
        if (name == SecurityProfiles[x].Name)
        {
          return SecurityProfiles[x];
        }
      }
      return null;
    }

    private static void CheckSecurityAccess(Core.SecurityProfile sp)
    {
      if (sp.AdministrationUsers >= Core.SecurityProfile.SECURITY_ACCESS_LEVEL.Readonly)
      {
        //Global.FormMain.
      }
    }

    private static void Client_ConnectionClosed(object? sender, Core.Administration.EventArgsTcpClient e)
    {
      Global.FormMain!.Invoke((MethodInvoker)delegate
      {
        Global.FormMain!.tsServer.Text = "Disconnected";
        Global.FormMain!.tsServer.ForeColor = Color.Red;
        Global.FormMain!.tsLoggedInAs.Text = "";
      });
    }

    private static void Client_NewPacket(object? sender, Core.Administration.EventArgsPacket e)
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

          Global.FormMain!.Invoke((MethodInvoker)delegate
          {
            cEventManager.RaiseEventTracer(SENDER.FlowEngineServer, $"Received [{e.Packet.PacketType}], Response code [{e.Packet.PeekResponseCode()}]", e.Packet.PeekResponseCode(), callbackInfo.Value.ElapsedTime.End().Ticks);
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

    }

    public static void RequestSingleResponse(Core.Administration.Packet.PACKET_TYPE packetType)
    {
    }

    public static void Disconnect()
    {
      lock (CriticalSection)
      {
        if (cServer.UserLoggedIn is not null)
        {
          Send(new BaseMessage(Packet.PACKET_TYPE.CloseConnection).GetPacket());
        }
        if (Client is not null)
          Client.Close();
        cServer.UserLoggedIn = new User();
        cEventManager.RaiseEventTracer(SENDER.FlowEngineServer, "Disconnected");
      }
    }


    private static bool Send(Core.Administration.Packet packet)
    {
      lock (CriticalSection)
      {
        if (Client is null)
          return false;

        return Client.Send(packet);
      }
    }

    public static bool SendAndResponse(Core.Administration.Packet packet, ResponseDelegate callback, ResponseDelegate? callback_also = null)
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
