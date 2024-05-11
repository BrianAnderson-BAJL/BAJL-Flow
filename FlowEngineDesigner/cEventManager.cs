using FlowEngineCore.Administration.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  public class cEventManager
  {
    public enum TRACER_TYPE
    {
      Information,
      Warning,
      Error,
    }

    public enum SENDER
    {
      FlowDebug,
      Compiler,
      FlowEngineServer,
    }

    public static event EventHandler<TracerEventArgs>? Tracer;

    public static void RaiseEventTracer(SENDER sender, string data, BaseResponse.RESPONSE_CODE response, long ticks = 0)
    {
      TRACER_TYPE tracer = TRACER_TYPE.Information;
      if (response == BaseResponse.RESPONSE_CODE.AccessDenied || response == BaseResponse.RESPONSE_CODE.BadRequest)
        tracer = TRACER_TYPE.Warning;
      else if (response == BaseResponse.RESPONSE_CODE.Error)
        tracer = TRACER_TYPE.Error;
      RaiseEventTracer(sender, data, tracer, ticks);
    }

    public static void RaiseEventTracer(SENDER sender, string data, TRACER_TYPE tracerType = TRACER_TYPE.Information, long ticks = 0, string xmlData = "")
    {
      if (Tracer is not null)
      {
        Tracer(sender, new TracerEventArgs(data, tracerType, xmlData, ticks));
      }
    }

    public static void RaiseEventTracer(FlowEngineCore.Administration.Packet packet)
    {
      if (packet.PacketType != FlowEngineCore.Administration.Packet.PACKET_TYPE.Trace)
        return;

      FlowEngineCore.Administration.Messages.TraceResponse trace = new TraceResponse(packet);
      TRACER_TYPE type = TRACER_TYPE.Information;
      if (trace.Success == false)
        type = TRACER_TYPE.Error;
      RaiseEventTracer(SENDER.FlowDebug, trace.PreviousStepName, type, trace.ExecutionTicks, trace.ResponseXml);
    }
  }

  public class TracerEventArgs : EventArgs
  {
    public string Trace;
    public cEventManager.TRACER_TYPE TracerType;
    public string XmlData = "";
    public long Ticks = 0;
    public TracerEventArgs(string trace, cEventManager.TRACER_TYPE tracerType, string xmlData = "", long ticks = 0)
    {
      Trace = trace;
      TracerType = tracerType;
      XmlData = xmlData;
      Ticks = ticks;
    }
  }
}
