using Core;
using Core.Interfaces;
using MySqlX.XDevAPI.Relational;
using System;
using System.Drawing;

namespace Session
{
    public class Session : Core.Plugin
  {
    private IDatabase? Database;
    private ILog? Log;
    private uint USER_REGISTER_DUPLICATE_OUTPUT = 1;
    private uint USER_REGISTER_GENERIC_ERROR_OUTPUT = 2;

    private const int ERROR_INSERT_DB = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 1;
    private const int ERROR_DUPLICATE_LOGIN_ID = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 2;
    private const int ERROR_INVALID_SESSION = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 3;
    private const int ERROR_BAD_LOGIN_ID = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 4;
    private const int ERROR_BAD_PASSWORD = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 5;
    private const int ERROR_DB = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 6;
    private const int ERROR_ACCOUNT_LOCKED_OUT = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 7;


    public override void Init()
    {
      base.Init();

      Function function = new Function("User Register", this, UserRegister);
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.Parms.Add("Password", DATA_TYPE.String);
      function.OutputAddSuccess();
      function.OutputAdd("Duplicate");  //USER_REGISTER_DUPLICATE
      function.OutputAddError();        //USER_REGISTER_ERROR
      Functions.Add(function);

      function = new Function("User Login", this, UserLogin);
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.Parms.Add("Password", DATA_TYPE.String);
      Functions.Add(function);

      function = new Function("User Logout", this, UserLogout);
      function.Parms.Add("SessionToken", DATA_TYPE.String);
      Functions.Add(function);

      function = new Function("User Logout All", this, UserLogoutAll, "Log out all sessions on all devices.", "Log out all sessions on all devices.");
      function.Parms.Add("SessionToken", DATA_TYPE.String);
      Functions.Add(function);

      function = new Function("Device Register", this, DeviceRegister);
      function.Parms.Add("DeviceToken", DATA_TYPE.String);
      function.Parms.Add("AppVersion", DATA_TYPE.String);
      function.Parms.Add("OsFamily", DATA_TYPE.String);
      function.Parms.Add("OsVersion", DATA_TYPE.String);
      function.Parms.Add("Model", DATA_TYPE.String);
      function.Parms.Add("MaxTextureSize", DATA_TYPE.String);
      function.Parms.Add("PixelsMaxX", DATA_TYPE.String);
      function.Parms.Add("PixelsMaxY", DATA_TYPE.String);
      Functions.Add(function);

      function = new Function("Check Device", this, CheckDevice);
      function.Parms.Add("DeviceToken", DATA_TYPE.String);
      Functions.Add(function);

      function = new Function("Check Session", this, CheckSession);
      function.Parms.Add("SessionToken", DATA_TYPE.String);
      function.DefaultSaveResponseVariable = true;
      Functions.Add(function);

      //SETTINGS
      {
        SettingAdd(new Setting("LoginAttemptsBeforeLock", 3));
        SettingAdd(new Setting("LockAccountMinutes", 30));
        SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        SettingAdd(new Setting("", "Designer", "BorderColor", Color.Teal));
        SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));
      }
      //SETTINGS

    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);

      //Let's grab the database from the database plugin
      if (GlobalPluginValues.ContainsKey("db") == true)
      {
        Database = GlobalPluginValues["db"] as IDatabase;
      }
      else
      {
        Global.Write("No shared database connection pool found. Session plugin will not work!", LOG_TYPE.ERR);
      }
      //Let's grab the log from the log plugin
      if (GlobalPluginValues.ContainsKey("log") == true)
      {
        Log = GlobalPluginValues["log"] as ILog;
      }
      else
      {
        Global.Write("No shared Log found. Session plugin will not Log errors.", LOG_TYPE.ERR);
      }
    }

    public override void StopPlugin()
    {
    }

    /// <summary>
    /// User is registering with the system with a login ID and password
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP UserRegister(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Session.UserRegister");
      if (Database is null)
        return RESP.SetError(1, "No active database connection", USER_REGISTER_GENERIC_ERROR_OUTPUT);

      vars[0].GetValue(out string loginId);
      vars[1].GetValue(out string password);

      //Check if the LoginId already exists in the database
      Variable varLoginId = new Variable("@LoginId", loginId); //Need to name the varialbe '@LoginId' to be used in the SQL
      Variable records = Database.Select("SELECT UserId FROM Users Where LoginId = @LoginId", varLoginId);
      if (records.SubVariables.Count > 0) //Has a record, means it found a user with the same LoginId
      {
        Global.Write("Session.UserRegister - Duplicate login Id", LOG_TYPE.WAR);
        return RESP.SetError((int)USER_REGISTER_DUPLICATE_OUTPUT, $"Duplicate LoginId [{loginId}]", USER_REGISTER_DUPLICATE_OUTPUT); //User already exists in the database, return error.
      }

      //LoginId doesn't exist, lets hash the password and insert it into the database
      string passwordHash = SecureHasherV1.Hash(password);
      Variable recordsAffected = Database.Execute("INSERT INTO Users (LoginId, Password) VALUES (@LoginId, @Password)", varLoginId, new Variable("@Password", passwordHash));
      if (recordsAffected.Value > 0 && recordsAffected.SubVariables.Count > 0)
      {
        int NEW_DB_ID_INDEX = 0;
        recordsAffected.SubVariables[NEW_DB_ID_INDEX].GetValue(out long newDbId); //The only sub variable will be the new Id of the record that was inserted
        return RESP.SetSuccess(Database.Select("SELECT * FROM Users WHERE UserId = @UserId", new Variable("@UserId", newDbId)));
      }
      else
        return RESP.SetError(ERROR_INSERT_DB, "Failed to insert user into database");
    }

    /// <summary>
    /// User Login function
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP UserLogin(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Session.UserLogin");

      if (Database is null)
        return RESP.SetError(1, "No active database connection", USER_REGISTER_GENERIC_ERROR_OUTPUT);

      vars[0].GetValue(out string loginId);
      vars[1].GetValue(out string password);
      Variable varLoginId = new Variable("@LoginId", loginId); //Need to name the varialbe '@LoginId' to be used in the SQL
      Variable records = Database.Select("SELECT UserId, Password, LoginAttempts, LockedUntil, NOW() AS CurrentTime FROM Users Where LoginId = @LoginId", varLoginId);
      if (records.SubVariables.Count == 0) //No record found
      {
        Log?.Write("Session.UserLogin - login Id does not exist", LOG_TYPE.INF);
        return RESP.SetError(ERROR_BAD_LOGIN_ID, $"Bad LoginId [{loginId}]", (int)Output.TYPE.Error);
      }
      if (records.SubVariables[0].SubVariables.Count < 5)
      {
        Log?.Write("Session.UserLogin - DB returned the wrong number of fields", LOG_TYPE.INF);
        return RESP.SetError(ERROR_DB, $"DB returned the wrong number of fields [{loginId}]", (int)Output.TYPE.Error);
      }
      records.SubVariables[0].SubVariables[2].GetValue(out long loginAttempts);
      records.SubVariables[0].SubVariables[3].GetValue(out string accountLockedUntilStr);
      records.SubVariables[0].SubVariables[4].GetValue(out string currentDbTimeStr);
      if (accountLockedUntilStr is not null)
      {
        bool lockedBool = DateTime.TryParse(accountLockedUntilStr, out DateTime lockedUntil);
        bool currentTimeBool = DateTime.TryParse(currentDbTimeStr, out DateTime currentDbTime);
        if (lockedBool == false || currentTimeBool == false)
        {
          Log?.Write("Session.UserLogin - Unable to parse DateTime from Db", LOG_TYPE.INF);
          return RESP.SetError(ERROR_DB, $"Unable to parse DateTime from Db [{loginId}]", (int)Output.TYPE.Error);
        }
        if (lockedUntil > currentDbTime)
        {
          Log?.Write($"Session.UserLogin - Account locked out until [{lockedUntil}]", LOG_TYPE.INF);
          return RESP.SetError(ERROR_ACCOUNT_LOCKED_OUT, $"Account locked out until [{lockedUntil}]", (int)Output.TYPE.Error);
        }
      }

      records.SubVariables[0].SubVariables[1].GetValue(out string passwordDb);
      string passwordHashed = SecureHasherV1.Hash(password);
      if (passwordHashed != passwordDb)
      {
        if (loginAttempts < SettingGetAsInt("LoginAttemptsBeforeLock"))
        {
          Database.Execute("UPDATE Users SET LoginAttempts = @LoginAttempts WHERE LoginId = @LoginId", new Variable("@LoginAttempts", loginAttempts + 1), varLoginId);
        }
        else
        {
          Variable varLoginAttempts = new Variable("@LoginAttempts", SettingGetAsInt("LoginAttemptsBeforeLock"));
          Variable varMinutesToLock = new Variable("@LockAccountMinutes", SettingGetAsInt("LockAccountMinutes"));
          Database.Execute("UPDATE Users SET LoginAttempts = @LoginAttempts, LockedUntil = Now() + interval @LockAccountMinutes MINUTE WHERE LoginId = @LoginId", varLoginAttempts, varMinutesToLock, varLoginId);
        }
        Log?.Write("Session.UserLogin - Bad Password", LOG_TYPE.INF);
        return RESP.SetError(ERROR_BAD_PASSWORD, $"Bad password for loginId [{loginId}]", (int)Output.TYPE.Error);
      }

      //We got this far, the loginId is good, and the password is good, lets reset the locks
      Database.Execute("UPDATE Users SET LoginAttempts = 0, LockedUntil = NULL WHERE LoginId = @LoginId", varLoginId);


      return RESP.SetSuccess();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP UserLogout(Core.Flow flow, Variable[] vars)
    {
      return RESP.SetError(2, "User failed to log out");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP UserLogoutAll(Core.Flow flow, Variable[] vars)
    {
      return RESP.SetError(2, "User failed to log out all");
    }

    /// <summary>
    /// Register a new device
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP DeviceRegister(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Session.DeviceRegister");
      if (Database is null)
        return RESP.SetError(1, "No active database connection");

      Variable varDeviceToken = vars[0].CloneWithNewName("@DeviceToken");
      Variable varAppVersion = vars[1].CloneWithNewName("@AppVersion");
      Variable varOsFamily = vars[2].CloneWithNewName("@OsFamily");
      Variable varOsVersion = vars[3].CloneWithNewName("@OsVersion");
      Variable varModel = vars[4].CloneWithNewName("@Model");
      Variable varMaxTextureSize = vars[5].CloneWithNewName("@MaxTextureSize");
      Variable varPixelsMaxX = vars[6].CloneWithNewName("@PixelsMaxX");
      Variable varPixelsMaxY = vars[7].CloneWithNewName("@PixelsMaxY");

      if (varDeviceToken.Value is null || varDeviceToken.Value == "")
      {
        varDeviceToken.Value = SecureHasherV1.Hash(Guid.NewGuid().ToString() + Guid.NewGuid().ToString()); //TWICE AS RANDOM!!!!  :)
        
        Variable recordsAffected = Database.Execute("INSERT INTO UserDevices (DeviceToken, AppVersion, OsFamily, OsVersion, Model, MaxTextureSize, PixelsMaxX, PixelsMaxY) VALUES (@DeviceToken, @AppVersion, @OsFamily, @OsVersion, @Model, @MaxTextureSize, @PixelsMaxX, @PixelsMaxY)", varDeviceToken, varAppVersion, varOsFamily, varOsVersion, varModel, varMaxTextureSize, varPixelsMaxX, varPixelsMaxY);
      }
      else
      {
        Variable records = Database.Select("SELECT UserDeviceId, UserId FROM UserDevices WHERE deviceToken = @DeviceToken", varDeviceToken);
        if (records.SubVariables.Count <= 0) //No record, means deviceToken was not found in the database
        {
          varDeviceToken.Value = SecureHasherV1.Hash(Guid.NewGuid().ToString() + Guid.NewGuid().ToString()); //TWICE AS RANDOM!!!!  :)

          Variable recordsAffected = Database.Execute("INSERT INTO UserDevices (DeviceToken, AppVersion, OsFamily, OsVersion, Model, MaxTextureSize, PixelsMaxX, PixelsMaxY) VALUES (@DeviceToken, @AppVersion, @OsFamily, @OsVersion, @Model, @MaxTextureSize, @PixelsMaxX, @PixelsMaxY)", varDeviceToken, varAppVersion, varOsFamily, varOsVersion, varModel, varMaxTextureSize, varPixelsMaxX, varPixelsMaxY);

          Log?.Write($"Session.DeviceRegister - found token [{varDeviceToken.Value}]", LOG_TYPE.INF);
          return RESP.SetSuccess(varDeviceToken.CloneWithNewName("DeviceToken"));
        }
        Log?.Write($"Session.DeviceRegister - Valid session [{varDeviceToken.Value}]", LOG_TYPE.INF);
      }


      return RESP.SetSuccess(varDeviceToken.CloneWithNewName("DeviceToken"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP CheckDevice(Core.Flow flow, Variable[] vars)
    {
      return RESP.SetError(2, "Device register failed");
    }


    /// <summary>
    /// Check if the session token is valid and return the user Id and expiration for that session
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP CheckSession(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Session.CheckSession");
      if (Database is null)
        return RESP.SetError(1, "No active database connection");

      vars[0].GetValue(out string sessionToken);

      //Check if the LoginId already exists in the database
      Variable varSessionToken = new Variable("@SessToken", sessionToken); //Need to name the varialbe '@LoginId' to be used in the SQL
      Variable records = Database.Select("SELECT UserId, Expiration FROM UserSessions WHERE SessionToken = @SessToken", varSessionToken);
      if (records.SubVariables.Count <= 0) //No record, means sessionToken was not found in the database
      {
        Log?.Write($"Session.CheckSession - INVALID session [{sessionToken}]", LOG_TYPE.INF);
        return RESP.SetError(ERROR_INVALID_SESSION, $"Invalid session [{sessionToken}]");
      }
      Log?.Write($"Session.CheckSession - Valid session [{sessionToken}]", LOG_TYPE.INF);

      return RESP.SetSuccess(records);
    }
    
  }
}