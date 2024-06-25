using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace FlowEngineCore.Administration
{
  public class TcpClient : TcpClientBase
  {


    public TcpClient(System.Net.Sockets.TcpClient oClient) : base(oClient)
    {
    }

    ~TcpClient()
    {
      mContinue = false;
    }

    public static FlowEngineCore.Administration.TcpClient? Connect(string hostUrl, int port, int connectTimeout = 5000, int readPacketTimeout = 5000)
    {
      FlowEngineCore.Administration.TcpClient? returnClient = null;
      try
      {
        System.Net.Sockets.TcpClient Client = new();
        if (Client.ConnectAsync(hostUrl, port).Wait(connectTimeout) == false)
        {
          Global.WriteToConsoleDebug($"Could not connect to url [{hostUrl}], port [{port}]");
          return null; //Couldn't connect within the connectTimeout period, return null (failure)
        }
        returnClient = new TcpClient(Client);
        returnClient.ReadPacketTimeout = readPacketTimeout;
        Thread T = new(new ParameterizedThreadStart(returnClient.ReadPacketsThread!));
        T.Start(returnClient);
      }
      catch 
      {
        Global.WriteToConsoleDebug($"Could not connect to url [{hostUrl}], port [{port}]");
      }
      return returnClient;
    }



  }
}