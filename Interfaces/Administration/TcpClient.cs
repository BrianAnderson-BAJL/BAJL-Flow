﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Core.Administration
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

    public static Core.Administration.TcpClient? Connect(string hostUrl, int port, int connectTimeout = 5000, int readPacketTimeout = 5000)
    {
      Core.Administration.TcpClient? returnClient = null;
      try
      {
        System.Net.Sockets.TcpClient Client = new System.Net.Sockets.TcpClient();
        if (Client.ConnectAsync(hostUrl, port).Wait(connectTimeout) == false)
        {
          Global.Write($"Could not connect to url [{hostUrl}], port [{port}]");
          return null; //Couldn't connect within the connectTimeout period, return null (failure)
        }
        returnClient = new TcpClient(Client);
        returnClient.ReadPacketTimeout = readPacketTimeout;
        Thread T = new Thread(new ParameterizedThreadStart(returnClient.ReadPacketsThread!));
        T.Start(returnClient);
      }
      catch 
      {
        Global.Write($"Could not connect to url [{hostUrl}], port [{port}]");
      }
      return returnClient;
    }



  }
}