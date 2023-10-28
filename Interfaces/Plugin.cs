using System.Drawing;
using System.Reflection;
using System.Web;


namespace Core
{

//#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

  /// <summary>
  /// New Plugin creation notes
  ///   1. Don't forget to use the mFlowsCriticalSection lock when finding a flow in the 'Flows' list This list could also be accessed by a user saving a flow with the 'FlowGoLive' option set
  /// </summary>


  public class Plugin
  {
    /// <summary>
    /// A List of functions, the functions all have a dictionary of parameters, and a dictionary of results
    /// </summary>
    public List<Function> Functions = new(16);
    
    /// <summary>
    /// Any constants the plugin would want to include in the flow
    /// </summary>
    public Dictionary<string, Variable> Constants = new(16);

    /// <summary>
    /// These are for global values for the plugin
    /// </summary>
    public List<Setting> Settings = new(16);

    /// <summary>
    /// The actual DLL that is loaded at runtime
    /// </summary>
    public Assembly? PluginAssembly;

    /// <summary>
    /// The name of the plugin, defaults to the name of the class, but can be overriden to name it something different
    /// </summary>
    public string Name = "";

    /// <summary>
    /// Defines which order plugins should be started, lower number starts earlier, multiple plugins with the same StartPriority the start up order is undefined (random) All plugins by default have the latest start order. int.MaxValue
    /// I put 1000 between each plugin's priority which will give other plugins some room to set a priority higher or lower than other plugins
    /// </summary>
    public int StartPriority = int.MaxValue;
    /// <summary>
    /// Information so the flow engine knows what flow to start based on the plugin that is requesting a flow to start
    /// </summary>
    public PARMS FlowStartCommands = new PARMS();
    public PARM_VARS FlowStartCommandsParmVars = new PARM_VARS();

    /// <summary>
    /// The flows that this plugin could start, this is used only by the Flow Engine
    /// </summary>
    protected List<Core.Flow> Flows = new();
    protected object mFlowsCriticalSection = new object();

    public void FlowAdd(Core.Flow flow)
    {
      lock (mFlowsCriticalSection)
      {

        flow.FileName = flow.FileName.Replace('\\', '/'); //Doesn't seem to be any way to get C# to use only forward slashes everywhere, makes it difficult to do file name string compares.
        FlowRemove(flow.FileName); //If a flow with the same file name is loaded, remove it so we can add the new one.

        Flows.Add(flow);
        Global.Write("Flow Added - " + flow.FileName);
      }
    }

    public void FlowRemove(string fileName)
    {
      lock (mFlowsCriticalSection)
      {
        for (int x = 0; x < Flows.Count; x++)
        {
          if (Flows[x].FileName == fileName)
          {
            Global.Write("Flow Removed - " + fileName);
            Flows.RemoveAt(x);
            break;
          }
        }
      }
    }

    /// <summary>
    /// Used in the Flow Engine Designer, allows users to see what variables will be in the flow when it starts.
    /// Not all variables will be showable, like in the Http plugin if it is a POST with JSON the designer won't know what data will appear.
    /// The designer will have the ability to include sample data when designing a flow, a JSON sample packet can be included
    /// </summary>
    public Dictionary<string, Variable> SampleVariables = new(4); 

    #region Settings (no need to look at most of the time)

    public virtual List<Setting> GetSettings()
    {
      return Settings;
    }

    public virtual Setting SettingAddOrUpdate(Setting s)
    {
      bool Found = false;
      for (int i = 0; i < Settings.Count; i++)
      {
        if (Settings[i].Key.ToUpper() == s.Key.ToUpper())
        {
          Settings[i].Value = s.Value;
          Settings[i].SubSettings = s.SubSettings;
          Found = true;
        }
      }

      if (Found == false)
      {
        Settings.Add(s);
      }
      return s;
    }

    public virtual Setting SettingAddIfMissing(Setting setting)
    {
      Setting? s = SettingFind(setting.Key);
      if (s == null)
      {
        s = SettingAddOrUpdate(setting);
      }
      //If the settings were loaded from the configuration file, then we need to update the other properties
      s.GroupName = setting.GroupName;
      s.SubSettings = setting.SubSettings;
      s.Description = setting.Description;
      s.DropDownGroupName = setting.DropDownGroupName;
      return s;
    }

