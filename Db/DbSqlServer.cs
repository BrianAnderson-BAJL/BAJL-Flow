using Core;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    public class DbSqlServer : IDatabase
  {
    private Db.Database Db;

    public DbSqlServer(Db.Database db)
    {
      this.Db = db;
    }

    public void Connect(string connectionString)
    {
      throw new NotImplementedException();
    }

    public Variable Execute(string SQL, params Variable[] vars)
    {
      throw new NotImplementedException();
    }

    public List<string> GetFields(string tableName)
    {
      throw new NotImplementedException();
    }

    public List<string> GetTables()
    {
      throw new NotImplementedException();
    }

    public Variable Select(string SQL, params Variable[] vars)
    {
      throw new NotImplementedException();
    }
  }
}
