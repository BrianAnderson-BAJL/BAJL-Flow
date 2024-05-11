using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class Options
  {
    public enum SERVER_TYPE
    {
      Development,
      Production,
    }

    private static Settings mSettings = new();

    public static Settings GetSettings
    {
      get {return mSettings;}
    }

    //public const int SettingsFileVersionExpected = 10; //Increment this when you make changes to the settings options, the settings.xml file will be recreated with the new values.
    //public static int SettingsFileVersion = 0;
    public static string SettingsPath = "./settings_newFormat.xml";
    //public static string PluginPath = @"C:\Users\brian\source\repos\FlowEngine\FlowEngineDesigner\bin\Debug\net6.0-windows\Plugins";  //"./Plugins/";
    //public static string PluginGraphicsPath = "./Plugins/Graphics/";
    //public static string PluginStaticGraphicsPath = "./Plugins/StaticGraphics/";
    //public static string FlowPath = @"C:\Users\brian\Documents\Flows";
    //public static bool FlowPathAllowSubDirectories = true;
    //public static string UserPath = "./users.xml";
    //public static string SecurityProfilesPath = "./securityProfiles.xml";
    //public static string TlsCertFileNamePath = "C:\\GameDev\\certkey.pem";
    //public static string TlsCertPassword = "";
    //public static bool FocusOnMouseEnter = true;
    //public static int FlowUserHistoryMax = 10;
    //public static bool IgnoreEndingForwardSlash = true;
    //public static SERVER_TYPE ServerType = SERVER_TYPE.Development;
    //public static string TenantPath = "./tenants.xml";
    //public static long ThreadNameStartingNumber = 10000;
    /// <summary>
    /// The Private key every user must have to get access to this server instance, this value is global to all users, it is used in conjunction with the users LoginId and password
    /// This value provides more security over just a basic loginid and password.
    /// </summary>
    //public static string AdministrationPrivateKey = "";

    /// <summary>
    /// How many duplicate user LoginIds will the system check trying to find an unused loginid. (i.e. brian, brian1, brian2, ... to this max number)
    /// </summary>
    //public static int AdministrationReadPacketTimeoutInMs = 5000;
    //public static int AdministrationUserMaxLoginIdCheck = 100;
    //public static int AdministrationUserMaxLoginAttempts = 3;  //0 = Infinate
    //public static int AdministrationUserLockOutMinutes = 15;   //0 = No Lockout
    //public static int AdministrationSaltSize = 32;
    //public static int AdministrationHashSize = 32;
    //public static int AdministrationHashInterations = 20000;
    //public static int AdministrationPortNumber = 7000;
    //public static int AdministrationUserSessionKeyTimeoutInMinutes = 720; //12 hours default
    //public static int AdministrationSessionSize = 64;

    /// <summary>
    /// The settings.xml file path could be defined in the args, so need to preparse some values
    /// </summary>
    /// <param name="args"></param>
    public static void PreParseArgs(string[] args)
    {
      Global.WriteToConsoleDebug("PreParseArgs...");

      for (int x = 0; x < args.Length; x++)
      {
        Global.WriteToConsoleDebug($"PreParseArgs arg[{x}] = [{args[x]}]");
        if (args[x].StartsWith("settings=") == true)
        {
          string val = GetArgValue(args[x]);
          if (val.ToLower() == "create")
          {
            CreateAndLoadSettings();
            Global.WriteToConsoleDebug($"Default settings file create!...Settings path = [{SettingsPath}]");
          }
          else
          {
            SettingsPath = GetArgValue(args[x]);
            Global.WriteToConsoleDebug($"PreParseArgs...Settings path = [{SettingsPath}]");
          }
        }
        else if (args[x].StartsWith("?") == true || args[x].StartsWith("-?") == true || args[x].StartsWith("/?") == true || args[x].StartsWith("help") == true)
        {
          Console.WriteLine("");
          Console.WriteLine("Command line parameters:");
          Console.WriteLine("");
          Console.WriteLine("  Most parameters are in a key=value format with spaces between parameters");
          Console.WriteLine("");
          Console.WriteLine("  ?          - get this information that you are reading right now, obviously you figured this out, congratulations!");
          Console.WriteLine("  settings   - path to the settings.xml file or the key word 'create' to create a default settings.xml file");
          Console.WriteLine("  privatekey - only accepted value is 'create' which will create a new administrator private key");
          Console.WriteLine("");
          Console.WriteLine("");
          Console.WriteLine("");
          Console.WriteLine("");
          Environment.Exit(0); //When showing the help, we don't start the whole application, just show this and exit.
        }
      }
    }

    private static string GetArgValue(string arg)
    {
      int index = arg.IndexOf("="); //Don't use split, the parameter value could have an '=' in it
      if (index != -1)
      {
        return arg.Substring(index + 1);
      }
      else
      {
        return "";
      }
    }

    public static void ParseArgs(string[] args)
    {
      Global.WriteToConsoleDebug("ParseArgs...");

      for (int x = 0; x < args.Length; x++)
      {
        Global.WriteToConsoleDebug($"ParseArgs arg[{x}] = [{args[x]}]");
        if (args[x].StartsWith("privatekey=") == true)
        {
          string val = GetArgValue(args[x]);
          if (val.ToLower() == "create")
          {
            CreateAndLoadSettings();
            string key = SecureHasherV1.SessionIdCreate();
            mSettings.SettingUpdate(new Setting("PrivateKey", "Administration", key));
            mSettings.SaveSettings(SettingsPath);
            Global.WriteToConsoleDebug($"ParseArgs...private key created [{key}] and saved to [{SettingsPath}]");
            Environment.Exit(0); //We don't want to run the application, just get the private key and save it in the settings
          }
        }
      }
    }
    public static void CreateAndLoadSettings()
    {
      mSettings.SettingAdd(new Setting("PluginPath", "./Plugins"));
      mSettings.SettingAdd(new Setting("PluginGraphicsPath", "./Plugins/Graphics/"));
      mSettings.SettingAdd(new Setting("FlowPath", "./Flows"));
      mSettings.SettingAdd(new Setting("FlowPathAllowSubDirectories", true));
      mSettings.SettingAdd(new Setting("TlsCertFileNamePath", "./cert.pfx"));
      mSettings.SettingAdd(new Setting("TlsCertPassword", ""));
      mSettings.SettingAdd(new Setting("FlowUserHistoryMax", 10));
      mSettings.SettingAdd(new Setting("UserPath", "./users.xml"));
      mSettings.SettingAdd(new Setting("SecurityProfilesPath", "./securityProfiles.xml"));

      Setting setting = mSettings.SettingAdd(new Setting("ServerType", SERVER_TYPE.Development.ToString(), STRING_SUB_TYPE.DropDownList));
      setting.OptionAdd(SERVER_TYPE.Development.ToString());
      setting.OptionAdd(SERVER_TYPE.Production.ToString());
      mSettings.SettingAdd(new Setting("ThreadNameStartingNumber", 10000L));


      mSettings.SettingAdd(new Setting("PrivateKey", "Administration", ""));
      mSettings.SettingAdd(new Setting("ReadPacketTimeoutInMs", "Administration", 5000));
      mSettings.SettingAdd(new Setting("UserMaxLoginIdCheck", "Administration", 100));
      mSettings.SettingAdd(new Setting("UserMaxLoginAttempts", "Administration", 3));
      mSettings.SettingAdd(new Setting("UserLockOutMinutes", "Administration", 15));
      mSettings.SettingAdd(new Setting("SaltSize", "Administration", 32));
      mSettings.SettingAdd(new Setting("HashSize", "Administration", 32));
      mSettings.SettingAdd(new Setting("HashInterations", "Administration", 20000));
      mSettings.SettingAdd(new Setting("PortNumber", "Administration", 7000));
      mSettings.SettingAdd(new Setting("UserSessionKeyTimeoutInMinutes", "Administration", 720));
      mSettings.SettingAdd(new Setting("SessionSize", "Administration", 64));

      mSettings.LoadSettingsFromFile(SettingsPath);
      mSettings.SaveSettings(SettingsPath); //I save the settings here again during development to write out any new <Setting> blocks
    }


    public static string GetFullPath(string path, string fileName = "")
    {
      string FullPath = path;
      if (FullPath[0] == '.')
      {
        string? codeBase = Assembly.GetExecutingAssembly().Location;
        codeBase = Path.GetDirectoryName(codeBase);
        if (codeBase is null)
          return "";
        FullPath = codeBase + FullPath.Substring(1);
      }
      if (fileName != "")
      {
        if (FullPath.EndsWith("/") == false && FullPath.EndsWith("\\") == false)
        {
          if (fileName.StartsWith("/") == false && fileName.StartsWith("\\") == false)
            FullPath += "/";
        }
        FullPath += fileName;
      }

      //Global.Write($"Options.GetFullPath() - Original path [{path}], fixed new path [{FullPath}]");

      return FullPath;
    }

    /// <summary>
    /// Chop off the full path to the flow file name, just return the relative path
    ///  c:\users\brian\My documents\Flows\Test.flow    --->   \Test.flow
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetFlowFileNameRelativePath(string fileName)
    {
      string flowPath = Options.GetSettings.SettingGetAsString("FlowPath");
      string temp = fileName;
      if (fileName.Length > flowPath.Length)
      {
        temp = fileName.Substring(flowPath.Length);
      }
      return temp;
    }

    public static string FixUrl(string url)
    {
      if (true) //Core.Options.IgnoreEndingForwardSlash == 
      {
        if (url.Length > 0)
        {
          if (url[url.Length - 1] == '/')
            url = url.Substring(0, url.Length - 1);
        }
      }
      return url;
    }
  }
}
