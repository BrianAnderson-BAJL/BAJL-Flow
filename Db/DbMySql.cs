﻿using FlowEngineCore;
using FlowEngineCore.Interfaces;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;

namespace Db
{
  public class DbMySql : IDatabase
  {
    private string ConnectionString = "";
    private Db.Database mDb;
    private ILog? mLog;

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
      //Skip the 0 (zero) entry, that is the actual SQL, get all the paramenters after that
      for (int x = 0; x < vars.Length; x++)
      {
        vars[x].GetValue(out dynamic val);

        cmd.Parameters.AddWithValue(vars[x].Name, val);
        

        //else if (vars[x].DataType == DATA_TYPE.Boolean)
        //{
        //  vars[x].GetValue(out bool val);
        //  byte valByte = 0;
        //  if (val == true)
        //    valByte = 1;
        //  cmd.Parameters.AddWithValue(vars[x].Name, valByte);
        //}

        //cmd.Parameters.AddWithValue(vars[x].Name, 
      }
    }

    public Variable Execute(string SQL, params Variable[] vars)
    {
      Variable root = new Variable("RecordsAffected", 0L);
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
              root.SubVariables.Add(new Variable("LastInsertedId", command.LastInsertedId));

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

    public Variable Select(string SQL, params Variable[] vars)
    {
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
  }
}
