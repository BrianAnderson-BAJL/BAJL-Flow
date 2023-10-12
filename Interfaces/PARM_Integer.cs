using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class PARM_Integer : PARM
  {
    public long Max = long.MaxValue;
    public long Min = long.MinValue;
    public long Increment = 1;
    public long DefaultValue = 0;
    public long Value = 0;

    public PARM_Integer(string name, PARM_REQUIRED required = PARM_REQUIRED.Yes, long defaultValue = 0)
    {
      this.Name = name;
      DataType = DATA_TYPE.Integer;
      DefaultValue = defaultValue;
      Value = defaultValue;
      Required = required;

    }

    public override PARM Clone()
    {
      PARM_Integer p = new PARM_Integer(this.Name);
      p.DataType = this.DataType;
      p.Required = this.Required;
      p.Max = this.Max;
      p.Min = this.Min;
      p.Increment = this.Increment;
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
      Value = (long)value;
    }

    //public virtual Variable Resolve()
    //{
    //  if (this.ParmLiteral == PARM_L_OR_V.Literal)
    //  {
    //    return new VariableInteger(this.Value);
    //  }
    //  else
    //  {
    //  }
    //}

  }
}
