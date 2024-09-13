using FlowEngineCore;
using FlowEngineCore.Interfaces;
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

    public DATABASE_TYPE DatabaseType
    {
      get { return DATABASE_TYPE.SqlServer; }
    }

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

    public Variable Execute(object connection, string SQL, params Variable[] vars)
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

    public Variable SelectId(string SQL, params Variable[] vars)
    {
      throw new NotImplementedException();
    }
    public Variable SelectOneRecord(string SQL, params Variable[] vars)
    {
      throw new NotImplementedException();
    }

    public object TransactionBegin()
    {
      throw new NotImplementedException();
    }

    public void TransactionCommit(object connection)
    {
      throw new NotImplementedException();
    }

    public void TransactionRollback(object connection)
    {
      throw new NotImplementedException();
    }
    public Variable TestSelect(string sql, params Variable[] vars)
    {
      throw new NotImplementedException();
    }
    public Variable TestExecute(string sql, params Variable[] vars)
    {
      throw new NotImplementedException();
    }
    public Variable InsertMany(string SQL, params Variable[] vars)
    {
      throw new NotImplementedException();
    }
    public Variable UpdateMany(string SQL, params Variable[] vars)
    {
      throw new NotImplementedException();
    }

  }
}
