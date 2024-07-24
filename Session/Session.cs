using FlowEngineCore;
using FlowEngineCore.Administration.Messages;
using FlowEngineCore.Interfaces;
using MySqlX.XDevAPI.Relational;
using System;
using System.Drawing;
using System.Security.Cryptography;

namespace Session
{
  public class Session : FlowEngineCore.Plugin
  {
    private IDatabase? Database;
    private const uint OUTPUT_USER_REGISTER_GENERIC_ERROR = 1;
    private const uint OUTPUT_USER_REGISTER_DUPLICATE = 2;

    private const int ERROR_INSERT_DB = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 1;
    private const int ERROR_DUPLICATE_LOGIN_ID = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 2;
    private const int ERROR_INVALID_SESSION = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 3;
    private const int ERROR_BAD_CREDENTIALS = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 4;
    private const int ERROR_INVALID_DEVICE_TOKEN = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 5;
    private const int ERROR_DB = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 6;
    private const int ERROR_ACCOUNT_LOCKED_OUT = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 7;
    private const int ERROR_NO_DB_CONNECTION = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 8;
    private const int ERROR_PASSWORD_RESET_BAD_LOGIN_CODE = (int)STEP_ERROR_NUMBERS.SessionErrorMin + 9;

    Random rand = new Random();
    private const string RESET_PASSWORD_CHARACTERS = "ABCDEFGHJKLMNPQRSTWXYZ23456789"; //removed letters/numbers 'I', '1', 'O', '0' so people don't get confused
    public override void Init()
    {
      base.Init();

      // Create a session info Variable with a bunch of sub variables, this is used for a bunch of functions, this will make sure they have the same output signature.
      // Used in 'User Register', 'User Login', 'Check Session', & 'Change Password', all of these return a sessionInfo block
      Variable sessionInfoWithUserId = new Variable("sessionInfo");
      sessionInfoWithUserId.SubVariableAdd(new Variable("userId", 0L));
      sessionInfoWithUserId.SubVariableAdd(new Variable("loginId", ""));
      sessionInfoWithUserId.SubVariableAdd(new Variable("statusId", 0L));
      sessionInfoWithUserId.SubVariableAdd(new Variable("sessionToken", ""));
      sessionInfoWithUserId.SubVariableAdd(new Variable("sessionExpiration", ""));
      sessionInfoWithUserId.SubVariableAdd(new Variable("deviceToken", ""));

      Variable sessionInfo = sessionInfoWithUserId.Clone();
      sessionInfo.SubVariableDeleteByName("userId");

      Function function = new("User Register", this, UserRegister);
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.Parms.Add("Password", DATA_TYPE.String);
      PARM parm = function.Parms.Add("Device", DATA_TYPE.Various);
      parm.Description = "This parameter could be a Device ID (Database ID from 'Session.Device Register' or 'Session.Check Device') or device token directly from the inbound request.";
      parm.AllowMultiple = PARM.PARM_ALLOW_MULTIPLE.Single;
      function.OutputAdd("Duplicate");  //USER_REGISTER_DUPLICATE
      function.DefaultSaveResponseVariable = true;
      function.RespNames = sessionInfo;
      Functions.Add(function);

      function = new Function("User Login", this, UserLogin);
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.Parms.Add("Password", DATA_TYPE.String);
      parm = function.Parms.Add("Device", DATA_TYPE.Various);
      parm.Description = "This parameter could be a Device ID (Database ID from 'Session.Device Register' or 'Session.Check Device') or device token directly from the inbound request.";
      parm.AllowMultiple = PARM.PARM_ALLOW_MULTIPLE.Single;
      function.DefaultSaveResponseVariable = true;
      function.RespNames = sessionInfo;
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
      function.DefaultSaveResponseVariable = true;
      function.RespNames.Name = "deviceId";
      Functions.Add(function);

      function = new Function("Check Device", this, CheckDevice);
      function.Parms.Add("DeviceToken", DATA_TYPE.String);
      function.DefaultSaveResponseVariable= true;
      function.RespNames.Name = "deviceId";
      Functions.Add(function);

      function = new Function("Check Session", this, CheckSession);
      function.Parms.Add("SessionToken", DATA_TYPE.String);
      function.DefaultSaveResponseVariable = true;
      function.RespNames = sessionInfoWithUserId;
      Functions.Add(function);

      function = new Function("Change Password", this, ChangePassword);
      function.Parms.Add("Session Token", DATA_TYPE.String);
      function.Parms.Add("Old password", DATA_TYPE.String);
      function.Parms.Add("New password", DATA_TYPE.String);
      function.DefaultSaveResponseVariable = true;
      function.RespNames = sessionInfoWithUserId;
      Functions.Add(function);

      function = new Function("User Forgot Password", this, UserForgotPassword);
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.DefaultSaveResponseVariable = true;
      function.RespNames.Name = "PasswordResetCode";
      Functions.Add(function);

      function = new Function("User Password Reset", this, UserPasswordReset, "Set a new password with code", "Used to reset a users password after they receive the reset code provided by 'User Forgot Password'");
      function.Parms.Add("LoginId", DATA_TYPE.String);
      function.Parms.Add("Code", DATA_TYPE.String);
      function.Parms.Add("New Password", DATA_TYPE.String);
      function.Parms.Add("Device", DATA_TYPE.Various);
      function.DefaultSaveResponseVariable = true;
      function.RespNames.Name = "sessionInfo";
      Functions.Add(function);

      //SETTINGS
      {
        mSettings.SettingAdd(new Setting("LoginAttemptsBeforeLock", 3));
        mSettings.SettingAdd(new Setting("LockAccountMinutes", 30));
        mSettings.SettingAdd(new Setting("ForgotPasswordCodeLength", 6));
        mSettings.SettingAdd(new Setting("ForgotPasswordCodeMinutes", 30));
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

    public RESP UserPasswordReset(FlowEngineCore.Flow flow, Variable[] vars)
    {
      if (Database is null)
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      string? deviceToken = null;
      long? deviceId = null;

      Variable loginId = vars[0].CloneWithNewName("@LoginId");
      Variable code = vars[1].CloneWithNewName("@Code");
      Variable newPassword = vars[2].CloneWithNewName("@NewPassword");

      if (vars[3].DataType == DATA_TYPE.String) //It must be a device token straight from the request data
      {
        vars[3].GetValue(out deviceToken);
      }
      else if (vars[3].DataType == DATA_TYPE.Integer)
      {
        vars[3].GetValue(out long tempDeviceId);
        deviceId = tempDeviceId;
      }
      else
      {
        return RESP.SetError(ERROR_INVALID_DEVICE_TOKEN, "'Device' Variable is not set");
      }


      Variable dbUserId = Database.SelectId("SELECT UserId FROM UserForgotPasswordReset WHERE Expiration > NOW() AND LoginId = @LoginId AND Code = @Code", loginId, code);
      if (dbUserId.Value <= 0)
        return RESP.SetError(ERROR_PASSWORD_RESET_BAD_LOGIN_CODE, "Could not find password reset entry with provided LoginId and Code.");
      dbUserId.Name = "@UserId";

      if (deviceId is not null)
      {
        Variable deviceIdVar = new("@UserDeviceID", deviceId);
        Database.Execute("UPDATE UserDevices SET UserId = @UserId WHERE UserDeviceId = @UserDeviceID", dbUserId, deviceIdVar);
        Variable token = Database.SelectId("SELECT DeviceToken FROM UserDevices WHERE UserDeviceId = @UserDeviceID", deviceIdVar);
        if (token.DataType == DATA_TYPE.String)
          deviceToken = token.Value;
      }
      else if (deviceToken is not null)
      {
        Variable deviceTokenVar = new("@DeviceToken", deviceToken);
        Database.Execute("UPDATE UserDevices SET UserId = @UserId WHERE DeviceToken = @DeviceToken", dbUserId, deviceTokenVar);
        Variable id = Database.SelectId("SELECT UserDeviceId FROM UserDevices WHERE DeviceToken = @DeviceToken", deviceTokenVar);
        if (id.DataType == DATA_TYPE.Integer)
          deviceId = id.Value;
      }


      //Remove all the existing user sessions, this
      Variable recordsAffected = Database.Execute("DELETE FROM UserSessions WHERE UserId = @UserId", dbUserId);
      if (recordsAffected.Value > 0)
        mLog?.Write($"Deleted [{recordsAffected.Value}] session User duplicates from UserSessions with UserId [{dbUserId.GetValueAsString()}], only one user can be signed into a device");

      string sessionToken = SecureHasherV1.SessionIdCreate();
      recordsAffected = Database.Execute("INSERT INTO UserSessions (UserId, DeviceId, SessionToken) VALUES (@UserId, @DeviceId, @SessionToken)", dbUserId, new Variable("@DeviceId", deviceId!), new Variable("@SessionToken", sessionToken));
      if (recordsAffected.Value == 0 || recordsAffected.SubVariables.Count == 0)
        return RESP.SetError(ERROR_INSERT_DB, "Failed to insert session into database");

      Variable? varSession = CheckSessionInternal(Database, new Variable("", sessionToken));
      if (varSession is null)
        return RESP.SetError(ERROR_DB, "Failed to retrieve session from database");

      Variable sessionInfo = new Variable("sessionInfo");
      sessionInfo.SubVariableAdd(new Variable("loginId", varSession[1].Value));
      sessionInfo.SubVariableAdd(new Variable("statusId", varSession[2].Value));
      sessionInfo.SubVariableAdd(new Variable("sessionToken", sessionToken));
      sessionInfo.SubVariableAdd(new Variable("sessionExpiration", varSession[4].Value));
      sessionInfo.SubVariableAdd(new Variable("deviceToken", deviceToken!));


      return RESP.SetSuccess(sessionInfo);
    }

    /// <summary>
    /// User forgot their password, so we will send them a code to their verified communication channel (email or text message).
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP UserForgotPassword(FlowEngineCore.Flow flow, Variable[] vars)
    {
      if (Database is null)
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      Variable loginId = vars[0].CloneWithNewName("@LoginId");

      //Check if the loginid exists
      Variable records = Database.Select("SELECT UserId FROM Users Where LoginId = @LoginId", loginId);
      if (records.SubVariables.Count == 0)
        return RESP.SetError(ERROR_BAD_CREDENTIALS, "LoginId does not exist");

      int codeLength = mSettings.SettingGetAsInt("ForgotPasswordCodeLength");
      int codeMinutes = mSettings.SettingGetAsInt("ForgotPasswordCodeMinutes");

      //Generate a random code
      string code = "";
      for (int x = 0; x < codeLength; x++)
      {
        int index = RandomNumberGenerator.GetInt32(0, RESET_PASSWORD_CHARACTERS.Length);
        code += RESET_PASSWORD_CHARACTERS[index];
      }

      //Clean up and insert the new record
      Variable recordsAffected = Database.Execute("DELETE FROM UserForgotPasswordReset WHERE LoginId = @LoginId", loginId);
      recordsAffected = Database.Execute($"INSERT INTO UserForgotPasswordReset (LoginId, Code, Expiration) VALUES (@LoginId, @Code, (current_timestamp() + interval {codeMinutes} minute))", loginId, new Variable("@Code", code));
      if (recordsAffected.Value == 0 || recordsAffected.SubVariables.Count == 0)
        return RESP.SetError(ERROR_INSERT_DB, "Failed to insert user password reset into database");

      return RESP.SetSuccess(new Variable("PasswordResetCode", code));
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
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      string? deviceToken = null;
      long? deviceId = null;

      vars[0].GetValue(out string loginId);
      vars[1].GetValue(out string password);

      if (vars[2].DataType == DATA_TYPE.String) //It must be a device token straight from the request data
      {
        vars[2].GetValue(out deviceToken);
      }
      else if (vars[2].DataType == DATA_TYPE.Integer)
      {
        vars[2].GetValue(out long tempDeviceId);
        deviceId = tempDeviceId;
      }
      else
      {
        return RESP.SetError(ERROR_INVALID_DEVICE_TOKEN, "'Device' Variable is not set");
      }

      //Check if the LoginId already exists in the database
      Variable varLoginId = new("@LoginId", loginId); //Need to name the varialbe '@LoginId' to be used in the SQL
      Variable records = Database.Select("SELECT UserId FROM Users Where LoginId = @LoginId", varLoginId);
      if (records.SubVariables.Count > 0) //Has a record, means it found a user with the same LoginId
      {
        //mLog?.Write("Session.UserRegister - Duplicate login Id", LOG_TYPE.WAR); //TODO: Send email to registered user (if email is verified)
        return RESP.SetError(ERROR_DUPLICATE_LOGIN_ID, $"Duplicate LoginId [{loginId}]", OUTPUT_USER_REGISTER_DUPLICATE); //User already exists in the database, return error.
      }

      //LoginId doesn't exist, lets hash the password and insert it into the database
      string passwordHash = SecureHasherV1.Hash(password);
      Variable recordsAffected = Database.Execute("INSERT INTO Users (LoginId, Password) VALUES (@LoginId, @Password)", varLoginId, new Variable("@Password", passwordHash));
      if (recordsAffected.Value == 0 || recordsAffected.SubVariables.Count == 0)
        return RESP.SetError(ERROR_INSERT_DB, "Failed to insert user into database");

      //
      int NEW_DB_ID_INDEX = 0;
      recordsAffected[NEW_DB_ID_INDEX].GetValue(out long newDbId); //The only sub variable will be the new Id of the record that was inserted
      Variable userDb = Database.Select("SELECT LoginId, StatusId FROM Users WHERE UserId = @UserId", new Variable("@UserId", newDbId));
      if (userDb.SubVariables.Count == 0)
        return RESP.SetError(ERROR_DB, "Failed to retrieve user from database");

      if (deviceId is not null)
      {
        Variable deviceIdVar = new("@UserDeviceID", deviceId);
        Database.Execute("UPDATE UserDevices SET UserId = @UserId WHERE UserDeviceId = @UserDeviceID", new Variable("@UserId", newDbId), deviceIdVar);
        Variable token = Database.SelectId("SELECT DeviceToken FROM UserDevices WHERE UserDeviceId = @UserDeviceID", deviceIdVar);
        if (token.DataType == DATA_TYPE.String)
          deviceToken = token.Value;
      }
      else if (deviceToken is not null)
      {
        Variable deviceTokenVar = new("@DeviceToken", deviceToken);
        Database.Execute("UPDATE UserDevices SET UserId = @UserId WHERE DeviceToken = @DeviceToken", new Variable("@UserId", newDbId), deviceTokenVar);
        Variable id = Database.SelectId("SELECT UserDeviceId FROM UserDevices WHERE DeviceToken = @DeviceToken", deviceTokenVar);
        if (id.DataType == DATA_TYPE.Integer)
          deviceId = id.Value;
      }

      //Only a single user can be logged into a device, so delete any other users that were using this device
      recordsAffected = Database.Execute("DELETE FROM UserSessions WHERE DeviceId = @DeviceId", new Variable("@DeviceId", deviceId!));
      if (recordsAffected.Value > 0)
        mLog?.Write($"Deleted [{recordsAffected.Value}] session device duplicates from UserDevices with DeviceID [{deviceId}], only one user can be signed into a device");

      string sessionToken = SecureHasherV1.SessionIdCreate();
      recordsAffected = Database.Execute("INSERT INTO UserSessions (UserId, DeviceId, SessionToken) VALUES (@UserId, @DeviceId, @SessionToken)", new Variable("@UserId", newDbId), new Variable("@DeviceId", deviceId!), new Variable("@SessionToken", sessionToken));
      if (recordsAffected.Value == 0 || recordsAffected.SubVariables.Count == 0)
        return RESP.SetError(ERROR_INSERT_DB, "Failed to insert session into database");

      Variable? varSession = CheckSessionInternal(Database, new Variable("", sessionToken));
      if (varSession is null)
        return RESP.SetError(ERROR_DB, "Failed to retrieve session from database");

      userDb.Name = "sessionInfo";
      userDb.SubVariables.Clear();
      userDb.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Block;
      userDb.SubVariableAdd(new Variable("loginId", varSession[1].Value));
      userDb.SubVariableAdd(new Variable("statusId", varSession[2].Value));
      userDb.SubVariableAdd(new Variable("sessionToken", sessionToken));
      userDb.SubVariableAdd(new Variable("sessionExpiration", varSession[4].Value));
      userDb.SubVariableAdd(new Variable("deviceToken", deviceToken!));

      return RESP.SetSuccess(userDb);
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
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      string? deviceToken = null;
      long? deviceId = null;

      vars[0].GetValue(out string loginId);
      vars[1].GetValue(out string password);
      if (vars[2].DataType == DATA_TYPE.String) //It must be a device token straight from the request data
      {
        vars[2].GetValue(out deviceToken);
      }
      else if (vars[2].DataType == DATA_TYPE.Integer)
      {
        vars[2].GetValue(out long tempDeviceId);
        deviceId = tempDeviceId;
      }
      else
      {
        return RESP.SetError(ERROR_DB, "'Device' Variable is not set");
      }


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
          mLog?.Write($"Session.UserLogin - Account locked out until [{accountLockedUntilStr}]", LOG_TYPE.INF);
          return RESP.SetError(ERROR_ACCOUNT_LOCKED_OUT, $"Account locked out until [{accountLockedUntilStr}]", (int)Output.TYPE.Error);
        }
        else
        {
          loginAttempts = 0;
          Database.Execute("UPDATE Users SET LoginAttempts = 0, LockedUntil = NULL WHERE UserId = @UserId", varUserId); //Reset to give the user X number of attempts again
        }
      }

      records[0]["Password"].GetValue(out string passwordDb);
      if (SecureHasherV1.Verify(password, passwordDb) == false) //Check if the password matches the database
      {
        loginAttempts++;
        if (loginAttempts < mSettings.SettingGetAsInt("LoginAttemptsBeforeLock"))
        {
          Database.Execute("UPDATE Users SET LoginAttempts = LoginAttempts + 1 WHERE UserId = @UserId", varUserId);
        }
        else
        {
          Variable varLoginAttempts = new("@LoginAttempts", mSettings.SettingGetAsInt("LoginAttemptsBeforeLock"));
          Variable varMinutesToLock = new("@LockAccountMinutes", mSettings.SettingGetAsInt("LockAccountMinutes"));
          Database.Execute("UPDATE Users SET LoginAttempts = @LoginAttempts, LockedUntil = Now() + interval @LockAccountMinutes MINUTE WHERE UserId = @UserId", varLoginAttempts, varMinutesToLock, varUserId);
          mLog?.Write($"Session.UserLogin - Bad credentials, Locking Account", LOG_TYPE.INF);
          return RESP.SetError(ERROR_ACCOUNT_LOCKED_OUT, $"Account locked", (int)Output.TYPE.Error);
        }
        mLog?.Write("Session.UserLogin - Bad Password", LOG_TYPE.INF);
        return RESP.SetError(ERROR_BAD_CREDENTIALS, $"Bad credentials", (int)Output.TYPE.Error); //Don't want to return what is wrong, either user id or password (make life more difficult for hackers)
      }

      //We got this far, the loginId is good, and the password is good, lets reset the locks
      Database.Execute("UPDATE Users SET LoginAttempts = 0, LockedUntil = NULL, LastLogin = NOW() WHERE UserId = @UserId", varUserId);

      if (deviceId is not null)
      {
        Variable deviceIdVar = new("@UserDeviceID", deviceId);
        Database.Execute("UPDATE UserDevices SET UserId = @UserId WHERE UserDeviceId = @UserDeviceID", varUserId, deviceIdVar);
        Variable token = Database.SelectId("SELECT DeviceToken FROM UserDevices WHERE UserDeviceId = @UserDeviceID", deviceIdVar);
        if (token.DataType == DATA_TYPE.String)
          deviceToken = token.Value;
      }
      else if (deviceToken is not null)
      {
        Variable deviceTokenVar = new("@DeviceToken", deviceToken);
        Database.Execute("UPDATE UserDevices SET UserId = @UserId WHERE DeviceToken = @DeviceToken", varUserId, deviceTokenVar);
        Variable id = Database.SelectId("SELECT UserDeviceId FROM UserDevices WHERE DeviceToken = @DeviceToken", deviceTokenVar);
        if (id.DataType == DATA_TYPE.Integer)
          deviceId = id.Value;
      }


      //Only a single user can be logged into a device, so delete any other users that were using this device
      Variable recordsAffected = Database.Execute("DELETE FROM UserSessions WHERE DeviceId = @DeviceId", new Variable("@DeviceId", deviceId!));
      if (recordsAffected.Value > 0)
        mLog?.Write($"Deleted [{recordsAffected.Value}] session device duplicates from UserDevices with DeviceID [{deviceId}], only one user can be signed into a device");

      string sessionToken = SecureHasherV1.SessionIdCreate();
      recordsAffected = Database.Execute("INSERT INTO UserSessions (UserId, DeviceId, SessionToken) VALUES (@UserId, @DeviceId, @SessionToken)", varUserId, new Variable("@DeviceId", deviceId!), new Variable("@SessionToken", sessionToken));
      if (recordsAffected.Value == 0 || recordsAffected.SubVariables.Count == 0)
        return RESP.SetError(ERROR_INSERT_DB, "Failed to insert session into database");

      Variable userDb = new Variable("sessionInfo");
      Variable? varSession = CheckSessionInternal(Database, new Variable("", sessionToken));
      if (varSession is null)
        return RESP.SetError(ERROR_DB, "Failed to retrieve session from database");

      userDb.SubVariableAdd(new Variable("loginId", varSession[1].Value));
      userDb.SubVariableAdd(new Variable("statusId", varSession[2].Value));
      userDb.SubVariableAdd(new Variable("sessionToken", sessionToken));
      userDb.SubVariableAdd(new Variable("sessionExpiration", varSession[4].Value));
      userDb.SubVariableAdd(new Variable("deviceToken", deviceToken!));

      return RESP.SetSuccess(userDb);
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
      if (Database is null)
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      Variable varSessionToken = vars[0].CloneWithNewName("@SessionToken");

      Variable recordsAffected = Database.Execute("DELETE FROM UserSessions WHERE SessionToken = @SessionToken", varSessionToken);
      if (recordsAffected.Value == 0)
        return RESP.SetError(2, "User failed to log out");

      mLog?.Write($"User Logged out, deleted [{recordsAffected.Value}] session from UserSessions with SessionToken [{varSessionToken.GetValueAsString()}]");

      return RESP.SetSuccess();
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
      if (Database is null)
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      Variable varSessionToken = vars[0].CloneWithNewName("@SessionToken");

      Variable records = Database.Select("SELECT UserId FROM UserSessions Where SessionToken = @SessionToken", varSessionToken);
      if (records.SubVariables.Count == 0) //Has a record, means it found a user
        return RESP.SetError(2, "User failed to log out");

      Variable varUserId = records[0][0].CloneWithNewName("@UserId"); //[0][0] first record, first column

      UserLogoutAllInternal(Database, varUserId);

      return RESP.SetSuccess();
    }

