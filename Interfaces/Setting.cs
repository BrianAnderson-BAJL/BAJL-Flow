using System;
using System.Collections.Generic;
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
    public string DropDownGroupName = "";
    public string Key = "";
    public object Value = "";
    public string Description = "";
    public DATA_TYPE DataType = DATA_TYPE.String;
    public List<Setting> SubSettings = new(0);

    public Setting(DATA_TYPE dataType)
    {
      Key = "";
      Value = "";
      DataType = dataType;
      SubSettings = new List<Setting>(0);
    }
    public Setting(string key, DATA_TYPE value)
    {

      Key = key;
      Value = "";
      DataType = value;
      SubSettings = new List<Setting>(0);
    }

    public Setting(string key, Color value)
    {

      Key = key;
      Value = value;
      DataType = DATA_TYPE.Color;
      SubSettings = new List<Setting>(0);
    }

    public Setting(string key, int value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Integer;
      SubSettings = new List<Setting>(0);
    }
    public Setting(string key, decimal value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Decimal;
      SubSettings = new List<Setting>(0);
    }
    public Setting(string key, string value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
      SubSettings = new List<Setting>(0);
    }
    public Setting(string key, bool value)
    {
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Boolean;
      SubSettings = new List<Setting>(0);
    }

    public Setting(string dropDownGroupName, string groupName, string key, string value)
    {
      GroupName = groupName;
      DropDownGroupName = dropDownGroupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
      SubSettings = new List<Setting>(0);
    }
    public Setting(string dropDownGroupName, string groupName, string key, Color value)
    {
      GroupName = groupName;
      DropDownGroupName = dropDownGroupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Color;
      SubSettings = new List<Setting>(0);
    }
    public Setting(string dropDownGroupName, string groupName, string key, string value, List<Setting> subsettings)
    {
      GroupName = groupName;
      DropDownGroupName = dropDownGroupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.DropDownList;
      SubSettings = subsettings;
    }

    public Setting(string groupName, string key, string value)
    {
      GroupName = groupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.String;
      SubSettings = new List<Setting>(0);
    }
    public Setting(string groupName, string key, int value)
    {
      GroupName = groupName;
      Key = key;
      Value = value;
      DataType = DATA_TYPE.Integer;
      SubSettings = new List<Setting>(0);
    }

    public Setting(string dataToBeParsed)
    {
      Parse(dataToBeParsed);
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
        if (this.Value != null)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
          val = this.Value.ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
      }
      return HttpUtility.UrlEncode(Key) + "=" + DataType + ":" + HttpUtility.UrlEncode(val);
    }

    private void Parse(string data)
    {
      try
      {
        string[] strings = data.Split('=');
        if (strings.Length == 2)
        {
          Key = HttpUtility.UrlDecode(strings[0]);
          string[] ValueArray = strings[1].Split(":");
          if (ValueArray.Length == 2)
          {
            DataType = Enum.Parse<DATA_TYPE>(ValueArray[0], true);
            if (DataType == DATA_TYPE.Color)
            {
              Value = ColorTranslator.FromHtml(HttpUtility.UrlDecode(ValueArray[1]));
            }
            else if (DataType == DATA_TYPE.Boolean)
            {
              Value = bool.Parse(ValueArray[1]);
            }
            else
            {
              Value = HttpUtility.UrlDecode(ValueArray[1]);
            }
          }
        }
        else
        {
          throw new Exception("Invalid setting, the key and value must each be URL encoded and be in the format \"key=Value\"");
        }
      }
      catch (Exception ex)
      {
        throw new Exception("Invalid setting, the key and value must each be URL encoded and be in the format \"key=DataType:Value\"", ex);
      }
    }
  }
}
