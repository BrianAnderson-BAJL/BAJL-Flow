using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class Options
  {
    public static string SettingsPath = "./settings.xml";
    public static string PluginPath = @"C:\Users\brian\source\repos\FlowEngine\FlowEngineDesigner\bin\Debug\net6.0-windows\Plugins";  //"./Plugins/";
    public static string PluginGraphicsPath = "./Plugins/Graphics/";
    public static string PluginStaticGraphicsPath = "./Plugins/StaticGraphics/";
    public static string FlowPath = @"C:\Users\brian\Documents";
    public static bool FlowPathAllowSubDirectories = true;
    public static string UserPath = "./users.xml";
    public static string SecurityProfilesPath = "./securityProfiles.xml";
    public static bool FocusOnMouseEnter = true;
    public static int FlowUserHistoryMax = 10;
    public static int ThreadPoolSize = 0;
    public static bool IgnoreEndingForwardSlash = true;

    /// <summary>
    /// The Private key every user must have to get access to this server instance, this value is global to all users, it is used in conjunction with the users LoginId and password
    /// This value provides more security over just a basic loginid and password.
    /// </summary>
    public static string AdministrationPrivateKey = "";

    /// <summary>
    /// How many duplicate user LoginIds will the system check trying to find an unused loginid. (i.e. brian, brian1, brian2, ... to this max number)
    /// </summary>
    public static int AdministrationUserMaxLoginIdCheck = 100;
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
        Global.Write(String.Format("PreParseArgs arg[{0}] = [{1}]", x, args[x]));
        if (args[x].StartsWith("settings=") == true)
        {
          string val = GetArgValue(args[x]);
          if (val.ToLower() == "create")
          {
            SaveSettings();
            Global.Write(String.Format("Default settings file create!...Settings path = [{0}]", SettingsPath));
          }
          else
          {
            SettingsPath = GetArgValue(args[x]);
            Global.Write(String.Format("PreParseArgs...Settings path = [{0}]", SettingsPath));
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
      Global.Write("PreParseArgs...");

      for (int x = 0; x < args.Length; x++)
      {
        Global.Write(String.Format("ParseArgs arg[{0}] = [{1}]", x, args[x]));
        if (args[x].StartsWith("privatekey=") == true)
        {
          string val = GetArgValue(args[x]);
          if (val.ToLower() == "create")
          {
            AdministrationPrivateKey = SecureHasherV1.Hash(Guid.NewGuid().ToString());
            SaveSettings();
            Global.Write(String.Format("ParseArgs...private key created [{0}] and saved to [{1}]", AdministrationPrivateKey, SettingsPath));
          }
        }
      }
    }
    public static void LoadSettings()
    {
      Core.Xml xml = new Core.Xml();
      string settings = xml.FileRead(SettingsPath);
      settings = Xml.GetXMLChunk(ref settings, "Settings");
      PluginPath = Xml.GetXMLChunk(ref settings, "PluginPath");
      PluginGraphicsPath = Xml.GetXMLChunk(ref settings, "PluginGraphicsPath");
      FlowPath = Xml.GetXMLChunk(ref settings, "FlowPath");
      FlowPathAllowSubDirectories = Xml.GetXMLChunkAsBool(ref settings, "FlowPathAllowSubDirectories", FlowPathAllowSubDirectories);
      FlowUserHistoryMax = Xml.GetXMLChunkAsInt(ref settings, "FlowUserHistoryMax");

      string adminXml = Xml.GetXMLChunk(ref settings, "Administration");
      AdministrationPrivateKey = Xml.GetXMLChunk(ref adminXml, "AdministrationPrivateKey");
      AdministrationUserMaxLoginIdCheck = Xml.GetXMLChunkAsInt(ref adminXml, "AdministrationUserMaxLoginIdCheck", AdministrationUserMaxLoginIdCheck);
      AdministrationSaltSize = Xml.GetXMLChunkAsInt(ref adminXml, "AdministrationSaltSize", AdministrationSaltSize);
      AdministrationHashSize = Xml.GetXMLChunkAsInt(ref adminXml, "AdministrationHashSize", AdministrationHashSize);
      AdministrationHashInterations = Xml.GetXMLChunkAsInt(ref adminXml, "AdministrationHashInterations", AdministrationHashInterations);
      AdministrationPortNumber = Xml.GetXMLChunkAsInt(ref adminXml, "AdministrationPortNumber", AdministrationPortNumber);
      AdministrationUserSessionKeyTimeoutInMinutes = Xml.GetXMLChunkAsInt(ref adminXml, "AdministrationUserSessionKeyTimeoutInMinutes", AdministrationUserSessionKeyTimeoutInMinutes);
    }

    public static void SaveSettings()
    {
      Core.Xml xml = new Core.Xml();
      xml.WriteFileNew(SettingsPath);
      xml.WriteTagStart("Settings");
      xml.WriteTagAndContents("PluginPath", PluginPath);
      xml.WriteTagAndContents("PluginGraphicsPath", PluginGraphicsPath);
      xml.WriteTagAndContents("FlowPath", FlowPath);
      xml.WriteTagAndContents("FlowPathAllowSubDirectories", FlowPathAllowSubDirectories);
      xml.WriteTagAndContents("FlowUserHistoryMax", FlowUserHistoryMax);
      xml.WriteTagAndContents("UserPath", UserPath);
      
      xml.WriteTagStart("Administration");
      xml.WriteTagAndContents("AdministrationPrivateKey", AdministrationPrivateKey);
      xml.WriteTagAndContents("AdministrationUserMaxLoginIdCheck", AdministrationUserMaxLoginIdCheck);
      xml.WriteTagAndContents("AdministrationSaltSize", AdministrationSaltSize);
      xml.WriteTagAndContents("AdministrationHashSize", AdministrationHashSize);
      xml.WriteTagAndContents("AdministrationHashInterations", AdministrationHashInterations);
      xml.WriteTagAndContents("AdministrationPortNumber", AdministrationPortNumber);
      xml.WriteTagAndContents("AdministrationUserSessionKeyTimeoutInMinutes", AdministrationUserSessionKeyTimeoutInMinutes);
      xml.WriteTagEnd("Administration");

      xml.WriteTagEnd("Settings");
      xml.WriteFileClose();
    }

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
