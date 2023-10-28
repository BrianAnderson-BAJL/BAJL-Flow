using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Core.Administration;

namespace Core.Administration
{
  public class TcpTlsClient : TcpClientBase
  {
    public SslStream mStream;
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

    public static Core.Administration.TcpTlsClient? Connect(string hostUrl, int port, int connectTimeout = 5000, int ReadPacketTimeout = 5000)
    {
      Core.Administration.TcpTlsClient? returnClient = null;
      try
      {
        System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
        if (client.ConnectAsync(hostUrl, port).Wait(connectTimeout) == false)
        {
          Global.Write("Could not connect to url [{0}], port [{1}]", hostUrl, port.ToString());
          return null; //Couldn't connect within the connectTimeout period, return null (failure)
        }
        SslStream stream = new SslStream(client.GetStream(), false, ValidateServerCertificate!);
        stream.AuthenticateAsClient(hostUrl);
        returnClient = new TcpTlsClient(client, stream);
        Thread T = new Thread(new ParameterizedThreadStart(returnClient.ReadTlsPacketsThread!));
        T.Start(returnClient);
      }
      catch (Exception ex)
      {
        Global.Write(ex.Message);
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

        //Client.mStream.ReadTimeout = ReadPacketTimeout;
        //SslStream NS = Client.mStream;

        while (mContinue == true)
        {
          try
          {
            if (Client.mStream.CanRead)
            {
              //if (Br is null)
                //Br = new BinaryReader(NS);
              
              Packet Packet = new Packet();
              //Packet.ReadAllData(Br);
              Packet.ReadAllTlsData(Client.mStream);
              if (Packet.PacketType == Packet.PACKET_TYPE.CloseConnection)
                break;
              if (Packet.PacketType != Packet.PACKET_TYPE._Unknown)
                OnNewPacket(Packet, Client);
            }
            Thread.Sleep(1);
          }
          catch (SocketException ex)
          {
            Global.Write(ex.Message);
            break;
          }
          catch (EndOfStreamException ex)
          {
            //Connection Closed
            Global.Write(ex.Message);
            break;
          }
          catch (IOException ex)
          {
            //No Data
            Global.Write(ex.Message);
            break;
          }
          catch (Exception ex)
          {
            //Unknown Error
            Global.Write(ex.Message);

            break; //Exit from Loop
          }
        }
        OnConnectionClosed(Client);
        //Client.Stream.Close();
        Client.Close();
      }
      catch (Exception e)
      {
        Global.Write(e.Message);
      }
    }
  

  public override bool Send(Packet Packet)
    {
      bool Rc = false;
      try
      {
        
        Packet.FinalizePacketBeforeSending();
        mStream.Write(Packet.DataToSend, 0, Packet.SendLength); //Need to add the 4 byte header to the length being sent
        mStream.Flush();
        Rc = true;
      }
      catch (Exception ex)
      {
        Global.Write(ex.Message);
      }
      return Rc;

    }
  }
}
