using Core;
using System.Drawing;
using System.Numerics;

namespace Logger
{
  public class Log : Core.Plugin
  {
    public override void Init()
    {
      base.Init();


      Functions.Add(new Function("Create", this, Create));
      Function f = new Function("Write", this, Write);
      f.OutputClear();
      f.OutputAdd("Complete");

      Functions.Add(f);

      SettingAddIfMissing("LogPath", "");
      SettingAddIfMissing("MaxLogSizeBytes", 1024000);
      Setting s = SettingAddIfMissing("RetentionStyle", DATA_TYPE.DropDownList);  //Number of Logs, Time Period, Size
      if (s != null)
      {
        s.DataType = DATA_TYPE.DropDownList;
        s.SubSettings.Add(new Setting("NumberOfLogs", "NumberOfLogs", 7));
        s.SubSettings.Add(new Setting("TimePeriod", "RollLogEveryHours", 24));
        s.SubSettings.Add(new Setting("FileSize", "MaxFileSizeInBytes", 1024000));

      }
      SettingAddIfMissing(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      SettingAddIfMissing(new Setting("", "Designer", "BorderColor", Color.Yellow));
      SettingAddIfMissing(new Setting("", "Designer", "FontColor", Color.Black));

    }

    public override void Dispose()
    {
      //Nothing to dispose of yet!
    }

    public static RESP Create(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Log.Create");
      return RESP.SetSuccess();
    }

    public static RESP Write(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Log.Write");
      return RESP.SetSuccess();
    }


  }
}