using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Authentication;

namespace Core.Administration
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
    private List<TcpTlsClient> mClients = new List<TcpTlsClient>(32);
    private int mPort;
    private object mCriticalSection = new object();
    private bool mContinue = true;
    private int mReadPacketTimeout = 5000;

    protected void OnConnectionClosed(TcpTlsClient client)
    {
      if (ConnectionClosed != null)
      {
        EventArgsTcpClient EA = new EventArgsTcpClient(client);
        ConnectionClosed(this, EA);
      }
      client.OnConnectionClosed(client);
    }

    protected void OnNewPacket(Packet packet, TcpTlsClient client)
    {
      if (NewPacket != null)
      {
        EventArgsPacket EA = new EventArgsPacket(packet, client);
        NewPacket(this, EA);
      }
      client.OnNewPacket(packet, client);
    }

    protected void OnNewConnection(TcpTlsClient client)
    {
      if (NewConnection != null)
      {
        EventArgsTcpClient EA = new EventArgsTcpClient(client);
        NewConnection(this, EA);
      }
    }

    public TcpTlsServer(int ReadPacketTimeout, string pathToCert) //string CertificateSerialNumber, 
    {

      mReadPacketTimeout = ReadPacketTimeout;
      //X509Store Store = new X509Store(StoreName.TrustedPublisher, StoreLocation.CurrentUser);
      //Store.Open(OpenFlags.ReadOnly);
      //X509Certificate2Collection Certificates = Store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, "CN=flowengine.bajlllc.com", false);
      //Store.Close();
      
      mCertificate = new X509Certificate("c:\\gamedev\\cert.pfx", "b1581a");

      //if (Certificates.Count > 0)
      //{
      //  mCertificate = Certificates[0];

      //}

    }

    public void Start(int PortNumber)
    {
      mPort = PortNumber;
      Thread T = new Thread(ListenThread);
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
      try
      {
        TcpListener Listener = new TcpListener(IPAddress.Any, mPort);
        Listener.Start();

        while (mContinue == true)
        {
          try
          {
            if (Listener.Pending() == true)
            {
              System.Net.Sockets.TcpClient Client = Listener.AcceptTcpClient();
              Client.NoDelay = true;
              Client.LingerState = new LingerOption(false, 0);
              TcpTlsClient C = ClientAdd(Client);
              C.NewPacket += this.NewPacket;
              C.ConnectionClosed += this.ConnectionClosed;
              Thread T = new Thread(new ParameterizedThreadStart(C.ReadTlsPacketsThread!));
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
      catch //(Exception ex)
      {

      }
    }

    private void C_ConnectionClosed(object? sender, EventArgsTcpClient e)
    {
      throw new NotImplementedException();
    }

    private TcpTlsClient ClientAdd(System.Net.Sockets.TcpClient Client)
    {
      SslStream stream = new SslStream(Client.GetStream(), false);
      stream.AuthenticateAsServer(mCertificate, false, SslProtocols.Tls, true);
      TcpTlsClient C = new TcpTlsClient(Client, stream);

      lock (mCriticalSection)
      {
        mClients.Add(C);
      }
      OnNewConnection(C);
      return C;
    }

  }
}
