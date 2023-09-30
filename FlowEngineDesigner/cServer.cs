using Core.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class cServer
  {
    struct CALLBACK_INFO
    {
      public ResponseDelegate Callback;

      public CALLBACK_INFO(ResponseDelegate callback)
      {
        Callback = callback;
      }
    }
    public delegate void ResponseDelegate(Core.Administration.EventArgsPacket e);
    private static Core.Administration.TcpClient? Client;
    private static Dictionary<Packet.PACKET_TYPE, CALLBACK_INFO> responseHandlers = new Dictionary<Packet.PACKET_TYPE, CALLBACK_INFO>(8);
    private static object CriticalSection = new object();
    public static Core.User? UserLoggedIn = null;

    public static bool Connect(string domainName, int port)
    {
      lock (CriticalSection)
      {
        if (Client is not null && Client.Connected == true) //Already connected, just return true, they need to call Disconnect to create a new connection first.
          return true;

        Client = Core.Administration.TcpClient.Connect(domainName, port);
        if (Client is null)
          return false;
        else
        {
          Client.NewPacket += Client_NewPacket;
          Client.ConnectionClosed += Client_ConnectionClosed;
          return true;
        }
      }
    }

    private static void Client_ConnectionClosed(object? sender, Core.Administration.EventArgsTcpClient e)
    {
      throw new NotImplementedException();
    }

    private static void Client_NewPacket(object? sender, Core.Administration.EventArgsPacket e)
    {
      CALLBACK_INFO? callbackInfo = null;
      lock (CriticalSection)
      {
        if (responseHandlers.ContainsKey(e.Packet.PacketType) == true)
        {
          callbackInfo = responseHandlers[e.Packet.PacketType];
          responseHandlers.Remove(e.Packet.PacketType);
        }
      }
      if (callbackInfo.HasValue)
      {
        try
        {
          Global.FormMain!.Invoke((MethodInvoker)delegate
          {
            callbackInfo.Value.Callback(e);
          });
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }
    }

    public static void RequestSingleResponse(Core.Administration.Packet.PACKET_TYPE packetType)
    {
    }

    public static void Disconnect()
    {
      lock (CriticalSection)
      {
        if (Client is not null)
          Client.Close();
      }
    }

    //public static bool Send(Core.Administration.Packet packet)
    //{
    //  lock (CriticalSection)
    //  {
    //    if (Client is null)
    //      return false;

    //    return Client.Send(packet);
    //  }
    //}

    public static bool SendAndResponse(Core.Administration.Packet packet, ResponseDelegate callback, Packet.PACKET_TYPE responsePacketType)
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
        if (responseHandlers.ContainsKey(responsePacketType) == true)
          return false;

        responseHandlers.Add(responsePacketType, new CALLBACK_INFO(callback)); //Add one to the packet type for the Response packet type
        return Client.Send(packet);
      }
    }
  }
}
