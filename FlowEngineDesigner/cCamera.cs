using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowEngineDesigner
{
  public class cCamera
  {
    public Vector2 Position = new Vector2();
    private float ZoomLevelActual = 1.0f;
    public frmFlow FormFlow;
    

    public cCamera(frmFlow formFlow)
    {
      this.FormFlow = formFlow; 
      
    }
    
    public float ZoomLevel
    {
      get { return ZoomLevelActual; }
      set { ZoomLevelActual = value; }
    }

    public void ZoomIn()
    {
      ZoomLevelActual = ZoomLevelActual * 1.1f;
      if (ZoomLevelActual > 5.0f)
      {
        ZoomLevelActual = 5f;
      }
    }

    public void ZoomOut()
    {
      ZoomLevelActual = ZoomLevelActual * 0.9f;
      if (ZoomLevelActual < 0.1f)
      {
        ZoomLevelActual = 0.1f;
      }
    }


    public Rectangle CreateDrawingRect(Vector2 pos, Size size)
    {
      int x = (int)((pos.X * ZoomLevelActual) + this.Position.X);
      int y = (int)((pos.Y * ZoomLevelActual) + this.Position.Y);
      int width = (int)(size.Width * ZoomLevelActual);
      int height = (int)(size.Height * ZoomLevelActual);
      return new Rectangle(x, y, width, height);
    }

    public Vector2 CreateDrawingPosition(Vector2 v)
    {
      v *= this.ZoomLevelActual;
      v += this.Position;
      return v;
    }

    public Size CreateRealSize(Vector2 vector)
    {
      vector = vector / ZoomLevelActual;
      return new Size((int)vector.X, (int)vector.Y);
    }

    public Size CreateRealSize(float width, float height)
    {
      width = width / ZoomLevelActual;
      height = height / ZoomLevelActual;
      return new Size((int)width, (int)height);
    }


    public Vector2 CreateRealPosition(Vector2 vector)
    {
      vector -= ((Position));
      vector /= ZoomLevelActual;
      return vector;
    }

    public Vector2 CreateRealPosition(float x, float y)
    {
      Vector2 v = new Vector2(x, y);
      return CreateRealPosition(v);
    }


    public void Move(Vector2 v)
    {
      Position += v;
    }

  }
}