    private void UserLogoutAllInternal(IDatabase database, Variable varUserId)
    {
      varUserId.Name = "@UserId";
      Variable recordsAffected = database.Execute("DELETE FROM UserSessions WHERE UserId = @UserId", varUserId);
      if (recordsAffected.Value == 0)
        mLog?.Write($"FAILED to log the user out of all sessions, deleted [{recordsAffected.Value}] session(s) from UserSessions with UserId [{varUserId.GetValueAsString()}]", LOG_TYPE.WAR);

      mLog?.Write($"User Logged out all sessions, deleted [{recordsAffected.Value}] session(s) from UserSessions with UserId [{varUserId.GetValueAsString()}]");
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
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      Variable varDeviceToken = vars[0].CloneWithNewName("@DeviceToken");
      Variable varAppVersion = vars[1].CloneWithNewName("@AppVersion");
      Variable varOsFamily = vars[2].CloneWithNewName("@OsFamily");
      Variable varOsVersion = vars[3].CloneWithNewName("@OsVersion");
      Variable varModel = vars[4].CloneWithNewName("@Model");
      Variable varMaxTextureSize = vars[5].CloneWithNewName("@MaxTextureSize");
      Variable varPixelsMaxX = vars[6].CloneWithNewName("@PixelsMaxX");
      Variable varPixelsMaxY = vars[7].CloneWithNewName("@PixelsMaxY");

      if (varDeviceToken.Value is null || varDeviceToken.Value == "") //User has no device token, it is a new device, so create and insert
      {
        mLog?.Write($"Session.DeviceRegister - Existing device token is blank, creating a new one", LOG_TYPE.DBG);
        varDeviceToken.Value = SecureHasherV1.SessionIdCreate();
        
        Variable recordsAffected = Database.Execute("INSERT INTO UserDevices (DeviceToken, AppVersion, OsFamily, OsVersion, Model, MaxTextureSize, PixelsMaxX, PixelsMaxY) VALUES (@DeviceToken, @AppVersion, @OsFamily, @OsVersion, @Model, @MaxTextureSize, @PixelsMaxX, @PixelsMaxY)", varDeviceToken, varAppVersion, varOsFamily, varOsVersion, varModel, varMaxTextureSize, varPixelsMaxX, varPixelsMaxY);
        if (recordsAffected.Value > 0 && recordsAffected.SubVariables.Count > 0)
        {
          mLog?.Write($"Session.DeviceRegister - Created a new token [{varDeviceToken.Value}]", LOG_TYPE.INF);
          varDeviceToken.Name = "deviceToken";
          return RESP.SetSuccess(varDeviceToken);
        }
        else
        {
          mLog?.Write($"Session.DeviceRegister - Failed inserting a new record into the DB", LOG_TYPE.ERR);
          return RESP.SetError(ERROR_DB, "Failed inserting a new record into the DB");
        }
      }
      else //There is a device token already, if it exists update the info, if it doesn't exist create and insert a new one
      {
        Variable records = Database.Select("SELECT UserDeviceId FROM UserDevices WHERE deviceToken = @DeviceToken", varDeviceToken);
        if (records.SubVariables.Count <= 0) //No record, means deviceToken was not found in the database
        {
          mLog?.Write($"Session.DeviceRegister - Existing token wasn't found [{vars[0].Value}]", LOG_TYPE.DBG);

          varDeviceToken.Value = SecureHasherV1.SessionIdCreate();

          Variable recordsAffected = Database.Execute("INSERT INTO UserDevices (DeviceToken, AppVersion, OsFamily, OsVersion, Model, MaxTextureSize, PixelsMaxX, PixelsMaxY) VALUES (@DeviceToken, @AppVersion, @OsFamily, @OsVersion, @Model, @MaxTextureSize, @PixelsMaxX, @PixelsMaxY)", varDeviceToken, varAppVersion, varOsFamily, varOsVersion, varModel, varMaxTextureSize, varPixelsMaxX, varPixelsMaxY);

          if (recordsAffected.Value > 0 && recordsAffected.SubVariables.Count > 0)
          {
            mLog?.Write($"Session.DeviceRegister - Existing token wasn't found [{vars[0].Value}], created a new token [{varDeviceToken.Value}]", LOG_TYPE.INF);
            return RESP.SetSuccess(recordsAffected[0].CloneWithNewName("DeviceId"));
          }
          else
          {
            mLog?.Write($"Session.DeviceRegister - Existing token wasn't found [{vars[0].Value}], Failed inserting a new record into the DB", LOG_TYPE.ERR);
            return RESP.SetError(ERROR_DB, "Failed inserting a new record into the DB");
          }
        }
        else
        {
          Variable recordsAffected = Database.Execute("UPDATE UserDevices SET AppVersion = @AppVersion, OsFamily = @OsFamily, OsVersion = @OsVersion, Model = @Model, MaxTextureSize = @MaxTextureSize, PixelsMaxX = @PixelsMaxX, PixelsMaxY = @PixelsMaxY, LastUsedDateTime = current_timestamp() WHERE DeviceToken = @DeviceToken", varAppVersion, varOsFamily, varOsVersion, varModel, varMaxTextureSize, varPixelsMaxX, varPixelsMaxY, varDeviceToken);
          mLog?.Write($"Session.DeviceRegister - Valid device token [{varDeviceToken.Value}]", LOG_TYPE.INF);
          return RESP.SetSuccess(records[0][0].CloneWithNewName("DeviceId"));
        } 
      }
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
      if (Database is null)
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      //Check if the LoginId already exists in the database
      Variable varDeviceToken = vars[0].CloneWithNewName("@DeviceToken");
      Variable records = Database.Select("SELECT UserDeviceId FROM UserDevices WHERE DeviceToken = @DeviceToken", varDeviceToken);
      if (records.Count <= 0) //No record, means sessionToken was not found in the database
      {
        mLog?.Write($"Session.CheckDevice - INVALID session [{varDeviceToken.GetValueAsString()}]", LOG_TYPE.INF);
        return RESP.SetError(ERROR_INVALID_DEVICE_TOKEN, $"Invalid device token [{varDeviceToken.GetValueAsString()}]");
      }
      mLog?.Write($"Session.CheckDevice - Valid device token [{varDeviceToken.GetValueAsString()}]", LOG_TYPE.INF);

      return RESP.SetSuccess(new Variable("deviceId", records[0][0].Value));
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
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      Variable? records = CheckSessionInternal(Database, vars[0]);
      if (records is null)
        return RESP.SetError(ERROR_INVALID_SESSION, "Invalid sessionToken");

      return RESP.SetSuccess(records);
    }

