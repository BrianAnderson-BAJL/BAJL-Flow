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
    }
    public bool Hit;
    public HIT_TYPE Type;
    public Core.FlowBase? HitItem;
    public Core.FlowBase? ParentItem;
    public Vector2 Position;

    public HIT_RESULT()
    {
      Hit = false;
      Type = HIT_TYPE.None;
      HitItem = null;
      ParentItem = null;
      Position = Vector2.Zero;
    }
  }


}
