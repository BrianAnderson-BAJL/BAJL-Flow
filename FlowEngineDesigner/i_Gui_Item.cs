using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace FlowEngineDesigner
{
  public interface I_Gui_Item
  {
    public bool Selected { get; set; }

    public HIT_RESULT HitTest(Vector2 v, cCamera camera)
    {
      return new HIT_RESULT();
    }
    public void Move(Vector2 v)
    {
    }

    public void DrawSelection(Graphics g, cCamera camera)
    {

    }
  }
}
