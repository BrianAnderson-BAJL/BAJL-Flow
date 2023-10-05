using Core.Administration.Messages;
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

    public static event EventHandler<TracerEventArgs>? Tracer;

    public static void RaiseEventTracer(object sender, string data, BaseResponse.RESPONSE_CODE response)
    {
      TRACER_TYPE tracer = TRACER_TYPE.Information;
      if (response == BaseResponse.RESPONSE_CODE.AccessDenied)
        tracer = TRACER_TYPE.Warning;
      else if (response == BaseResponse.RESPONSE_CODE.Error)
        tracer = TRACER_TYPE.Error;
      RaiseEventTracer(sender, data, tracer);
    }

    public static void RaiseEventTracer(object sender, string data, TRACER_TYPE tracerType = TRACER_TYPE.Information)
    {
      if (Tracer != null)
      {
        Tracer(sender, new TracerEventArgs(data, tracerType));
      }
    }
  }

  public class TracerEventArgs : EventArgs
  {
    public string Trace;
    public cEventManager.TRACER_TYPE TracerType;
    public TracerEventArgs(string trace, cEventManager.TRACER_TYPE tracerType)
    {
      Trace = trace;
      TracerType = tracerType;
    }
  }
}
