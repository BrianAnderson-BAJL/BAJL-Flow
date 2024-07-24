using FlowEngineCore;
using FlowEngineCore.Interfaces;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Text;

namespace Db
{
  public class DbMySql : IDatabase
  {
    private string ConnectionString = "";
    private Db.Database mDb;
    private ILog? mLog;

    const int LIMIT_RECORDS = 100;
    public DbMySql(Db.Database db, ILog? mLog)
    {
      this.mDb = db;
      this.mLog = mLog;
    }
    public void Connect(string connectionString)
    {
      ConnectionString = connectionString;
      using (MySqlConnection conn = new MySqlConnection(ConnectionString))
      {
        try
        {
          conn.Open();
          mLog?.Write("Connected to database!");
        }
        catch (Exception ex)
        {
          mLog?.Write("Failed to connect to DB with connection string:  " + ConnectionString, ex, LOG_TYPE.WAR);
        }
      }
    }

    private void PopulateParameters(MySqlCommand cmd, params Variable[] vars)
    {
      for (int x = 0; x < vars.Length; x++)
      {
        if (vars[x].Name == "SQL")
          continue;
        vars[x].GetValue(out dynamic val);

        cmd.Parameters.AddWithValue(vars[x].Name, val);
      }
    }

    private void LogSql(string SQL, params Variable[] vars)
    {
      for (int x = 0; x < vars.Length; x++)
      {
        if (vars[x].Name == "SQL")
          continue;

        if (vars[x].DataType == DATA_TYPE.String)
          SQL = SQL.Replace(vars[x].Name, "'" + vars[x].GetValueAsString() + "'");
        else
          SQL = SQL.Replace(vars[x].Name, vars[x].GetValueAsString());
      }

      FlowEngine.Log?.Write("DB - " + SQL, LOG_TYPE.DBG);

    }

    private string ManuallyReplaceParameter(string SQL, string name, Variable data)
    {
      string temp = "";
      if (data.DataType == DATA_TYPE.String)
        temp = SQL.Replace(name, "'" + MySqlHelper.EscapeString(data.GetValueAsString()) + "'");
      else
        temp = SQL.Replace(name, MySqlHelper.EscapeString(data.GetValueAsString()));
      return temp;
    }
    public Variable UpdateMany(string SQL, params Variable[] vars)
    {
      return InsertMany(SQL, vars); //There is currently nothing special about an UpdateMany, just execute the InsertMany code.
    }
    public Variable InsertMany(string SQL, params Variable[] vars)
    {
      //vars[0] = SQL
      //vars[1] = Variable with sub variables in array or block
      //vars[2] = More Variables to use, but these will be duplicated (probably an ID number to a linked table)
      LogSql(SQL, vars);
      Variable root = new Variable("recordsAffected", 0L);
      SQL = SQL.Trim();
      if (SQL.Length <= 0)
        return root;

      if (SQL[SQL.Length - 1] != ';')
        SQL += ';';

      if (vars[1].Count == 0) //If no records to insert, return zero
        return root;

      string origSql = SQL;
      StringBuilder sb = new StringBuilder((SQL.Length + 2) * vars[1].Count); //Allocate space for all the SQL statements
      int countOfSql = 0;
      for (int x = 0; x < vars[1].Count; x++)
      {
        SQL = origSql;
        Variable data = vars[1][x];
        if (data.DataType == DATA_TYPE._None)
        {
          for (int y = 0; y < data.Count; y++)
          {
            Variable subData = data[y];
            SQL = ManuallyReplaceParameter(SQL, "@" + subData.Name, subData);
          }
        }
        else
        {
          SQL = ManuallyReplaceParameter(SQL, vars[1].Name, data);
        }

        for (int y = 2; y < vars.Length; y++)
        {
          SQL = ManuallyReplaceParameter(SQL, vars[y].Name, vars[y]);
        }

        sb.AppendLine(SQL);
        countOfSql++;
      }
      try
      {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
          connection.Open();

          using (MySqlCommand command = new MySqlCommand(sb.ToString(), connection))
          {
            //PopulateParameters(command, vars);
            long recordsAffected = command.ExecuteNonQuery();

            root.Value = recordsAffected * countOfSql; //Try to get a more accurate number for the number of records affected, ExecuteNonQuery only returns 1 if anything was affected.
          }
        }
      }
      catch (Exception ex)
      {
        mLog?.Write(ex, LOG_TYPE.WAR);
        throw;
      }
      return root;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="SQL"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public Variable Execute(string SQL, params Variable[] vars)
    {
      LogSql(SQL, vars);
      Variable root = new Variable("recordsAffected", 0L);
      try
      {
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
          connection.Open();

          using (MySqlCommand command = new MySqlCommand(SQL, connection))
          {
            PopulateParameters(command, vars);
            long recordsAffected = command.ExecuteNonQuery();
            if (SQL.StartsWith("INSERT", StringComparison.InvariantCultureIgnoreCase) == true)
              root.SubVariables.Add(new Variable("lastInsertedId", command.LastInsertedId));

            root.Value = recordsAffected;
          }
        }
      }
      catch (Exception ex)
      {
        mLog?.Write(ex, LOG_TYPE.WAR);
        throw;
      }
      return root;
    }


