﻿using Core;
using Core.Interfaces;
using System.Drawing;
using System.Numerics;


#pragma warning disable CA1822 // Mark members as static

namespace Db
{
  public class Database : Core.Plugin
  {
    private IDatabase? mDatabase = null;
    private const string DB_TYPE = "DatabaseType";
    private const string DB_TYPE_MY_SQL = "MySql";
    private const string DB_TYPE_SQL_SERVER = "Sql Server";
    private const string DB_CONN_POOL_MAX = "ConnPoolSizeMax";
    private const string DB_CONN_POOL_MIN = "ConnPoolSizeMin";
    private const string DB_SHARE_WITH_PLUGINS = "ShareDatabaseConnWithOtherPlugins";
    public const string DB_TREAT_TINYINT_AS_BOOLEAN = "TreatTinyintAsBoolean";
    public override void Init()
    {
      base.Init();

      Function func;
      Functions.Add(new Function("Open", this, Open));
      Functions.Add(new Function("Close", this, Close));
      func = new Function("Select", this, Select);
      func.Parms.Add(new PARM("SQL", STRING_SUB_TYPE.Sql));
      PARM parm = new PARM("@Param", DATA_TYPE.Various, PARM.PARM_REQUIRED.No, PARM.PARM_ALLOW_MULTIPLE.Multiple) { NameChangeable = true, NameChangeIncrement = true };
      func.Parms.Add(parm);
      
      func.RespNames = new Variable("recs");
      func.DefaultSaveResponseVariable = true;
      Functions.Add(func);
      func = new Function("Insert", this, Insert);
      func.Parms.Add(new PARM("SQL", STRING_SUB_TYPE.Sql));
      parm = new PARM("SQL Parameter", DATA_TYPE.Various, PARM.PARM_REQUIRED.No, PARM.PARM_ALLOW_MULTIPLE.Multiple);
      parm.NameChangeable = true;
      func.Parms.Add(parm);
      func.RespNames.Add(new Variable("recordsInserted"));
      Functions.Add(func);
      Functions.Add(new Function("InsertMany", this, InsertMany));
      Functions.Add(new Function("Delete", this, Delete));
      Functions.Add(new Function("Update", this, Update));
      Functions.Add(new Function("GetInt", this, GetInt));



      Setting setting = SettingAdd(new Setting(DB_TYPE, DB_TYPE_MY_SQL, STRING_SUB_TYPE.DropDownList));
      setting.OptionAdd(DB_TYPE_MY_SQL);
      setting.OptionAdd(DB_TYPE_SQL_SERVER);
      SettingAdd(new Setting("Database", ""));
      SettingAdd(new Setting("Url", ""));
      SettingAdd(new Setting("Port", 0));
      SettingAdd(new Setting("User", ""));
      SettingAdd(new Setting("Password", ""));
      SettingAdd(new Setting(DB_CONN_POOL_MAX, 10));
      SettingAdd(new Setting(DB_CONN_POOL_MIN, 3));
      SettingAdd(new Setting(DB_SHARE_WITH_PLUGINS, true));
      SettingAdd(new Setting(DB_TREAT_TINYINT_AS_BOOLEAN, true));
      SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      SettingAdd(new Setting("", "Designer", "BorderColor", Color.Blue));
      SettingAdd(new Setting("", "Designer", "FontColor", Color.White));

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
      //Open database connection
      string dbType = SettingGetAsString(DB_TYPE);
      mDatabase = null;
      if (dbType == DB_TYPE_MY_SQL)
      {
        string dbName = SettingGetAsString("Database");
        string dbUrl = SettingGetAsString("Url");
        string dbPort = SettingGetAsString("Port");
        string dbUser = SettingGetAsString("User");
        string dbPassword = SettingGetAsString("Password");
        string dbPoolMin = SettingGetAsString(DB_CONN_POOL_MIN);
        string dbPoolMax = SettingGetAsString(DB_CONN_POOL_MAX);
        mDatabase = new DbMySql(this);
        string connectionString = $"Server={dbUrl};Port={dbPort};Database={dbName};Uid={dbUser};Pwd={dbPassword};Pooling=true;MinimumPoolSize={dbPoolMin};MaximumPoolSize={dbPoolMax};Connection Lifetime=0;";
        mDatabase.Connect(connectionString);
        if (this.SettingGetAsBoolean(DB_SHARE_WITH_PLUGINS) == true)
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
        Global.Write($"Invalid DatabaseType [{dbType}]", DEBUG_TYPE.Error);
      }
    }

    public override void StopPlugin()
    {

    }

    public override void Dispose()
    {
      //Nothing to dispose of yet!
    }

    public RESP Open(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Db.Open - ");
      return RESP.SetSuccess();
    }

    public RESP Close(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Db.Close");
      return RESP.SetSuccess();
    }

    public RESP Select(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Db.Select");
      if (mDatabase is null)
        return RESP.SetError(1, "No active database connection");

      vars[0].GetValue(out string sql);

      Variable var = mDatabase.Select(sql, vars);
      return RESP.SetSuccess(var);
    }

    public RESP Insert(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Db.Insert" );
      return RESP.SetSuccess();
    }

    public RESP InsertMany(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Db.InsertMany");
      return RESP.SetSuccess();
    }

    public RESP GetInt(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Db.GetInt");
      return RESP.SetSuccess();
    }


    public RESP Delete(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Db.Delete");
      return RESP.SetSuccess();
    }


    public RESP Update(Core.Flow flow, Variable[] vars)
    {

      Global.Write("Db.Update");

      return RESP.SetSuccess();
    }
  }
}