    /// <summary>
    /// Checks if a session is valid
    /// </summary>
    /// <param name="database"></param>
    /// <param name="varSessionToken"></param>
    /// <returns>Contains 6 sub variables. 0:UserId, 1:LoginId, 2:StatusId, 3:SessionToken, 4:Expiration, 5:DeviceToken</returns>
    private Variable? CheckSessionInternal(IDatabase database, Variable varSessionToken)
    {
      string oldName = varSessionToken.Name;
      varSessionToken.Name = "@SessToken";
      Variable record = database.SelectOneRecord("SELECT UserId as userId, LoginId as loginId, StatusId as statusId, SessionToken as sessionToken, Expiration as expiration, DeviceToken as deviceToken FROM viewUserSession WHERE Expiration > current_timestamp() AND SessionToken = @SessToken", varSessionToken);
      if (record.SubVariables.Count <= 0) //No record, means sessionToken was not found in the database
      {
        mLog?.Write($"Session.CheckSession - INVALID session [{varSessionToken.GetValueAsString()}]", LOG_TYPE.INF);
        varSessionToken.Name = oldName;
        return null;
      }
      mLog?.Write($"Session.CheckSession - Valid session [{varSessionToken.GetValueAsString()}]", LOG_TYPE.INF);
      varSessionToken.Name = oldName;

      return record;
    }

