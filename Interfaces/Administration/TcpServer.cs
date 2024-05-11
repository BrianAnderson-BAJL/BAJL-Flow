using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FlowEngineCore;


namespace FlowEngineCore.Administration
{
  public class TcpServer
  {
    /// <summary>
    /// The NewConnection event will be triggered every time a new connection request is received from a client
    /// </summary>
    public event EventHandler<EventArgsTcpClient>? NewConnection;

    public event EventHandler<EventArgsTcpClient>? ConnectionClosed;
    /// <summary>
    /// The NewPacket event will be triggered every time a packet of data is received from the client
    /// This event will be triggered from a seperate thread, so be sure to protect it using lock.
    /// </summary>
    public event EventHandler<EventArgsPacket>? NewPacket;
    private List<TcpClient> mClients = new List<TcpClient>(32);
    private int mPort;
    private object mCriticalSection = new object();
    private bool mContinue = true;
    private int mReadPacketTimeout = 5000;
    private System.Net.Sockets.TcpListener? Listener;

    protected void OnConnectionClosed(TcpClientBase Client)
    {
      if (ConnectionClosed is not null)
      {
        EventArgsTcpClient EA = new EventArgsTcpClient(Client);
        ConnectionClosed(this, EA);
      }
    }

    protected void OnNewPacket(Packet Packet, TcpClientBase Client)
    {
      if (NewPacket is not null)
      {
        EventArgsPacket EA = new EventArgsPacket(Packet, Client);
        NewPacket(this, EA);
      }
      Client.OnNewPacket(Packet, Client);
    }

    protected void OnNewConnection(TcpClientBase Client)
    {
      if (NewConnection is not null)
      {
        EventArgsTcpClient EA = new EventArgsTcpClient(Client);
        NewConnection(this, EA);
      }
    }

    public TcpServer(int ReadPacketTimeout)
    {
      mReadPacketTimeout = ReadPacketTimeout;
    }

    public void Start(int PortNumber)
    {
      mPort = PortNumber;
      Thread T = new Thread(ListenThread);
      T.Start();
    }

    public void Stop()
    {
      mContinue = false;

      if (Listener is not null)
        Listener.Stop();

      lock (mCriticalSection)
      {
        for (int x = 0; x < mClients.Count; x++)
        {
          mClients[x].Close();
        }
      }

    }

    private void ListenThread()
    {
      try
      {
        Listener = new System.Net.Sockets.TcpListener(IPAddress.Any, mPort);
        Listener.Start();
        while (mContinue == true)
        {
          try
          {
            if (Listener.Pending() == true)
            {
              System.Net.Sockets.TcpClient Client = Listener.AcceptTcpClient(); //This is getting the low level TcpClient
              TcpClient C = ClientAdd(Client);
              C.NewPacket += this.NewPacket;
              Thread T = new Thread(new ParameterizedThreadStart(C.ReadPacketsThread!));
              T.Start(C);
            }
          }
          catch
          {
          }
          Thread.Sleep(1);
        }
        Listener.Stop();
      }
      catch (Exception ex)
      {
        Global.WriteToConsoleDebug(ex.Message);

      }
    }

    private TcpClient ClientAdd(System.Net.Sockets.TcpClient Client)
    {
      TcpClient C = new TcpClient(Client);

      lock (mCriticalSection)
      {
        mClients.Add(C);
      }
      OnNewConnection(C);
      return C;
    }

  }
}