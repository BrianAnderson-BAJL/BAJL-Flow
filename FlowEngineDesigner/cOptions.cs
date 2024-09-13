using FlowEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static FlowEngineCore.Options;

namespace FlowEngineDesigner
{
  internal class cOptions
  {
    //public static string PluginPath = "./Plugins/";
    //public static string PluginGraphicsPath = "./Plugins/Graphics/";
    //public static string PluginStaticGraphicsPath = "./Plugins/StaticGraphics/";
    //public static int FlowUserHistoryMax = 0;
    //public static string BaseNewVariableName = "var";
    //public static Color CommentColorBackgroundDefault = Color.FromArgb(255, 188, 255, 189);
    //public static Color CommentColorTextDefault = Color.Black;
    public static int LinkLineSelectDistance = 15;
    //public static bool AdministrationAutoConnect = true;
    //public static string AdministrationLastFilePath = "/Http";
    //public static bool AdministrationDebugAlways = true;

    private static Settings mSettings = new Settings();
    //public static string ServerPrivateKey = "HJQrOHUOn0q/vkQTbzGcL9U/cGW4rTDFH/JF9Mtg477R+1a9ev/I/RybinIAFlr7eYM/ilQtgDMEth8H0CnroQ==";

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

    public static string PluginGraphicsPath
    {
      get { return "./Plugins/Graphics/"; }
    }

    public static string StaticGraphicsPath
    {
      get { return "./Plugins/StaticGraphics/"; }
    }
    public static string PluginPath
    {
      get { return "./Plugins"; }
    }

    public static Color CommentColorBackgroundDefault
    {
      get { return Color.FromArgb(255, 188, 255, 189); }
    }

    public static Color CommentColorTextDefault
    {
      get { return Color.Black; }
    }

    public static string BaseNewVariableName
    {
      get { return "var"; }
    }

    public static int FlowUserHistoryMax
    {
      get { return 10; }
    }

    public static void CreateAndLoadSettings()
    {
      mSettings.SettingAdd(new Setting("PluginPath", "./Plugins"));
      mSettings.SettingAdd(new Setting("PluginGraphicsPath", "./Plugins/Graphics/"));

      mSettings.SettingAdd(new Setting("BaseNewVariableName", "var"));
      mSettings.SettingAdd(new Setting("PluginStaticGraphicsPath", "./Plugins/StaticGraphics/"));
      mSettings.SettingAdd(new Setting("CommentColorBackgroundDefault", Color.FromArgb(255, 188, 255, 189)));
      mSettings.SettingAdd(new Setting("CommentColorTextDefault", Color.Black));


      //mSettings.SettingAdd(new Setting("PrivateKey", "Server", "HJQrOHUOn0q/vkQTbzGcL9U/cGW4rTDFH/JF9Mtg477R+1a9ev/I/RybinIAFlr7eYM/ilQtgDMEth8H0CnroQ=="));
      //mSettings.SettingAdd(new Setting("AutoConnect", "Server", true));
      //mSettings.SettingAdd(new Setting("LastFilePath", "Server", "/Http"));
      //mSettings.SettingAdd(new Setting("DebugAlways", "Server", true));


      mSettings.LoadSettingsFromFile(SettingsPath);
      //mSettings.SaveSettings(SettingsPath); //I save the settings here again during development to write out any new <Setting> blocks

      //ServerPrivateKey = mSettings.SettingGetAsString("PrivateKey");
      
    }
  }
}