    public Variable Execute(object transaction, string SQL, params Variable[] vars)
    {
      LogSql(SQL, vars);
      MySqlTransaction? myTrans = transaction as MySqlTransaction;
      if (myTrans is null)
        throw new ArgumentException("DB bad transaction, expected MySqlTransaction");
      if (myTrans.Connection is null)
        throw new ArgumentException("DB bad connection, expected MySqlTransaction.Connection is null");
      if (myTrans.Connection.State != System.Data.ConnectionState.Open)
        throw new ArgumentException("DB Connection is not open");

      Variable root = new Variable("recordsAffected", 0L);
      try
      {
        using (MySqlCommand command = new MySqlCommand(SQL, myTrans.Connection, myTrans))
        {
          PopulateParameters(command, vars);
          long recordsAffected = command.ExecuteNonQuery();
          if (SQL.StartsWith("INSERT", StringComparison.InvariantCultureIgnoreCase) == true)
            root.SubVariables.Add(new Variable("lastInsertedId", command.LastInsertedId));

          root.Value = recordsAffected;
        }
      }
      catch (Exception ex)
      {
        mLog?.Write(ex, LOG_TYPE.WAR);
        throw;
      }
      return root;
    }


    public Variable SelectOneRecord(string SQL, params Variable[] vars)
    {
      Variable temp = Select(SQL, vars);
      if (temp.Count > 0)
      {
        if (temp[0].Count > 0)
        {
          return temp[0];
        }
      }
      return new Variable("data", 0L);
    }

    public Variable SelectId(string SQL, params Variable[] vars)
    {
      Variable temp = Select(SQL, vars);
      if (temp.Count > 0)
      {
        if (temp[0].Count > 0)
        {
          return temp[0][0];
        }
      }
      return new Variable("data", 0L);
    }

