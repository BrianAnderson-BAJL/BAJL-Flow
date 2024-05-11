using FlowEngineCore.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  internal class Debugger
  {
    public User DebugUser;
    public int DebugMessagePacketId = 0;  //IF the user is debugging a specific flow this will be assigned so the client can map up the response to the original request
    public string FlowFileName = "";

    public Debugger(User debugUser, int debugMessagePacketId = 0, string flowName = "")
    {
      DebugUser = debugUser;
      DebugMessagePacketId = debugMessagePacketId;
      FlowFileName = flowName;
    }
  }
}
