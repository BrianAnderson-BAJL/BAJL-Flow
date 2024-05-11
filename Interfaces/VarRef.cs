using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class VarRef 
  {
    private string mRef = "";

    public string Value
    {
      get { return mRef; }
      set { mRef = value; }
    }

    public VarRef(string valName)
    {
      mRef = valName;
    }
  }
}
