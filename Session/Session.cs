using Core;
using Core.Interfaces;
using System.Drawing;

namespace Session
{
    public class Session : Core.Plugin
  {
    private IDatabase? Database;
    private uint USER_REGISTER_DUPLICATE = 1;
    private uint USER_REGISTER_ERROR = 2;
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
      Functions.Add(new Function("Device Register", this, DeviceRegister));

      //SETTINGS
      {
        SettingAddIfMissing(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        SettingAddIfMissing(new Setting("", "Designer", "BorderColor", Color.Teal));
        SettingAddIfMissing(new Setting("", "Designer", "FontColor", Color.Black));
      }
      //SETTINGS

    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      //Let's grab the database from the database plugin
      if (GlobalPluginValues.ContainsKey("db") == true)
      {
        Database = GlobalPluginValues["db"] as IDatabase;
      }
      else
      {
        Global.Write("No shared database connection pool found. Session plugin will not work!", DEBUG_TYPE.Error);
      }
    }

    public override void StopPlugin()
    {
    }

    public RESP UserRegister(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Session.UserRegister");
      if (Database is null)
        return RESP.SetError(1, "No active database connection", USER_REGISTER_ERROR);

      vars[0].GetValue(out string loginId);
      vars[1].GetValue(out string password);

      //Check if the LoginId already exists in the database
      Variable varLoginId = new VariableString("@LoginId", loginId);
      Variable records = Database.Select("SELECT UserId FROM Users Where LoginId = @LoginId", varLoginId);
      if (records.SubVariables.Count > 0) //Has a record, means it found a user with the same LoginId
      {
        Global.Write("Session.UserRegister - Duplicate login Id", DEBUG_TYPE.Warning);
        return RESP.SetError(1, $"Duplicate LoginId [{loginId}]", USER_REGISTER_DUPLICATE); //User already exists in the database, return error.
      }

      //LoginId doesn't exist, lets hash the password and insert it into the database
      string passwordHash = SecureHasherV1.Hash(password);
      VariableInteger recordsAffected = Database.Execute("INSERT INTO Users (LoginId, Password) VALUES (@LoginId, @Password)", varLoginId, new VariableString("@Password", passwordHash));
      if (recordsAffected.Value > 0 && recordsAffected.SubVariables.Count > 0)
      {
        recordsAffected.SubVariables[0].GetValue(out long newId); //The only sub variable will be the new Id of the record that was inserted
        return RESP.SetSuccess(Database.Select("SELECT * FROM Users WHERE UserId = @UserId", new VariableInteger("@UserId", newId)));
      }
      else
        return RESP.SetError(2, "Failed to insert user into database");
    }

    public RESP DeviceRegister(Core.Flow flow, Variable[] vars)
    {

      return RESP.SetSuccess();
    }
  }
}