﻿using System;
using System.Collections.Generic;
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
    private object CriticalSection = new object(); //Had to add a critical section for the SslStream, it doesn't handle sending and receiving at the same time from differnet threads
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
        stream.AuthenticateAsClient(hostUrl);
        returnClient = new(client, stream);
        Thread T = new(new ParameterizedThreadStart(returnClient.ReadTlsPacketsThread!));
        T.Start(returnClient);
      }
      catch (Exception ex)
      {
        Global.WriteToConsoleDebug(ex.Message);
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

        //SslStream NS = Client.mStream;

        while (mContinue == true)
        {
          try
          {
            if (Client.mStream.CanRead)
            {
              //if (Br is null)
              //Br = new BinaryReader(NS);

              Packet Packet = new();
              //Packet.ReadAllData(Br);
              lock (CriticalSection)
              {
                Packet.ReadAllTlsData(Client.mStream);
              }
              if ((int)Packet.PacketType < (int)Packet.PACKET_TYPE._Unknown || (int)Packet.PacketType >= (int)Packet.PACKET_TYPE.zMaxValue)
                throw new Exception("UNKNOWN PacketType TCP message is corrupted");

              if (Packet.PacketType != Packet.PACKET_TYPE._Unknown)
                OnNewPacket(Packet, Client);

            }
            Thread.Sleep(1);
          }
          catch (SocketException ex)
          {
            Global.WriteToConsoleDebug(ex.Message);
            break;
          }
          catch (EndOfStreamException ex)
          {
            //Connection Closed
            Global.WriteToConsoleDebug(ex.Message);
            break;
          }
          catch (IOException ex)
          {
            //No Data
            //Global.WriteToConsoleDebug(ex.Message);
            //break;
          }
          catch (Exception ex)
          {
            //Unknown Error
            Global.WriteToConsoleDebug(ex.Message);

            break; //Exit from Loop
          }
        }
        OnConnectionClosed(Client);
        //Client.Stream.Close();
        Client.Close();
      }
      catch (Exception e)
      {
        Global.WriteToConsoleDebug(e.Message);
      }
    }


    public override bool Send(Packet Packet)
    {
      bool Rc = false;
      try
      {

        Packet.FinalizePacketBeforeSending();
        lock (CriticalSection)
        {
          mStream.Write(Packet.DataToSend, 0, Packet.SendLength); //Need to add the 4 byte header to the length being sent
          mStream.Flush();
        }
        Rc = true;
      }
      catch (Exception ex)
      {
        Global.WriteToConsoleDebug(ex.Message);
      }
      return Rc;

    }
  }
}
