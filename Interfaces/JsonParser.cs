using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Variable;

namespace Core
{
  public class JsonParser
  {
    private struct JSON_NEXT_TYPE_STRUCT
    {
      public JSON_NEXT_TYPE NextType;
      public JSON_VALUE_TYPE NextValueType;
    }
    public enum JSON_NEXT_TYPE
    {
      Unknown,
      None,
      Root,
      Block,
      Key,
      Value,
      Array,
    }

    public enum JSON_VALUE_TYPE
    {
      _Not_A_Value,
      String,
      Integer,
      Decimal,
      Boolean,
      Block,
      Array,
    }

    private enum JSON_NEXT_DELIMMITER
    {
      _None_End_Of_JSON,
      Colon,
      Comma,
      ArrayStart,
      ArrayEnd,
      BlockStart,
      BlockEnd,
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <param name="previousType"></param>
    /// <returns></returns>
    private JSON_NEXT_TYPE_STRUCT LookForwardFindNextType(ref string jsonStr, JSON_NEXT_TYPE_STRUCT previousType)
    {
      JSON_NEXT_TYPE_STRUCT nextType = new JSON_NEXT_TYPE_STRUCT();
      nextType.NextType = JSON_NEXT_TYPE.Unknown;
      nextType.NextValueType = JSON_VALUE_TYPE._Not_A_Value;
      if (jsonStr.Length == 0)
      {
        nextType.NextType = JSON_NEXT_TYPE.None;
        return nextType;
      }
      while (StripOff_Chars.Contains(jsonStr[0]) == true)
      {
        jsonStr = jsonStr.Substring(1);
      }

      if (jsonStr[0] == '{')
      {
        nextType.NextType = JSON_NEXT_TYPE.Block;
      }
      else if (jsonStr[0] == '"')
      {
        int endingPos = FindClosingQuotePosition(ref jsonStr, 1);
        JSON_NEXT_DELIMMITER nextDelimmiter = LookForwardNextDelimmiter(ref jsonStr, endingPos + 1);
        if (nextDelimmiter == JSON_NEXT_DELIMMITER.Comma || nextDelimmiter == JSON_NEXT_DELIMMITER._None_End_Of_JSON)
        {
          nextType.NextType = JSON_NEXT_TYPE.Value;
          nextType.NextValueType = FindNextValueType(ref jsonStr);
        }
        else if (nextDelimmiter == JSON_NEXT_DELIMMITER.Colon)
        {
          nextType.NextType = JSON_NEXT_TYPE.Key;
        }
      }
      else if (jsonStr[0] == ':' && previousType.NextType == JSON_NEXT_TYPE.Key)
      {
        nextType.NextType = JSON_NEXT_TYPE.Value;
        nextType.NextValueType = FindNextValueType(ref jsonStr);
      }
      else if (Numeric_Chars.Contains(jsonStr[0]) == true)
      {
        nextType.NextType = JSON_NEXT_TYPE.Value;
        nextType.NextValueType = FindNextValueType(ref jsonStr);
        return nextType;
      }
      else if (Boolean_Chars.Contains(jsonStr[0]) == true)
      {
        nextType.NextType = JSON_NEXT_TYPE.Value;
        nextType.NextValueType = FindNextValueType(ref jsonStr);
        return nextType;
      }
      else
      {
        throw new Exception("Unknown Json Data type!"); //If we reach here we have a problem
      }


      return nextType;
    }

