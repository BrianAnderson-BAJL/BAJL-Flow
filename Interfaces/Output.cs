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

    public static readonly string SUCCESS_LABEL = "Success";
    public static readonly string ERROR_LABEL = "Error";

    internal static Vector2 HighlightCenterOffset = new Vector2(15, 15);
    public static readonly Vector2 SUCCESS_OUTPUT_POS = new Vector2(160, 90);
    public static readonly Vector2 OUTPUT_OFFSET = new Vector2(0, 40);
    //public static readonly Vector2 OUTPUT2_POS = new Vector2(160, 130);
    //public static readonly Vector2 OUTPUT3_POS = new Vector2(160, 170);
    //public static readonly Vector2 OUTPUT4_POS = new Vector2(160, 210);
    //public static readonly Vector2 OUTPUT5_POS = new Vector2(160, 250);
    //public static readonly Vector2 OUTPUT6_POS = new Vector2(160, 290);

    internal Output() 
    {
    }

    public Output(string label, int outputIndex)
    {
      Label = label;
      Offset = SUCCESS_OUTPUT_POS + (OUTPUT_OFFSET * outputIndex);
      OutputIndex = outputIndex;
    }
    public Output(string label, FunctionStep functionStep, int outputIndex)
    {
      Label = label;
      Offset = SUCCESS_OUTPUT_POS + (OUTPUT_OFFSET * outputIndex);
      this.Step = functionStep;
      this.OutputIndex = outputIndex;
    }

    public Output Clone(FunctionStep fs)
    {
      return new Output(this.Label, fs, this.OutputIndex);
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
