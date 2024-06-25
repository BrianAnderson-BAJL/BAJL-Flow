using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static FlowEngineCore.Parsers.SqlParser.ParsedUnit;

namespace FlowEngineCore.Parsers
{
  public class SqlParser
  {
    public class ParsedUnit
    {
      public enum UNIT_TYPE
      {
        _Unknown,
        Keyword,
        Astrisk,
        Function,
        FieldOrResult,
        DatabaseStructure,   //Field or table name or such
        Delimter, //White space, or comma
        Parentheses,
        Comparison,
        Parameter,
      }

      /// <summary>
      /// The actual word or delimiter that is part of the SQL. ('SELECT', 'WHERE', ...)
      /// </summary>
      public string Value = "";

      /// <summary>
      /// If this is a FieldOrResult UNIT_TYPE it could be renamed to this if the previous keyword was 'AS'
      /// </summary>
      public string Name = "";

      /// <summary>
      /// What kind of unit is this (Keyword, Function, Delimiter, ...)
      /// </summary>
      public UNIT_TYPE UnitType;
      
      /// <summary>
      /// What color should the text be shown as, this holds the actual RTF color value linked in the color look up table in the header of the RTF doc.
      /// </summary>
      public string Color = "";

      /// <summary>
      /// If values are encapsulated by parentheses the units will go in here
      /// </summary>
      public List<ParsedUnit> SubUnits = new List<ParsedUnit>();

      public ParsedUnit(string value, bool beforeFrom)
      {
        this.Value = value;
        DiscoverUnitType(beforeFrom);
      }

      /// <summary>
      /// Get the RTF text to put into the RTF control
      /// </summary>
      /// <returns></returns>
      public string GetRtfText()
      {
        if (Value == "")
          return "";

        if (Value == "\n")
          return "\\par ";

        string val = Color + Value;

        return val;
      }

      /// <summary>
      /// Can use this to find how many parameters are in the SQL, this can be used in the error checking of the step/flow.
      /// </summary>
      /// <param name="unitType"></param>
      /// <returns></returns>
      public int GetCountOf(UNIT_TYPE unitType)
      {
        int count = 0;
        if (UnitType == UNIT_TYPE.Keyword)
          count++;

        for (int x = 0; x < SubUnits.Count; x++)
        {
          if (SubUnits[x].UnitType == unitType)
            count++;
        }
        return count;
      }

      /// <summary>
      /// This does not currently support sub queries
      /// </summary>
      /// <param name="unitType"></param>
      /// <returns></returns>
      public List<string> GetListOf(UNIT_TYPE unitType)
      {
        List<string> list = new List<string>();

        if (unitType == UNIT_TYPE.FieldOrResult)
        {
          bool isAs = false;
          for (int x = 0; x < SubUnits.Count; x++)
          {
            if (SubUnits[x].Value.Equals("AS", StringComparison.InvariantCultureIgnoreCase) == true)
              isAs = true;
            if (isAs == false)
              break;
            //Skip over delimiters
            if (SubUnits[x].UnitType == unitType)
            {
              list.Add(SubUnits[x].Value);
              break;
            }
          }
        }

        if (list.Count == 0) //If there wasn't an AS in the SubUnits
        {
          if (UnitType == unitType)
            list.Add(Value);
        }
        return list;
      }

      public ParsedUnit? GetLastOf(UNIT_TYPE unitType)
      {
        for (int x = SubUnits.Count - 1; x >= 0; x--)
        {
          if (SubUnits[x].UnitType == unitType)
            return SubUnits[x];
        }
        return null;
      }
      

      /// <summary>
      /// Set which kind of unit this word of text is and set the color for this unit
      /// </summary>
      private void DiscoverUnitType(bool beforeFrom)
      {
        if (this.Value == "*")
        {
          UnitType = UNIT_TYPE.Astrisk;
          Color = DEFAULT_COLOR;
          return;
        }
        if (this.Value == "(" || this.Value == ")")
        {
          UnitType = UNIT_TYPE.Parentheses;
          Color = PARENTHESES_COLOR;
          return;
        }
        if (this.Value.StartsWith('@') == true)
        {
          UnitType = UNIT_TYPE.Parameter;
          Color = PARAM_COLOR;
          return;
        }
        for (int x = 0; x < Keywords.Length; x++)
        {
          if (String.Equals(Keywords[x], this.Value, StringComparison.OrdinalIgnoreCase) == true)
          {
            UnitType = UNIT_TYPE.Keyword;
            Color = KEYWORD_COLOR;
            return;
          }
        }
        for (int x = 0; x < Functions.Length; x++)
        {
          if (String.Equals(Functions[x], this.Value, StringComparison.OrdinalIgnoreCase) == true)
          {
            UnitType = UNIT_TYPE.Function;
            Color = FUNCTION_COLOR;
            return;
          }
        }
        for (int x = 0; x < Delimiters.Length; x++)
        {
          if (String.Equals(Delimiters[x], this.Value, StringComparison.OrdinalIgnoreCase) == true)
          {
            UnitType = UNIT_TYPE.Delimter;
            Color = DELIMTER_COLOR;
            return;
          }
        }
        for (int x = 0; x < Comparer.Length; x++)
        {
          if (String.Equals(Comparer[x], this.Value, StringComparison.OrdinalIgnoreCase) == true)
          {
            UnitType = UNIT_TYPE.Comparison;
            Color = COMPARER_COLOR;
            return;
          }
        }
        if (beforeFrom == true)
          UnitType = UNIT_TYPE.FieldOrResult;
        else
          UnitType = UNIT_TYPE.DatabaseStructure;

        Color = DEFAULT_COLOR;
      }
    }

