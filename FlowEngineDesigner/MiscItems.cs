using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{

  public struct HIT_RESULT
  {
    public enum HIT_TYPE
    {
      None,
      Function,
      Input,    //Has parent - step
      Output,   //Has parent - step
      Link,
      Comment,

      //Resize handles
      Resize,
      //ResizeTopMiddle = 7,
      //ResizeTopRight = 8,
      //ResizeMiddleLeft = 9,
      //ResizeMiddleRight = 10,
      //ResizeBottomLeft = 11,
      //ResizeBottomMiddle = 12,
      //ResizeBottomRight = 13,
    }
    public bool Hit;
    public HIT_TYPE Type;
    public FlowEngineCore.FlowBase? HitItem;
    public FlowEngineCore.FlowBase? ParentItem;
    public Vector2 Position;

    public HIT_RESULT()
    {
      Hit = false;
      Type = HIT_TYPE.None;
      HitItem = null;
      ParentItem = null;
      Position = Vector2.Zero;
    }

    public HIT_RESULT Clone()
    {
      HIT_RESULT hr = new HIT_RESULT();
      hr.Hit = this.Hit;
      hr.Type = this.Type;
      hr.HitItem = this.HitItem;
      hr.ParentItem = this.ParentItem;
      hr.Position = this.Position;
      return hr;
    }
  }


}
