using Core;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using static Core.PARM;

namespace Logger
{
  public class Log : Core.Plugin, Core.Interfaces.ILog
  {
    private class LogEntry
    {
      public LOG_TYPE LogType;
      public string Value = "";
      public string ThreadName = "";
      public DateTime LogDateTime = DateTime.MinValue;

      public string Format()
      {

        return $"[{LogType}] {LogDateTime} [{ThreadName}] - {Value}";
      }
    }

    private Thread? LogThread;
    private bool KeepRunning = true;
    private List<LogEntry> LogsToWrite = new List<LogEntry>(128);
    private object CriticalSection = new object();
    private Func<DateTime> GetDateTime = LocalTime;
    private string LogFileName = "";
    private const string SETTING_TIME_STYLE = "TimeStyle";
    private const string SETTING_TIME_STYLE_LOCAL = "Local";
    private const string SETTING_TIME_STYLE_UTC = "UTC";
    private const string LOG_SHARE_WITH_PLUGINS = "ShareLogWithOtherPlugins";

    public override void Init()
    {
      base.Init();


      Functions.Add(new Function("Create", this, Create));
      Function function = new Function("Write", this, Write);
      function.OutputClear();
      function.OutputAdd("Complete");
      PARM pddl = function.Parms.Add("Log Type", STRING_SUB_TYPE.DropDownList);
      pddl.ValidatorAdd(PARM_VALIDATION.StringDefaultValue, "INF");
      foreach (int val in Enum.GetValues(typeof(LOG_TYPE)))
      {
        String name = Enum.GetName(typeof(LOG_TYPE), val)!;
        pddl.OptionAdd($"{name}");
      }
      Functions.Add(function);
      function.Parms.Add(new PARM("Log Values", DATA_TYPE.Various));


      SettingAdd(new Setting(LOG_SHARE_WITH_PLUGINS, true));
      SettingAdd(new Setting("LogPath", ""));

      Setting setting = SettingAdd(new Setting(SETTING_TIME_STYLE, SETTING_TIME_STYLE_LOCAL, STRING_SUB_TYPE.DropDownList));
      setting.OptionAdd(SETTING_TIME_STYLE_LOCAL);
      setting.OptionAdd(SETTING_TIME_STYLE_UTC);

      Setting retentionStyle = new Setting(DATA_TYPE.String)
      {
        Key = "RetentionStyle",
        Value = "Time based",
        DropDownGroupName = "RetentionStyle",
        StringSubType = STRING_SUB_TYPE.DropDownList
      };

      Setting s = SettingAdd(retentionStyle);  //Number of Logs, Time Period, Size
      if (s is not null)
      {
        s.OptionAdd("Time based");
        s.OptionAdd("Size based");
        s.OptionAdd("Time & Size based");

        Setting timeBased = new Setting(DATA_TYPE.Integer)
        {
          DropDownGroupName = "RetentionStyleTime based",
          GroupName = "Retention Style - Time based",
          Key = "RollLogEveryHours",
          Value = 24,
        };
        Setting sizeBased = new Setting(DATA_TYPE.Integer)
        {
          DropDownGroupName = "RetentionStyleSize based",
          GroupName = "Retention Style - Size based",
          Key = "MaxFileSizeInBytes",
          Value = 1024000,
        };
        Setting timeBased2 = new Setting(DATA_TYPE.Integer) //Adding a duplicate of the Time based settings, easier to just duplicate it rather than try to get both to show up somehow
        {
          DropDownGroupName = "RetentionStyleTime & Size based",
          GroupName = "Retention Style - Time & Size based",
          Key = "RollLogEveryHours", //Key name is the same to only have one value show up in the log.xml file
          Value = 24,
        };
        Setting sizeBased2 = new Setting(DATA_TYPE.Integer) //Adding a duplicate of the Time based settings, easier to just duplicate it rather than try to get both to show up somehow
        {
          DropDownGroupName = "RetentionStyleTime & Size based",
          GroupName = "Retention Style - Time & Size based",
          Key = "MaxFileSizeInBytes",
          Value = 1024000,
        };
        s.SubSettingsAdd(timeBased);
        s.SubSettingsAdd(sizeBased);
        s.SubSettingsAdd(timeBased2);
        s.SubSettingsAdd(sizeBased2);

      }

      SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      SettingAdd(new Setting("", "Designer", "BorderColor", Color.Yellow));
      SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));