    public Variable Select(string SQL, params Variable[] vars)
    {
      LogSql(SQL, vars);
      Variable root = new Variable("Recordset");
      try
      {
        root.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;
        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        {
          connection.Open();

          using (MySqlCommand command = new MySqlCommand(SQL, connection))
          {
            PopulateParameters(command, vars);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
              bool tinyAsBool = mDb.GetSettings.SettingGetAsBoolean(Db.Database.DB_TREAT_TINYINT_AS_BOOLEAN);
              string dataFormat = mDb.GetSettings.SettingGetAsString(Db.Database.DB_DATE_FORMAT);
              while (reader.Read())
              {
                Variable row = new Variable("record");
                row.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Block;
                Variable column;
                for (int x = 0; x < reader.FieldCount; x++)
                {
                  //Need to translate database fields to Flow Engine variables.
                  string columnName = reader.GetName(x);
                  Type type = reader.GetFieldType(x);
                  if (reader.IsDBNull(x) == true)
                  {
                    column = new Variable(columnName, "");
                  }
                  else if (type == typeof(string))
                  {
                    column = new Variable(columnName, reader.GetString(x));
                  }
                  else if (type == typeof(short))
                  {
                    column = new Variable(columnName, (long)reader.GetInt16(x));
                  }
                  else if (type == typeof(int))
                  {
                    column = new Variable(columnName, (long)reader.GetInt32(x));
                  }
                  else if (type == typeof(long))
                  {
                    column = new Variable(columnName, reader.GetInt64(x));
                  }
                  else if (type == typeof(float))
                  {
                    column = new Variable(columnName, (decimal)reader.GetFloat(x));
                  }
                  else if (type == typeof(double))
                  {
                    column = new Variable(columnName, (decimal)reader.GetDouble(x));
                  }
                  else if (type == typeof(decimal))
                  {
                    column = new Variable(columnName, reader.GetDecimal(x));
                  }
                  else if (type == typeof(DateTime))
                  {
                    column = new Variable(columnName, reader.GetDateTime(x).ToString(dataFormat));
                  }
                  else if (type == typeof(byte) && tinyAsBool == false)
                  {
                    column = new Variable(columnName, reader.GetByte(x));
                  }
                  else if (type == typeof(sbyte) && tinyAsBool == false)
                  {
                    column = new Variable(columnName, reader.GetSByte(x));
                  }
                  else if ((type == typeof(sbyte) || type == typeof(byte)) && tinyAsBool == true)
                  {
                    column = new Variable(columnName, reader.GetSByte(x) >= 1);
                  }
                  else if (type == typeof(byte) && tinyAsBool == true)
                  {
                    column = new Variable(columnName, reader.GetByte(x) >= 1);
                  }
                  else
                  {
                    mLog?.Write($"Unknown column type [{type.Name}]", LOG_TYPE.WAR);
                    column = new Variable(columnName, $"Unknown column type [{type.Name}]");
                  }
                  row.SubVariables.Add(column);
                }
                root.SubVariables.Add(row);

              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        mLog?.Write(ex, LOG_TYPE.WAR);
        throw;
      }
      return root;
      

    }

    public List<string> GetTables()
    {
      List<string> tableNames = new List<string>(32);
      using (MySqlConnection connection = new MySqlConnection(ConnectionString))
      {
        connection.Open();
        using (MySqlCommand command = new MySqlCommand("SHOW TABLES", connection))
        {
          using (MySqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              tableNames.Add(reader.GetString(0));
            }
          }
        }
      }

      return tableNames;
    }

    public List<string> GetFields(string tableName)
    {
      List<string> fieldNames = new List<string>(32);
      using (MySqlConnection connection = new MySqlConnection(ConnectionString))
      {
        connection.Open();
        using (MySqlCommand command = new MySqlCommand($"SHOW COLUMNS FROM {tableName}" , connection))
        {
          using (MySqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              //for (int x = 0; x < reader.FieldCount; x++)
              //{
                fieldNames.Add(reader.GetString(0));
              //}
            }
          }
        }
      }

      return fieldNames;
    }

    public object TransactionBegin()
    {
      FlowEngine.Log?.Write("DB - TransactionBegin", LOG_TYPE.DBG);
      try
      {
        MySqlConnection connection = new MySqlConnection(ConnectionString);
        connection.Open();
        MySqlTransaction trans = connection.BeginTransaction();
        return trans;
      }
      catch (Exception ex)
      {
        mLog?.Write(ex, LOG_TYPE.WAR);
        throw;
      }
      
    }

    public void TransactionRollback(object transaction)
    {
      MySqlTransaction? myTrans = transaction as MySqlTransaction;
      if (myTrans is null)
        throw new ArgumentException("DB bad transaction, expected MySqlTransaction");

      myTrans.Rollback();
      myTrans.Connection.Dispose();
      myTrans.Dispose();
    }

    public void TransactionCommit(object transaction)
    {
      MySqlTransaction? myTrans = transaction as MySqlTransaction;
      if (myTrans is null)
        throw new ArgumentException("DB bad transaction, expected MySqlTransaction");

      myTrans.Commit();
      myTrans.Connection.Dispose();
      myTrans.Dispose();
    }

    public Variable TestSelect(string sql, params Variable[] vars)
    {
      LogSql(sql, vars);

      sql = sql.Trim();
      if (sql.Length > 0 && sql.Contains(" LIMIT ", StringComparison.InvariantCultureIgnoreCase) == false)
      {
        if (sql[sql.Length - 1] == ';')
          sql = sql.Substring(0, sql.Length - 1) + " LIMIT " + LIMIT_RECORDS + ";";
        else
          sql = sql + " LIMIT " + LIMIT_RECORDS;
      }
      return Select(sql, vars);
    }
    public Variable TestExecute(string sql, params Variable[] vars)
    {
      MySqlTransaction trans = (TransactionBegin() as MySqlTransaction)!;
      Variable var = Execute(trans, sql, vars);
      TransactionRollback(trans);
      return var;
    }
  }
}
