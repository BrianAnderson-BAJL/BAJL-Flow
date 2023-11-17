using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Parsers.ParserJson;

namespace Core.Parsers
{
    public class ParserXml
    {
        private struct XML_NEXT_TYPE_STRUCT
        {
            public XML_NEXT_TYPE NextType;
            public XML_VALUE_TYPE NextValueType;

            public XML_NEXT_TYPE_STRUCT()
            {
                NextType = XML_NEXT_TYPE.None;
                NextValueType = XML_VALUE_TYPE.Empty;
            }
        }
        public enum XML_NEXT_TYPE
        {
            None,
            Block,
            Value,
        }
        public enum XML_VALUE_TYPE
        {
            Empty,
            String,
            Integer,
            Decimal,
            Boolean,
        }

        private char[] StripOff_Chars = { ' ', '\r', '\n', '\t' };

        private XML_NEXT_TYPE_STRUCT LookForwardFindNextType(string xmlStr)
        {
            XML_NEXT_TYPE_STRUCT nextType = new XML_NEXT_TYPE_STRUCT();

            //Strip off white space
            xmlStr = xmlStr.Trim();

            int startPos = xmlStr.IndexOf('<');
            if (startPos < 0)
                return nextType;
            int endPosForwardSlash = xmlStr.IndexOf('/');
            int endPos = xmlStr.IndexOf('>');
            if (endPosForwardSlash < endPos) //Found a <Element/> tag, empty value
            {
                nextType.NextType = XML_NEXT_TYPE.Value;
                nextType.NextValueType = XML_VALUE_TYPE.Empty;
                return nextType;
            }



            return nextType;
        }

        public void ParseXml(ref string xmlStr, Variable var)
        {
            if (xmlStr.StartsWith("<?") == true)
            {
                int endPos = xmlStr.IndexOf("?>");
                if (endPos >= 0)
                {
                    xmlStr = xmlStr.Substring(endPos + 2); //Trim off the header, I ignore it.
                }


            }
        }
    }
}
