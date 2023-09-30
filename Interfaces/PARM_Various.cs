using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class PARM_Various : PARM
  {
    public long Max = int.MaxValue;
    public long Min = 0;
    public string DefaultValue = "";
    public string Value = "";

    public PARM_Various(string name, DATA_TYPE dataTyoe, PARM_REQUIRED required = PARM_REQUIRED.Yes)
    {
      Name = name;
      DataType = dataTyoe; //It seems kind of weird that a string paramater woulnd't have a data type of string, but for objects the Value just contains a string link to the variable name, hence a string
      DefaultValue = "";
      Value = "";
      Required = required;

    }

    public PARM_Various(string name, PARM_REQUIRED required = PARM_REQUIRED.Yes, string defaultValue = "", PARM_ALLOW_MULTIPLE allowMultiple = PARM_ALLOW_MULTIPLE.Single)
    {
      Name = name;
      DataType = DATA_TYPE.String;
      DefaultValue = defaultValue;
      Value = defaultValue;
      Required = required;
      AllowMultiple = allowMultiple;
    }

    public override PARM Clone()
    {
      PARM_Various p = new PARM_Various(this.Name);
      p.DataType = this.DataType;
      p.Required = this.Required;
      p.Max = this.Max;
      p.Min = this.Min;
      p.DefaultValue = this.DefaultValue;
      p.Value = this.Value;
      p.AllowMultiple = this.AllowMultiple;
      p.ParmLiteral = this.ParmLiteral;
      return p;
    }

    public override string ToString()
    {
      return Value.ToString();
    }

    public override void SetValue(object value)
    {
      Value = (string)value;
    }

  }
}
