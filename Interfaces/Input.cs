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
    internal static Vector2 HighlightCenterOffset = new Vector2(15, 15);
    public string Label = "";
    public Vector2 Offset;
    public FunctionStep? Step = null;

    public Input(string label, Vector2 offset)
    {
      Label = label;
      Offset = offset;
    }
    public Input(string label, Vector2 offset, FunctionStep? fs)
    {
      Label = label;
      Offset = offset;
      this.Step = fs;
    }

    public Input Clone(FunctionStep fs)
    {
      return new Input(this.Label, this.Offset, fs);
    }
    public Input Clone()
    {
      return new Input(this.Label, this.Offset, this.Step);
    }

    public new Vector2 Position
    {
      get 
      {
        if (Step is null)
          return Offset;
        else
          return Step.Position + Offset + HighlightCenterOffset; 
      }
    }
  }
}
