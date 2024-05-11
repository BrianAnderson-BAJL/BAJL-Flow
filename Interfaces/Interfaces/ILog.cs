using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Interfaces
{
  public interface ILog
  {
    public void Write(string val, LOG_TYPE debug = LOG_TYPE.INF, string? overrideThreadName = null);
    public void Write(Exception ex, LOG_TYPE debug = LOG_TYPE.INF);
    public void Write(string customErrorMessage, Exception ex, LOG_TYPE debug = LOG_TYPE.INF);
  }
}
