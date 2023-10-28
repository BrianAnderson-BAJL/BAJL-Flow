using Core;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class cPluginManagerWrapper
  {
    

    public static bool PluginsLoaded = false;
    internal static Size HighlightSize = new Size(30, 30);
    internal static Vector2 HighlightCenterOffset = new Vector2(15, 15);

    public static void LoadPlugins(string FullPath)
    {
      Core.PluginManager.LoadPlugins(FullPath);

      PluginsLoaded = true;
    }

    public static void CreateAllGrphics()
    {
      for (int x = 0; x < Core.PluginManager.Plugins.Count; x++)
      {
        for (int y = 0; y < Core.PluginManager.Plugins[x].Functions.Count; y++)
        {
          CreateGraphic(Core.PluginManager.Plugins[x], Core.PluginManager.Plugins[x].Functions[y]);
        }
      }

      if (File.Exists(cOptions.GetFullPath(cOptions.PluginGraphicsPath) + "Selector.png") == true)
        return;

      using (Bitmap b = new Bitmap(220, 200))
      {
        float i = b.HorizontalResolution;
        //b.SetResolution(600, 600);
        using (Graphics g = Graphics.FromImage(b))
        {
          g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

          Pen p = new Pen(Color.Yellow);
          p.Width = 3;
          g.Clear(Color.Transparent);
          g.DrawRoundedRectangle(p, new Rectangle(2, 2, 215, 195), 10);
        }
        b.Save(cOptions.GetFullPath(cOptions.PluginGraphicsPath) + "Selector.png", ImageFormat.Png);
      }

    }



    public static void CreateGraphic(Core.Plugin plugin, Core.Function function)
    {
      if (File.Exists(cOptions.GetFullPath(cOptions.PluginGraphicsPath) + plugin.Name + "." + function.Name + ".png") == true)
          return;

      Color backgroundColor = Color.Transparent;
      Color borderColor = Color.Black;
      Color fontColor = Color.White;
      Setting? s = plugin.SettingFind("BackgroundColor");
      if (s != null)
        backgroundColor = (Color)s.Value;

      s = plugin.SettingFind("BorderColor");
      if (s != null)
        borderColor = (Color)s.Value;
      s = plugin.SettingFind("FontColor");
      if (s != null)
        fontColor = (Color)s.Value;

      int height = 100 + (function.Outputs.Count * (int)Output.OUTPUT_OFFSET.Y);
      using (Bitmap b = new Bitmap(200, height))
      {
        float i = b.HorizontalResolution;
        //b.SetResolution(600, 600);
        using (Graphics g = Graphics.FromImage(b))
        {
          g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

          Pen p = new Pen(borderColor);
          p.Width = 3;
          g.Clear(backgroundColor);
          Brush brush = new SolidBrush(borderColor);
          g.FillRoundedRectangle(brush, new Rectangle(2, 2, 197, height - 3), 10);
          //g.DrawRoundedRectangle(p, new Rectangle(2, 2, 197, 177), 10);
          using (Font font1 = new Font("Arial", 20, FontStyle.Regular, GraphicsUnit.Pixel))
          {

            StringFormat f = new StringFormat();
            f.LineAlignment = StringAlignment.Center;
            f.Alignment = StringAlignment.Center;

            brush = new SolidBrush(fontColor);

            g.DrawString(plugin.Name + "." + function.Name, font1, brush, new RectangleF(4, 5, 190, 45), f);

            //Draw Input label
            if (function.Input != null)
            {
              f.Alignment = StringAlignment.Near;
              Vector2 pos = function.Input.Offset;
              RectangleF r = new RectangleF(pos.X + HighlightSize.Width, pos.Y, 170, HighlightSize.Height);
              g.DrawString(function.Input.Label, font1, brush, r, f);
            }

            //Draw Output labels
            f.Alignment = StringAlignment.Far;
            for (int x = 0; x < function.Outputs.Count; x++)
            {
              Vector2 pos = function.Outputs[x].Offset;
              RectangleF r = new RectangleF(pos.X - 170, pos.Y, 170, HighlightSize.Height);
              g.DrawString(function.Outputs[x].Label, font1, brush, r, f);
            }
          }
        }
        b.Save(cOptions.GetFullPath(cOptions.PluginGraphicsPath) + plugin.Name + "." + function.Name + ".png", ImageFormat.Png);
      }

    }


  }
}
