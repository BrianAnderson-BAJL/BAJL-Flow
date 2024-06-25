using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class ResizeHandle : FlowBase
  {
    public enum RESIZE_LOCATION
    {
      TopLeft,
      TopMiddle,
      TopRight,
      MiddleLeft,
      MiddleRight,
      BottomLeft,
      BottomMiddle,
      BottomRight,
    }

    public Comment Parent;
    public RESIZE_LOCATION Location;
    public const int MIN_SIZE = 200;

    public ResizeHandle(Comment parent, RESIZE_LOCATION resizeLocation)
    {
      this.Parent = parent;
      this.Location = resizeLocation;
    }

    public Rectangle GetDrawingRect()
    {
      int halfSize = Comment.ResizeHandleSize / 2;

      if (Location == RESIZE_LOCATION.TopLeft)
        return new Rectangle((int)(Parent.Position.X - halfSize), (int)(Parent.Position.Y - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
      else if (Location == RESIZE_LOCATION.TopMiddle)
        return new Rectangle((int)(Parent.Position.X + (Parent.Size.Width / 2) - halfSize), (int)(Parent.Position.Y - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
      else if (Location == RESIZE_LOCATION.TopRight)
        return new Rectangle((int)(Parent.Position.X + (Parent.Size.Width) - halfSize), (int)(Parent.Position.Y - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
      else if (Location == RESIZE_LOCATION.MiddleLeft)
        return new Rectangle((int)(Parent.Position.X - halfSize), (int)(Parent.Position.Y + (Parent.Size.Height / 2) - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
      else if (Location == RESIZE_LOCATION.MiddleRight)
        return new Rectangle((int)(Parent.Position.X + (Parent.Size.Width) - halfSize), (int)(Parent.Position.Y + (Parent.Size.Height / 2) - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
      else if (Location == RESIZE_LOCATION.BottomLeft)
        return new Rectangle((int)(Parent.Position.X - halfSize), (int)(Parent.Position.Y + (Parent.Size.Height) - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
      else if (Location == RESIZE_LOCATION.BottomMiddle)
        return new Rectangle((int)(Parent.Position.X + (Parent.Size.Width / 2) - halfSize), (int)(Parent.Position.Y + (Parent.Size.Height) - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
      else
        return new Rectangle((int)(Parent.Position.X + (Parent.Size.Width) - halfSize), (int)(Parent.Position.Y + (Parent.Size.Height) - halfSize), Comment.ResizeHandleSize, Comment.ResizeHandleSize);
    }

    

    public void Resize(Vector2 v)
    {
      if (Location == RESIZE_LOCATION.TopLeft)
        ResizeTopLeft(v);
      else if (Location == RESIZE_LOCATION.TopMiddle)
        ResizeTopMiddle(v);
      else if (Location == RESIZE_LOCATION.TopRight)
        ResizeTopRight(v);
      else if (Location == RESIZE_LOCATION.MiddleLeft)
        ResizeMiddleLeft(v);
      else if (Location == RESIZE_LOCATION.MiddleRight)
        ResizeMiddleRight(v);
      else if (Location == RESIZE_LOCATION.BottomLeft)
        ResizeBottomLeft(v);
      else if (Location == RESIZE_LOCATION.BottomMiddle)
        ResizeBottomMiddle(v);
      else if (Location == RESIZE_LOCATION.BottomRight)
        ResizeBottomRight(v);
    }
    private void ResizeTopLeft(Vector2 v)
    {
      float width = Parent.Size.Width - v.X;
      float height = Parent.Size.Height - v.Y;
      if (width < MIN_SIZE)
        return;
      if (height < MIN_SIZE)
        return;
      Parent.Position += v;
      Parent.Size = new SizeF(width, height);
    }
    private void ResizeTopMiddle(Vector2 v)
    {
      v.X = 0;
      float height = Parent.Size.Height - v.Y;
      if (height < MIN_SIZE)
        return;
      Parent.Position += v;
      Parent.Size = new SizeF(Parent.Size.Width, height);
    }
    private void ResizeTopRight(Vector2 v)
    {
      float width = Parent.Size.Width + v.X;
      float height = Parent.Size.Height - v.Y;
      if (width < MIN_SIZE)
        return;
      if (height < MIN_SIZE)
        return;
      v.X = 0;
      Parent.Position += v;
      Parent.Size = new SizeF(width, height);
    }
    private void ResizeMiddleLeft(Vector2 v)
    {
      v.Y = 0;
      float width = Parent.Size.Width - v.X;
      if (width < MIN_SIZE)
        return;
      Parent.Position += v;
      Parent.Size = new SizeF(width, Parent.Size.Height);
    }
    private void ResizeMiddleRight(Vector2 v)
    {
      v.Y = 0;
      float width = Parent.Size.Width + v.X;
      if (width < MIN_SIZE)
        return;
      Parent.Size = new SizeF(width, Parent.Size.Height);
    }
    private void ResizeBottomLeft(Vector2 v)
    {
      float width = Parent.Size.Width - v.X;
      float height = Parent.Size.Height + v.Y;
      if (width < MIN_SIZE)
        return;
      if (height < MIN_SIZE)
        return;
      v.Y = 0;
      Parent.Position += v;
      Parent.Size = new SizeF(width, height);
    }
    private void ResizeBottomMiddle(Vector2 v)
    {
      float height = Parent.Size.Height + v.Y;
      if (height < MIN_SIZE)
        return;
      Parent.Size = new SizeF(Parent.Size.Width, height);
    }
    private void ResizeBottomRight(Vector2 v)
    {
      float width = Parent.Size.Width + v.X;
      float height = Parent.Size.Height + v.Y;
      if (width < MIN_SIZE)
        return;
      if (height < MIN_SIZE)
        return;
      Parent.Size = new SizeF(width, height);
    }
  }
}
