using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  /// <summary>
  /// Lazy mans XML parser / writer, doesn't support attributes or duplicate block names embedded inside blocks (i.e. <Parm><Parm></Parm></Parm>
  /// </summary>
  public class Xml
  {
    public enum BASE_64_ENCODE
    {
      None,
      Encoded,

    }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    private string mFileName = "";
    private string mFileContents = "";
    private StreamWriter? mWriter = null;
    private MemoryStream? mMemoryWriter = null;
    private int mIndentLevel = 0;
    private static CultureInfo culture = CultureInfo.InvariantCulture;

    public int IndentLevel
    {
      get { return mIndentLevel; }
    }
    public bool WriteFileNew(string FileName)
    {
      bool Rc = false;
      try
      {
        if (culture == null)
        {
          culture = CultureInfo.InvariantCulture;
          //culture = CultureInfo.CreateSpecificCulture("en-US");
        }

        mWriter = new StreamWriter(FileName, false, System.Text.Encoding.UTF8);
        mIndentLevel = 0;
        Rc = true;
      }
      catch
      {
        Rc = false;
      }
      return Rc;
    }

    public bool WriteMemoryNew(int startingIndentLevel = 0)
    {
      try
      {
        mIndentLevel = startingIndentLevel;
        mMemoryWriter = new MemoryStream(1024);
        mWriter = new StreamWriter(mMemoryWriter);
        return true;
      }
      catch 
      { 
        return false; 
      }
    }

    public string ReadMemory()
    {
      try
      {
        mWriter.Flush();
        mMemoryWriter.Seek(0, SeekOrigin.Begin);
        StreamReader sr = new StreamReader(mMemoryWriter);
        string data = sr.ReadToEnd();
        mWriter.Dispose();
        mMemoryWriter.Dispose();
        sr.Dispose();
        return data;
      }
      catch { }
      return "";
    }

    private string BuildTabsBasedOnIndentLevel()
    {
      string Tabs = "";
      for (int x = 0; x < mIndentLevel; x++)
      {
        Tabs += "\t";
      }
      return Tabs;
    }

    public void WriteTagAndContents(string TagName, System.Enum Value)
    {
      string? Val = System.Enum.GetName(Value.GetType(), Value);
      if (Val != null)
      {
        WriteTagAndContents(TagName, Val);
      }
    }

    public void WriteTagAndContents(string TagName, Rectangle Value)
    {
      WriteTagAndContents(TagName, Value.ToString());
    }


    public void WriteTagAndContents(string TagName, TimeSpan Value)
    {
      WriteTagAndContents(TagName, Value.ToString());
    }

    public void WriteTagAndContents(string TagName, Quaternion Value)
    {
      WriteTagAndContents(TagName, Value.ToString());
    }

    public void WriteTagAndContents(string TagName, Color Value)
    {
      WriteTagAndContents(TagName, Value.ToString());
    }

    public void WriteTagAndContents(string TagName, Vector3 Value)
    {
      WriteTagAndContents(TagName, Value.ToString());
    }

    public void WriteTagAndContents(string TagName, Vector2 Value)
    {
      WriteTagAndContents(TagName, Value.X.ToString() + "," + Value.Y.ToString());
    }
    public void WriteTagAndContents(string TagName, Size Value)
    {
      WriteTagAndContents(TagName, Value.Width.ToString() + "," + Value.Height.ToString());
    }

    public void WriteTagAndContents(string TagName, decimal Value)
    {
      WriteTagAndContents(TagName, Value.ToString(culture));
    }

    public void WriteTagAndContents(string TagName, float Value)
    {
      WriteTagAndContents(TagName, Value.ToString(culture));
    }

    public void WriteTagAndContents(string TagName, bool Value)
    {
      WriteTagAndContents(TagName, Value.ToString(culture));
    }

    public void WriteTagAndContents(string TagName, int Value)
    {
      WriteTagAndContents(TagName, Value.ToString(culture));
    }

    public void WriteTagAndContents(string TagName, long Value)
    {
      WriteTagAndContents(TagName, Value.ToString(culture));
    }

    public void WriteTagAndContents(string TagName, DateTime Value)
    {
      WriteTagAndContents(TagName, Value.ToString(culture));
    }

    //public void WriteTagAndContents(string TagName, string Value)
    //{
    //  string Tabs = BuildTabsBasedOnIndentLevel();
    //  TagName = Tabs + "<" + TagName + ">" + WebUtility.HtmlEncode(Value) + "</" + TagName + ">";
    //  mWriter.WriteLine(TagName);
    //}
    public void WriteTagAndContents(string TagName, string Value, BASE_64_ENCODE encoding = BASE_64_ENCODE.None)
    {
      string Tabs = BuildTabsBasedOnIndentLevel();
      if (encoding == BASE_64_ENCODE.Encoded)
      {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Value);
        Value = System.Convert.ToBase64String(plainTextBytes);
      }
      TagName = Tabs + "<" + TagName + ">" + Value + "</" + TagName + ">";
      mWriter.WriteLine(TagName);
    }

    /// <summary>
    /// Will insert preformatted xml data into this xml.
    /// </summary>
    /// <param name="xml"></param>
    public void WriteXml(string xml)
    {
      mWriter.Write(xml);
    }
    public void WriteTagStart(string TagName)
    {
      string Tabs = BuildTabsBasedOnIndentLevel();
      TagName = Tabs + "<" + TagName + ">";
      mWriter.WriteLine(TagName);
      mIndentLevel++;
    }

    public void WriteTagContents(string Value, BASE_64_ENCODE encoding = BASE_64_ENCODE.None)
    {
      //mIndentLevel++;
      string Tabs = BuildTabsBasedOnIndentLevel();
      if (encoding == BASE_64_ENCODE.Encoded)
      {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Value);
        Value = System.Convert.ToBase64String(plainTextBytes);
      }
      mWriter.WriteLine(Tabs + Value);
      //mIndentLevel--;
    }

    public void WriteTagEnd(string TagName)
    {
      mIndentLevel--;
      string Tabs = BuildTabsBasedOnIndentLevel();
      TagName = Tabs + "</" + TagName + ">";
      mWriter.WriteLine(TagName);
    }

    public void WriteFileClose()
    {
      mWriter.Close();
      mWriter.Dispose();
    }

    /// <summary>
    /// Open and read in an entire XML text file as a string
    /// </summary>
    /// <param name="FileName">The path and file name to open</param>
    public string FileRead(string FileName)
    {
      //string Line;
      int Index = 0;
      mFileName = FileName;
      mFileContents = "";
      if (culture == null)
      {
        culture = CultureInfo.InvariantCulture;
        //culture = CultureInfo.CreateSpecificCulture("en-US");
      }

      try
      {
        char[] Buffer = new char[4096];
        StringBuilder SB = new StringBuilder(4096);
        using (StreamReader Reader = new StreamReader(FileName))
        {
          while (Reader.EndOfStream == false)
          {
            Index = Reader.Read(Buffer, 0, 4096);
            //Line = Line.Trim();
            SB.Append(Buffer, 0, Index);
          }
        }
        mFileContents = SB.ToString();
      }
      catch
      {
      }
      return mFileContents;
    }




    public static char[] DelimiterComma = new char[] { ',' };
    public static char[] DelimiterSpace = new char[] { ' ' };
    public static char[] DelimiterColon = new char[] { ':' };
    public static char[] DelimiterForwardSlash = new char[] { '/' };

    //public string ReadTag(string TagName)
    //{
    //    return GetXMLChunk(ref mFileContents, TagName);
    //}
    public static TimeSpan GetXMLChunkAsTimeSpan(ref string XML, string Tag)
    {
      TimeSpan Time = TimeSpan.Zero;
      string Temp = GetXMLChunk(ref XML, Tag);
      TimeSpan.TryParse(Temp, culture, out Time);
      return Time;
    }

    public static DateTime GetXMLChunkAsDateTime(ref string XML, string Tag)
    {
      try
      {
        string Temp = GetXMLChunk(ref XML, Tag);
        DateTime Time = DateTime.Parse(Temp, culture);
        return Time;
      }
      catch
      {
      }
      return DateTime.MinValue;
    }


    public static Rectangle GetXMLChunkAsRectangle(ref string x, string Tag)
    {
      string Value = Xml.GetXMLChunk(ref x, Tag);
      //{X:0 Y:0 Z:0}
      Rectangle Rect = Rectangle.Empty;
      if (Value.Length >= 26)
      {
        Value = Value.Substring(1, Value.Length - 2);
        string[] Chunks = Value.Split(DelimiterSpace);
        string[] Xstr = Chunks[0].Split(DelimiterColon);
        string[] Ystr = Chunks[1].Split(DelimiterColon);
        string[] Zstr = Chunks[2].Split(DelimiterColon);
        string[] Wstr = Chunks[3].Split(DelimiterColon);

        Rect.X = int.Parse(Xstr[1], culture);
        Rect.Y = int.Parse(Ystr[1], culture);
        Rect.Width = int.Parse(Zstr[1], culture);
        Rect.Height = int.Parse(Wstr[1], culture);
      }
      return Rect;
    }

    public static int GetXMLChunkAsInt(ref string XML, string Tag, ref int StartPos)
    {
      string Temp = GetXMLChunk(ref XML, Tag, ref StartPos);
      int Value = 0;
      if (Temp.Length > 0)
      {
        Value = int.Parse(Temp, culture);
      }
      return Value;
    }

    public static int GetXMLChunkAsInt(ref string XML, string Tag)
    {
      return GetXMLChunkAsInt(ref XML, Tag, 0);
    }
    public static int GetXMLChunkAsInt(ref string XML, string Tag, int defaultValue)
    {
      string Temp = GetXMLChunk(ref XML, Tag);
      int Value = defaultValue;
      if (Temp.Length > 0)
      {
        try
        {
          Value = int.Parse(Temp, culture);
        }
        catch
        {
        }
      }
      return Value;
    }

    public static long GetXMLChunkAsLong(ref string XML, string Tag)
    {
      string Temp = GetXMLChunk(ref XML, Tag);
      long Value = 0;
      if (Temp.Length > 0)
      {
        try
        {
          Value = long.Parse(Temp, culture);
        }
        catch
        {
        }
      }
      return Value;
    }

    public static Vector2 GetXMLChunkAsVector2(ref string XML, string Tag, ref int StartPos)
    {
      string Value = Xml.GetXMLChunk(ref XML, Tag, ref StartPos);
      //{X:0 Y:0}
      Vector2 NewVector = ConvertStringToVector2(Value);
      return NewVector;
    }

    public static Vector2 GetXMLChunkAsVector2(ref string XML, string Tag)
    {
      string Value = Xml.GetXMLChunk(ref XML, Tag);
      //{X:0 Y:0}
      Vector2 NewVector = ConvertStringToVector2(Value);
      return NewVector;
    }

    public static Vector2 ConvertStringToVector2(string Value)
    {
      Vector2 NewVector = Vector2.Zero;
      if (Value.Length >= 3)
      {
        //Value = Value.Substring(1, Value.Length - 2);
        string[] Chunks = Value.Split(DelimiterComma);
        //string[] Xstr = Chunks[0].Split(DelimiterColon);
        //string[] Ystr = Chunks[1].Split(DelimiterColon);
        if (Chunks.Length == 2)
        {
          NewVector.X = float.Parse(Chunks[0], culture);
          NewVector.Y = float.Parse(Chunks[1], culture);
        }
      }
      return NewVector;
    }

    public static Size GetXMLChunkAsSize(ref string XML, string Tag)
    {
      string Value = Xml.GetXMLChunk(ref XML, Tag);
      //0,0
      Size s = ConvertStringToSize(Value);
      return s;
    }
    public static Size ConvertStringToSize(string Value)
    {
      Size s = new Size(0, 0);
      if (Value.Length >= 3)
      {
        string[] Chunks = Value.Split(DelimiterComma);
        if (Chunks.Length == 2)
        {
          s.Width = int.Parse(Chunks[0], culture);
          s.Height = int.Parse(Chunks[1], culture);
        }
      }
      return s;
    }

    public static Vector3 GetXMLChunkAsVector3(ref string XML, string Tag)
    {
      string Value = Xml.GetXMLChunk(ref XML, Tag);
      //{X:0 Y:0 Z:0}
      Vector3 NewVector = Vector3.Zero;
      if (Value.Length >= 13)
      {
        Value = Value.Substring(1, Value.Length - 2);
        string[] Chunks = Value.Split(DelimiterSpace);
        string[] Xstr = Chunks[0].Split(DelimiterColon);
        string[] Ystr = Chunks[1].Split(DelimiterColon);
        string[] Zstr = Chunks[2].Split(DelimiterColon);

        NewVector.X = float.Parse(Xstr[1], culture);
        NewVector.Y = float.Parse(Ystr[1], culture);
        NewVector.Z = float.Parse(Zstr[1], culture);
      }
      return NewVector;
    }

    public static Color GetXMLChunkAsColor(ref string XML, string Tag, ref int StartPos)
    {
      string Value = Xml.GetXMLChunk(ref XML, Tag, ref StartPos);
      //{R:0 G:0 B:0 A:0}
      Color NewColor = Color.White;
      if (Value.Length >= 17)
      {
        Value = Value.Substring(1, Value.Length - 2);
        string[] Chunks = Value.Split(DelimiterSpace);
        string[] Rstr = Chunks[0].Split(DelimiterColon);
        string[] Gstr = Chunks[1].Split(DelimiterColon);
        string[] Bstr = Chunks[2].Split(DelimiterColon);
        string[] Astr = Chunks[3].Split(DelimiterColon);
       
        byte R = byte.Parse(Rstr[1], culture);
        byte G = byte.Parse(Gstr[1], culture);
        byte B = byte.Parse(Bstr[1], culture);
        byte A = byte.Parse(Astr[1], culture);

        NewColor = Color.FromArgb(A, R, G, B);
      }
      return NewColor;
    }

    public static Color GetXMLChunkAsColor(ref string XML, string Tag)
    {
      string Value = Xml.GetXMLChunk(ref XML, Tag);
      Color NewColor = ConvertStringToColor(Value);
      return NewColor;
    }

    public static Color ConvertStringToColor(string Value)
    {
      Color NewColor = Color.White;

      if (Value.Length >= 17) //{R:0 G:0 B:0 A:0}
      {
        Value = Value.Substring(1, Value.Length - 2);
        string[] Chunks = Value.Split(DelimiterSpace);
        string[] Rstr = Chunks[0].Split(DelimiterColon);
        string[] Gstr = Chunks[1].Split(DelimiterColon);
        string[] Bstr = Chunks[2].Split(DelimiterColon);
        string[] Astr = Chunks[3].Split(DelimiterColon);


        byte R = byte.Parse(Rstr[1], culture);
        byte G = byte.Parse(Gstr[1], culture);
        byte B = byte.Parse(Bstr[1], culture);
        byte A = byte.Parse(Astr[1], culture);

        NewColor = Color.FromArgb(A, R, G, B);
      }
      return NewColor;
    }


    public static Quaternion GetXMLChunkAsQuaternion(ref string XML, string Tag)
    {
      string Value = Xml.GetXMLChunk(ref XML, Tag);
      //{X:0 Y:0 Z:0 W:1}
      Quaternion NewQ = Quaternion.Identity;
      if (Value.Length >= 17)
      {
        Value = Value.Substring(1, Value.Length - 2);
        string[] Chunks = Value.Split(DelimiterSpace);
        string[] Xstr = Chunks[0].Split(DelimiterColon);
        string[] Ystr = Chunks[1].Split(DelimiterColon);
        string[] Zstr = Chunks[2].Split(DelimiterColon);
        string[] Wstr = Chunks[3].Split(DelimiterColon);

        NewQ.X = float.Parse(Xstr[1], culture);
        NewQ.Y = float.Parse(Ystr[1], culture);
        NewQ.Z = float.Parse(Zstr[1], culture);
        NewQ.W = float.Parse(Wstr[1], culture);
      }
      return NewQ;
    }

    public static float GetXMLChunkAsFloat(ref string XML, string Tag, ref int StartPos)
    {
      string Temp = GetXMLChunk(ref XML, Tag, ref StartPos);
      float Value = 0;
      if (Temp.Length > 0)
      {
        Value = float.Parse(Temp, culture);
      }
      return Value;
    }

    public static float GetXMLChunkAsFloat(ref string XML, string Tag)
    {
      string Temp = GetXMLChunk(ref XML, Tag);
      float Value = 0;
      if (Temp.Length > 0)
      {
        try
        {
          Value = float.Parse(Temp, culture);
        }
        catch
        {
        }
      }
      return Value;
    }

    public static decimal GetXMLChunkAsDecimal(ref string XML, string Tag)
    {
      string Temp = GetXMLChunk(ref XML, Tag);
      decimal Value = 0;
      if (Temp.Length > 0)
      {
        try
        {
          Value = decimal.Parse(Temp, culture);
        }
        catch
        {
        }
      }
      return Value;
    }

    /// <summary>
    /// Parse the referenced XML string, searching for the Tag.
    /// </summary>
    /// <param name="XML">the actual XML data to search</param>
    /// <param name="Tag">XML Element name to search for in the XML parameter</param>
    /// <param name="Value">The actual value of the XML tag element</param>
    /// <returns>Returns true if the element tag name was found in the XML string</returns>
    public static bool GetXMLChunkAsBool(ref string XML, string Tag, out bool Value, bool DefaultValue)
    {
      string Temp = GetXMLChunk(ref XML, Tag);
      bool FieldFound = false;
      Value = DefaultValue;
      if (Temp.Length > 0)
      {
        try
        {
          Value = bool.Parse(Temp);
          FieldFound = true;
        }
        catch
        {
        }
      }
      return FieldFound;
    }

    public static bool GetXMLChunkAsBool(ref string XML, string Tag, out bool Value)
    {
      Value = false;
      GetXMLChunkAsBool(ref XML, Tag, out Value, false);
      return Value;
    }

    public static bool GetXMLChunkAsBool(ref string XML, string Tag)
    {
      bool Value;
      GetXMLChunkAsBool(ref XML, Tag, out Value, false);
      return Value;
    }

    public static bool GetXMLChunkAsBool(ref string XML, string Tag, bool DefaultValue)
    {
      bool Value;
      GetXMLChunkAsBool(ref XML, Tag, out Value, DefaultValue);
      return Value;
    }




    public static string GetXMLChunk(ref string XML, string Tag, ref int StartPos)
    {
      string ReturnValue = "";

      if (StartPos >= XML.Length)
        return "";

      StartPos = (int)XML.IndexOf("<" + Tag + ">", StartPos, StringComparison.OrdinalIgnoreCase);

      int EndPos = 0;

      if (StartPos < 0) //We didn't find the tag, lets jump out
      {
        return "";
      }
      EndPos = (int)XML.IndexOf("</" + Tag + ">", StartPos, StringComparison.OrdinalIgnoreCase);

      if ((StartPos > -1) && (EndPos > -1))
      {
        StartPos = StartPos + (int)Tag.Length + 2;
        int Length = EndPos - StartPos;
        ReturnValue = XML.SubStr(StartPos, Length);
        //string StartCharacters = SubStr(ref XML, 0, StartPos - Tag.Length - 2);
        //string EndCharacters = SubStr(ref XML, EndPos + Tag.Length + 3, XML.Length - EndPos - Tag.Length - 3);
        //XML = StartCharacters + EndCharacters;
        StartPos = EndPos;
      }
      return ReturnValue;
    }

    public static string GetXMLChunkBeforeTag(ref string XML, string Tag)
    {
      string ReturnValue = "";
      int StartPos = (int)XML.IndexOf("<" + Tag + ">", StringComparison.OrdinalIgnoreCase);
      if (StartPos < 0)
        return "";
      ReturnValue = XML.Substring(0, StartPos - 1);
      XML = XML.Substring(StartPos);
      return ReturnValue;
    }

    /// <summary>
    /// Retrieve an XML value that can be Base 64 encoded (HTML or XML stored in XML, or similar)
    /// </summary>
    /// <param name="XML"></param>
    /// <param name="Tag"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string GetXMLChunk(ref string XML, string Tag, BASE_64_ENCODE encoding)
    {
      string val = GetXMLChunk(ref XML, Tag);
      byte[] data = System.Convert.FromBase64String(val);
      return System.Text.Encoding.UTF8.GetString(data);
    }


    public static string GetXMLChunk(ref string XML, string Tag, string? ParentTag = null)
    {
      string ReturnValue = "";

      int StartPos = (int)XML.IndexOf("<" + Tag + ">", StringComparison.OrdinalIgnoreCase);
      int EndPos = 0;

      if (StartPos < 0) //We didn't find the tag, lets jump out
      {
        return "";
      }
      //EndPos = (int)XML.IndexOf("</" + Tag + ">", StartPos, StringComparison.OrdinalIgnoreCase);
      EndPos = FindCorrectEndTagIndex(ref XML, StartPos + 1, Tag);

      if (ParentTag is not null)
      {
        int ParentTagPos = XML.IndexOf("<" + ParentTag + ">", StringComparison.OrdinalIgnoreCase);
        if (ParentTagPos != -1 && ParentTagPos < StartPos) //We have found a recursive tag, lets exit
          return "";
      }

      if ((StartPos > -1) && (EndPos > -1))
      {
        StartPos = StartPos + (int)Tag.Length + 2;
        int Length = EndPos - StartPos;
        ReturnValue = XML.SubStr(StartPos, Length);
        string StartCharacters = XML.SubStr(0, StartPos - Tag.Length - 2);
        string EndCharacters = XML.SubStr(EndPos + Tag.Length + 3, XML.Length - EndPos - Tag.Length - 3);
        XML = StartCharacters + EndCharacters;
      }
      return ReturnValue;
    }

    public static int FindCorrectEndTagIndex(ref string XML, int StartPos, string Tag)
    {
      string startTag = "<" + Tag + ">";
      string endTag = "</" + Tag + ">";

      int count = 0;
      int start = XML.IndexOf(startTag, StartPos, StringComparison.OrdinalIgnoreCase);
      int end = XML.IndexOf(endTag, StartPos, StringComparison.OrdinalIgnoreCase);
      if (start == -1 || start > end)
        return end;
      while (start < end)
      {
        count++;
        StartPos = start + 1;
        start = XML.IndexOf(startTag, StartPos, StringComparison.OrdinalIgnoreCase);
      }
      end = start;
      while (count > 0)
      {
        count--;
        StartPos = end + 1;
        start = XML.IndexOf(startTag, StartPos, StringComparison.OrdinalIgnoreCase);
        end = XML.IndexOf(endTag, StartPos, StringComparison.OrdinalIgnoreCase);
        if (start < end)
          count++;
      }
      return end;
    }

  }
}
