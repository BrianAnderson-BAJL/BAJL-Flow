﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class Output : FlowBase
  {
    public enum TYPE
    {
      Success,
      Error,
      OutherOutput0, 
      OutherOutput1,
    }

    public string Label = "";
    public Vector2 Offset;
    public FunctionStep? Step = null;
    //public int OutputIndex = 0;
    public TYPE Type = TYPE.Success;

    public const string SUCCESS_LABEL = "Success";
    public const string ERROR_LABEL = "Error";

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

    public Output(string label, TYPE outputType)
    {
      Label = label;
      Offset = SUCCESS_OUTPUT_POS + (OUTPUT_OFFSET * (int)outputType);
      Type = outputType;
    }
    public Output(string label, FunctionStep? functionStep, TYPE outputType)
    {
      Label = label;
      Offset = SUCCESS_OUTPUT_POS + (OUTPUT_OFFSET * (int)outputType);
      this.Step = functionStep;
      Type = outputType;
    }

    public Output Clone(FunctionStep fs)
    {
      return new Output(this.Label, fs, this.Type);
    }

    public Output Clone()
    {
      return new Output(this.Label, this.Step, this.Type);
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
