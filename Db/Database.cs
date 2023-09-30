using Core;
using System.Drawing;
using System.Numerics;


#pragma warning disable CA1822 // Mark members as static

namespace Db
{
  public class Database : Core.Plugin
  {
    public override void Init()
    {
      base.Init();

      
      Functions.Add(new Function("Open", this, Open));
      Functions.Add(new Function("Close", this, Close));
      Functions.Add(new Function("Select", this, Select));
      Functions.Add(new Function("Insert", this, Insert));
      Functions.Add(new Function("InsertMany", this, InsertMany));
      Functions.Add(new Function("Delete", this, Delete));
      Functions.Add(new Function("Update", this, Update));
      Functions.Add(new Function("GetInt", this, GetInt));



      List<Setting> settings = new List<Setting>(2);
      settings.Add(new Setting("DatabaseType", "MySql"));
      settings.Add(new Setting("DatabaseType", "Sql Server"));
      SettingAddIfMissing(new Setting("DatabaseType", "", "DatabaseType", "MySql", settings));
      SettingAddIfMissing("Database", "");
      SettingAddIfMissing("Url", "");
      SettingAddIfMissing("Port", 0);
      SettingAddIfMissing("User", "");
      SettingAddIfMissing("Password", "");
      SettingAddIfMissing("ConnectionPoolSizeMax", "");
      SettingAddIfMissing("ConnectionPoolSizeMin", "");
      SettingAddIfMissing(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      SettingAddIfMissing(new Setting("", "Designer", "BorderColor", Color.Blue));
      SettingAddIfMissing(new Setting("", "Designer", "FontColor", Color.White));
    }

    public override void Dispose()
    {
      //Nothing to dispose of yet!
    }

    public RESP Open(PARMS Parms)
    {
      Global.Write("Db.Open - ");
      return RESP.SetSuccess();
    }

    public RESP Close(PARMS Parms)
    {
      Global.Write("Db.Close");
      return RESP.SetSuccess();
    }

    public RESP Select(PARMS Parms)
    {
      Global.Write("Db.Select");
      return RESP.SetSuccess();
    }

    public RESP Insert(PARMS Parms)
    {
      Global.Write("Db.Insert" );
      return RESP.SetSuccess();
    }

    public RESP InsertMany(PARMS Parms)
    {
      Global.Write("Db.InsertMany");
      return RESP.SetSuccess();
    }

    public RESP GetInt(PARMS Parms)
    {
      Global.Write("Db.GetInt");
      return RESP.SetSuccess();
    }


    public RESP Delete(PARMS Parms)
    {
      Global.Write("Db.Delete");
      return RESP.SetSuccess();
    }


    public RESP Update(PARMS Parms)
    {

      Global.Write("Db.Update");

      return RESP.SetSuccess();
    }
  }
}