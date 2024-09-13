using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using FlowEngineCore.Administration;

namespace FlowEngineCore.Administration
{
  public class TcpTlsClient : TcpClientBase
  {
    public SslStream mStream;
    //Reading about the SslStream threading problem, it seems like it could be just a problem of writing to the same stream from multiple threads or reading from the same stream from multiple threads
    //So I've changed the critical section to be wrapped around the stream write only for now, will need to perform a load test again.
    private object CriticalSectionWrite = new object();
    //private object CriticalSectionRead = new object(); 
    //private X509Certificate Certificate;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. The base class TcpClientBase has a Stream member variable, we aren't using it in this class
    public TcpTlsClient(System.Net.Sockets.TcpClient client, System.Net.Security.SslStream stream) : base(client)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. 
    {

      try
      {
        this.Client = client;
        this.mStream = stream;

      }
      catch //(AuthenticationException ex)
      {
      }
      //catch (Exception ex)
      //{
      //}
    }

    ~TcpTlsClient()
    {
      mContinue = false;
    }

    public new virtual SslStream Stream
    {
      get
      {

        return mStream;
      }
    }

    public static FlowEngineCore.Administration.TcpTlsClient? Connect(string hostUrl, int port, int connectTimeout = 5000, int ReadPacketTimeout = 5000)
    {
      FlowEngineCore.Administration.TcpTlsClient? returnClient = null;
      try
      {
        System.Net.Sockets.TcpClient client = new();
        if (client.ConnectAsync(hostUrl, port).Wait(connectTimeout) == false)
        {
          Global.WriteToConsoleDebug($"Could not connect to url [{hostUrl}], port [{port}]");
          return null; //Couldn't connect within the connectTimeout period, return null (failure)
        }
        SslStream stream = new(client.GetStream(), false, ValidateServerCertificate!);
        stream.ReadTimeout = connectTimeout;
        stream.AuthenticateAsClient(hostUrl);
        returnClient = new(client, stream);
        Thread T = new(new ParameterizedThreadStart(returnClient.ReadTlsPacketsThread!));
        T.Start(returnClient);
      }
      catch (Exception ex)
      {
        Global.WriteToConsoleDebug(Global.FullExceptionMessage(ex));
      }
      return returnClient;
    }


    private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      if (sslPolicyErrors == SslPolicyErrors.None)
      {
        //Console.WriteLine("Validated Server Certificate, connected to Core");
        //return true;
      }


      // Do not allow this client to communicate with unauthenticated servers. 
      return true;
    }

    public void ReadTlsPacketsThread(object oClient)
    {
      try
      {
        TcpTlsClient? Client = oClient as TcpTlsClient;
        if (Client is null)
          return;

        int sizeOfInt = sizeof(int);
        byte[] fourByteHeader = new byte[sizeOfInt];


        while (mContinue == true)
        {
          try
          {
            if (Client.mStream.CanRead)
            {
              Packet? packet = null;

              Client.mStream.ReadTimeout = Timeout.Infinite;
              int dataRead = Client.mStream.Read(fourByteHeader, 0, sizeOfInt);
              if (dataRead == 4)
              {
                packet = new Packet();
                packet.ReadAllTlsData(Client.mStream, fourByteHeader);
              }
              if (packet is not null)
              {
                if ((int)packet.PacketType <= (int)Packet.PACKET_TYPE._Unknown || (int)packet.PacketType >= (int)Packet.PACKET_TYPE.zMaxValue)
                  throw new Exception("UNKNOWN PacketType TCP message is corrupted");

                if (packet.PacketType != Packet.PACKET_TYPE._Unknown)
                  OnNewPacket(packet, Client);
              }
            }
            Thread.Sleep(10);
          }
          catch (SocketException ex)
          {
            Global.WriteToConsoleDebug(Global.FullExceptionMessage(ex));
            break;
          }
          catch (EndOfStreamException ex)
          {
            //Connection Closed
            Global.WriteToConsoleDebug(Global.FullExceptionMessage(ex));
            break;
          }
          catch (IOException ex)
          {
            //Read timeout
            Thread.Sleep(100);
          }
          catch (Exception ex)
          {
            //Unknown Error
            Global.WriteToConsoleDebug(Global.FullExceptionMessage(ex));
            break; 
          }
        }
        OnConnectionClosed(Client);
        Client.Close();
      }
      catch (Exception ex)
      {
        Global.WriteToConsoleDebug(Global.FullExceptionMessage(ex));
      }
    }


    public override bool Send(Packet Packet)
    {
      bool Rc = false;
      try
      {

        Packet.FinalizePacketBeforeSending();
        lock (CriticalSectionWrite)
        {
          mStream.Write(Packet.DataToSend, 0, Packet.SendLength); //Need to add the 4 byte header to the length being sent
          mStream.Flush();
        }
        Rc = true;
      }
      catch (Exception ex)
      {
        Global.WriteToConsoleDebug(Global.FullExceptionMessage(ex));
      }
      return Rc;

    }
  }
}
