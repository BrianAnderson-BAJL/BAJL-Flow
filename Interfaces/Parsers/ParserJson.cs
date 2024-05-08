using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Parsers
{
  internal class ParserJson
  {

    public Variable Parse(string json)
    {
      try
      {
        JObject jo = JObject.Parse(json);
        Variable data = new Variable();
        Parse(jo, data);
        return data;
      }
      catch (Exception ex)
      { 
        Global.WriteToConsoleDebug(ex.ToString());
        throw;
      }
    }

    private void Parse(JValue? jt, Variable var)
    {
      
      if (jt is null)
        return;

      if (jt.Type == JTokenType.String)
      {
        var.Value = jt.Value<string>()!;
        var.DataType = DATA_TYPE.String;
      }
      else if (jt.Type == JTokenType.Float)
      {
        var.Value = (decimal)jt.Value<float>()!;
        var.DataType = DATA_TYPE.Decimal;
      }
      else if (jt.Type == JTokenType.Integer)
      {
        var.Value = (long)jt.Value<int>();
        var.DataType = DATA_TYPE.Integer;
      }
      else if (jt.Type == JTokenType.Boolean)
      {
        var.Value = (bool)jt.Value<bool>();
        var.DataType = DATA_TYPE.Boolean;
      }
    }


    private void Parse(JArray ja, Variable parent)
    {
      
      foreach (JValue item in ja)
      {
        Variable var = new Variable();
        parent.SubVariableAdd(var);
        Parse(item, var);
      }
    }

    private void Parse(JObject jo, Variable parent)
    {
      foreach (var item in jo)
      {
        JToken? name = item.Key;
        if (name is null)
          continue;

        Variable var = new Variable(name.ToString());
        parent.SubVariableAdd(var);

        if (item.Value is null)
          continue;

        JValue? jv = item.Value as JValue;
        if (jv is not null)
        {
          Parse(jv, var);
          continue; //We parsed a value, lets go to the next one
        }

        JArray? ja = item.Value as JArray;
        if (ja is not null)
        {
          var.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;
          Parse(ja, var);
          continue;
        }

        JObject? jo2 = item.Value as JObject;
        if (jo2 is not null)
        {
          Parse(jo2, var);
        }
      }
    }
  }
}
