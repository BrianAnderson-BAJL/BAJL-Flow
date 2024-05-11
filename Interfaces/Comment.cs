using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class Comment : FlowBase
  {
    public string Text = "";
    public Color ColorBackground;
    public Color ColorText;
    public const int ResizeHandleSize = 10;
    public ResizeHandle[] ResizeRects = new ResizeHandle[8];

    public Comment()
    {
      ResizeRects[0] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.TopLeft);
      ResizeRects[1] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.TopMiddle);
      ResizeRects[2] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.TopRight);
      ResizeRects[3] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.MiddleLeft);
      ResizeRects[4] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.MiddleRight);
      ResizeRects[5] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.BottomLeft);
      ResizeRects[6] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.BottomMiddle);
      ResizeRects[7] = new ResizeHandle(this, ResizeHandle.RESIZE_LOCATION.BottomRight);
    }
  }
}
