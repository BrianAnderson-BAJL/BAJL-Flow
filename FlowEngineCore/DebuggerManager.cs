using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  internal class DebuggerManager
  {
    private static List<Debugger> Debuggers = new(8);
    private static object CriticalSection = new();
    public static Debugger Add(User user, int debugMessagePacketId = 0, string flowName = "")
    {

      Debugger? debugger = FindByUser(user);
      if (debugger is null)
      {
        debugger = new Debugger(user, debugMessagePacketId, flowName);
        lock (CriticalSection)
        {
          Debuggers.Add(debugger);
        }
      }
      return debugger;
    }

    public static Debugger? FindByUser(User user) 
    {
      lock (CriticalSection)
      {
        for (int x = 0; x < Debuggers.Count; x++)
        {
          if (Debuggers[x].DebugUser == user)
          {
            return Debuggers[x];
          }
        }
      }
      return null;
    }

    public static int AttachedDebuggersCount
    {
      get
      {
        lock (CriticalSection)
        {
          return Debuggers.Count;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flowName"></param>
    /// <returns></returns>
    public static Debugger? FindByFileName(string flowName)
    {
      lock (CriticalSection)
      {
        for (int x = 0; x < Debuggers.Count; x++)
        {
          if (Debuggers[x].FlowFileName == flowName)
          {
            return Debuggers[x];
          }
        }
      }
      return null;
    }

    public static void RemoveByUser(User user)
    {
      lock (CriticalSection)
      {
        for (int x = 0; x < Debuggers.Count; x++)
        {
          if (Debuggers[x].DebugUser == user)
          {
            Debuggers.RemoveAt(x);
            break;
          }
        }
      }
    }

    public static void SendToAllDebuggers(Administration.Packet packet)
    {
      List<Debugger> copy;
      lock (CriticalSection)
      {
        copy = new List<Debugger>(Debuggers);
      }
      for (int x = 0; x < copy.Count; x++)
      {
        Debugger debug = copy[x];

        if (debug.DebugUser is not null && debug.DebugUser.TcpClientConnection is not null)
        {
          debug.DebugUser.TcpClientConnection.Send(packet);
        }
      }
    }
  }
}
