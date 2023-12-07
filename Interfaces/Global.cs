using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core
{
  public enum DATA_TYPE
  {
    _None,
    Various, //A PARM could have various/unknown data types to be passed into a 'multiple' parameter value (Database SELECT statement will have unknown variable types for the data parameters except during runtime)
    Object,  //Could be a connection handle, or other custom object
    String,
    Integer,
    Decimal,
    Boolean,
    Color,
  }

  public enum STRING_SUB_TYPE
  {
    _None, //Just a normal string, nothing special
    DropDownList, //Maps to String
    Sql, //Maps to String
  }

  /// <summary>
  /// This needs to be defined for recreating JSON data from the variables. Are the sub variables stored in a Block { } or an Array [ ], only needed for JSON
  /// </summary>
  public enum DATA_FORMAT_SUB_VARIABLES
  {
    Block, //Json or XML block of elements
    Array, //Json array
  }
  public enum DATA_FORMAT
  {
    _None,
    Json,
    Xml,
  }

  public enum RECORD_RESULT
  {
    Success,
    Error,
    Duplicate,
  }

  public enum DEBUG_TYPE
  {
    Information,
    Warning,
    Error,
  }

  public enum STEP_ERROR_NUMBERS
  {
    //-1 to -1000 = FunctionValidator errors
    FV_MustHaveOneRecord = -1,
    _NoError = 0,
  }

  public class Global
  {
    public static string[] IllegalFileNameCharacters = { "..", "*", "?", "#", "<", ">", "&", "{", "}", "\\\\", "$", "!", "'", "\"", ":", "`", "|", "=" };
    public static string[] IllegalFileNameStartCharacters = { " ", ".", "-", "_" };

    public static int XmlDepthMax = 100;
    public static int JsonDepthMax = 100;

    public static void WriteAllways(string val, DEBUG_TYPE debug = DEBUG_TYPE.Information)
    {
      Console.WriteLine(val);
    }

    [Conditional("DEBUG")]
    public static void Write(string val, DEBUG_TYPE debug = DEBUG_TYPE.Information)
    {
      if (debug == DEBUG_TYPE.Information)
        Console.ForegroundColor = ConsoleColor.White;
      else if (debug == DEBUG_TYPE.Warning)
        Console.ForegroundColor = ConsoleColor.Yellow;
      else
        Console.ForegroundColor = ConsoleColor.Red;

      Console.Write(val);
      
      Console.ForegroundColor = ConsoleColor.White;  //Change the color back to white when done, otherwise the console will stay the color if it exits after an error.
      Console.WriteLine("");
    }


    public static string StripOff(string rootPath, string path)
    {
      return path.Substring(rootPath.Length + 1);
    }
  }

  public static class GlobalExtensions
  {

    /// <summary>
    /// Will return only the numeric characters only in the start of a string.
    /// '200 - OK' - Will return '200'
    /// '45Hello' - Will return '45'
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string StartingNumericOnly(this string Text)
    {
      if (Text is null || Text.Length == 0)
        return "";
      string returnVal = "";
      int x = 0;
      char temp = Text[x++];
      while ("-+0123456789".Contains(temp))
      {
        returnVal += temp;
        temp = Text[x++];
      }
      return returnVal;
    }

    public static string SubStr(this string Text, int Start, int Characters)
    {
      int Length = Text.Length;
      string ReturnText = "";
      if (((Start + Characters) <= Length))
        ReturnText = Text.Substring(Start, Characters);
      else
      {
        ReturnText = "";
      }
      return ReturnText;
    }

    public static string Right(this string Text, int NumberOfCharacters)
    {
      if (Text is null)
        return "";
      if (NumberOfCharacters > Text.Length)
        return "";

      int StartPos = Text.Length - NumberOfCharacters;
      Text = Text.Substring(StartPos, NumberOfCharacters); 
      return Text;
    }

    public static string SubStrTrim(this string Text, int Start, int Characters)
    {
      string ReturnText = Text.SubStr(Start, Characters);
      return ReturnText.Trim();
    }

    public static int IndexOfNotIn(this string Text, char[] MustBeChars, int StartIndex = 0)
    {
      int Found = -1;
      for (int x = StartIndex; x < Text.Length; x++)
      {
        if (MustBeChars.Contains(Text[x]) == false)
        {
          Found = x;
          break;
        }
        else if (x == Text.Length - 1) //We have reached the end of the string, so it is all good
        {
          Found = x + 1;
        }
      }
      return Found;
    }


  }


}
