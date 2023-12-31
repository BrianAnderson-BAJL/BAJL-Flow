using Core.Interfaces;
using Core.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core
{

  public class Variable : IToJson
  {
    public DATA_TYPE DataType = DATA_TYPE._None;
    public string Name = "";
    public List<Variable> SubVariables;
    public DATA_FORMAT_SUB_VARIABLES SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Block;
    public Variable? Parent = null;
    public dynamic Value { get; set; }

    /// <summary>
    /// Perform a deep copy of a variable and all subvariables
    /// </summary>
    /// <param name="var"></param>
    public Variable(Variable var)
    {
      this.DataType = var.DataType;
      this.Value = var.Value;
      this.Name = var.Name;
      this.Parent = var.Parent;

      this.SubVariables = new List<Variable>(var.SubVariables.Count);
      for (int x = 0; x < var.SubVariables.Count; x++)
      {
        this.Add(var.SubVariables[x].Clone());
      }
    }


    public Variable()
    {
      DataType = DATA_TYPE._None;
      Name = "";
      Value = "";
      SubVariables = new List<Variable>();
    }
    public Variable(string name)
    {
      DataType = DATA_TYPE._None;
      Name = name;
      Value = "";
      SubVariables = new List<Variable>();
    }
    public Variable(string name, long val)
    {
      DataType = DATA_TYPE.Integer;
      Name = name;
      Value = val;
      SubVariables = new List<Variable>();
    }
    public Variable(string name, string val)
    {
      DataType = DATA_TYPE.String;
      Name = name;
      Value = val;
      SubVariables = new List<Variable>();
    }
    public Variable(string name, bool val)
    {
      DataType = DATA_TYPE.Boolean;
      Name = name;
      Value = val;
      SubVariables = new List<Variable>();
    }
    public Variable(string name, decimal val)
    {
      DataType = DATA_TYPE.Decimal;
      Name = name;
      Value = val;
      SubVariables = new List<Variable>();
    }
    public Variable(string name, object val)
    {
      DataType = DATA_TYPE.Object;
      Name = name;
      Value = val;
      SubVariables = new List<Variable>();
    }

    public void GetValue(out dynamic val)
    {
      val = Value;
      return;
    }

    public void GetValue(out string val)
    {
      if (DataType != DATA_TYPE.String)
        throw new Exception("Expected DATA_TYPE.String, actual DATA_TYPE == " + DataType.ToString());

      val = Value;
      return;
    }

    public void GetValue(out bool val)
    {
      if (DataType != DATA_TYPE.Boolean)
        throw new Exception("Expected DATA_TYPE.Boolean, actual DATA_TYPE == " + DataType.ToString());

      val = (bool)Value;
      return;
    }
    public void GetValue(out long val)
    {
      if (DataType != DATA_TYPE.Integer)
        throw new Exception("Expected DATA_TYPE.Integer, actual DATA_TYPE == " + DataType.ToString());

      val = (long)Value;
      return;
    }
    public void GetValue(out decimal val)
    {
      if (DataType != DATA_TYPE.Decimal)
        throw new Exception("Expected DATA_TYPE.Decimal, actual DATA_TYPE == " + DataType.ToString());

      val = (decimal)Value;
      return;
    }

    public void Add(Variable? v)
    {
      if (v is null)
        return;

      v.Parent = this;
      SubVariables.Add(v);
    }

    public void Remove(Variable v)
    {
      SubVariables.Remove(v);
    }

    public Variable Clone()
    {
      return new Variable(this);
    }


    public virtual string ToJson(JSON_ROOT_BLOCK_OPTIONS options = JSON_ROOT_BLOCK_OPTIONS.None, int TabIndents = 0)
    {
      string tabs = new string('\t', TabIndents);
      TabIndents++;
      string jsonStr = "";
      if (this.Name != "" && options == JSON_ROOT_BLOCK_OPTIONS.None)
      {
        if (this.Parent is null || this.Parent.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Block)
          jsonStr += tabs + "\"" + this.Name + "\":";
      }
      if (this.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Block || options == JSON_ROOT_BLOCK_OPTIONS.StripNameFromRootAndAddBlock)
      {
        jsonStr += "{";
        if (this.SubVariables.Count > 0)
          jsonStr += Environment.NewLine;
      }
      else if (this.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Array)
      {
        jsonStr += "[";
        if (this.SubVariables.Count > 0)
          jsonStr += Environment.NewLine;
      }

      for (int x = 0; x < this.SubVariables.Count; x++)
      {
        jsonStr += this.SubVariables[x].ToJson(JSON_ROOT_BLOCK_OPTIONS.None, TabIndents);
        if (x < this.SubVariables.Count - 1)
          jsonStr += "," + Environment.NewLine;
      }

      if (this.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Array)
      {
        if (this.SubVariables.Count > 0)
          jsonStr += Environment.NewLine + tabs;
        jsonStr += "]";
      }
      else if (this.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Block || options == JSON_ROOT_BLOCK_OPTIONS.StripNameFromRootAndAddBlock)
      {
        if (this.SubVariables.Count > 0)
          jsonStr += Environment.NewLine + tabs;
        jsonStr += "}";
      }
      return jsonStr;
    }

    public static Variable? JsonParse(ref string jsonStr)
    {
      if (jsonStr is null)
        throw new ArgumentNullException(nameof(jsonStr));
      jsonStr = jsonStr.Trim();
      if (jsonStr.Length == 0)
        throw new ArgumentException("JSON is a zero length string");
      //if (jsonStr[0] != '[' && jsonStr[0] != '{')
      //  throw new ArgumentException("JSON is invalid, it doesn't start with '[' or '{'");

      //Variable? var = new Variable();
      //var.Name = "data";
      ParserJson parser = new ParserJson();
      Variable? var = parser.ParseJsonBlock(ref jsonStr);
      if (var is not null && var.Name == "" && var.SubVariables.Count > 0)
        return var.SubVariables[0];
      return var;
    }


    public Variable? FindSubVariableByName(string name)
    {
      for (int x = 0; x < this.SubVariables.Count; x++)
      {
        Variable v = this.SubVariables[x];
        if (v.Name == name)
          return v;
      }
      return null;
    }

    public bool DeleteSubVariableByName(string name)
    {
      for (int x = 0; x < this.SubVariables.Count; x++)
      {
        Variable v = this.SubVariables[x];
        if (v.Name == name)
        {
          this.SubVariables.RemoveAt(x);
          return true;
        }
      }
      return true; //Didn't find the variable, so it isn't there, it wasn't deleted, but it is gone, so still a success
    }

    public void GetValueAsString(out string value)
    {
      value = Value.ToString();
      return;
    }
  }
}
