using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class TimeElapsed
  {
    private DateTime StartTime = DateTime.UtcNow;

    public TimeSpan HowLong()
    {
      return DateTime.UtcNow - StartTime;
    }
  }
}
