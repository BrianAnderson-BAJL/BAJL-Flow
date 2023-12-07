using Core;
using Core.Interfaces;
using MySql.Data.MySqlClient;

namespace Db
{
  public class DbMySql : IDatabase
  {
    private string ConnectionString = "";
    private Db.Database mDb;

    public DbMySql(Db.Database db)
    {
      this.mDb = db;
    }
    public void Connect(string connectionString)
    {
      ConnectionString = connectionString;
      using (MySqlConnection conn = new MySqlConnection(ConnectionString))
      {
        try
        {
          conn.Open();
          Global.Write("Connected to database!");
        }
        catch (Exception ex)
        {
          Global.Write("Failed to connect to DB with connection string:  " + ConnectionString + "\n" + ex.Message, DEBUG_TYPE.Error);
        }
      }
    }

    private void PopulateParameters(MySqlCommand cmd, params Variable[] vars)
    {
      //Skip the 0 (zero) entry, that is the actual SQL, get all the paramenters after that
      for (int x = 1; x < vars.Length; x++)
      {
        if (vars[x].DataType == DATA_TYPE.String)
        {
          vars[x].GetValue(out string val);
          cmd.Parameters.AddWithValue(vars[x].Name, val);
        }
        else if (vars[x].DataType == DATA_TYPE.Integer)
        {
          vars[x].GetValue(out long val);
          cmd.Parameters.AddWithValue(vars[x].Name, val);
        }
        else if (vars[x].DataType == DATA_TYPE.Decimal)
        {
          vars[x].GetValue(out decimal val);
          cmd.Parameters.AddWithValue(vars[x].Name, val);
        }
        else if (vars[x].DataType == DATA_TYPE.Boolean)
        {
          vars[x].GetValue(out bool val);
          byte valByte = 0;
          if (val == true)
            valByte = 1;
          cmd.Parameters.AddWithValue(vars[x].Name, valByte);
        }

        //cmd.Parameters.AddWithValue(vars[x].Name, 
      }
    }

    public VariableInteger Execute(string SQL, params Variable[] vars)
    {
      VariableInteger root = new VariableInteger("RecordsAffected", 0);
      using (MySqlConnection connection = new MySqlConnection(ConnectionString))
      {
        connection.Open();

        using (MySqlCommand command = new MySqlCommand(SQL, connection))
        {
          PopulateParameters(command, vars);
          long recordsAffected = command.ExecuteNonQuery();
          if (SQL.StartsWith("INSERT", StringComparison.InvariantCultureIgnoreCase) == true)
            root.SubVariables.Add(new VariableInteger("LastInsertedId", command.LastInsertedId));

          root.Value = recordsAffected;
        }
      }
      return root;
    }

    public Variable Select(string SQL, params Variable[] vars)
    {
      Variable root = new Variable("Recordset");
      using (MySqlConnection connection = new MySqlConnection(ConnectionString))
      {
        connection.Open();

        using (MySqlCommand command = new MySqlCommand(SQL, connection))
        {
          PopulateParameters(command, vars);
          using (MySqlDataReader reader = command.ExecuteReader())
          {
            bool tinyAsBool = mDb.SettingGetAsBoolean(Db.Database.DB_TREAT_TINYINT_AS_BOOLEAN);
            while (reader.Read())
            {
              Variable row = new Variable();
              Variable column;
              for (int x = 0; x < reader.FieldCount; x++) 
              {
                //Need to translate database fields to Flow Engine variables.
                string columnName = reader.GetName(x);
                Type type = reader.GetFieldType(x);
                if (reader.IsDBNull(x) == true)
                {
                  column = new Variable(columnName);
                }
                else if (type == typeof(string))
                {
                  column = new VariableString(columnName, reader.GetString(x));
                }
                else if (type == typeof(short))
                {
                  column = new VariableInteger(columnName, reader.GetInt16(x));
                }
                else if (type == typeof(int))
                {
                  column = new VariableInteger(columnName, reader.GetInt32(x));
                }
                else if (type == typeof(long))
                {
                  column = new VariableInteger(columnName, reader.GetInt64(x));
                }
                else if (type == typeof(decimal))
                {
                  column = new VariableDecimal(columnName, reader.GetDecimal(x));
                }
                else if (type == typeof(DateTime))
                {
                  column = new VariableString(columnName, reader.GetDateTime(x).ToString());
                }
                else if (type == typeof(byte) && tinyAsBool == false)
                {
                  column = new VariableInteger(columnName, reader.GetByte(x));
                }
                else if (type == typeof(sbyte) && tinyAsBool == false)
                {
                  column = new VariableInteger(columnName, reader.GetSByte(x));
                }
                else if ((type == typeof(sbyte) || type == typeof(byte)) && tinyAsBool == true)
                {
                  column = new VariableBoolean(columnName, reader.GetSByte(x) >= 1);
                }
                else if (type == typeof(byte) && tinyAsBool == true)
                {
                  column = new VariableBoolean(columnName, reader.GetByte(x) >= 1);
                }
                else
                {
                  Global.Write($"Unknown column type [{type.Name}]", DEBUG_TYPE.Warning);
                  column = new VariableString(columnName, $"Unknown column type [{type.Name}]");
                }
                row.SubVariables.Add(column);
              }
              root.SubVariables.Add(row);
            }
          }
        }
        return root;
      }

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
