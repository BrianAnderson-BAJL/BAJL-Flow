using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FlowEngineCore.Administration.Messages;
using FlowEngineCore.Administration;
using FlowEngineCore.Administration.Packets;
using FlowEngineCore.Interfaces;
using FlowEngineCore.Statistics;

namespace FlowEngineCore
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

    List<Variable> GlobalVariables = new();
    private static List<FlowRequest> WaitingRequests = new(256);
    Administration.TcpTlsServer? tcpServer;
    private static STATE mState = STATE._None;
    private static long ThreadName = 0;// Options.ThreadNameStartingNumber;
    internal static FlowEngine Instance = new();
    public static ILog? Log;

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
      Options.CreateAndLoadSettings();
      Options.ParseArgs(args); //Command line arguments always override the settings.xml file
      ThreadName = Options.GetSettings.SettingGetAsLong("ThreadNameStartingNumber");
      Options.ForceLoadDlls();
      Global.WriteToConsoleDebug($"Initializing...Loading plugins from [{Options.GetFullPath(Options.GetSettings.SettingGetAsString("PluginPath"))}]");
      PluginManager.Load(Options.GetFullPath(Options.GetSettings.SettingGetAsString("PluginPath"))); //Open all the *.dlls and load them  //PluginManager.GlobalPluginValues is populated here
      if (PluginManager.GlobalPluginValues.ContainsKey("log") == true)
        Log = PluginManager.GlobalPluginValues["log"] as ILog;

      Log?.Write($"Initializing... Current working directory [{Directory.GetCurrentDirectory()}]");
      Log?.Write($"Initializing...Loading plugins from [{Options.GetFullPath(Options.GetSettings.SettingGetAsString("PluginPath"))}]");
      Log?.Write($"Initializing...Loading flows from [{Options.GetFullPath(Options.GetSettings.SettingGetAsString("FlowPath"))}]");
      FlowManager.Load(Options.GetFullPath(Options.GetSettings.SettingGetAsString("FlowPath")), PluginManager.GlobalPluginValues);  //Parse all the flows in the path and attach them to the plugins
      Log?.Write("Initializing...Starting all plugins");
      PluginManager.StartPlugins();

      Log?.Write("Initializing...Loading users");
      SecurityProfileManager.Load();
      UserManager.Load();
      MessageProcessor.Init(PluginManager.GlobalPluginValues);
      StatisticsManager.Start();
      ThreadPool.GetMaxThreads(out int threads, out int compThreads);
      Log?.Write($"Threads in Pool, worker [{threads}], I/O threads [{compThreads}]");

      try
      {
        tcpServer = new Administration.TcpTlsServer(Options.GetSettings.SettingGetAsInt("ReadPacketTimeoutInMs"), Options.GetSettings.SettingGetAsString("TlsCertFileNamePath"), Options.GetSettings.SettingGetAsString("TlsCertPassword"));
        tcpServer.NewConnection += Administration_TcpServer_NewConnection;
        tcpServer.ConnectionClosed += Administration_TcpServer_ConnectionClosed;
        tcpServer.NewPacket += Administration_TcpServer_NewPacket;
        tcpServer.Start(Options.GetSettings.SettingGetAsInt("PortNumber"));
      }
      catch (FileNotFoundException ex)
      {
        Log?.Write(ex.Message + $" - [{Options.GetSettings.SettingGetAsString("TlsCertFileNamePath")}]", LOG_TYPE.ERR);
        Environment.Exit(0);
      }
      catch (Exception ex1)
      {
        Log?.Write(ex1.Message, LOG_TYPE.ERR);
        Environment.Exit(0);
      }
    }


    /// <summary>
    /// Called when the flow engine actually starts
    /// </summary>
    public void Run()
    {
      Log?.Write("Flow Engine Running");
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
      Log?.Write("Flow Engine Stopping");
      tcpServer!.Stop();
      PluginManager.StopPlugins();
      StatisticsManager.Stop();
      Log?.Write("Flow Engine exiting");
    }

    private void Administration_TcpServer_NewPacket(object? sender, Administration.EventArgsPacket e)
    {
      Log?.Write($"Received new TCP Administration packet, type [{e.Packet.PacketType}]");
      MessageProcessor.ProcessMessage(e.Packet, e.Client);
    }

    private void Administration_TcpServer_ConnectionClosed(object? sender, Administration.EventArgsTcpClient e)
    {
      Log?.Write("TCP Administration connection closed");
    }

    private void Administration_TcpServer_NewConnection(object? sender, Administration.EventArgsTcpClient e)
    {
      Log?.Write("New TCP Administration connection");

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
        Log?.Write($"Plugin [{request.PluginStartedFrom.Name}] Starting new flow [{request.FlowToStart}] in new thread", LOG_TYPE.INF, request.OverrideThreadName);
      }
      else
      {
        Log?.Write($"Starting new flow [{request.FlowToStart}] in new thread", LOG_TYPE.INF, request.OverrideThreadName);
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
        Log?.Write($"Plugin [{request.PluginStartedFrom.Name}] Starting new flow [{request.FlowToStart}]");
      }
      else
      {
        Log?.Write($"Starting new flow [{request.FlowToStart}]");
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

      flowRequest.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;
      StatisticsManager.RequestStarted(flowRequest);

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
      StatisticsManager.RequestFinished(flowRequest);

    }




  }
}
