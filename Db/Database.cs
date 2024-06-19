using FlowEngineCore;
using FlowEngineCore.Interfaces;
using Database;
using System.Drawing;
using System.Numerics;


#pragma warning disable CA1822 // Mark members as static

namespace Db
{
  public class Database : FlowEngineCore.Plugin
  {
    private IDatabase? mDatabase = null;
    private const string DB_TYPE = "DatabaseType";
    private const string DB_TYPE_MY_SQL = "MySql";
    private const string DB_TYPE_SQL_SERVER = "Sql Server";
    private const string DB_CONN_POOL_MAX = "ConnPoolSizeMax";
    private const string DB_CONN_POOL_MIN = "ConnPoolSizeMin";
    private const string DB_SHARE_WITH_PLUGINS = "ShareDatabaseConnWithOtherPlugins";
    public const string DB_TREAT_TINYINT_AS_BOOLEAN = "TreatTinyintAsBoolean";
    public const string DB_DATE_FORMAT = "DateFormat";

    private const int DB_ERROR = (int)STEP_ERROR_NUMBERS.DatabaseErrorMin;
    private const int DB_ERROR_UNKNOWN = (int)STEP_ERROR_NUMBERS.DatabaseErrorMin + 1;
    private const int DB_ERROR_ZERO_RECORDS = (int)STEP_ERROR_NUMBERS.DatabaseErrorMin + 2;
    public override void Init()
    {
      base.Init();

      Function func;
      //Functions.Add(new Function("Open", this, Open));
      //Functions.Add(new Function("Close", this, Close));


      func = new Function("Select", this, Select);
      func.Validators.Add(new ValidatorAtLeastOneRec());
      func.Validators.Add(new ValidatorZeroRecords());
      func.Parms.Add(new PARM("SQL", STRING_SUB_TYPE.Sql));
      PARM parm = new PARM("@Param", DATA_TYPE.Various, PARM.PARM_REQUIRED.No, PARM.PARM_ALLOW_MULTIPLE.Multiple) { NameChangeable = true, NameChangeIncrement = true };
      func.Parms.Add(parm);
      func.RespNames = new Variable("recs");
      func.DefaultSaveResponseVariable = true;
      Functions.Add(func);

      func = new Function("Select Single", this, SelectSingle, "", "Select a single record only, if more than one record is found it will just return the first one selected.");
      //func.Validators.Add(new ValidatorAtLeastOneRec());
      //func.Validators.Add(new ValidatorZeroRecords());
      func.Parms.Add(new PARM("SQL", STRING_SUB_TYPE.Sql));
      parm = new PARM("@Param", DATA_TYPE.Various, PARM.PARM_REQUIRED.No, PARM.PARM_ALLOW_MULTIPLE.Multiple) { NameChangeable = true, NameChangeIncrement = true };
      func.Parms.Add(parm);
      func.RespNames = new Variable("rec");
      func.DefaultSaveResponseVariable = true;
      Functions.Add(func);


      func = new Function("Execute", this, Execute);
      func.Parms.Add(new PARM("SQL", STRING_SUB_TYPE.Sql));
      parm = new PARM("@Param", DATA_TYPE.Various, PARM.PARM_REQUIRED.No, PARM.PARM_ALLOW_MULTIPLE.Multiple) { NameChangeable = true, NameChangeIncrement = true };
      func.Parms.Add(parm);
      func.RespNames.Name = "recordsAffected";
      func.RespNames.SubVariableAdd(new Variable("lastInsertedId"));
      Functions.Add(func);


      Functions.Add(new Function("InsertMany", this, InsertMany));

      //func = new Function("GetInt", this, GetInt);
      //func.Parms.Add(new PARM("SQL", STRING_SUB_TYPE.Sql));
      //parm = new PARM("@Param", DATA_TYPE.Various, PARM.PARM_REQUIRED.No, PARM.PARM_ALLOW_MULTIPLE.Multiple) { NameChangeable = true, NameChangeIncrement = true };
      //func.Parms.Add(parm);
      //func.RespNames.Add(new Variable("recordsInserted"));
      //Functions.Add(func);



      Setting setting = mSettings.SettingAdd(new Setting(DB_TYPE, DB_TYPE_MY_SQL, STRING_SUB_TYPE.DropDownList));
      setting.OptionAdd(DB_TYPE_MY_SQL);
      setting.OptionAdd(DB_TYPE_SQL_SERVER);
      mSettings.SettingAdd(new Setting("Database", ""));
      mSettings.SettingAdd(new Setting("Url", ""));
      mSettings.SettingAdd(new Setting("Port", 0));
      mSettings.SettingAdd(new Setting("User", ""));
      mSettings.SettingAdd(new Setting("Password", ""));
      mSettings.SettingAdd(new Setting(DB_CONN_POOL_MAX, 10));
      mSettings.SettingAdd(new Setting(DB_CONN_POOL_MIN, 3));
      mSettings.SettingAdd(new Setting(DB_SHARE_WITH_PLUGINS, true));
      mSettings.SettingAdd(new Setting(DB_TREAT_TINYINT_AS_BOOLEAN, true));
      mSettings.SettingAdd(new Setting(DB_DATE_FORMAT, "yyyy-MM-dd HH:mm:ss zzz"));
      mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Blue));
      mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.White));

      this.StartPriority = 10000; //Database should start before other plugins, put at least a 1000 between other plugins to give room to manipulate the start up priority
    }

    public override void StartPluginDesigner(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPluginDesigner(GlobalPluginValues);
      //We just want to open the database like normal
      StartPlugin(GlobalPluginValues);
    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);
      //Open database connection
      string dbType = mSettings.SettingGetAsString(DB_TYPE);
      mDatabase = null;


      if (dbType == DB_TYPE_MY_SQL)
      {
        string dbName = mSettings.SettingGetAsString("Database");
        string dbUrl = mSettings.SettingGetAsString("Url");
        string dbPort = mSettings.SettingGetAsString("Port");
        string dbUser = mSettings.SettingGetAsString("User");
        string dbPassword = mSettings.SettingGetAsString("Password");
        string dbPoolMin = mSettings.SettingGetAsString(DB_CONN_POOL_MIN);
        string dbPoolMax = mSettings.SettingGetAsString(DB_CONN_POOL_MAX);
        mDatabase = new DbMySql(this, mLog);
        string connectionString = $"Server={dbUrl};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword};Pooling=true;MinimumPoolSize={dbPoolMin};MaximumPoolSize={dbPoolMax};Connection Lifetime=0;";
        mDatabase.Connect(connectionString);
        if (mSettings.SettingGetAsBoolean(DB_SHARE_WITH_PLUGINS) == true)
        {
          GlobalPluginValues.Add("db", mDatabase);
        }
      }
      else if (dbType == DB_TYPE_SQL_SERVER) 
      {
        //TODO: Implement Microsoft SQL server support
        mDatabase = new DbSqlServer(this);
      }
      else
      {
       mLog?.Write($"Invalid DatabaseType [{dbType}]", LOG_TYPE.ERR);
      }
    }

    public override void StopPlugin()
    {

    }


    public override void Dispose()
    {
      //Nothing to dispose of yet!
    }

    //public RESP Open(Core.Flow flow, Variable[] vars)
    //{
    //  Global.Write("Db.Open - ");
    //  return RESP.SetSuccess();
    //}

    //public RESP Close(Core.Flow flow, Variable[] vars)
    //{
    //  Global.Write("Db.Close");
    //  return RESP.SetSuccess();
    //}

    public RESP Select(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Db.Select", LOG_TYPE.DBG);
      if (mDatabase is null)
        return RESP.SetError(2000, "No active database connection");

      vars[0].GetValue(out string sql);

      Variable? var = null;
      try
      {
        var = mDatabase.Select(sql, vars);
      }
      catch (Exception ex)
      {
        return RESP.SetError(DB_ERROR_UNKNOWN, ex.Message);
      }
      return RESP.SetSuccess(var);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP SelectSingle(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Db.Select", LOG_TYPE.DBG);
      if (mDatabase is null)
        return RESP.SetError(2000, "No active database connection");

      vars[0].GetValue(out string sql);

      Variable? var = null;
      try
      {
        var = mDatabase.Select(sql, vars);
      }
      catch (Exception ex)
      {
        return RESP.SetError(DB_ERROR_UNKNOWN, ex.Message);
      }
      if (var.Count > 0)
        return RESP.SetSuccess(var[0]);
      else
        return RESP.SetError(DB_ERROR_ZERO_RECORDS, "Zero records found");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP Execute(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Db.Execute", LOG_TYPE.DBG);
      if (mDatabase is null)
        return RESP.SetError(2002, "No active database connection");

      vars[0].GetValue(out string sql);
      Variable? var = null;
      try
      {
        var = mDatabase.Execute(sql, vars);
      }
      catch (Exception ex)
      {
        return RESP.SetError(DB_ERROR_UNKNOWN, ex.Message);
      }
      return RESP.SetSuccess(var);
    }

    public RESP InsertMany(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("Db.InsertMany", LOG_TYPE.DBG);
      return RESP.SetSuccess();
    }

    //public RESP GetInt(Core.Flow flow, Variable[] vars)
    //{
    //  Global.Write("Db.GetInt");
    //  return RESP.SetSuccess();
    //}


  }
}