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
  }
}
