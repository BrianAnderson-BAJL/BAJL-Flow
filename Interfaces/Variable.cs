using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{

  //public class Var<T>
  //{
  //  public const string BASE_VAR_NAME = "base";

  //  public DATA_TYPE DataType = DATA_TYPE._None;
  //  public string Key = "";
  //  public List<Var<T>> SubVariables;
  //  private T Value;

  //  //Object,
  //  //String,
  //  //Integer,
  //  //Decimal,
  //  //Boolean,
  //  //Color,
  //  //DropDownList,
  //  //DropDownAllowCustom,
  //  //Block, //Json or XML block of elements
  //  //Array, //Json array

  //  public Var(T val)
  //  {
  //    if (typeof(T) == typeof(long) || typeof(T) == typeof(int))
  //      DataType = DATA_TYPE.Integer;
  //    else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(float))
  //      DataType = DATA_TYPE.Decimal;
  //    else if (typeof(T) == typeof(bool))
  //      DataType = DATA_TYPE.Boolean;
  //    else if (typeof(T) == typeof(System.Drawing.Color))
  //      DataType = DATA_TYPE.Color;
  //    else if (typeof(T) == typeof(string))
  //      DataType = DATA_TYPE.String;
  //      SubVariables = new List<Var<T>>();
  //    Value = val;
  //  }


  //  public static object CreateVar(string xml)
  //  {
  //    string nameTemp = Xml.GetXMLChunk(ref xml, "Name");
  //    string literalTemp = Xml.GetXMLChunk(ref xml, "Literal");
  //    string dataTypeTemp = Xml.GetXMLChunk(ref xml, "DataType");
  //    string valueTemp = Xml.GetXMLChunk(ref xml, "Value");
  //    return new Var<long>(val);
  //  }
  //}

  //public class Type_DropDownList
  //{
  //  string Value;
  //  public static bool operator ==(Type_DropDownList dd1, Type_DropDownList dd2)
  //  {
  //    if (dd1.ToString() == dd2.ToString())
  //      return true;
  //    return false;
  //  }

  //  public static bool operator !=(Type_DropDownList dd1, Type_DropDownList dd2)
  //  {
  //    if (dd1.ToString() == dd2.ToString())
  //      return false;
  //    return true;
  //  }

  //  public bool Equals(Type_DropDownList? dd)
  //  {
  //    if (dd == null)
  //      return false;
  //    return (this == dd);
  //  }

  //  public override bool Equals(object obj) => Equals(obj as Type_DropDownList);

  //  public override string ToString()
  //  {
  //    return Value;
  //  }
  //}

  public class Variable
  {

    public DATA_TYPE DataType = DATA_TYPE._None;
    public string Name = "";
    public List<Variable> SubVariables = new List<Variable>();

    public Variable() { }
    public Variable(string name) 
    {
      Name = name;
    }
    public Variable(string name, DATA_TYPE dataType)
    {
      Name = name;
      DataType = dataType;
    }

    public void Add(Variable? v)
    {
      if (v == null)
        return;
      SubVariables.Add(v);
    }

    public void Remove(Variable v)
    {
      SubVariables.Remove(v);
    }

    public Variable Clone()
    {
      Variable v = new Variable(Name);
      v.DataType = DataType;
      for (int x = 0; x < SubVariables.Count; x++)
      {
        v.SubVariables.Add(SubVariables[x].Clone());
      }
      return v;
    }

    public virtual string JsonCreate(bool stripNameAndAddBlock = false) //
    {
      string jsonStr = "";
      if (this.Name != "" && stripNameAndAddBlock == false) //
        jsonStr += "\"" + this.Name + "\":";
      if (this.DataType == DATA_TYPE.Block || stripNameAndAddBlock == true)
        jsonStr += "{" + Environment.NewLine;
      else if (this.DataType == DATA_TYPE.Array)
        jsonStr += "[" + Environment.NewLine;

      for (int x = 0; x < this.SubVariables.Count; x++)
      {
        jsonStr += this.SubVariables[x].JsonCreate();
        if (x < this.SubVariables.Count - 1)
          jsonStr += "," + Environment.NewLine;
      }

      if (this.DataType == DATA_TYPE.Array)
        jsonStr += "]" + Environment.NewLine;
      else if (this.DataType == DATA_TYPE.Block || stripNameAndAddBlock == true)
        jsonStr += "}" + Environment.NewLine;

      return jsonStr;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception">Generic JSON parsing errors</exception>
    public static Variable? JsonParse(ref string jsonStr)
    {
      if (jsonStr == null)
        throw new ArgumentNullException(nameof(jsonStr));
      jsonStr = jsonStr.Trim();
      if (jsonStr.Length == 0)
        throw new ArgumentException("JSON is a zero length string");
      if (jsonStr[0] != '[' && jsonStr[0] != '{')
        throw new ArgumentException("JSON is invalid, it doesn't start with '[' or '{'");

      Variable? var = new Variable();
      var.Name = "data";
      JsonParser parser = new JsonParser();
      parser.ParseJsonBlock(ref jsonStr, var);
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


    public static Variable XmlParse(string xmlStr)
    {
      Variable xml = new Variable();

      return xml;
    }

    
  }

  public class VariableString : Variable
  {
    public string Value = "";
    public VariableString(Variable v)
    {
      this.Name = v.Name;
      this.SubVariables = v.SubVariables;
      this.DataType = DATA_TYPE.String;
    }

    public VariableString(string key, string value)
    {
      Name = key;
      Value = value;
      DataType = DATA_TYPE.String;
    }

    public override string JsonCreate(bool stripNameAndAddBlock = false)
    {
      string jsonStr = "";
      if (this.Name != "")
        jsonStr += "\"" + this.Name + "\":";
      jsonStr += "\"" + this.Value + "\"";
      
      return jsonStr;
    }

    public override string ToString()
    {
      return this.Name + " [string] " + this.Value;
    }
  }

  public class VariableInteger : Variable
  {
    public long Value;
    public VariableInteger(Variable v)
    {
      this.Name = v.Name;
      this.SubVariables = v.SubVariables;
      this.DataType = DATA_TYPE.Integer;
    }

    public override string JsonCreate(bool stripNameAndAddBlock = false)
    {
      string jsonStr = "";
      if (this.Name != "")
        jsonStr += "\"" + this.Name + "\":";
      jsonStr += this.Value.ToString();

      return jsonStr;
    }


    public override string ToString()
    {
      return this.Name + " [int] " + this.Value.ToString();
    }

  }
  public class VariableDecimal : Variable
  {
    public decimal Value;
    public VariableDecimal(Variable v)
    {
      this.Name = v.Name;
      this.SubVariables = v.SubVariables;
      this.DataType = DATA_TYPE.Decimal;
    }
    public override string JsonCreate(bool stripNameAndAddBlock = false)
    {
      string jsonStr = "";
      if (this.Name != "")
        jsonStr += "\"" + this.Name + "\":";
      jsonStr += this.Value.ToString();

      return jsonStr;
    }

    public override string ToString()
    {
      return this.Name + " [dec] " + this.Value.ToString();
    }
  }

  public class VariableBoolean : Variable
  {
    public bool Value;
    public VariableBoolean(Variable v)
    {
      this.Name = v.Name;
      this.SubVariables = v.SubVariables;
      this.DataType = DATA_TYPE.Boolean;
    }

    public override string JsonCreate(bool stripNameAndAddBlock = false)
    {
      string jsonStr = "";
      if (this.Name != "")
        jsonStr += "\"" + this.Name + "\":";
      jsonStr += this.Value.ToString();

      return jsonStr;
    }


    public override string ToString()
    {
      return this.Name + " [bool] " + this.Value.ToString();
    }

  }

  public class VariableObject : Variable
  {
    public object Value;
    public VariableObject(string key, object v)
    {
      Name = key;
      Value = v;
    }
    public override string ToString()
    {
      return this.Name + " [obj] " + this.Value.ToString();
    }

  }

}
