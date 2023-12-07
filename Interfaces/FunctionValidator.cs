using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{

  /// <summary>
  /// Function validators was originally created for database steps. While a SELECT statement might execute properly, but if it doesn't pull back any records it can still be considered a failure.
  /// </summary>
  public abstract class FunctionValidator
  {
    public delegate void ValidatorDelegate(RESP resp);

    public string Name = "";

    public virtual void Validate(RESP resp)
    {
    }
  }
}
