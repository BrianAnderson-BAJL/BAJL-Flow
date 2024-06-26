using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Authentication;

namespace FlowEngineCore.Administration
{
  public class TcpTlsServer
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
    private X509Certificate mCertificate;
    private List<TcpTlsClient> mClients = new(32);
    private int mPort;
    private object mCriticalSection = new();
    private bool mContinue = true;
    private int mReadPacketTimeout = 5000;

    protected void OnConnectionClosed(TcpTlsClient client)
    {
      if (ConnectionClosed is not null)
      {
        EventArgsTcpClient EA = new(client);
        ConnectionClosed(this, EA);
      }
      client.OnConnectionClosed(client);
    }

    protected void OnNewPacket(Packet packet, TcpTlsClient client)
    {
      if (NewPacket is not null)
      {
        EventArgsPacket EA = new(packet, client);
        NewPacket(this, EA);
      }
      client.OnNewPacket(packet, client);
    }

    protected void OnNewConnection(TcpTlsClient client)
    {
      if (NewConnection is not null)
      {
        EventArgsTcpClient EA = new(client);
        NewConnection(this, EA);
      }
    }

    public TcpTlsServer(int ReadPacketTimeout, string pathToCert, string certPassword) //string CertificateSerialNumber, 
    {

      mReadPacketTimeout = ReadPacketTimeout;
      try
      {
        mCertificate = new X509Certificate(pathToCert, certPassword);
      }
      catch (Exception ex)
      {
        FlowEngine.Log?.Write($"FAILED to load [{pathToCert}] with password [{new string('*', certPassword.Length)}] Remote Administration access is disabled", ex, LOG_TYPE.ERR);
      }

    }

    public void Start(int PortNumber)
    {
      mPort = PortNumber;
      Thread T = new(ListenThread);
      T.Start();
    }

    public void Stop()
    {
      lock (mCriticalSection)
      {
        for (int x = 0; x < mClients.Count; x++)
        {
          mClients[x].Close();
        }
      }
      mContinue = false;
    }

    private void ListenThread()
    {
      FlowEngine.Log?.Write("Initializing...Starting Administration TCP server");

      try
      {
        TcpListener Listener = new(IPAddress.Any, mPort);
        Listener.Start();
        FlowEngine.Log?.Write($"TcpTlsServer Listening on - [IpAddress.Any], [{mPort}]");

        while (mContinue == true)
        {
          try
          {
            if (Listener.Pending() == true)
            {
              FlowEngine.Log?.Write($"TcpTlsServer Waiting to accept connections");
              System.Net.Sockets.TcpClient Client = Listener.AcceptTcpClient();
              FlowEngine.Log?.Write($"TcpTlsServer accepted connection");
              Client.NoDelay = true;
              Client.LingerState = new LingerOption(false, 0);
              TcpTlsClient C = ClientAdd(Client);
              C.NewPacket += this.NewPacket;
              C.ConnectionClosed += this.ConnectionClosed;
              Thread T = new(new ParameterizedThreadStart(C.ReadTlsPacketsThread!));
              T.Start(C);
            }
          }
          catch //(Exception ex)
          {

          }
          Thread.Sleep(1);
        }
        Listener.Stop();
      }
      catch (Exception ex)
      {
        FlowEngine.Log?.Write($"TcpTlsServer ERROR - [{ex.Message}]", LOG_TYPE.ERR);
      }
    }

    private void C_ConnectionClosed(object? sender, EventArgsTcpClient e)
    {
      throw new NotImplementedException();
    }

    private TcpTlsClient ClientAdd(System.Net.Sockets.TcpClient Client)
    {
      TcpTlsClient? tlsClient = null;
      try
      {
        SslStream stream = new(Client.GetStream(), false);
        stream.AuthenticateAsServer(mCertificate, false, SslProtocols.None, true); //SslProtocols.None means that the OS is allowed to decide which protocol to use, default is currently TLS 1.2, 1.3 is only available in Windows 11
        tlsClient = new(Client, stream);
      }
      catch (Exception ex)
      {
        FlowEngine.Log?.Write("FAILED to AuthenticateAsServer with SslStream", ex, LOG_TYPE.ERR);
        throw;
      }

      lock (mCriticalSection)
      {
        mClients.Add(tlsClient);
      }
      OnNewConnection(tlsClient);
      return tlsClient;
    }

  }
}
