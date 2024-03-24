using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class Options
  {
    public enum SERVER_TYPE
    {
      Development,
      Production,
    }
    public const int SettingsFileVersionExpected = 8; //Increment this when you make changes to the settings options, the settings.xml file will be recreated with the new values.
    public static int SettingsFileVersion = 0;
    public static string SettingsPath = "./settings.xml";
    public static string PluginPath = @"C:\Users\brian\source\repos\FlowEngine\FlowEngineDesigner\bin\Debug\net6.0-windows\Plugins";  //"./Plugins/";
    public static string PluginGraphicsPath = "./Plugins/Graphics/";
    public static string PluginStaticGraphicsPath = "./Plugins/StaticGraphics/";
    public static string FlowPath = @"C:\Users\brian\Documents\Flows";
    public static bool FlowPathAllowSubDirectories = true;
    public static string UserPath = "./users.xml";
    public static string SecurityProfilesPath = "./securityProfiles.xml";
    public static string TlsCertFileNamePath = "C:\\GameDev\\certkey.pem";
    public static string TlsCertPassword = "";
    public static bool FocusOnMouseEnter = true;
    public static int FlowUserHistoryMax = 10;
    public static bool IgnoreEndingForwardSlash = true;
    public static SERVER_TYPE ServerType = SERVER_TYPE.Development;
    public static string TenantPath = "./tenants.xml";
    /// <summary>
    /// The Private key every user must have to get access to this server instance, this value is global to all users, it is used in conjunction with the users LoginId and password
    /// This value provides more security over just a basic loginid and password.
    /// </summary>
    public static string AdministrationPrivateKey = "";

    /// <summary>
    /// How many duplicate user LoginIds will the system check trying to find an unused loginid. (i.e. brian, brian1, brian2, ... to this max number)
    /// </summary>
    public static int AdministrationReadPacketTimeoutInMs = 5000;
    public static int AdministrationUserMaxLoginIdCheck = 100;
    public static int AdministrationUserMaxLoginAttempts = 3;  //0 = Infinate
    public static int AdministrationUserLockOutMinutes = 15;   //0 = No Lockout
    public static int AdministrationSaltSize = 32;
    public static int AdministrationHashSize = 32;
    public static int AdministrationHashInterations = 20000;
    public static int AdministrationPortNumber = 7000;
    public static int AdministrationUserSessionKeyTimeoutInMinutes = 720; //12 hours default

    /// <summary>
    /// The settings.xml file path could be defined in the args, so need to preparse some values
    /// </summary>
    /// <param name="args"></param>
    public static void PreParseArgs(string[] args)
    {
      Global.Write("PreParseArgs...");

      for (int x = 0; x < args.Length; x++)
      {
        Global.Write($"PreParseArgs arg[{x}] = [{args[x]}]");
        if (args[x].StartsWith("settings=") == true)
        {
          string val = GetArgValue(args[x]);
          if (val.ToLower() == "create")
          {
            SaveSettings();
            Global.Write($"Default settings file create!...Settings path = [{SettingsPath}]");
          }
          else
          {
            SettingsPath = GetArgValue(args[x]);
            Global.Write($"PreParseArgs...Settings path = [{SettingsPath}]");
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
      Global.Write("ParseArgs...");

      for (int x = 0; x < args.Length; x++)
      {
        Global.Write($"ParseArgs arg[{x}] = [{args[x]}]");
        if (args[x].StartsWith("privatekey=") == true)
        {
          string val = GetArgValue(args[x]);
          if (val.ToLower() == "create")
          {
            AdministrationPrivateKey = SecureHasherV1.Hash(Guid.NewGuid().ToString());
            SaveSettings();
            Global.Write($"ParseArgs...private key created [{AdministrationPrivateKey}] and saved to [{SettingsPath}]");
          }
        }
      }
    }
    public static void LoadSettings()
    {
      Core.Xml xml = new Core.Xml();
      string settings = xml.FileRead(SettingsPath);
      settings = Xml.GetXMLChunk(ref settings, "Settings");
      SettingsFileVersion = Xml.GetXMLChunkAsInt(ref settings, "SettingsFileVersion");
      PluginPath = Xml.GetXMLChunk(ref settings, "PluginPath");
      PluginGraphicsPath = Xml.GetXMLChunk(ref settings, "PluginGraphicsPath");
      FlowPath = Xml.GetXMLChunk(ref settings, "FlowPath");
      FlowPathAllowSubDirectories = Xml.GetXMLChunkAsBool(ref settings, "FlowPathAllowSubDirectories", FlowPathAllowSubDirectories);
      FlowUserHistoryMax = Xml.GetXMLChunkAsInt(ref settings, "FlowUserHistoryMax");
      ServerType = Xml.GetXmlChunkAsEnum<SERVER_TYPE>(ref settings, "ServerType", SERVER_TYPE.Production);
      TlsCertFileNamePath = Xml.GetXMLChunk(ref settings, "TlsCertFileNamePath");
      TlsCertPassword = Xml.GetXMLChunk(ref settings, "TlsCertPassword");

      string adminXml = Xml.GetXMLChunk(ref settings, "Administration");
      AdministrationPrivateKey = Xml.GetXMLChunk(ref adminXml, "PrivateKey");
      AdministrationUserMaxLoginIdCheck = Xml.GetXMLChunkAsInt(ref adminXml, "UserMaxLoginIdCheck", AdministrationUserMaxLoginIdCheck);
      AdministrationReadPacketTimeoutInMs = Xml.GetXMLChunkAsInt(ref adminXml, "AdministrationReadPacketTimeoutInMs", AdministrationReadPacketTimeoutInMs);
      AdministrationUserMaxLoginAttempts = Xml.GetXMLChunkAsInt(ref adminXml, "UserMaxLoginAttempts", AdministrationUserMaxLoginAttempts);
      AdministrationUserLockOutMinutes = Xml.GetXMLChunkAsInt(ref adminXml, "UserLockOutMinutes", AdministrationUserLockOutMinutes);
      AdministrationSaltSize = Xml.GetXMLChunkAsInt(ref adminXml, "SaltSize", AdministrationSaltSize);
      AdministrationHashSize = Xml.GetXMLChunkAsInt(ref adminXml, "HashSize", AdministrationHashSize);
      AdministrationHashInterations = Xml.GetXMLChunkAsInt(ref adminXml, "HashInterations", AdministrationHashInterations);
      AdministrationPortNumber = Xml.GetXMLChunkAsInt(ref adminXml, "PortNumber", AdministrationPortNumber);
      AdministrationUserSessionKeyTimeoutInMinutes = Xml.GetXMLChunkAsInt(ref adminXml, "UserSessionKeyTimeoutInMinutes", AdministrationUserSessionKeyTimeoutInMinutes);

      Global.Write($"Flow Engine ServerType running as [{ServerType}]");
      if (SettingsFileVersion != SettingsFileVersionExpected)
      {
        Global.Write($"SettingsFileVersion [{SettingsFileVersion}] has changed to [{SettingsFileVersionExpected}] Saving new settings.xml");
        SaveSettings();
      }
    }

    public static void SaveSettings()
    {
      Core.Xml xml = new Core.Xml();
      xml.WriteFileNew(SettingsPath);
      xml.WriteTagStart("Settings");
      xml.WriteTagAndContents("SettingsFileVersion", SettingsFileVersionExpected);
      xml.WriteTagAndContents("PluginPath", PluginPath);
      xml.WriteTagAndContents("PluginGraphicsPath", PluginGraphicsPath);
      xml.WriteTagAndContents("FlowPath", FlowPath);
      xml.WriteTagAndContents("FlowPathAllowSubDirectories", FlowPathAllowSubDirectories);
      xml.WriteTagAndContents("TlsCertFileNamePath", TlsCertFileNamePath);
      xml.WriteTagAndContents("TlsCertPassword", TlsCertPassword);
      xml.WriteTagAndContents("FlowUserHistoryMax", FlowUserHistoryMax);
      xml.WriteTagAndContents("UserPath", UserPath);
      xml.WriteTagAndContents("ServerType", ServerType);
      
      xml.WriteTagStart("Administration");
      xml.WriteTagAndContents("PrivateKey", AdministrationPrivateKey);
      xml.WriteTagAndContents("AdministrationReadPacketTimeoutInMs", AdministrationReadPacketTimeoutInMs);
      xml.WriteTagAndContents("UserMaxLoginIdCheck", AdministrationUserMaxLoginIdCheck);
      xml.WriteTagAndContents("UserMaxLoginAttempts", AdministrationUserMaxLoginAttempts);
      xml.WriteTagAndContents("UserLockOutMinutes", AdministrationUserLockOutMinutes);
      xml.WriteTagAndContents("SaltSize", AdministrationSaltSize);
      xml.WriteTagAndContents("HashSize", AdministrationHashSize);
      xml.WriteTagAndContents("HashInterations", AdministrationHashInterations);
      xml.WriteTagAndContents("PortNumber", AdministrationPortNumber);
      xml.WriteTagAndContents("UserSessionKeyTimeoutInMinutes", AdministrationUserSessionKeyTimeoutInMinutes);
      xml.WriteTagEnd("Administration");

      xml.WriteTagEnd("Settings");
      xml.WriteFileClose();
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
      string temp = fileName;
      if (fileName.Length > Options.FlowPath.Length)
      {
        temp = fileName.Substring(Options.FlowPath.Length);
      }
      return temp;
    }

    public static string FixUrl(string url)
    {
      if (Core.Options.IgnoreEndingForwardSlash == true)
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
