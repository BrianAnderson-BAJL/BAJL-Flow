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
    Object,
    String,
    Integer,
    Decimal,
    Boolean,
    Color,
    DropDownList,
    DropDownAllowCustom,
    Block, //Json or XML block of elements
    Array, //Json array
  }

  public enum DATA_FORMAT
  {
    _None,
    Json,
    Xml,
  }

  

  public class Global
  {
    public static int XmlDepthMax = 100;
    public static int JsonDepthMax = 100;

    public static void WriteAllways(string val)
    {
      Console.WriteLine(val);
    }

    [Conditional("DEBUG")]
    public static void Write(string val)
    {
      Console.WriteLine(val);
    }
    public static void Write(string val, params string[] otherVals)
    {
      Console.WriteLine(String.Format(val, otherVals));
    }
  }

  public static class GlobalExtensions
  {

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
