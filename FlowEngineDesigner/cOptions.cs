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
    public static int FlowUserHistoryMax = 0;
    public static string BaseNewVariableName = "var";
    public static Color CommentColorBackgroundDefault = Color.FromArgb(255, 188, 255, 189);
    public static Color CommentColorTextDefault = Color.Black;
    public static int LinkLineSelectDistance = 15;
    public static bool AdministrationAutoConnect = true;
    public static string AdministrationLastFilePath = "/Http";
    public static bool AdministrationDebugAlways = true;

    public static string AdministrationPrivateKey = "HJQrOHUOn0q/vkQTbzGcL9U/cGW4rTDFH/JF9Mtg477R+1a9ev/I/RybinIAFlr7eYM/ilQtgDMEth8H0CnroQ==";

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
