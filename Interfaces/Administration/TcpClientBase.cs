using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core.Administration
{
  public class EventArgsTcpClient : EventArgs
  {
    public TcpClientBase Client;

    public EventArgsTcpClient(TcpClientBase client)
    {
      Client = client;
    }
  }
  public class EventArgsPacket : EventArgs
  {
    public Packet Packet;
    public TcpClientBase Client;

    public EventArgsPacket(Packet packet, TcpClientBase client)
    {
      Packet = packet;
      Client = client;
    }
  }
  public class TcpClientBase
  {
    protected System.Net.Sockets.TcpClient? Client;
    protected string mHostUrl;
    protected int mPort;
    protected bool mContinue = true;
    protected int ReadPacketTimeout { get; set; } = 5000;
    protected BinaryReader? Br;


    public TcpClientBase(System.Net.Sockets.TcpClient client)
    {
      Client = client;
      mHostUrl = "";
    }

    ~TcpClientBase()
    {
      mContinue = false;
      
    }

    public bool Connected
    {
      get 
      {
        if (Client is null)
          return false;
        return Client.Connected; 
      }
    }

    public NetworkStream Stream
    {
      get 
      {
        if (Client is null)
          throw new InvalidOperationException("Client object is null, this client socket isn't connected to anything!");
        return Client.GetStream(); 
      }
    }

    
    /// <summary>
    /// The NewPacket event will be triggered every time a packet of data is received from the client
    /// This event will be triggered from a seperate thread, so be sure to protect it using lock.
    /// </summary>
    public event EventHandler<EventArgsPacket>? NewPacket;

    public event EventHandler<EventArgsTcpClient>? ConnectionClosed;


    internal void OnNewPacket(Packet Packet, TcpClientBase Client)
    {
      if (NewPacket is not null)
      {
        EventArgsPacket EA = new EventArgsPacket(Packet, Client);
        EA.Packet = Packet;
        NewPacket(this, EA);
      }
    }

    protected void OnConnectionClosed(TcpClientBase Client)
    {
      if (ConnectionClosed is not null)
      {
        EventArgsTcpClient EA = new EventArgsTcpClient(Client);
        EA.Client = Client;
        ConnectionClosed(this, EA);
      }
    }


    public virtual void Close()
    {
      mContinue = false;
      if (Client is not null) 
        Client.Close();
    }

    public virtual bool Send(Packet Packet)
    {
      bool Rc = false;
      try
      {
        Packet.FinalizePacketBeforeSending();
        Stream.Write(Packet.Data, 0, Packet.Length); //Need to add the 4 byte header to the length being sent
        Rc = true;
      }
      catch (Exception ex)
      {
        Global.Write(ex.Message);
      }
      return Rc;

    }



    public void ReadPacketsThread(object oClient)
    {
      try
      {
        TcpClientBase? Client = oClient as TcpClientBase;
        if (Client is null)
          return;

        Client.Stream.ReadTimeout = ReadPacketTimeout;
        NetworkStream NS = Client.Stream;

        while (mContinue == true)
        {
          try
          {
            if (NS.DataAvailable == true)
            {
              if (Br is null)
                Br = new BinaryReader(NS);

              Packet Packet = new Packet();
              Packet.ReadAllData(Br);
              OnNewPacket(Packet, Client);
            }
            Thread.Sleep(1);
          }
          catch (SocketException ex)
          {
            Global.Write(ex.Message);
            break;
          }
          catch (EndOfStreamException)
          {
            //Connection Closed
            break;
          }
          catch (IOException)
          {
            //No Data
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
      catch
      {

      }
    }
  }
}
