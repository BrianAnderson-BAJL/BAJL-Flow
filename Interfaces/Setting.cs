using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Core
{
  public class Setting
  {
    public string GroupName = "";
    
    /// <summary>
    /// This is used to have specific settings when another drop down setting is selected
    /// </summary>
    public string DropDownGroupName = "";
    public string Key = "";
    public object Value = "";
    public string Description = "";
    public DATA_TYPE DataType = DATA_TYPE.String;
    public STRING_SUB_TYPE StringSubType = STRING_SUB_TYPE._None;
    private List<Setting> mSubSettings = new(0);
    private List<string>? mOptions = null; //For drop down list

    public ReadOnlyCollection<Setting> SubSettings
    {
      get
      {
        return mSubSettings.AsReadOnly();
      }
    }

    public ReadOnlyCollection<string>? Options
    {
      get
      {
        if (mOptions == null)
          return null;

        return mOptions.AsReadOnly();
      }
    }

    public Setting(DATA_TYPE dataType)
    {
      Key = "";
      Value = "";
      DataType = dataType;
    }
    public Setting(string key, DATA_TYPE value)
    {

      Key = key;
      Value = "";
      DataType = value;
    }
    public Setting(string key, STRING_SUB_TYPE stringSubType)
    {

      Key = key;
      Value = "";
      DataType = DATA_TYPE.String;
      StringSubType = stringSubType;
    }
    public Setting(string key, string value, STRING_SUB_TYPE stringSubType)
    {

      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
      StringSubType = stringSubType;
    }

    public Setting(string key, Color value)
    {

      Key = key;
      Value = value;
      DataType = DATA_TYPE.Color;
    }

    public Setting(string key, int value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Integer;
    }
    public Setting(string key, decimal value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Decimal;
    }
    public Setting(string key, string value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
    }
    public Setting(string key, bool value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Boolean;
    }

    public Setting(string dropDownGroupName, string groupName, string key, string value)
    {
      GroupName = groupName;
      DropDownGroupName = dropDownGroupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
    }
    public Setting(string dropDownGroupName, string groupName, string key, Color value)
    {
      GroupName = groupName;
      DropDownGroupName = dropDownGroupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Color;
    }
    public Setting(string dropDownGroupName, string groupName, string key, string value, List<Setting> subsettings)
    {
      GroupName = groupName;
      DropDownGroupName = dropDownGroupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
      StringSubType = STRING_SUB_TYPE.DropDownList;
      mSubSettings = subsettings;
    }

    public Setting(string groupName, string key, string value)
    {
      GroupName = groupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
    }
    public Setting(string groupName, string key, int value)
    {
      GroupName = groupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Integer;
    }

    public Setting(string dataToBeParsed)
    {
      FromXml(dataToBeParsed);
    }


    public void OptionAdd(string option)
    {
      if (mOptions is null)
        mOptions = new List<string>();
      mOptions.Add(option);
    }

    public override string ToString()
    {
      string val = "";
      if (DataType == DATA_TYPE.Color)
      {
        Color c = (Color)this.Value;
        val = ColorTranslator.ToHtml(c);
      }
      else
      {
        if (this.Value is not null)
        {
          val = this.Value.ToString()!;
        }
      }
      return HttpUtility.UrlEncode(Key) + "=" + DataType + ":" + HttpUtility.UrlEncode(val);
    }

    public void SubSettingsUpdate(ReadOnlyCollection<Setting> settings)
    {
      for (int x = 0; x < settings.Count; x++)
      {
        SubSettingsUpdate(settings[x]);
      }
    }


    public void SubSettingsUpdate(Setting setting)
    {
      for (int y = 0; y < this.mSubSettings.Count; y++)
      {
        if (this.mSubSettings[y].Key == setting.Key)
        {
          this.mSubSettings[y].Value = setting.Value;
        }
      }
    }

    public void SubSettingsAdd(Setting setting)
    {
      this.mSubSettings.Add(setting);
    }



    public bool SubSettingActive(Setting subSetting)
    {
      if ((this.DropDownGroupName + this.Value.ToString()) == subSetting.DropDownGroupName)
      {
        return true;
      }
      return false;
    }
    public string ToXml()
    {
      Xml xml = new Xml();
      xml.WriteMemoryNew(1);
      xml.WriteTagStart("Setting");
      xml.WriteTagAndContents("DataType", DataType);
      xml.WriteTagAndContents("Key", Key);
      xml.WriteTagAndContents("Value", Value.ToString()!);
      if (this.SubSettings.Count > 0)
      {
        xml.WriteTagStart("SubSettings");
        for (int x = 0; x < this.SubSettings.Count; x++)
        {
          if (this.SubSettingActive(this.SubSettings[x]) == true)
          {
            xml.WriteTagStart("SubSetting");
            xml.WriteTagAndContents("DataType", this.SubSettings[x].DataType);
            xml.WriteTagAndContents("Key", this.SubSettings[x].Key);
            xml.WriteTagAndContents("Value", this.SubSettings[x].Value.ToString()!);
            xml.WriteTagEnd("SubSetting");
          }
        }
        xml.WriteTagEnd("SubSettings");
      }
      xml.WriteTagEnd("Setting");
      return xml.ReadMemory();
    }

    public void FromXml(string xml)
    {
      DataType = Xml.GetXmlChunkAsEnum<DATA_TYPE>(ref xml, "DataType");
      Key = Xml.GetXMLChunk(ref xml, "Key");
      ParseValue(ref xml, DataType);
      string subXml = Xml.GetXMLChunk(ref xml, "SubSettings");
      do
      {
        string subSettingStr = Xml.GetXMLChunk(ref subXml, "SubSetting");
        if (subSettingStr.Length <= 0)
          break;

        DATA_TYPE dataType = Xml.GetXmlChunkAsEnum<DATA_TYPE>(ref subSettingStr, "DataType");
        Setting setting = new Setting(dataType);
        setting.Key = Xml.GetXMLChunk(ref subSettingStr, "Key");
        ParseValue(ref subSettingStr, dataType, setting);
        this.SubSettingsAdd(setting); //Parsing into a temp setting, so need to add, it will be updated later

      } while (subXml.Length > 0);

    }

    private void ParseValue(ref string data, DATA_TYPE dataType, Setting? setting = null)
    {
      if (setting is null)
        setting = this;

      if (dataType == DATA_TYPE.String)
        setting.Value = Xml.GetXMLChunk(ref data, "Value");
      else if (dataType == DATA_TYPE.Boolean)
        setting.Value = Xml.GetXMLChunkAsBool(ref data, "Value");
      else if (dataType == DATA_TYPE.Integer)
        setting.Value = Xml.GetXMLChunkAsLong(ref data, "Value");
      else if (dataType == DATA_TYPE.Decimal)
        setting.Value = Xml.GetXMLChunkAsDecimal(ref data, "Value");
      else if (dataType == DATA_TYPE.Color)
        setting.Value = Xml.GetXMLChunkAsColor(ref data, "Value");
    }
  }
}
