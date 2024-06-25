using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace FlowEngineCore
{

  public class FlowBase
  {
    public int Id;
    public Vector2 Position;
    public SizeF Size;
    public bool Selected = false;
  }
}