      this.StartPriority = 9000; //Log should start before other plugins, put at least a 1000 between other plugins to give room to manipulate the start up priority

    }

    public override void LoadSettings(Dictionary<string, object> GlobalPluginValues)
    {
      base.LoadSettings(GlobalPluginValues);

      if (this.SettingGetAsBoolean(LOG_SHARE_WITH_PLUGINS) == true)
      {
        GlobalPluginValues.Add("log", this);
      }

    }

    [Conditional("DEBUG")]
    public static void WriteToConsole(string val, LOG_TYPE debug = LOG_TYPE.INF)
    {
      if (debug == LOG_TYPE.INF || debug == LOG_TYPE.DBG)
        Console.ForegroundColor = ConsoleColor.White;
      else if (debug == LOG_TYPE.WAR)
        Console.ForegroundColor = ConsoleColor.Yellow;
      else
        Console.ForegroundColor = ConsoleColor.Red;

      Console.Write(val);

      Console.ForegroundColor = ConsoleColor.White;  //Change the color back to white when done, otherwise the console will stay the color if it exits after an error.
      Console.WriteLine("");
    }

    private static DateTime LocalTime()
    { 
      return DateTime.Now; 
    }

    private static DateTime UtcTime()
    {
      return DateTime.UtcNow;
    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);

      //Set the function pointer to the time style I want.
      string timeStyle = this.SettingGetAsString(SETTING_TIME_STYLE);
      if (timeStyle == SETTING_TIME_STYLE_LOCAL)
        GetDateTime = LocalTime;
      else
        GetDateTime = UtcTime;

      LogFileName = this.SettingGetAsString("LogPath");

      LogThread = new Thread(LogThreadRuntime);
      LogThread.Start();


      base.StartPlugin(GlobalPluginValues);
    }

    public override void StopPlugin()
    {
      KeepRunning = false;
      base.StopPlugin();
    }


    public override void Dispose()
    {
      //Nothing to dispose of yet!
    }

    private void AddLogEntry(string value, string logTypeStr, string? overrideThreadName = null)
    {
      LogEntry logEntry = new LogEntry();
      logEntry.LogType = Enum.Parse<LOG_TYPE>(logTypeStr);
      logEntry.Value = value;
      logEntry.LogDateTime = GetDateTime(); //Get local or UTC time
      if (overrideThreadName is null)
        logEntry.ThreadName = Thread.CurrentThread.Name!;
      else
        logEntry.ThreadName = overrideThreadName;

      lock (CriticalSection)
      {
        LogsToWrite.Add(logEntry);
      }
    }
    private void LogThreadRuntime()
    {
      List<LogEntry> localLogs = new List<LogEntry>(128);
      while (KeepRunning == true)
      {
        lock (CriticalSection)
        {
          localLogs.AddRange(LogsToWrite);
          LogsToWrite.Clear();
        }

        
        for (int x = 0; x < localLogs.Count; x++)
        {
          string temp = localLogs[x].Format();
          Log.WriteToConsole(temp, localLogs[x].LogType);
          File.AppendAllText(LogFileName, temp + Environment.NewLine);
        }
        localLogs.Clear();

        Thread.Sleep(1);
      }
    }

    public RESP Create(Core.Flow flow, Variable[] vars)
    {
      Log.WriteToConsole("Log.Create");
      return RESP.SetSuccess();
    }

    public RESP Write(Core.Flow flow, Variable[] vars)
    {
      vars[0].GetValue(out string logTypeStr);
      Variable? var = null;
      string value = "";
      if (vars.Length > 1)
      {
        var = vars[1];
        value = var.GetValueAsString();
      }
      if (var is not null && var.SubVariables.Count > 0)
      {
        value = var.ToJson();
      }

      AddLogEntry(value, logTypeStr);

      return RESP.SetSuccess();
    }

    public void Write(string val, LOG_TYPE debug = LOG_TYPE.INF, string? overrideThreadName = null)
    {
      AddLogEntry(val, debug.ToString(), overrideThreadName);
    }

    public void Write(Exception ex, LOG_TYPE debug = LOG_TYPE.INF)
    {
      Write(Global.FullExceptionMessage(ex), debug);
    }

    public void Write(string customErrorMessage, Exception ex, LOG_TYPE debug = LOG_TYPE.INF)
    {
      Write(customErrorMessage + Environment.NewLine + Global.FullExceptionMessage(ex), debug);
    }
  }
}