using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class cOptions
  {
    public static string PluginPath = "./Plugins/";
    public static string PluginGraphicsPath = "./Plugins/Graphics/";
    public static string PluginStaticGraphicsPath = "./Plugins/StaticGraphics/";
    public static bool FocusOnMouseEnter = false;
    public static int FlowUserHistoryMax = 0;
    public static bool HighlightStepsOnExecution = true;
    public static string BaseNewVariableName = "var";
    public static Color CommentColorBackgroundDefault = Color.FromArgb(255, 188, 255, 189);
    public static Color CommentColorTextDefault = Color.Black;
    public static int LinkLineSelectDistance = 15;
    public static bool AdministrationAutoConnect = true;
    public static string AdministrationLastFilePath = "/Http";

    public static string AdministrationPrivateKey = "BAJL_HASH~V1~20000~E13iVFBdofhxSbrk2jUGoTRGVvs/APQBsJZkDjfBj4sMj1PMwfbUjg/gmAR4faaLyKGtZ6MYBKxe/hcY3jVh+A==";

    public static string GetFullPath(string path)
    {
      string FullPath = path;
      if (FullPath[0] == '.')
      {
        string codeBase = Assembly.GetExecutingAssembly().Location;
        UriBuilder uri = new UriBuilder(codeBase);
        path = Uri.UnescapeDataString(uri.Path);
        FullPath = FullPath.Replace(".", Path.GetDirectoryName(path));

      }
      return FullPath;
    }
  }
}
