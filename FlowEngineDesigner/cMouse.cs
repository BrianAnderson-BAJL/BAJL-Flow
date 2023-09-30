using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class cMouse
  {
    public enum OVERALL_STATE
    {
      None,
      DrawComment,
    }

    public enum BUTTON_STATE
    {
      Up,
      Down,
    }
    public static Vector2 pos;

    //public static Interfaces.Output? mOutput = null;
    //public static Interfaces.Input? mInput = null;
    public static BUTTON_STATE ButtonLeft = BUTTON_STATE.Up;
    public static BUTTON_STATE ButtonRight = BUTTON_STATE.Up;
    public static frmFlow? CurrentForm = null;
    public static Vector2? DraggingStart = null;
    public static FlowBase? FlowItem = null;
    public static HIT_RESULT PreviousHitItem = new HIT_RESULT();
    public static OVERALL_STATE OverallState = OVERALL_STATE.None;

    public void Clear()
    {
      CurrentForm = null;
      DraggingStart = null;
      FlowItem = null;
      PreviousHitItem = new HIT_RESULT();
    }
    //public static Interfaces.Output? output
    //{
    //  get { return mOutput; }
    //  private set { }
    //}

    //public static Interfaces.Input? input 
    //{ 
    //  get { return mInput; }
    //  private set { }
    //}

    //public static void SetOutput(Interfaces.Output? o)
    //{
    //  if (mInput == null)
    //  {
    //    mOutput = o;
        
    //  }
    //  else //Input is set, must of dragged from input to output
    //  {
    //  }
    //}

    //public static void SetInput(Interfaces.Input? i)
    //{
    //  if (mOutput == null)
    //  {
    //    mInput = i;
    //  }
    //  else //Output is set, must of dragged from output to input
    //  {
    //  }
    //}
  }
}