    public RESP ChangePassword(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Session.ChangePassword", LOG_TYPE.DBG);
      if (Database is null)
        return RESP.SetError(ERROR_NO_DB_CONNECTION, "No active database connection");

      Variable? varUserSession = CheckSessionInternal(Database, vars[0]);
      if (varUserSession is null)
        return RESP.SetError(ERROR_INVALID_SESSION, $"Invalid session [{vars[0].GetValueAsString()}]");

      Variable varUserId = varUserSession[0][0].CloneWithNewName("@UserId");
      Variable varUserPassword = Database.Select("SELECT Password FROM Users WHERE UserId = @UserId", varUserId);
      if (varUserPassword is null || varUserPassword.Count == 0)
        return RESP.SetError(ERROR_DB, "Unknown Error");

      string oldPassword = vars[1].GetValueAsString();
      if (SecureHasherV1.Verify(oldPassword, varUserPassword[0][0].GetValueAsString()) == false)
        return RESP.SetError(ERROR_BAD_CREDENTIALS, $"Bad credentials");

      //We got this far, everything is good
      string newPassword = vars[2].GetValueAsString();
      string newPasswordHashed = SecureHasherV1.Hash(newPassword);
      Variable recordsAffected = Database.Execute("UPDATE Users SET Password = @Password WHERE UserId = @UserId", new Variable("@Password", newPasswordHashed), varUserId);
      if (recordsAffected.Value == 0)
        return RESP.SetError(ERROR_DB, "Not implemented");

      return RESP.SetSuccess(varUserSession);
    }
  }
}