using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{

  //public abstract class aVar
  //{
  //  public string Name;
  //  public List<aVar> SubVariables;
    
  //}

  //public class Var<T> : aVar
  //{
  //  public const string BASE_VAR_NAME = "base";

  //  //public DATA_TYPE DataType = DATA_TYPE._None;
  //  //public string Name = "";
  //  //public List<aVar> SubVariables;
  //  public T Value {get; set; }


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
  //    //if (typeof(T) == typeof(long) || typeof(T) == typeof(int))
  //    //  DataType = DATA_TYPE.Integer;
  //    //else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(float))
  //    //  DataType = DATA_TYPE.Decimal;
  //    //else if (typeof(T) == typeof(bool))
  //    //  DataType = DATA_TYPE.Boolean;
  //    //else if (typeof(T) == typeof(System.Drawing.Color))
  //    //  DataType = DATA_TYPE.Color;
  //    //else if (typeof(T) == typeof(string))
  //    //  DataType = DATA_TYPE.String;
  //    SubVariables = new List<aVar>();
  //    Value = val;
  //  }


  //  public static object CreateVar(string xml)
  //  {
  //    string nameTemp = Xml.GetXMLChunk(ref xml, "Name");
  //    string literalTemp = Xml.GetXMLChunk(ref xml, "Literal");
  //    string dataTypeTemp = Xml.GetXMLChunk(ref xml, "DataType");
  //    string valueTemp = Xml.GetXMLChunk(ref xml, "Value");
  //    return new Var<long>(0);
  //  }

  //  public override bool Equals(object? obj)
  //  {
  //    if (obj is null)
  //      return false;
      
  //    return base.Equals(obj);
  //  }

  //  public override int GetHashCode()
  //  {
  //    return base.GetHashCode();
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

  public class Variable : IToJson
  {

    public DATA_TYPE DataType = DATA_TYPE._None;
    public string Name = "";
    public List<Variable> SubVariables = new List<Variable>();

    public Variable() { }
    public Variable(string name) 
    {
      Name = name.ToLower();
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

    public virtual string ToJson(bool stripNameAndAddBlock = false) //
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
        jsonStr += this.SubVariables[x].ToJson();
        if (x < this.SubVariables.Count - 1)
          jsonStr += "," + Environment.NewLine;
      }

      if (this.DataType == DATA_TYPE.Array)
        jsonStr += "]" + Environment.NewLine;
      else if (this.DataType == DATA_TYPE.Block || stripNameAndAddBlock == true)
        jsonStr += "}" + Environment.NewLine;

      return jsonStr;
    }

    public virtual string XmlCreate()
    {
      Xml xml = new Xml();
      xml.WriteMemoryNew();
      xml.WriteTagStart("Variables");

      xml.WriteTagEnd("Variables");
      return xml.ReadMemory();
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
      ParserJson parser = new ParserJson();
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


    public static Variable XmlParse(ref string xmlStr)
    {
      if (xmlStr == null)
        throw new ArgumentNullException(nameof(xmlStr));
      xmlStr = xmlStr.Trim();
      if (xmlStr.Length == 0)
        throw new ArgumentException("XML is a zero length string");
      if (xmlStr[0] != '<')
        throw new ArgumentException("XML is invalid, it doesn't start with '<'");
      Variable xml = new Variable();
      xml.Name = "data";
      ParserXml parser = new ParserXml();
      parser.ParseXml(ref xmlStr, xml);
      return xml;
    }


    public void GetValue(out string value)
    {
      value = "";
      VariableString? vs = this as VariableString;
      if (vs is not null)
        value = vs.Value;
    }

    public void GetValue(out long value)
    {
      value = 0;
      VariableInteger? vi = this as VariableInteger;
      if (vi is not null)
      {
        value = vi.Value;
      }
      return;
    }

    public void GetValue(out decimal value)
    {
      value = 0;
      VariableDecimal? vd = this as VariableDecimal;
      if (vd is not null)
      {
        value = vd.Value;
      }
      return;
    }

    public void GetValue(out bool value)
    {
      value = false;
      VariableBoolean? vb = this as VariableBoolean;
      if (vb is not null)
      {
        value = vb.Value;
      }
      return;
    }

    public void GetValue(out object value)
    {
      value = "";
      VariableObject? vs = this as VariableObject;
      if (vs is not null)
        value = vs.Value;
    }

    public void GetValue<T>(out T? value) where T : class
    {
      value = null;
      VariableObject? vs = this as VariableObject;
      if (vs is not null)
        value = (T)vs.Value;
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

    public VariableString(string name, string value)
    {
      Name = name;
      Value = value;
      DataType = DATA_TYPE.String;
    }

    public override string ToJson(bool stripNameAndAddBlock = false)
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
    public VariableInteger(string name, long val)
    {
      this.Name = name;
      this.Value = val;
      this.DataType = DATA_TYPE.Integer;
    }

    public override string ToJson(bool stripNameAndAddBlock = false)
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
    public VariableDecimal(string name, decimal value)
    {
      this.Name = name;
      this.Value= value;
      this.DataType = DATA_TYPE.Decimal;
    }

    public override string ToJson(bool stripNameAndAddBlock = false)
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
      this.Name = v.Name.ToLower();
      this.SubVariables = v.SubVariables;
      this.DataType = DATA_TYPE.Boolean;
    }
    public VariableBoolean(string name, bool value)
    {
      this.Name = name;
      this.Value = value;
      this.DataType = DATA_TYPE.Boolean;
    }

    public override string ToJson(bool stripNameAndAddBlock = false)
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
      this.DataType = DATA_TYPE.Object;

    }
    public override string ToString()
    {
      return this.Name + " [obj] " + this.Value.ToString();
    }

  }

}
