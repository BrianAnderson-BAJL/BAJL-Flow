using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class Input : FlowBase
  {
    public string Label = "";
    public Vector2 Offset;
    public FunctionStep? Step = null;

    public Input(string label, Vector2 offset)
    {
      Label = label;
      Offset = offset;
    }
    public Input(string label, Vector2 offset, FunctionStep fs)
    {
      Label = label;
      Offset = offset;
      this.Step = fs;
    }

    public Input Clone(FunctionStep fs)
    {
      return new Input(this.Label, this.Offset, fs);
    }
  }
}
