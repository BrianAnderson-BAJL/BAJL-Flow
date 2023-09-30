using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.PARM;

namespace Core
{
  public class PARM_Decimal : PARM
  {
    public decimal Max = decimal.MaxValue;
    public decimal Min = decimal.MinValue;
    public long Increment = 1;
    public decimal DefaultValue = 0;
    public decimal Value = 0;

    public PARM_Decimal(string name, PARM_REQUIRED required = PARM_REQUIRED.Yes, decimal defaultValue = 0)
    {
      Name = name;
      DataType = DATA_TYPE.Decimal;
      DefaultValue = defaultValue;
      Value = defaultValue;
      Required = required;

    }

    public override PARM Clone()
    {
      PARM_Decimal p = new PARM_Decimal(this.Name);
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
      Value = (decimal)value;
    }

  }
}
