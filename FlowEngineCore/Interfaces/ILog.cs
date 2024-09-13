using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Interfaces
{
  public interface ILog
  {
    void Write(string val, LOG_TYPE debug = LOG_TYPE.INF, string? overrideThreadName = null);
    void Write(Exception ex, LOG_TYPE debug = LOG_TYPE.INF);
    void Write(string customErrorMessage, Exception ex, LOG_TYPE debug = LOG_TYPE.INF);

    /// <summary>
    /// Writes the contents of 'val' to the file bypassing the log queue, use this if the application is shuting down quickly (SIGTERM received, ...)
    /// </summary>
    /// <param name="val"></param>
    /// <param name="overrideLogFileName"></param>
    /// <param name="logType"></param>
    void WriteInstantly(string val, string overrideLogFileName = "", LOG_TYPE logType = LOG_TYPE.INF);

    /// <summary>
    /// Flush the log queue and write the contents to the currently open log file, , use this if the application is shuting down quickly (SIGTERM received, ...)
    /// </summary>
    public void FlushLogEntries();
  }
}
