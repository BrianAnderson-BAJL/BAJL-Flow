using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  public enum FORM_MODE
  {
    Add,
    Edit,
    Delete,
    ReadOnly,
  }

  public enum FILE_MODE
  {
    Select,
    Open,
    Save,
  }

  internal class Global
  {

    private static Bitmap junk = new Bitmap(1, 1);
    public static Bitmap HighlightBlack = junk;
    public static Bitmap HighlightYellow = junk;
    public static Bitmap HighlightRed = junk;
    public static Bitmap HighlightGreen = junk;
    public static Bitmap SelectorStep = junk;
    public static Bitmap ExecutingCurrentStep = junk;

    public static string EXTRA_VALUE_IMAGE = "image";

    public static cLayout Layout = new cLayout();
    public static frmMain? FormMain = null;
    //public static Size InputOutputConnectorSize = new Size(30,30);

    public static void LoadStaticImages()
    {
      HighlightBlack = new Bitmap(cOptions.GetFullPath(cOptions.StaticGraphicsPath) + "HighlightBlack_30.png");
      HighlightYellow = new Bitmap(cOptions.GetFullPath(cOptions.StaticGraphicsPath) + "HighlightYellow_30.png");
      HighlightRed = new Bitmap(cOptions.GetFullPath(cOptions.StaticGraphicsPath) + "HighlightRed_30.png");
      HighlightGreen = new Bitmap(cOptions.GetFullPath(cOptions.StaticGraphicsPath) + "HighlightGreen_30.png");
      SelectorStep = new Bitmap(cOptions.GetFullPath(cOptions.StaticGraphicsPath) + "Selector.png");
      ExecutingCurrentStep = new Bitmap(cOptions.GetFullPath(cOptions.StaticGraphicsPath) + "ExecutingCurrentStep.png");
    }

    public static void CatchAssemblyResolve()
    {
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }
    private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
      string baseName = args.Name.Substring(0, args.Name.IndexOf(","));
      if (baseName == "FlowEngineDesigner.resources")
      {
        return null;
      }
      else
      {
        string assemblyFileName = baseName + ".dll";
        assemblyFileName = AppDomain.CurrentDomain.BaseDirectory + assemblyFileName;
        return Assembly.LoadFile(assemblyFileName);
      }
    }

    public static Form? FindOpenFormByTitleText(string titleText)
    {
      for (int x = 0; x < Application.OpenForms.Count; x++)
      {
        if (Application.OpenForms[x].Text == titleText)
        {
          return Application.OpenForms[x];
        }
      }
      return null;
    }
    public static Form? FindOpenFormByName(string name)
    {
      for (int x = 0; x < Application.OpenForms.Count; x++)
      {
        if (Application.OpenForms[x].Name == name)
        {
          return Application.OpenForms[x];
        }
      }
      return null;
    }

    public static DateTime CurrentDateTime //TODO: Need to get time from server and convert it to local time
    {
      get {return DateTime.UtcNow;} 
    }

    public static void ComboBoxSetIndex(ComboBox comboBox, string Text)
    {
      for (int x = 0; x < comboBox.Items.Count; x++)
      {
        if (comboBox.Items[x].ToString() == Text)
        {
          comboBox.SelectedIndex = x;
          return;
        }
      }
    }

    public static TreeNode? TreeViewSelectNodeByText(TreeNodeCollection collection, string Text)
    {
      foreach (TreeNode node in collection)
      {
        if (node.Text == Text)
        {
          return node;
        }
        else
          return Global.TreeViewSelectNodeByText(node.Nodes, Text);
      }
      return null;
    }


    /// <summary>
    /// There are no built in functions for math with points, had to create soime
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static Point Add(Point p1, Vector2 p2)
    {
      return new Point(p1.X + (int)p2.X, p1.Y + (int)p2.Y);
    }
    public static Point Add(Point p1, Point p2)
    {
      return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }
    public static Point Subtract(Point p1, Vector2 p2)
    {
      return new Point(p1.X - (int)p2.X, p1.Y - (int)p2.Y);
    }

    public static Point Subtract(Point p1, Point p2)
    {
      return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }
    public static Point Divide(Point p1, float devisor)
    {
      return new Point((int)(p1.X / devisor), (int)(p1.Y / devisor));
    }
    public static Size Divide(Size s, float devisor)
    {
      return new Size((int)(s.Width / devisor), (int)(s.Height / devisor));
    }
    public static Size Multiply(Size s, float multiplier)
    {
      return new Size((int)(s.Width * multiplier), (int)(s.Height * multiplier));
    }
    public static bool Inside(Vector2 p1, Vector2 PointContainer, Size SizeContainer)
    {
      return Global.Inside(p1.X, p1.Y, PointContainer, SizeContainer);
    }

    public static bool Inside(float x, float y, Vector2 PointContainer, Size SizeContainer)
    {
      if (x >= PointContainer.X && y >= PointContainer.Y)
      {
        if (x <= (PointContainer.X + SizeContainer.Width) && y <= (PointContainer.Y + SizeContainer.Height))
        {
          return true;
        }
      }
      return false;
    }

    //public static Point ConvertToPoint(Vector2 Pos)
    //{
    //  return new Point((int)Pos.X, (int)Pos.Y);
    //}
    

  }

  public static class StringExtension
  {
    public static int? FindEndingNumber(this string text)
    {
      string foundNumbers = "";
      for (int x = text.Length - 1; x >= 0; x--)
      {
        if ("0123456789".Contains(text[x]) == true)
        {
          foundNumbers = text[x] + foundNumbers;
        }
        else
        {
          break;
        }
      }
      if (foundNumbers.Length > 0)
        if (int.TryParse(foundNumbers, out int val) == true)
          return val;
        else
          return null;
      else
        return null;
    }
  }

  public static class Vector2Extension
  {
    public static Point ToPoint(this Vector2 vec)
    {
      return new Point((int)vec.X, (int)vec.Y);
    }
  }
  public static class PointExtension
  {
    public static Vector2 ToVector2(this Point pos)
    {
      return new Vector2(pos.X, pos.Y);
    }

    public static Point Add(this Point pos, Point pos2)
    {
      return new Point(pos.X + pos2.X, pos.Y + pos2.Y);
    }
  }

  public static class RectangleExtension
  {
    public static bool Contains(this Rectangle r, Vector2 v)
    {
      return r.Contains((int)v.X, (int)v.Y);
    }
  }

  public static class GlobalExtension
  {


    public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
      int diameter = radius * 2;
      Size size = new(diameter, diameter);
      Rectangle arc = new(bounds.Location, size);
      GraphicsPath path = new();
      path.FillMode = FillMode.Winding;
      if (radius == 0)
      {
        path.AddRectangle(bounds);
        return path;
      }

      // top left arc  
      path.AddArc(arc, 180, 90);

      // top right arc  
      arc.X = bounds.Right - diameter;
      path.AddArc(arc, 270, 90);

      // bottom right arc  
      arc.Y = bounds.Bottom - diameter;
      path.AddArc(arc, 0, 90);

      // bottom left arc 
      arc.X = bounds.Left;
      path.AddArc(arc, 90, 90);

      path.CloseFigure();
      return path;
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle b, int r, bool fill = false)
    {
      var path = new GraphicsPath();
      var r2 = (int)r / 2;
      var fix = fill ? 1 : 0;

      b.Location = new Point(b.X - 1, b.Y - 1);
      if (!fill)
        b.Size = new Size(b.Width - 1, b.Height - 1);

      path.AddArc(b.Left, b.Top, r, r, 180, 90);
      path.AddLine(b.Left + r2, b.Top, b.Right - r2 - fix, b.Top);

      path.AddArc(b.Right - r - fix, b.Top, r, r, 270, 90);
      path.AddLine(b.Right, b.Top + r2, b.Right, b.Bottom - r2);

      path.AddArc(b.Right - r - fix, b.Bottom - r - fix, r, r, 0, 90);
      path.AddLine(b.Right - r2, b.Bottom, b.Left + r2, b.Bottom);

      path.AddArc(b.Left, b.Bottom - r - fix, r, r, 90, 90);
      path.AddLine(b.Left, b.Bottom - r2, b.Left, b.Top + r2);

      return path;
    }
    public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
    {
      if (graphics is null)
        throw new ArgumentNullException("graphics");
      if (pen is null)
        throw new ArgumentNullException("pen");

      using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
      {
        graphics.DrawPath(pen, path);
      }
    }

    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
    {
      if (graphics is null)
        throw new ArgumentNullException("graphics");
      if (brush is null)
        throw new ArgumentNullException("brush");

      using (GraphicsPath path = CreateRoundedRectangle(bounds, cornerRadius, true))
      {
        
        graphics.FillPath(brush, path);
      }
    }

    
  }

}
