using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowEngineCore.Parsers
{
  internal class ParserJson
  {

    public Variable Parse(string json)
    {
      try
      {
        JsonSerializerSettings s = new JsonSerializerSettings();
        s.MaxDepth = 10;
        JObject? jo = JsonConvert.DeserializeObject<JObject>(json, s);
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

      if (jt.Type == JTokenType.String || jt.Type == JTokenType.Date || jt.Type == JTokenType.Guid || jt.Type == JTokenType.TimeSpan || jt.Type == JTokenType.Uri)
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
      for (int x = 0; x < ja.Count; x++)
      {
        Variable var = new Variable();
        JValue? jv = ja[x] as JValue;
        if (jv is not null)
        {
          Parse(jv, var);
          parent.SubVariableAdd(var);
        }
        JObject? jo = ja[x] as JObject;
        if (jo is not null)
        {
          Parse(jo, var);
          parent.SubVariableAdd(var);
        }
        JArray? ja2 = ja[x] as JArray; //Array could contain another array
        if (ja2 is not null)
        {
          Parse(ja2, var);
          parent.SubVariableAdd(var);
        }

      }
    }

    private void Parse(JObject? jo, Variable parent)
    {
      if (jo is null)
        return;

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
