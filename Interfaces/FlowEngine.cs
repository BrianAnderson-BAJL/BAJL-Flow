﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Core.Administration.Messages;
using Core.Administration;
using Core.Administration.Packets;

namespace Core
{
    public class FlowEngine
  {
    public enum STATE
    {
      _None,
      Initializing,
      Running,
      Stopping,
    }

    List<Variable> GlobalVariables = new List<Variable>();
    private static List<FlowRequest> WaitingRequests = new List<FlowRequest>(256);
    Administration.TcpTlsServer? tcpServer;
    private static STATE mState = STATE._None;
    private static long ThreadName = Options.ThreadNameStartingNumber;
    internal static FlowEngine Instance = new FlowEngine();

    public static string GetNextThreadName()
    {
      Interlocked.Increment(ref ThreadName);
      return ThreadName.ToString();
    }

    public void Init(string[] args)
    {
      Global.WriteToConsoleDebug($"Initializing... Current working directory [{Directory.GetCurrentDirectory()}]");
      Instance = this;
      Packet.ReversePacketId = true;
      Console.CancelKeyPress += Console_CancelKeyPress;
      mState = STATE.Initializing;
      Options.PreParseArgs(args);
      Options.LoadSettings();
      Options.ParseArgs(args); //Command line arguments always override the settings.xml file
      ThreadName = Options.ThreadNameStartingNumber;
      try
      {
        tcpServer = new Administration.TcpTlsServer(Options.AdministrationReadPacketTimeoutInMs, Options.TlsCertFileNamePath, Options.TlsCertPassword);
        tcpServer.NewConnection += Administration_TcpServer_NewConnection;
        tcpServer.ConnectionClosed += Administration_TcpServer_ConnectionClosed;
        tcpServer.NewPacket += Administration_TcpServer_NewPacket;
        tcpServer.Start(Options.AdministrationPortNumber);
      }
      catch (FileNotFoundException ex)
      {
        Global.WriteToConsoleDebug(ex.Message + $" - [{Options.TlsCertFileNamePath}]", LOG_TYPE.ERR);
        Environment.Exit(0);
      }
      catch (Exception ex1)
      {
        Global.WriteToConsoleDebug(ex1.Message, LOG_TYPE.ERR);
        Environment.Exit(0);
      }

      Global.WriteToConsoleDebug($"Initializing...Loading plugins from [{Options.GetFullPath(Options.PluginPath)}]");
      PluginManager.Load(Options.GetFullPath(Options.PluginPath)); //Open all the *.dlls and load them  //PluginManager.GlobalPluginValues is populated here
      Global.WriteToConsoleDebug($"Initializing...Loading flows from [{Options.GetFullPath(Options.FlowPath)}]");
      FlowManager.Load(Options.GetFullPath(Options.FlowPath), PluginManager.GlobalPluginValues);  //Parse all the flows in the path and attach them to the plugins
      Global.WriteToConsoleDebug("Initializing...Starting all plugins");
      PluginManager.StartPlugins(); 

      Global.WriteToConsoleDebug("Initializing...Loading users");
      SecurityProfileManager.Load();
      UserManager.Load();
      MessageProcessor.Init(PluginManager.GlobalPluginValues);

      ThreadPool.GetMaxThreads(out int threads, out int compThreads);
      Global.WriteToConsoleDebug($"Threads in Pool, worker [{threads}], I/O threads [{compThreads}]");
    }


    /// <summary>
    /// Called when the flow engine actually starts
    /// </summary>
    public void Run()
    {
      Global.WriteToConsoleDebug("Running...");
      mState = STATE.Running;

      for (int i = 0; i < WaitingRequests.Count; i++)
      {
        ThreadPool.QueueUserWorkItem(ThreadProc, WaitingRequests[i]);
      }
      WaitingRequests.Clear();

      do
      {
        Thread.Sleep(1); //Main thread doesn't need to do anything, it just sleeps while the threadpool and plugins do all the work.
#if DEBUG
        if (Console.KeyAvailable == true)
        {
          ConsoleKeyInfo cki = Console.ReadKey();
          if (cki.Key == ConsoleKey.Escape || cki.Key == ConsoleKey.Spacebar)
          {
            mState = STATE.Stopping;
          }
        }
#endif
      } while (mState == STATE.Running);
      Global.WriteToConsoleDebug("Stopping...");
      tcpServer!.Stop();
      PluginManager.StopPlugins();
      Global.WriteToConsoleDebug("Flow Engine exiting");
    }

