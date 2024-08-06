using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class Settings
  {
    private List<Setting> mSettings = new(16);
    private string mFileName = "";

    public virtual List<Setting> SettingsList
    {
      get { return mSettings; }
      
    }

    public virtual void SettingUpdate(Setting s)
    {
      for (int i = 0; i < mSettings.Count; i++)
      {
        if (mSettings[i].Key.ToUpper() == s.Key.ToUpper())
        {
          mSettings[i].Value = s.Value;
          mSettings[i].SubSettingsUpdate(s.SubSettings);
        }
      }
    }

    public Setting SettingAdd(Setting setting)
    {
      mSettings.Add(setting);
      return setting;
    }

    public virtual Setting? SettingFind(string key)
    {
      key = key.ToUpper();
      for (int i = 0; i < mSettings.Count; i++)
      {
        if (mSettings[i].Key.ToUpper() == key)
          return mSettings[i];
      }
      return null;
    }

    public virtual bool SettingGetAsBoolean(string key)
    {
      Setting? setting = SettingFind(key);
      if (setting is null)
        throw new ArgumentException($"key [{key}] was not found in settings [{mFileName}]");

      return (bool)setting.Value;
    }

    public virtual string SettingGetAsString(string key)
    {
      Setting? setting = SettingFind(key);
      if (setting is null || setting.Value is null)
        throw new ArgumentException($"key [{key}] was not found in settings [{mFileName}]");

      return setting.Value.ToString()!;
    }

    public virtual int SettingGetAsInt(string key)
    {
      Setting? setting = SettingFind(key);
      if (setting is null || setting.Value is null || (setting.Value is not long && setting.Value is not int))
        throw new ArgumentException($"key [{key}] was not found in settings [{mFileName}]");

      return (int)setting.Value;
    }

    public virtual long SettingGetAsLong(string key)
    {
      Setting? setting = SettingFind(key);
      if (setting is null || setting.Value is null || (setting.Value is not long && setting.Value is not int))
        throw new ArgumentException($"key [{key}] was not found in settings [{mFileName}]");

      return (long)setting.Value;
    }

    public void SaveSettings()
    {
      SaveSettings(mFileName);
    }

    public string GetXml()
    {
      Xml xml = new Xml();
      xml.WriteMemoryNew();
      xml.WriteTagStart("Settings");
      for (int x = 0; x < mSettings.Count; x++)
      {
        xml.WriteTagContentsBare(mSettings[x].ToXml());
      }
      xml.WriteTagEnd("Settings");
      return xml.ReadMemory();
    }

    public virtual void SaveSettings(string path)
    {
      mFileName = path;
      Xml xml = new Xml();
      xml.WriteFileNew(path);
      xml.WriteTagStart("Settings");
      for (int x = 0; x < mSettings.Count; x++)
      {
        xml.WriteTagContentsBare(mSettings[x].ToXml());
      }
      xml.WriteTagEnd("Settings");
      xml.WriteFileClose();
    }

    public virtual void LoadSettingsFromFile(string settingsPath, Dictionary<string, object>? GlobalPluginValues = null)
    {
      if (settingsPath.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) == true)
      {
        settingsPath = settingsPath.Substring(0, settingsPath.Length - 4);
        settingsPath = settingsPath + ".xml";
      }

      mFileName = settingsPath;
      try
      {
        string cfgPath = Options.GetFullPath(settingsPath);
        Xml xml = new Xml();
        string data = xml.FileRead(cfgPath);
        LoadSettingsFromXml(data);
      }
      catch (Exception e)
      {
        Global.WriteToConsoleDebug($"Error loading settings from [{settingsPath}], Exception [{e.Message}]", LOG_TYPE.ERR);
      }

    }

    public virtual void LoadSettingsFromXml(string xml)
    {
      try
      {
        do
        {
          string settingStr = Xml.GetXMLChunk(ref xml, "Setting");
          if (settingStr.Length <= 0)
            break;

          Setting s = new(settingStr);
          if (this.SettingFind(s.Key) is null)
            this.SettingAdd(s);
          else
            this.SettingUpdate(s);
        } while (xml.Length > 0);

      }
      catch (Exception e)
      {
        Global.WriteToConsoleDebug($"Error loading settings from XML, Exception [{e.Message}]", LOG_TYPE.ERR);
      }
    }


  }
}
