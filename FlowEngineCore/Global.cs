﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace FlowEngineCore
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

  /// <summary>
  /// All these types map to the String data type above.
  /// </summary>
  public enum STRING_SUB_TYPE
  {
    _None, //Just a normal string, nothing special
    DropDownList,
    Sql, 
    TemplateFile, 
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

  public enum DATABASE_TYPE
  {
    MariaDb,
    SqlServer,
  }

  public enum RECORD_RESULT
  {
    Success,
    Error,
    Duplicate,
  }

  public enum LOG_TYPE
  {
    DBG,  // Debug
    INF,  // Information
    WAR,  // Warning
    ERR,  // Error
  }

  public enum STEP_ERROR_NUMBERS
  {
    //-1 to -1000 = FunctionValidator errors
    FV_MustHaveZeroRecords = -2,
    FV_MustHaveOneRecord = -1,
    _NoError = 0, //SUCCESS
    NotImplemented = 1,
    CustomErrorMin = 1000,
    CustomErrorMax = 1999,
    FlowCoreErrorMin = 2000,
    FlowCoreErrorMax = 2999,
    DatabaseErrorMin = 3000,
    DatabaseErrorMax = 3999,
    HttpErrorMin = 4000,
    HttpErrorMax = 4999,
    LoggerErrorMin = 5000,
    LoggerErrorMax = 5999,
    SessionErrorMin = 6000,
    SessionErrorMax = 6999,
    ValidationErrorMin = 7000,
    ValidationErrorMax = 7999,
    EmailErrorMin = 8000,
    EmailErrorMax = 8999,
    DataErrorMin = 9000,
    DataErrorMax = 9999,
    GoogleErrorMin = 10000,
    GoogleErrorMax = 10999
  }

  public class Global
  {
    public static string[] IllegalFileNameCharacters = { "..", "*", "?", "#", "<", ">", "&", "{", "}", "\\\\", "$", "!", "'", "\"", ":", "`", "|", "=" };
    public static string[] IllegalFileNameStartCharacters = { " ", ".", "-", "_" };

    public static string[] NUMERIC = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+", "-", ".", ",", };
    public static int XmlDepthMax = 100;
    public static int JsonDepthMax = 100;

    public static string ConvertBackslashesToForwardslashes(string orig)
    {
      return orig.Replace('\\', '/');
    }

    public static void WriteAlways(string val, LOG_TYPE debug = LOG_TYPE.INF)
    {
      if (debug == LOG_TYPE.INF)
        Console.ForegroundColor = ConsoleColor.White;
      else if (debug == LOG_TYPE.WAR)
        Console.ForegroundColor = ConsoleColor.Yellow;
      else
        Console.ForegroundColor = ConsoleColor.Red;

      Console.Write(val);

      Console.ForegroundColor = ConsoleColor.White;  //Change the color back to white when done, otherwise the console will stay the color if it exits after an error.
      Console.WriteLine("");
    }

    [Conditional("DEBUG")]
    public static void WriteToConsoleDebug(string val, LOG_TYPE debug = LOG_TYPE.INF)
    {
      if (debug == LOG_TYPE.INF)
        Console.ForegroundColor = ConsoleColor.White;
      else if (debug == LOG_TYPE.WAR)
        Console.ForegroundColor = ConsoleColor.Yellow;
      else
        Console.ForegroundColor = ConsoleColor.Red;

      Console.Write(val);
      
      Console.ForegroundColor = ConsoleColor.White;  //Change the color back to white when done, otherwise the console will stay the color if it exits after an error.
      Console.WriteLine("");
    }


    public static string FullExceptionMessage(Exception ex)
    {
      string msg = ex.Message;
      while (ex.InnerException is not null)
      {
        ex = ex.InnerException;
        msg += " -(inner)- " + ex.Message;
      }
      return msg;
    }

    public static string StripOff(string rootPath, string path)
    {
      return path.Substring(rootPath.Length + 1);
    }


    public static string ConvertToString(long ticks)
    {
      return ConvertToString(TimeSpan.FromTicks(ticks));
    }


    public static string ConvertToString(TimeSpan ts, bool includeNanoseconds = true)
    {
      if (ts.TotalSeconds < 1)
      {
        if (includeNanoseconds == true)
          return ts.TotalMilliseconds.ToString() + "ms";
        else
          return ts.TotalMilliseconds.ToString("0") + "ms";
      }
      else if (ts.TotalMinutes < 1)
      {
        return ts.Seconds.ToString("00") + "s " + ts.Milliseconds + "ms";
      }
      else if (ts.TotalHours < 1)
      {
        return ts.Minutes.ToString() + "m " + ts.Seconds.ToString() + "s ";
      }
      else if (ts.TotalDays >= 1)
      {
        return Math.Floor(ts.TotalDays).ToString() + "d " + ts.Hours.ToString() + "h " + ts.Minutes.ToString() + "m";
      }
      return ts.ToString();
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


  public static class Vector2Extension
  {
    public static Point ToPoint(this Vector2 vec)
    {
      return new Point((int)vec.X, (int)vec.Y);
    }
    public static Point ToPoint(this Vector2 vec, int addAmount)
    {
      return new Point((int)vec.X + addAmount, (int)vec.Y + addAmount);
    }
  }
}