    private char[] Numeric_Chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '+', '.' };
    private char[] Decimal_Chars = { '.' };
    private char[] Block_Chars = { '{' };
    private char[] Array_Chars = { '[' };
    private char[] String_Chars = { '\"' };
    private char[] StripOff_Chars = { ',', ' ', '\r', '\n', '\t' };
    private char[] Ignore_Chars = { ',', ':', ' ', '\r', '\n' };
    private char[] Boolean_Chars = { 't', 'r', 'u', 'e', 'f', 'a', 'l', 's', 'e' };
    private JSON_VALUE_TYPE FindNextValueType(ref string jsonStr)
    {
      JSON_VALUE_TYPE valType = JSON_VALUE_TYPE._Not_A_Value;
      for (int x = 0; x < jsonStr.Length; x++)
      {
        if (Ignore_Chars.Contains(jsonStr[x]) == false)
        {
          if (Block_Chars.Contains(jsonStr[x]) == true) //It is a Block, just return that
          {
            return JSON_VALUE_TYPE.Block;
          }
          else if (String_Chars.Contains(jsonStr[x]) == true) //Any quotes means it is a string
          {
            return JSON_VALUE_TYPE.String;
          }
          else if (Boolean_Chars.Contains(jsonStr[x]) == true)
          {
            return JSON_VALUE_TYPE.Boolean;
          }
          else if (Numeric_Chars.Contains(jsonStr[x]) == true)
          {
            valType = JSON_VALUE_TYPE.Integer; //At this point it could be an Integer, but it might be a decimal
            if (Decimal_Chars.Contains(jsonStr[x]) == true)
            {
              return JSON_VALUE_TYPE.Decimal; //Found a decimal only character '.', it is certainly a decimal number
            }
          }
          else if (Array_Chars.Contains(jsonStr[x]) == true)
          {
            return JSON_VALUE_TYPE.Array;
          }
        }
        else if (valType != JSON_VALUE_TYPE._Not_A_Value) //We have hit an invalid character after we selected a value type, return with what we say it is
        {
          return valType;
        }
      }
      return valType;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <param name="startPos"></param>
    /// <returns></returns>
    private int FindClosingQuotePosition(ref string jsonStr, int startPos)
    {
      //Need to loop through to check for escape characters
      for (int x = startPos; x < jsonStr.Length; x++)
      {
        if (jsonStr[x] == '"' && (jsonStr[x - 1] != '\\')) //Make sure it is a quote without an escape character in front of it
        {
          return x;
        }
      }
      return -1; //Could not find the ending quote
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <param name="startPos"></param>
    /// <returns></returns>
    private JSON_NEXT_DELIMMITER LookForwardNextDelimmiter(ref string jsonStr, int startPos)
    {
      for (int x = startPos; x < jsonStr.Length; x++)
      {
        if (jsonStr[x] == ':' && (jsonStr[x - 1] != '\\')) //Make sure it is a quote without an escape character in front of it
        {
          return JSON_NEXT_DELIMMITER.Colon;
        }
        else if (jsonStr[x] == ',' && (jsonStr[x - 1] != '\\'))
        {
          return JSON_NEXT_DELIMMITER.Comma;
        }
        else if (jsonStr[x] == '[')
        {
          return JSON_NEXT_DELIMMITER.ArrayStart;
        }
        else if (jsonStr[x] == ']')
        {
          return JSON_NEXT_DELIMMITER.ArrayEnd;
        }
        else if (jsonStr[x] == '{')
        {
          return JSON_NEXT_DELIMMITER.BlockStart;
        }
        else if (jsonStr[x] == '}')
        {
          return JSON_NEXT_DELIMMITER.BlockEnd;
        }
      }
      return JSON_NEXT_DELIMMITER._None_End_Of_JSON; //Could not find the ending quote

    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    private string ParseJsonKey(ref string jsonStr)
    {
      int startPos = jsonStr.IndexOf('\"');
      if (startPos == -1)
        return "";
      int endingPos = FindClosingQuotePosition(ref jsonStr, startPos + 1);
      if (endingPos == -1) //Didn't find the closing brace, bad JSON
        return "";

      string Key = jsonStr.SubStrTrim(startPos + 1, endingPos - startPos - 1);
      startPos = jsonStr.IndexOf(':', endingPos);
      if (startPos == -1)  //We found a key, but didn't find a value, something is wrong
        return "";

      jsonStr = jsonStr.SubStrTrim(endingPos + 1, jsonStr.Length - (endingPos + 1));
      return Key;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private string ParseJsonStringValue(ref string jsonStr)
    {
      int startPos = jsonStr.IndexOf('\"');
      if (startPos == -1)
        throw new Exception("JSON is invalid, could not find '\"' for start of string value element");
      int endingPos = FindClosingQuotePosition(ref jsonStr, startPos + 1);
      if (endingPos == -1) //Didn't find the closing brace, bad JSON
        throw new Exception("JSON is invalid, could not find '\"' for ending of string value element");

      string value = jsonStr.SubStrTrim(startPos + 1, endingPos - startPos - 1);

      jsonStr = jsonStr.SubStrTrim(endingPos + 1, jsonStr.Length - (endingPos + 1));
      return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private long ParseJsonIntegerValue(ref string jsonStr)
    {
      int startPos = jsonStr.IndexOfAny(Numeric_Chars);
      if (startPos == -1)
        throw new Exception("JSON is invalid, could not find numeric characters for Integer value element");
      int endingPos = jsonStr.IndexOfNotIn(Numeric_Chars, startPos);
      if (endingPos == -1) //Didn't find the closing brace, bad JSON
        throw new Exception("JSON is invalid, could not find ending for Integer value element");

      string value = jsonStr.SubStrTrim(startPos, endingPos - startPos);

      jsonStr = jsonStr.SubStrTrim(endingPos, jsonStr.Length - endingPos);
      return long.Parse(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private bool ParseJsonBooleanValue(ref string jsonStr)
    {
      int startPos = jsonStr.IndexOfAny(Boolean_Chars);
      if (startPos == -1)
        throw new Exception("JSON is invalid, could not find numeric characters for Integer value element");
      int endingPos = jsonStr.IndexOfNotIn(Boolean_Chars, startPos);
      if (endingPos == -1) //Didn't find the closing brace, bad JSON
        throw new Exception("JSON is invalid, could not find ending for Integer value element");

      string value = jsonStr.SubStrTrim(startPos, endingPos - startPos);

      jsonStr = jsonStr.SubStrTrim(endingPos, jsonStr.Length - endingPos);
      return bool.Parse(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private decimal ParseJsonDecimalValue(ref string jsonStr)
    {
      int startPos = jsonStr.IndexOfAny(Numeric_Chars);
      if (startPos == -1)
        throw new Exception("JSON is invalid, could not find numeric characters for Integer value element");
      int endingPos = jsonStr.IndexOfNotIn(Numeric_Chars, startPos);
      if (endingPos == -1) //Didn't find the closing brace, bad JSON
        throw new Exception("JSON is invalid, could not find ending for Integer value element");

      string value = jsonStr.SubStrTrim(startPos, endingPos - startPos);

      jsonStr = jsonStr.Substring(endingPos, jsonStr.Length - endingPos);
      return decimal.Parse(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private string GetArray(ref string jsonStr)
    {
      int startPos = jsonStr.IndexOf('[');
      if (startPos == -1)
        throw new Exception("JSON BAD!");
      int endingPos = FindEndingPos(ref jsonStr, startPos + 1, '[', ']');
      if (endingPos == -1) //Didn't find the closing brace, bad JSON
        throw new Exception("JSON BAD!");

      string val = jsonStr.SubStrTrim(startPos + 1, endingPos - startPos - 1);
      jsonStr = jsonStr.Substring(endingPos + 1);
      return val;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private string GetBlock(ref string jsonStr)
    {
      int startPos = jsonStr.IndexOf('{');
      if (startPos == -1)
        throw new Exception("JSON BAD!");
      int endingPos = FindEndingPos(ref jsonStr, startPos + 1);
      if (endingPos == -1) //Didn't find the closing brace, bad JSON
        throw new Exception("JSON BAD!");

      string val = jsonStr.SubStrTrim(startPos + 1, endingPos - startPos - 1);
      jsonStr = jsonStr.Substring(endingPos + 1);
      return val;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <param name="jsonVar"></param>
    /// <returns></returns>
    public Variable? ParseJsonBlock(ref string jsonStr, Variable jsonVar)
    {
      Variable newKey = new Variable();
      JSON_NEXT_TYPE_STRUCT jsonType = new JSON_NEXT_TYPE_STRUCT();
      if (jsonStr.Length == 0)
      {
        return jsonVar;
      }
      do
      {
        jsonType = LookForwardFindNextType(ref jsonStr, jsonType);
        if (jsonType.NextType == JSON_NEXT_TYPE.Block)
        {
          string subBlock = GetBlock(ref jsonStr);
          newKey = new Variable(); //There is no key for base blocks
          newKey.DataType = DATA_TYPE.Block;
          Variable? temp = ParseJsonBlock(ref subBlock, newKey);
          if (temp != null)
          {
            if (temp.Name == "") //This new block doesn't have a name, just add the subblocks to the parent
            {
              for (int x = 0; x < temp.SubVariables.Count; x++)
              {
                jsonVar.SubVariables.Add(temp.SubVariables[x]);
              }
            }
            else
            {
              jsonVar.DataType = DATA_TYPE.Block;
              jsonVar.SubVariables.Add(temp);
            }
          }
        }
        else if (jsonType.NextType == JSON_NEXT_TYPE.Array)
        {
          string subArray = GetArray(ref jsonStr);
          newKey = new Variable(); //There is no key for base arrays
          newKey.DataType = DATA_TYPE.Array;
          Variable? temp = ParseJsonBlock(ref subArray, newKey);
          if (temp != null)
          {
            jsonVar.DataType = DATA_TYPE.Array;
            jsonVar.SubVariables.Add(temp);
          }
        }
        else if (jsonType.NextType == JSON_NEXT_TYPE.Key)
        {
          newKey = new Variable(); //We don't know the value type yet, just create a generic variable, the JSON_NEXT_TYPE.Value will populate the data type
          newKey.Name = ParseJsonKey(ref jsonStr);
        }
        else if (jsonType.NextType == JSON_NEXT_TYPE.Value)
        {
          if (jsonType.NextValueType == JSON_VALUE_TYPE.String)
          {
            VariableString vs = new VariableString(newKey); //We now know it is a string, lets transform it into such
            vs.Value = ParseJsonStringValue(ref jsonStr);
            jsonVar.SubVariables.Add(vs); //Now we can add it to the sub variables list, now that we know the data type
          }
          else if (jsonType.NextValueType == JSON_VALUE_TYPE.Integer)
          {
            VariableInteger vi = new VariableInteger(newKey); //We now know it is a string, lets transform it into such
            vi.Value = ParseJsonIntegerValue(ref jsonStr);
            jsonVar.SubVariables.Add(vi); //Now we can add it to the sub variables list, now that we know the data type
          }
          else if (jsonType.NextValueType == JSON_VALUE_TYPE.Decimal)
          {
            VariableDecimal vd = new VariableDecimal(newKey); //We now know it is a string, lets transform it into such
            vd.Value = ParseJsonDecimalValue(ref jsonStr);
            jsonVar.SubVariables.Add(vd); //Now we can add it to the sub variables list, now that we know the data type
          }
          else if (jsonType.NextValueType == JSON_VALUE_TYPE.Boolean)
          {
            VariableBoolean vi = new VariableBoolean(newKey); //We now know it is a string, lets transform it into such
            vi.Value = ParseJsonBooleanValue(ref jsonStr);
            jsonVar.SubVariables.Add(vi); //Now we can add it to the sub variables list, now that we know the data type
          }
          else if (jsonType.NextValueType == JSON_VALUE_TYPE.Array)
          {
            string subArray = GetArray(ref jsonStr);
            Variable? temp = ParseJsonBlock(ref subArray, newKey);
            if (temp != null)
            {
              temp.DataType = DATA_TYPE.Array;
              jsonVar.SubVariables.Add(temp);
            }
          }
          else if (jsonType.NextValueType == JSON_VALUE_TYPE.Block)
          {
            string subBlock = GetBlock(ref jsonStr);
            Variable? temp = ParseJsonBlock(ref subBlock, newKey);
            if (temp != null)
            {
              temp.DataType = DATA_TYPE.Block;
              jsonVar.SubVariables.Add(temp);
            }
          }
        }
      } while (jsonStr.Length > 0);
      return jsonVar;
    }



    private int FindEndingPos(ref string jsonStr, int startPos, char startC = '{', char endC = '}')
    {
      char[] searchFor = new char[] { startC, endC };
      int foundPos = 0;
      int Count = 1;
      do
      {
        foundPos = jsonStr.IndexOfAny(searchFor, startPos);
        if (foundPos >= startPos)
        {
          startPos = foundPos + 1;
          if (jsonStr[foundPos] == startC)
          {
            Count++;
          }
          else
          {
            Count--;
          }
        }
      } while (Count > 0 && foundPos > -1);
      return foundPos;
    }
  }
}