    private void Administration_TcpServer_NewPacket(object? sender, Administration.EventArgsPacket e)
    {
      Global.WriteToConsoleDebug($"Received new TCP Administration packet, type [{e.Packet.PacketType}]");
      MessageProcessor.ProcessMessage(e.Packet, e.Client);
    }

    private void Administration_TcpServer_ConnectionClosed(object? sender, Administration.EventArgsTcpClient e)
    {
      Global.WriteToConsoleDebug("TCP Administration connection closed");
    }

    private void Administration_TcpServer_NewConnection(object? sender, Administration.EventArgsTcpClient e)
    {
      Global.WriteToConsoleDebug("New TCP Administration connection");

    }


    /// <summary>
    /// When Ctrl+C or Ctrl+Break are pressed stop the engine
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
      Stop();
    }

    public void Stop()
    {
      mState = STATE.Stopping;
    }

    /// <summary>
    /// Called from plugins when they want to start a flow (http plugin will start a flow when it receives a web request, GET, POST, ...)
    /// </summary>
    /// <param name="request"></param>
    public static void StartFlow(FlowRequest request)
    {
      if (request.PluginStartedFrom is not null)
      {
        RESP.Log?.Write($"Plugin [{request.PluginStartedFrom.Name}] Starting new flow [{request.FlowToStart}] in new thread", LOG_TYPE.INF, request.OverrideThreadName);
      }
      else
      {
        RESP.Log?.Write($"Starting new flow [{request.FlowToStart}] in new thread", LOG_TYPE.INF, request.OverrideThreadName);
      }
      if (mState == STATE.Initializing)
      {
        WaitingRequests.Add(request); //Queue up the events that want to execute during start up, the engine will execute them when the system is done starting.
      }
      else if (mState == STATE.Running) // || request.PluginStartedFrom.Name == "FlowCore"
      {
        ThreadPool.QueueUserWorkItem(ThreadProc, request);
      }
    }

    public static Flow StartFlowSameThread(FlowRequest request)
    {
      if (request.PluginStartedFrom is not null)
      {
        RESP.Log?.Write($"Plugin [{request.PluginStartedFrom.Name}] Starting new flow [{request.FlowToStart}]");
      }
      else
      {
        RESP.Log?.Write($"Starting new flow [{request.FlowToStart}]");
      }

      Flow clonedFlow = request.FlowToStart.Clone(); //Need to clone the flow before running it

      if (request.Var is not null)
      {
        clonedFlow.VariableAdd(request.Var.Name, request.Var);
      }

      clonedFlow.Execute();
      return clonedFlow;
    }


    /// <summary>
    /// This thread is started from the threadpool, it is what actually executes a flow
    /// </summary>
    /// <param name="stateInfo"></param>
    public static void ThreadProc(object? stateInfo)
    {
      FlowRequest ? flowRequest = stateInfo as FlowRequest;
      if (flowRequest is null)
        return;

      if (flowRequest.OverrideThreadName is null)
        Thread.CurrentThread.Name = GetNextThreadName();
      else
        Thread.CurrentThread.Name = flowRequest.OverrideThreadName;

      if (flowRequest.CloneFlow == FlowRequest.CLONE_FLOW.CloneFlow)
        flowRequest.FlowToStart = flowRequest.FlowToStart.Clone(); //Need to clone the flow before running it

      if (flowRequest.Var is not null)
      {
        flowRequest.FlowToStart.VariableAdd(flowRequest.Var.Name, flowRequest.Var);
      }

      flowRequest.FlowToStart.Execute();
    }




  }
}
