using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class ExceptionFlowLoad : Exception
  {
    public ExceptionFlowLoad(string? message) :base(message)
    {
    }
  }
}
