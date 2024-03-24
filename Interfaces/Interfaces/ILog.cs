using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
  public interface ILog
  {
    public void Write(string val, LOG_TYPE debug = LOG_TYPE.INF);

  }
}
