using FlowEngineCore;
using FlowEngineCore.Interfaces;
using MySqlX.XDevAPI.Relational;
using System;
using System.Drawing;

namespace Session
{
    public class Session : FlowEngineCore.Plugin
  {
    private IDatabase? Database;
    private const uint USER_REGISTER_DUPLICATE_OUTPUT = 1;
    private const uint USER_REGISTER_GENERIC_ERROR_OUTPUT = 2;

    private const int ERROR_INSERT_DB = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 1;
    //private const int ERROR_DUPLICATE_LOGIN_ID = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 2;
    private const int ERROR_INVALID_SESSION = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 3;
    private const int ERROR_BAD_CREDENTIALS = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 4;
    //private const int ERROR_BAD_PASSWORD = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 5;
    private const int ERROR_DB = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 6;
    private const int ERROR_ACCOUNT_LOCKED_OUT = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 7;


    public override void Init()
    {
      base.Init();

      Function function = new("User Register", this, UserRegister);
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.Parms.Add("Password", DATA_TYPE.String);
      function.OutputAddSuccess();
      function.OutputAdd("Duplicate");  //USER_REGISTER_DUPLICATE
      function.OutputAddError();        //USER_REGISTER_ERROR
      Functions.Add(function);

      function = new Function("User Login", this, UserLogin);
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.Parms.Add("Password", DATA_TYPE.String);
      function.DefaultSaveResponseVariable = true;
      function.RespNames.Name = "sessionInfo";
      function.RespNames.SubVariableAdd(new Variable("session", ""));
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
        mSettings.SettingAdd(new Setting("LoginAttemptsBeforeLock", 3));
        mSettings.SettingAdd(new Setting("LockAccountMinutes", 30));
        mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Teal));
        mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));
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
        mLog?.Write("No shared database connection pool found. Session plugin will not work!", LOG_TYPE.ERR);
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
    public RESP UserRegister(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.UserRegister", LOG_TYPE.DBG);
      if (Database is null)
        return RESP.SetError(1, "No active database connection", USER_REGISTER_GENERIC_ERROR_OUTPUT);

      vars[0].GetValue(out string loginId);
      vars[1].GetValue(out string password);

      //Check if the LoginId already exists in the database
      Variable varLoginId = new("@LoginId", loginId); //Need to name the varialbe '@LoginId' to be used in the SQL
      Variable records = Database.Select("SELECT UserId FROM Users Where LoginId = @LoginId", varLoginId);
      if (records.SubVariables.Count > 0) //Has a record, means it found a user with the same LoginId
      {
        mLog?.Write("Session.UserRegister - Duplicate login Id", LOG_TYPE.WAR);
        return RESP.SetError((int)USER_REGISTER_DUPLICATE_OUTPUT, $"Duplicate LoginId [{loginId}]", USER_REGISTER_DUPLICATE_OUTPUT); //User already exists in the database, return error.
      }

      //LoginId doesn't exist, lets hash the password and insert it into the database
      string passwordHash = SecureHasherV1.Hash(password);
      Variable recordsAffected = Database.Execute("INSERT INTO Users (LoginId, Password) VALUES (@LoginId, @Password)", varLoginId, new Variable("@Password", passwordHash));
      if (recordsAffected.Value > 0 && recordsAffected.SubVariables.Count > 0)
      {
        int NEW_DB_ID_INDEX = 0;
        recordsAffected[NEW_DB_ID_INDEX].GetValue(out long newDbId); //The only sub variable will be the new Id of the record that was inserted
        Variable userDb = Database.Select("SELECT * FROM Users WHERE UserId = @UserId", new Variable("@UserId", newDbId));
        if (userDb.SubVariables.Count > 0)
        {
          userDb = userDb.SubVariables[0]; //We just want to return the user data, not an array of a single user
          return RESP.SetSuccess(userDb);
        }
        return RESP.SetError(ERROR_DB, "Failed to retrieve user from database");
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
    public RESP UserLogin(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.UserLogin", LOG_TYPE.DBG);

      if (Database is null)
        return RESP.SetError(1, "No active database connection", USER_REGISTER_GENERIC_ERROR_OUTPUT);

      vars[0].GetValue(out string loginId);
      vars[1].GetValue(out string password);
      Variable varLoginId = new("@LoginId", loginId); //Need to name the varialbe '@LoginId' to be used in the SQL
      Variable records = Database.Select("SELECT UserId, Password, LoginAttempts, LockedUntil, NOW() AS CurrentTime FROM Users Where LoginId = @LoginId", varLoginId);
      if (records.SubVariables.Count == 0) //No record found
      {
        mLog?.Write("Session.UserLogin - login Id does not exist", LOG_TYPE.INF);
        return RESP.SetError(ERROR_BAD_CREDENTIALS, $"Bad credentials", (int)Output.TYPE.Error); //Don't want to return what is wrong, either user id or password (make life more difficult for hackers)
      }
      if (records[0].Count < 5)
      {
        mLog?.Write("Session.UserLogin - DB returned the wrong number of fields", LOG_TYPE.INF);
        return RESP.SetError(ERROR_DB, $"DB returned the wrong number of fields [{loginId}]", (int)Output.TYPE.Error);
      }
      Variable varUserId = records[0]["UserId"];
      records[0]["LoginAttempts"].GetValue(out long loginAttempts);
      records[0]["LockedUntil"].GetValue(out string accountLockedUntilStr);
      records[0]["CurrentTime"].GetValue(out string currentDbTimeStr);
      if (accountLockedUntilStr is not null && accountLockedUntilStr != "")
      {
        bool lockedBool = DateTime.TryParse(accountLockedUntilStr, out DateTime lockedUntil);
        bool currentTimeBool = DateTime.TryParse(currentDbTimeStr, out DateTime currentDbTime);
        if (lockedBool == false || currentTimeBool == false)
        {
          mLog?.Write("Session.UserLogin - Unable to parse DateTime from Db", LOG_TYPE.INF);
          return RESP.SetError(ERROR_DB, $"Unable to parse DateTime from Db [{loginId}]", (int)Output.TYPE.Error);
        }
        if (lockedUntil > currentDbTime)
        {
          mLog?.Write($"Session.UserLogin - Account locked out until [{lockedUntil}]", LOG_TYPE.INF);
          return RESP.SetError(ERROR_ACCOUNT_LOCKED_OUT, $"Account locked out until [{lockedUntil}]", (int)Output.TYPE.Error);
        }
      }

      records[0]["Password"].GetValue(out string passwordDb);
      if (SecureHasherV1.Verify(password, passwordDb) == false) //Check if the password matches the database
      {
        if (loginAttempts < mSettings.SettingGetAsInt("LoginAttemptsBeforeLock"))
        {
          Database.Execute("UPDATE Users SET LoginAttempts = LoginAttempts + 1 WHERE UserId = @UserId", varUserId);
        }
        else
        {
          Variable varLoginAttempts = new("@LoginAttempts", mSettings.SettingGetAsInt("LoginAttemptsBeforeLock"));
          Variable varMinutesToLock = new("@LockAccountMinutes", mSettings.SettingGetAsInt("LockAccountMinutes"));
          Database.Execute("UPDATE Users SET LoginAttempts = @LoginAttempts, LockedUntil = Now() + interval @LockAccountMinutes MINUTE WHERE UserId = @UserId", varLoginAttempts, varMinutesToLock, varUserId);
        }
        mLog?.Write("Session.UserLogin - Bad Password", LOG_TYPE.INF);
        return RESP.SetError(ERROR_BAD_CREDENTIALS, $"Bad credentials", (int)Output.TYPE.Error); //Don't want to return what is wrong, either user id or password (make life more difficult for hackers)
      }

      //We got this far, the loginId is good, and the password is good, lets reset the locks
      Database.Execute("UPDATE Users SET LoginAttempts = 0, LockedUntil = NULL WHERE UserId = @UserId", varUserId);

      //Database.Execute("INSERT INTO UserSessions (UserId, DeviceId, SessionToken) VALUES (@UserId, @DeviceId, @SessionToken)", varUserId);
      return RESP.SetSuccess(varUserId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP UserLogout(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.UserLogout", LOG_TYPE.DBG);
      return RESP.SetError(2, "User failed to log out");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP UserLogoutAll(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.UserLogoutAll", LOG_TYPE.DBG);
      return RESP.SetError(2, "User failed to log out all");
    }

    /// <summary>
    /// Register a new device
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP DeviceRegister(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.DeviceRegister", LOG_TYPE.DBG);
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
        mLog?.Write($"Session.DeviceRegister - Existing device token is blank, creating a new one", LOG_TYPE.DBG);
        varDeviceToken.Value = SecureHasherV1.SessionIdCreate();
        
        Variable recordsAffected = Database.Execute("INSERT INTO UserDevices (DeviceToken, AppVersion, OsFamily, OsVersion, Model, MaxTextureSize, PixelsMaxX, PixelsMaxY) VALUES (@DeviceToken, @AppVersion, @OsFamily, @OsVersion, @Model, @MaxTextureSize, @PixelsMaxX, @PixelsMaxY)", varDeviceToken, varAppVersion, varOsFamily, varOsVersion, varModel, varMaxTextureSize, varPixelsMaxX, varPixelsMaxY);
        if (recordsAffected.Value > 0)
        {
          mLog?.Write($"Session.DeviceRegister - Created a new token [{varDeviceToken.Value}]", LOG_TYPE.INF);
          return RESP.SetSuccess(varDeviceToken.CloneWithNewName("DeviceToken"));
        }
        else
        {
          mLog?.Write($"Session.DeviceRegister - Failed inserting a new record into the DB", LOG_TYPE.ERR);
          return RESP.SetError(ERROR_DB, "Failed inserting a new record into the DB");
        }
      }
      else
      {
        Variable records = Database.Select("SELECT UserDeviceId, UserId FROM UserDevices WHERE deviceToken = @DeviceToken", varDeviceToken);
        if (records.SubVariables.Count <= 0) //No record, means deviceToken was not found in the database
        {
          mLog?.Write($"Session.DeviceRegister - Existing token wasn't found [{vars[0].Value}]", LOG_TYPE.DBG);

          varDeviceToken.Value = SecureHasherV1.SessionIdCreate();

          Variable recordsAffected = Database.Execute("INSERT INTO UserDevices (DeviceToken, AppVersion, OsFamily, OsVersion, Model, MaxTextureSize, PixelsMaxX, PixelsMaxY) VALUES (@DeviceToken, @AppVersion, @OsFamily, @OsVersion, @Model, @MaxTextureSize, @PixelsMaxX, @PixelsMaxY)", varDeviceToken, varAppVersion, varOsFamily, varOsVersion, varModel, varMaxTextureSize, varPixelsMaxX, varPixelsMaxY);

          if (recordsAffected.Value > 0)
          {
            mLog?.Write($"Session.DeviceRegister - Existing token wasn't found [{vars[0].Value}], created a new token [{varDeviceToken.Value}]", LOG_TYPE.INF);
            return RESP.SetSuccess(varDeviceToken.CloneWithNewName("DeviceToken"));
          }
          else
          {
            mLog?.Write($"Session.DeviceRegister - Existing token wasn't found [{vars[0].Value}], Failed inserting a new record into the DB", LOG_TYPE.ERR);
            return RESP.SetError(ERROR_DB, "Failed inserting a new record into the DB");
          }
        }
        mLog?.Write($"Session.DeviceRegister - Valid device token [{varDeviceToken.Value}]", LOG_TYPE.INF);
      }


      return RESP.SetSuccess(varDeviceToken.CloneWithNewName("DeviceToken"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP CheckDevice(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.UserLogout", LOG_TYPE.DBG);
      return RESP.SetError(2, "Device register failed");
    }


    /// <summary>
    /// Check if the session token is valid and return the user Id and expiration for that session
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP CheckSession(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.CheckSession", LOG_TYPE.DBG);
      if (Database is null)
        return RESP.SetError(1, "No active database connection");

      vars[0].GetValue(out string sessionToken);

      //Check if the LoginId already exists in the database
      Variable varSessionToken = new("@SessToken", sessionToken); //Need to name the varialbe '@LoginId' to be used in the SQL
      Variable records = Database.Select("SELECT UserId, Expiration FROM UserSessions WHERE SessionToken = @SessToken", varSessionToken);
      if (records.SubVariables.Count <= 0) //No record, means sessionToken was not found in the database
      {
        mLog?.Write($"Session.CheckSession - INVALID session [{sessionToken}]", LOG_TYPE.INF);
        return RESP.SetError(ERROR_INVALID_SESSION, $"Invalid session [{sessionToken}]");
      }
      mLog?.Write($"Session.CheckSession - Valid session [{sessionToken}]", LOG_TYPE.INF);

      return RESP.SetSuccess(records);
    }
    
  }
}