    private List<ParsedUnit> Units = new List<ParsedUnit>(8);

    //TODO: These keywords and functions are for MySql, later I will need to pull these values from the Database plugin using the IDatabase interface. That way I can get different keywords based on the database type (MySQl, SQL Server, Oracle, ...)
    private static readonly string[] Keywords = { "SELECT", "UPDATE", "INSERT", "DELETE", "FROM", "WHERE", "HAVING", "VALUES", "IN", "BETWEEN", "INNER", "LEFT", "RIGHT", "JOIN", "ON", "INTO", "GROUP", "ORDER", "BY", "AS", "WITH", "AND", "OR", "DESC", "ASC" };
    private static readonly string[] Functions = { "COUNT", "MAX", "MIN", "AVG", "SUM", "MONTHNAME", "MONTH", "DAY", "YEAR", "CURRENT_TIMESTAMP" };
    private static readonly string[] Delimiters = { ",", " ", "(", ")", "\r\n", "\n", "\t", "=", "<=", ">=", "<", ">", ";"};
    private static readonly string[] Comparer = { "=", "<=", ">=", "<", ">" };

    private static Color KeywordColor = Color.Blue;
    private static Color FunctionColor = Color.DarkGreen;
    private static Color DelimiterColor = Color.DarkGray;
    private static Color ComparerColor = Color.DarkGray;
    private static Color DefaultColor = Color.Black;
    private static Color ParenthesesColor = Color.Red;
    private static Color ParamColor = Color.Teal;

    const string DEFAULT_COLOR = "\\cf0 ";
    const string KEYWORD_COLOR = "\\cf1 ";
    const string FUNCTION_COLOR = "\\cf2 ";
    const string DELIMTER_COLOR = "\\cf3 ";
    const string COMPARER_COLOR = "\\cf4 ";
    const string PARENTHESES_COLOR = "\\cf5 ";
    const string PARAM_COLOR = "\\cf6 ";

    private bool BeforeFrom = true;
    private ParsedUnit? PreviousParsedUnit = null;
    /// <summary>
    /// Find the next unit of the SQL
    /// </summary>
    /// <param name="sql">The SQL text</param>
    /// <param name="parent">If the sql is a parentheses then the parent will be the actual parentheses so sub units can be added</param>
    private void ReadNextUnit(ref string sql, ParsedUnit? parent = null, bool isAs = false)
    {
      if (sql.Length == 0)
        return;

      ParsedUnit? unit = null;
      (int index, int delimiterId) = FindIndexOfDelimter(ref sql);
      if (index == -1)
        return;

      string word = sql.Substring(0, index);
      if (sql.Length >= index + 1)
        sql = sql.Substring(index + 1);
      else
        sql = "";

      if (word.Length == 0 && delimiterId < 0)
      {
        ReadNextUnit(ref sql, parent, isAs);
        return;
      }
      
      unit = new ParsedUnit(word, BeforeFrom);

      if (parent is not null)
      {
        if (unit.Value != "")
          parent.SubUnits.Add(unit);
        if (delimiterId >= 0)
        {
          unit = new ParsedUnit(Delimiters[delimiterId], BeforeFrom);
          parent.SubUnits.Add(unit);
          if (isAs == true)
            ReadNextUnit(ref sql, parent, isAs);
        }
      }
      else// if (word.Equals("AS", StringComparison.OrdinalIgnoreCase) == false)
      {
        if (unit.Value != "")
          Units.Add(unit);
        if (delimiterId >= 0)
        {
          unit = new ParsedUnit(Delimiters[delimiterId], BeforeFrom);
          Units.Add(unit);
        }
      }
      //else if (word.Length > 0) //word == AS
      //{
      //  if (PreviousParsedUnit is not null) //Someone might of typed 'AS' as the first thing for some reason
      //  {
      //    PreviousParsedUnit.SubUnits.Add(unit);
      //    ReadNextUnit(ref sql, PreviousParsedUnit, true);
      //  }
      //}

      if (word == "(")
      {
        int endIndex = FindEndingParentheses(ref sql);
        if (endIndex > -1)
        {
          sql = sql.Substring(0, endIndex);
          ReadNextUnit(ref sql, unit);
        }
      }

      if (word.Equals("FROM", StringComparison.OrdinalIgnoreCase) == true)
      {
        BeforeFrom = false;
      }
      PreviousParsedUnit = unit;

      ReadNextUnit(ref sql); //Let's get recursive to parse the entire SQL statement
      
      
    }