    public virtual Setting SettingAddIfMissing(string key, DATA_TYPE dataType)
    {
      Setting? s = SettingFind(key);
      if (s == null)
      {
        s = SettingAddOrUpdate(new Setting(key, dataType));
      }

      return s;
    }

    public virtual Setting SettingAddIfMissing(string key, Color Defaultvalue)
    {
      Setting? s = SettingFind(key);
      if (s == null)
      {
        s = SettingAddOrUpdate(new Setting(key, Defaultvalue));
      }

      return s;
    }

    public virtual Setting SettingAddIfMissing(string key, int Defaultvalue)
    {
      Setting? s = SettingFind(key);
      if (s == null)
      {
        s = SettingAddOrUpdate(new Setting(key, Defaultvalue));
      }

      return s;
    }

    public virtual Setting SettingAddIfMissing(string key, string Defaultvalue)
    {
      Setting? s = SettingFind(key);
      if (s == null)
      {
        s = SettingAddOrUpdate(new Setting(key, Defaultvalue));
      }

      return s;
    }

    public virtual Setting SettingAddIfMissing(string key, bool Defaultvalue)
    {
      Setting? s = SettingFind(key);
      if (s == null)
      {
        s = SettingAddOrUpdate(new Setting(key, Defaultvalue));
      }

      return s;
    }

    public virtual Setting SettingAddIfMissing(string key, decimal Defaultvalue)
    {
      Setting? s = SettingFind(key);
      if (s == null)
      {
        s = SettingAddOrUpdate(new Setting(key, Defaultvalue));
      }

      return s;
    }


    public virtual Setting? SettingFind(string key)
    {
      key = key.ToUpper();
      for (int i = 0; i < Settings.Count; i++)
      {
        if (Settings[i].Key.ToUpper() == key)
          return Settings[i];
      }
      return null;
    }

    public virtual bool SettingGetAsBoolean(string key)
    {
      Setting? setting = SettingFind(key);
      if (setting is null)
        return false;

      return (bool)setting.Value;
    }

    public virtual string SettingGetAsString(string key)
    {
      Setting? setting = SettingFind(key);
      if (setting is null)
        return "";

      return setting.Value.ToString()!;
    }
    public virtual Color SettingFindAsColor(string key)
    {
      key = key.ToUpper();
      for (int i = 0; i < Settings.Count; i++)
      {
        if (Settings[i].Key.ToUpper() == key)
        {
          Color c = (Color)Settings[i].Value;
          return c;
        }
      }
      return Color.Black;
    }

    protected string[] GetSettingsAsStringArray()
    {
      
      List<Setting> Settings = GetSettings();
      string[] strArray = new string[Settings.Count];

      for (int x = 0; x < Settings.Count; x++)
      {
        
        strArray[x] = Settings[x].ToString();
      }
      return strArray;
    }

    public virtual void SaveSettings()
    {
      Type t = this.GetType();
      Console.Write("Plugin Type - " + t.Name);
      string path = Options.GetFullPath(Options.PluginPath, t.Name + ".cfg");
      File.WriteAllLines(path, GetSettingsAsStringArray());
    }

    public virtual void LoadSettings()
    {
      
      Type t = this.GetType();
      try
      {
        string cfgPath = Options.GetFullPath(Options.PluginPath + "/" + t.Name + ".cfg");
        string[] Data = File.ReadAllLines(cfgPath);
        for (int x = 0; x < Data.Length; x++)
        {
          Setting s = new(Data[x]);
          Settings.Add(s);
        }
      }
      catch
      { }
    }

    #endregion

    /// <summary>
    /// Will initialize the plugin, read the settings, define the functions.
    /// </summary>
    public virtual void Init()
    {
      Name = GetType().Name;
      LoadSettings();
    }

    /// <summary>
    /// Will start up the plugin for runtime use, this is called after Init, will only be called once when the Flow Engine first starts running
    /// </summary>
    public virtual void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
    }

    public virtual void StopPlugin()
    {
    }



    public virtual void Dispose()
    {
    }

  }
}