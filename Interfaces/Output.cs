using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class Output : FlowBase
  {
    public string Label = "";
    public Vector2 Offset;
    public FunctionStep? Step = null;
    public int OutputIndex = 0;

    public static readonly Vector2 SUCCESS_POS = new Vector2(160, 90);
    public static readonly Vector2 ERROR_POS = new Vector2(160, 130);

    internal Output() 
    {
    }

    public Output(string label, Vector2 offset, int outputIndex)
    {
      Label = label;
      Offset = offset;
      OutputIndex = outputIndex;
    }
    public Output(string label, FunctionStep functionStep, Vector2 offset, int outputIndex)
    {
      Label = label;
      Offset = offset;
      this.Step = functionStep;
      this.OutputIndex = outputIndex;
    }

    public Output Clone(FunctionStep fs)
    {
      return new Output(this.Label, fs, this.Offset, this.OutputIndex);
    }
  }
}
