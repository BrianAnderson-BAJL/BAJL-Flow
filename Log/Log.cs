using FlowEngineCore;
using K4os.Compression.LZ4;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using static FlowEngineCore.PARM;

namespace Logger
{
  public class Log : FlowEngineCore.Plugin, FlowEngineCore.Interfaces.ILog
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

    private LOG_TYPE mLogLevel = LOG_TYPE.INF;

    public LOG_TYPE LogLevel
    {
      get { return mLogLevel; }
      set 
      {
        if (value != mLogLevel)
          Write($"Changing log level from [{mLogLevel}] to [{value}]");
        mLogLevel = value; 
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

    private const string ROLL_STYLE = "RollStyle";
    private const string ROLL_STYLE_TIME_BASED = "Time based";
    private const string ROLL_STYLE_SIZE_BASED = "Size based";
    private const string ROLL_STYLE_TIME_AND_SIZE_BASED = "Time & Size based";

    private DateTime mLastLogRollTimeCheck = DateTime.MinValue;
    private DateTime mLastArchiveDeleteCheck = DateTime.MinValue;
    private bool mLoggingEnabled = true;

    public override void Init()
    {
      base.Init();


      Functions.Add(new Function("Create", this, Create));
      Function function = new Function("Write", this, Write);
      function.OutputClear();
      function.OutputAdd("Continue");
      PARM pddl = function.Parms.Add("Log Type", STRING_SUB_TYPE.DropDownList);
      pddl.ValidatorAdd(PARM_VALIDATION.StringDefaultValue, "INF");
      foreach (int val in Enum.GetValues(typeof(LOG_TYPE)))
      {
        String name = Enum.GetName(typeof(LOG_TYPE), val)!;
        pddl.OptionAdd($"{name}");
      }
      Functions.Add(function);
      function.Parms.Add(new PARM("Log Values", DATA_TYPE.Various));


      mSettings.SettingAdd(new Setting(LOG_SHARE_WITH_PLUGINS, true));
      mSettings.SettingAdd(new Setting("LogPath", "./"));

      Setting setting = mSettings.SettingAdd(new Setting(SETTING_TIME_STYLE, SETTING_TIME_STYLE_LOCAL, STRING_SUB_TYPE.DropDownList));
      setting.OptionAdd(SETTING_TIME_STYLE_LOCAL);
      setting.OptionAdd(SETTING_TIME_STYLE_UTC);

      setting = new Setting("Log file name format", "FlowEngineLog{Y}-{M}-{D}_{S}.log");
      setting.Description = "{Mon} = Month, {D} = Day, {Y} = Year, {H} = Hour, {Min} = Minute, {S} = Seconds, {Seq} = Sequence number (Example = 'FlowEngineLog{Y}-{M}-{D}_{Seq}.log')";
      mSettings.SettingAdd(setting);

      setting = new Setting("Archive logs after days", 7); //TODO: Need to actually implement archiving of log files
      setting.Description = "After this many days the log files will be compressed into zip format and stored in the archive directory. Set to 0 (zero) to disable archiving.";
      mSettings.SettingAdd(setting);

      setting = new Setting("Delete logs after days", 30); 
      setting.Description = "After this many days the log files (and archived log files) will be deleted. Set to 0 (zero) to disable deleting log files.";
      mSettings.SettingAdd(setting);

      setting = new Setting("Log Level", "WAR", STRING_SUB_TYPE.DropDownList); 
      setting.Description = "Log all events with this severity or greater. (i.e. 'DBG' will log debug and everything higher, 'WAR' will only log Warnings, and Errors";
      var values = Enum.GetValues(typeof(LOG_TYPE));
      foreach (var item in values)
      {
        setting.OptionAdd(item.ToString()!);
      }
      mSettings.SettingAdd(setting);

      Setting rollStyle = new Setting(DATA_TYPE.String)
      {
        Key = "RollStyle",
        Value = "Time based",
        DropDownGroupName = "RollStyle",
        StringSubType = STRING_SUB_TYPE.DropDownList
      };

      Setting s = mSettings.SettingAdd(rollStyle);  //Number of Logs, Time Period, Size
      if (s is not null)
      {
        s.OptionAdd(ROLL_STYLE_TIME_BASED);
        s.OptionAdd(ROLL_STYLE_SIZE_BASED);
        s.OptionAdd(ROLL_STYLE_TIME_AND_SIZE_BASED);

        Setting timeBased = new Setting(DATA_TYPE.Integer)
        {
          DropDownGroupName = "RollStyleTime based",
          GroupName = "Roll Style - Time based",
          Key = "RollLogAtHour",
          Description = "What hour do you want to start a new log in 24 hour clock (0 = Midnight, 12 = Noon, 18 = 6 PM)",
          Value = 0,
        };
        Setting sizeBased = new Setting(DATA_TYPE.Integer)
        {
          DropDownGroupName = "RollStyleSize based",
          GroupName = "Roll Style - Size based",
          Key = "MaxFileSizeInBytes",
          Description = "When the log file gets larger than this, it will start a new lof file. (the log file will not be exactly 1024000 bytes)",
          Value = 1024000,
        };
        Setting timeBased2 = new Setting(DATA_TYPE.Integer) //Adding a duplicate of the Time based settings, easier to just duplicate it rather than try to get both to show up somehow
        {
          DropDownGroupName = "RollStyleTime & Size based",
          GroupName = "Roll Style - Time & Size based",
          Key = "RollLogAtHour", //Key name is the same to only have one value show up in the log.xml file
          Description = "What hour do you want to start a new log in 24 hour clock (0 = Midnight, 12 = Noon, 18 = 6 PM)",
          Value = 0,
        };
        Setting sizeBased2 = new Setting(DATA_TYPE.Integer) //Adding a duplicate of the Time based settings, easier to just duplicate it rather than try to get both to show up somehow
        {
          DropDownGroupName = "RollStyleTime & Size based",
          GroupName = "Roll Style - Time & Size based",
          Key = "MaxFileSizeInBytes",
          Description = "When the log file gets larger than this, it will start a new lof file. (the log file will not be exactly 1024000 bytes)",
          Value = 1024000,
        };
        s.SubSettingsAdd(timeBased);
        s.SubSettingsAdd(sizeBased);
        s.SubSettingsAdd(timeBased2);
        s.SubSettingsAdd(sizeBased2);

      }

      mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Yellow));
      mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));

      this.StartPriority = 9000; //Log should start before other plugins, put at least a 1000 between other plugins to give room to manipulate the start up priority

    }

    public override void LoadSettings(string pluginPath, Dictionary<string, object> GlobalPluginValues)
    {
      base.LoadSettings(pluginPath, GlobalPluginValues);

      if (mSettings.SettingGetAsBoolean(LOG_SHARE_WITH_PLUGINS) == true)
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

      string logLevelStr = mSettings.SettingGetAsString("Log Level");
      if (Enum.TryParse<LOG_TYPE>(logLevelStr, out LOG_TYPE logLevel) == true)
      {
        LogLevel = logLevel;
      }
      //Set the function pointer to the time style I want.
      string timeStyle = mSettings.SettingGetAsString(SETTING_TIME_STYLE);
      if (timeStyle == SETTING_TIME_STYLE_LOCAL)
        GetDateTime = LocalTime;
      else
        GetDateTime = UtcTime;

      LogFileName = GetFileName();

      if (LogFileName.Length > 200)
      {
        Global.WriteAllways($"LOGGING DISABLED! Log path and file name combination is over 200 characters [{LogFileName}], Please select a shorter path.", LOG_TYPE.ERR);
        mLoggingEnabled = false;
        return;
      }
      LogThread = new Thread(LogThreadRuntime);
      LogThread.Start();


      base.StartPlugin(GlobalPluginValues);
    }

    private string GetFileName(bool rolling = false)
    {

      string filePath = mSettings.SettingGetAsString("LogPath");
      string fileNameOrig = mSettings.SettingGetAsString("Log file name format");
      string tempFileName = fileNameOrig;

      tempFileName = tempFileName.Replace("{Y}", GetDateTime().Year.ToString());
      tempFileName = tempFileName.Replace("{Mon}", GetDateTime().Month.ToString("00"));
      tempFileName = tempFileName.Replace("{D}", GetDateTime().Day.ToString("00"));
      tempFileName = tempFileName.Replace("{H}", GetDateTime().Hour.ToString("00"));
      tempFileName = tempFileName.Replace("{Min}", GetDateTime().Minute.ToString("00"));
      tempFileName = tempFileName.Replace("{S}", GetDateTime().Second.ToString("00"));

      string fileName = tempFileName;
      int sequenceNumber = 0;
      do
      {
        sequenceNumber++;
        fileName = tempFileName.Replace("{Seq}", sequenceNumber.ToString());
      } while (File.Exists(Options.GetFullPath(filePath, fileName)));

      return Options.GetFullPath(filePath, fileName);
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

    private void AddLogEntry(string value, LOG_TYPE logType, string? overrideThreadName = null)
    {
      if (mLoggingEnabled == false)
        return;

      string logTypeStr = logType.ToString();

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

    /// <summary>
    /// Trhead that actually writes the log file
    /// </summary>
    private void LogThreadRuntime()
    {
      
      Thread.CurrentThread.Name = "Logger Plugin";
      mLastLogRollTimeCheck = GetDateTime();
      List<LogEntry> localLogs = new List<LogEntry>(128);
      while (KeepRunning == true)
      {
        lock (CriticalSection)
        {
          localLogs.AddRange(LogsToWrite);
          LogsToWrite.Clear();
        }

        StringBuilder sb = new StringBuilder(4096);
        for (int x = 0; x < localLogs.Count; x++)
        {
          string temp = localLogs[x].Format();
          sb.Append(temp + Environment.NewLine);
          if (Options.ServerType == Options.SERVER_TYPE.Development)
            Log.WriteToConsole(temp, localLogs[x].LogType);
        }
        File.AppendAllText(LogFileName, sb.ToString());
        CheckForLogRoll();
        CheckForLogArchiveAndDelete();
        localLogs.Clear();

        Thread.Sleep(1);
      }
    }

    private void CheckForLogArchiveAndDelete()
    {
      TimeSpan ts = GetDateTime() - mLastArchiveDeleteCheck;
      if (ts.TotalHours < 0) //If at least an hour has elapsed, then we can check
        return;

      mLastArchiveDeleteCheck = GetDateTime();

      CheckForLogArchive();
      CheckForLogDelete();
    }

    private void CheckForLogDelete()
    {
      string? extension = Path.GetExtension(LogFileName);
      if (extension is null || extension == "")
        return;

      int DeleteAfterDays = mSettings.SettingGetAsInt("Delete logs after days");
      if (DeleteAfterDays <= 0)
        return;

      DateTime now = GetDateTime();
      string filePath = mSettings.SettingGetAsString("LogPath");
      extension = "*" + extension; 
      string[] AllFiles = Directory.GetFiles(filePath, extension, SearchOption.AllDirectories); //Get all the files with the extension defined in the Logger.xml file.
      for (int x = 0; x < AllFiles.Length; x++)
      {
        FileInfo fi = new FileInfo(AllFiles[x]);
        TimeSpan fileAge = now - fi.LastWriteTimeUtc;
        if (fileAge.TotalDays >= DeleteAfterDays)
        {
          Write($"Log file [{Global.ConvertToString(fileAge)}] is older than the 'Delete logs after days' [{DeleteAfterDays}] setting. File [{AllFiles[x]}]", LOG_TYPE.INF);
          try
          {
            File.Delete(AllFiles[x]);
          }
          catch (Exception ex)
          {
            Write(ex, LOG_TYPE.WAR);
          }
        }
      }
    }

    private void CheckForLogArchive()
    {
    }


    private void CheckForLogRoll()
    {
      Setting? setting = mSettings.SettingFind(ROLL_STYLE);
      if (setting is null)
        return;

      string oldFileName = LogFileName;

      if (setting.Value == ROLL_STYLE_SIZE_BASED)
      {
        RollLogSizeOnly(setting, oldFileName);
      }
      else if (setting.Value == ROLL_STYLE_TIME_BASED)
      {
        RollLogTimeOnly(setting, oldFileName);
      }
      else if (setting.Value == ROLL_STYLE_TIME_AND_SIZE_BASED)
      {
        bool rolledBySize = RollLogSizeOnly(setting, oldFileName);
        if (rolledBySize == false)
        {
          RollLogTimeOnly(setting, oldFileName);
        }
      }

      mLastLogRollTimeCheck = GetDateTime();
    }

    private bool RollLogSizeOnly(Setting parent, string oldFileName)
    {
      FileInfo fi = new FileInfo(LogFileName);
      Setting? maxFileSizeSetting = parent.SubSettingsFind("MaxFileSizeInBytes");
      if (maxFileSizeSetting is not null && fi.Length >= maxFileSizeSetting.Value)
      {
        LogFileName = GetFileName(); //We have reached the maximum file size, get a new file name
        AddLogRollingEntry(oldFileName, $"Rolling log file 'RollStyle' is set to [{ROLL_STYLE_SIZE_BASED}], 'MaxFileSizeInBytes' is set to [{maxFileSizeSetting!.Value}], actual log file size is [{fi.Length}], new file name is [{LogFileName}]");
        AddLogRollingEntry(LogFileName, $"Log File was Rolled to this log file 'RollStyle' was set to [{ROLL_STYLE_SIZE_BASED}], 'MaxFileSizeInBytes' was set to [{maxFileSizeSetting.Value}], old log file size was [{fi.Length}], old file name is [{oldFileName}]");
        return true;
      }
      return false;
    }

    private bool RollLogTimeOnly(Setting parent, string oldFileName)
    {
      Setting? rollAtHour = parent.SubSettingsFind("RollLogAtHour");
      DateTime currentTime = GetDateTime();
      if (rollAtHour is not null && currentTime.Hour == rollAtHour.Value && mLastLogRollTimeCheck.TimeOfDay.Hours != currentTime.Hour) //TimeOfDay will return the hours as 24 hour format
      {
        LogFileName = GetFileName(); //We have reached the time to roll the log, get a new file name
        AddLogRollingEntry(oldFileName, $"Rolling log file 'RollStyle' is set to [{ROLL_STYLE_TIME_BASED}], 'RollLogAtHour' is set to [{rollAtHour!.Value}] hour, actual time is [{currentTime}], new file name is [{LogFileName}]");
        AddLogRollingEntry(LogFileName, $"Log File was Rolled to this log file 'RollStyle' was set to [{ROLL_STYLE_TIME_BASED}], 'RollLogAtHour' was set to [{rollAtHour.Value}] hour, actual time was [{currentTime}], old file name is [{oldFileName}]");
        return true;
      }
      return false;
    }

    private void AddLogRollingEntry(string oldLogFileName, string val)
    {
      try
      {
        if (LogFileName is not null && LogFileName != "") //The log is being rolled, lets add a log entry to the existing file to show that it is rolling.
        {
          LogEntry le = new LogEntry();
          le.LogType = LOG_TYPE.INF;
          le.Value = val;
          le.LogDateTime = GetDateTime();
          File.AppendAllText(oldLogFileName, le.Format() + Environment.NewLine);

        }
      }
      catch (Exception ex)
      {
        Write(ex, LOG_TYPE.ERR);
      }
    }

    public RESP Create(FlowEngineCore.Flow flow, Variable[] vars)
    {
      Log.WriteToConsole("Log.Create");
      return RESP.SetSuccess();
    }

    public RESP Write(FlowEngineCore.Flow flow, Variable[] vars)
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

      if (Enum.TryParse<LOG_TYPE>(logTypeStr, out LOG_TYPE logType) == true)
      {
        AddLogEntry(value, logType);
      }
      return RESP.SetSuccess();
    }

    public void Write(string val, LOG_TYPE debug = LOG_TYPE.INF, string? overrideThreadName = null)
    {
      if (debug >= mLogLevel)
      {
        AddLogEntry(val, debug, overrideThreadName);
      }
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