    /// <summary>
    /// Find the ending of the parentheses skipping over any sub parentheses
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    private int FindEndingParentheses(ref string sql)
    {
      int openParen = sql.IndexOf('(');
      int closeParen = sql.IndexOf(")");

      if (openParen > -1 && openParen < closeParen)
      {
        do
        {
          openParen = sql.IndexOf("(", openParen + 1);
          if (openParen == -1)
            break;
          closeParen = sql.IndexOf(")", closeParen + 1); //Find them in pairs, need to keep going til we find the real end
        } while (openParen < closeParen);
      }
      return closeParen;
    }

    /// <summary>
    /// Find the index of the next delimiter in the SQL
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    private (int, int) FindIndexOfDelimter(ref string sql)
    {
      int minIndex = int.MaxValue;
      int delIndex = -1;
      for (int x = 0; x < Delimiters.Length; x++)
      {
        int index = sql.IndexOf(Delimiters[x]);
        if (index > -1 && index < minIndex)
        {
          minIndex = index;
          delIndex = x;
        }
      }
      if (minIndex < int.MaxValue)
      {
        return (minIndex, delIndex);
      }
      else
      {
        return (sql.Length, -1);
      }
    }

    /// <summary>
    /// The public interface for the actual parser
    /// </summary>
    /// <param name="sql"></param>
    /// <returns>A list of all units discovered in the SQL</returns>
    public List<ParsedUnit> ParseSql(string sql)
    {
      ReadNextUnit(ref sql);
      return Units;
    }

    /// <summary>
    /// Build the RTF text for the RTF control
    /// </summary>
    /// <returns>the RTF text that will be added to the RTF control</returns>
    public string GetRtfText()
    {
      StringBuilder sb = new StringBuilder(4096);

      //This is text I pulled from Microsoft WordPad basic RTF document 
      sb.AppendLine(@"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\fnil Courier New;}}"); //\fcharset0
      sb.AppendLine(@"{\colortbl ;\red" + KeywordColor.R + @"\green" + KeywordColor.G + @"\blue" + KeywordColor.B + @";\red" + FunctionColor.R + @"\green" + FunctionColor.G + @"\blue" + FunctionColor.B + @";\red" + DelimiterColor.R + @"\green" + DelimiterColor.G + @"\blue" + DelimiterColor.B + @";\red" + ComparerColor.R + @"\green" + ComparerColor.G + @"\blue" + ComparerColor.B + @";\red" + ParenthesesColor.R + @"\green" + ParenthesesColor.G + @"\blue" + ParenthesesColor.B + @";\red" + ParamColor.R + @"\green" + ParamColor.G + @"\blue" + ParamColor.B + ";}");
      sb.AppendLine(@"{\*\generator Riched20 10.0.19041}\viewkind4\uc1 ");
      sb.AppendLine(@"\pard\f0\fs24 ");
      //sb.AppendLine(@"\pard\sa200\sl276\slmult1\f0\fs22\lang9 "); //This line seems to control the line spacing, I didn't like it....
      
      for (int x = 0; x < Units.Count; x++)
      {
        if (Units[x].Value == "\n" && Units.Count - 1 == x) //Shitty RTF seems to need two \par for a blank line, something weird Microsoft did with RTF formats
        {
          sb.Append("\\par\\par");
        }
        else
        {
          sb.Append(Units[x].GetRtfText());
        }
      }
      sb.AppendLine();
      sb.AppendLine(@"}");
      //string val = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang14346{\fonttbl{\f0\fnil\fcharset0 Calibri;}} {\*\generator Riched20 10.0.10586}\viewkind4\uc1 \pard\sa200\sl276\slmult1\f0\fs22\lang10 Hello World.\par }";
      return sb.ToString();
    }


    /// <summary>
    /// Can use this to find how many parameters are in the SQL, this can be used in the error checking of the step/flow.
    /// </summary>
    /// <param name="unitType">The type of unit you want a count of (Keyword, Function, Parameters, ...)</param>
    /// <returns>The count of units found</returns>
    public int GetCountOf(ParsedUnit.UNIT_TYPE unitType)
    {
      int count = 0;
      for (int x = 0; x < Units.Count; x++)
      {
        count += Units[x].GetCountOf(unitType);
      }
      return count;
    }

    public List<string> GetListOf(ParsedUnit.UNIT_TYPE unitType)
    {
      List<string> list = new List<string>(32);
      for (int x = 0; x < Units.Count; x++)
      {
        list.AddRange(Units[x].GetListOf(unitType));
      }
      return list;
    }
  }
}
