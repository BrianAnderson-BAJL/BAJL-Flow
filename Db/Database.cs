using Core;
using Core.Interfaces;
using System.Drawing;
using System.Numerics;


#pragma warning disable CA1822 // Mark members as static

namespace Db
{
  public class Database : Core.Plugin
  {
    private IDatabase? mDatabase = null;
    private const string DB_TYPE = "Database Type";
    private const string DB_TYPE_MY_SQL = "MySql";
    private const string DB_TYPE_SQL_SERVER = "Sql Server";
    private const string DB_CONN_POOL_MAX = "Conn Pool Size Max";
    private const string DB_CONN_POOL_MIN = "Conn Pool Size Min";
    private const string DB_SHARE_WITH_PLUGINS = "Share Database Conn With Other Plugins";
    public const string DB_TREAT_TINYINT_AS_BOOLEAN = "Treat Tinyint as Boolean";
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



      List<Setting> settings = new List<Setting>(2);
      settings.Add(new Setting(DB_TYPE, DB_TYPE_MY_SQL));
      settings.Add(new Setting(DB_TYPE, DB_TYPE_SQL_SERVER));
      SettingAddIfMissing(new Setting(DB_TYPE, "", DB_TYPE, DB_TYPE_MY_SQL, settings));
      SettingAddIfMissing("Database", "");
      SettingAddIfMissing("Url", "");
      SettingAddIfMissing("Port", 0);
      SettingAddIfMissing("User", "");
      SettingAddIfMissing("Password", "");
      SettingAddIfMissing(DB_CONN_POOL_MAX, 10);
      SettingAddIfMissing(DB_CONN_POOL_MIN, 3);
      SettingAddIfMissing(DB_SHARE_WITH_PLUGINS, true);
      SettingAddIfMissing(DB_TREAT_TINYINT_AS_BOOLEAN, true);
      SettingAddIfMissing(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      SettingAddIfMissing(new Setting("", "Designer", "BorderColor", Color.Blue));
      SettingAddIfMissing(new Setting("", "Designer", "FontColor", Color.White));

      this.StartPriority = 10000; //Database should start before other plugins, put at least a 1000 between other plugins to give room to manipulate the start up priority
    }

    public override void StartPluginDesigner(Dictionary<string, object> GlobalPluginValues)
    {
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