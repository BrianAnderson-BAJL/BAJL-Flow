using FlowEngineCore.Interfaces;
using FlowEngineCore.Parsers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowEngineCore
{

  public class Variable : IToJson
  {
    public DATA_TYPE DataType = DATA_TYPE._None;
    public string Name = "";
    public List<Variable> SubVariables;
    public DATA_FORMAT_SUB_VARIABLES SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Block;
    public Variable? Parent = null;
    public dynamic Value { get; set; }
    
    //public List<Variable> sv
    //{
    //  get { return SubVariables; }
    //}

    public Variable this[int x]
    {
      get { return SubVariables[x]; }
      set { SubVariables[x] = value; }
    }

    public Variable this[string subVarName]
    {
      get 
      {
        Variable? var = SubVariableFindByName(subVarName);
        if (var is null)
          throw new ArgumentException($"Could not find SubVariable by name [{subVarName}]");

        return var; }
      set 
      {
        Variable? var = SubVariableFindByName(subVarName);
        if (var is null)
          throw new ArgumentException($"Could not find SubVariable by name [{subVarName}]");
        var = value; 
      }
    }

    public int Count
    {
      get {return SubVariables.Count; }
    }

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
        this.SubVariableAdd(var.SubVariables[x].Clone());
      }
    }

    public Variable(DATA_FORMAT_SUB_VARIABLES dataFormat)
    {
      SubVariablesFormat = dataFormat;
      DataType = DATA_TYPE._None;
      Name = "";
      Value = "";
      SubVariables = new List<Variable>();
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
    public Variable(string name, DATA_TYPE dataType)
    {
      DataType = dataType;
      Name = name;
      Value =""; //long, won't be expensive if the type changes later
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
        throw new Exception($"[{Name}] Expected DATA_TYPE.String, actual DATA_TYPE == " + DataType.ToString());

      val = Value;
      return;
    }

    public void GetValue(out bool val)
    {
      if (DataType != DATA_TYPE.Boolean)
        throw new Exception($"[{Name}] Expected DATA_TYPE.Boolean, actual DATA_TYPE == " + DataType.ToString());

      val = (bool)Value;
      return;
    }
    public void GetValue(out long val)
    {
      if (DataType != DATA_TYPE.Integer)
        throw new Exception($"[{Name}] Expected DATA_TYPE.Integer, actual DATA_TYPE == " + DataType.ToString());

      val = (long)Value;
      return;
    }
    public void GetValue(out decimal val)
    {
      if (DataType != DATA_TYPE.Decimal)
        throw new Exception($"[{Name}] Expected DATA_TYPE.Decimal, actual DATA_TYPE == " + DataType.ToString());

      val = (decimal)Value;
      return;
    }

    public void SubVariableAdd(Variable? v)
    {
      if (v is null)
        return;

      v.Parent = this;
      SubVariables.Add(v);
    }

    public void SubVariableRemove(Variable v)
    {
      SubVariables.Remove(v);
    }

    public Variable Clone()
    {
      return new Variable(this);
    }

    /// <summary>
    /// This is handy when using a variable in a paramaterized SQL statement, you can get the Variable with the new name that is needed for the parameter (i.e. 'phone' --> '@PhoneNumber')
    /// </summary>
    /// <param name="newName"></param>
    /// <returns></returns>
    public Variable CloneWithNewName(string newName)
    {
      Variable v = new Variable(this);
      v.Name = newName;
      return v;
    }




    public virtual string ToJson()
    {
      JObject baseObject = new JObject();

      if (this.DataType == DATA_TYPE._None && this.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Block)
      {
        baseObject.Add(this.Name, this.ToJsonObject(this));
      }
      else if (this.DataType == DATA_TYPE._None && this.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Array)
      {
        baseObject.Add(this.Name, ToJsonArray(this));
      }
      else
      {
        if (this.DataType == DATA_TYPE.Object)
          baseObject.Add(this.Name, this.Value.ToString() + $" [{this.Value.GetHashCode()}]");
        else
          baseObject.Add(this.Name, this.Value);
      }
      return baseObject.ToString();
    }



    private JObject ToJsonObject(Variable var, JObject? parentJObject = null)
    {
      JObject jo;
      if (parentJObject is null)
        jo = new JObject();
      else
        jo = parentJObject;

      if (var.DataType != DATA_TYPE._None)
      {
        if (var.DataType == DATA_TYPE.Object)
          jo.Add(var.Name, var.Value.ToString());
        else
          jo.Add(var.Name, var.Value);
      }

      for (int x = 0; x < var.SubVariables.Count; x++)
      {
        Variable varSub = var.SubVariables[x];
        if (varSub.DataType == DATA_TYPE._None && varSub.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Block)
        {
          jo.Add(new JProperty(varSub.Name, ToJsonObject(varSub)));
        }
        else if (varSub.DataType == DATA_TYPE._None && varSub.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Array)
        {
          jo.Add(new JProperty(varSub.Name, ToJsonArray(varSub)));
        }
        else
        {
          if (varSub.DataType == DATA_TYPE.Object)
            jo.Add(varSub.Name, varSub.Value.ToString() + $" [{varSub.Value.GetHashCode()}]");
          else
            jo.Add(varSub.Name, varSub.Value);
        }
      }
      return jo;
    }

    private JArray ToJsonArray(Variable var)
    {
      JArray ja = new JArray();
      for (int x = 0; x < var.SubVariables.Count; x++)
      {
        Variable varSub = var.SubVariables[x];
        if (varSub.DataType == DATA_TYPE._None && varSub.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Block)
        {
          ja.Add(ToJsonObject(varSub));
        }
        else if (varSub.DataType == DATA_TYPE._None && varSub.SubVariablesFormat == DATA_FORMAT_SUB_VARIABLES.Array)
        {
          ja.Add(ToJsonArray(varSub));
        }
        else
        {
          if (varSub.DataType == DATA_TYPE.Object)
            ja.Add(varSub.Value.ToString() + $" [{varSub.Value.GetHashCode()}]");
          else
            ja.Add(varSub.Value);
        }
      }

      return ja;
    }

    public static Variable? JsonParse(ref string jsonStr, string baseBlockNewName = "")
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
      string temp = jsonStr;
      ParserJson parser2 = new ParserJson();
      Variable? var = parser2.Parse(temp);
      //ParserJson parser = new ParserJson();
      //Variable? var = parser.ParseJsonBlock(ref jsonStr);
      if (var is not null && baseBlockNewName != "")
        var.Name = baseBlockNewName;
      if (var is not null && var.Name == "" && var.SubVariables.Count > 0)
        return var.SubVariables[0];
      return var;
    }


    public Variable? SubVariableFindByName(string name)
    {
      for (int x = 0; x < this.SubVariables.Count; x++)
      {
        Variable v = this.SubVariables[x];
        if (v.Name == name)
          return v;
      }
      return null;
    }

    public bool SubVariableDeleteByName(string name)
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

    public string GetValueAsString() //out string value
    {
      
      return Value.ToString();
    }

    public override string ToString()
    {
      return Name + ":" + Value.ToString();
    }
  }
}
