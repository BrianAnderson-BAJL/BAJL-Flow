using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.PARM;
using System.Xml.Linq;

namespace Core
{
  public class PARM_DropDownList : PARM
  {
    public string DefaultValue = "";
    public string Value = "";
    public List<string> Options = new List<string>();

    public PARM_DropDownList(string name, PARM_REQUIRED required = PARM_REQUIRED.Yes, string defaultValue = "")
    {
      Name = name;
      DataType = DATA_TYPE.DropDownList;
      DefaultValue = defaultValue;
      Value = defaultValue;
      Required = required;
    }

    public override PARM Clone()
    {
      PARM_DropDownList p = new PARM_DropDownList(this.Name);
      p.DataType = this.DataType;
      p.Required = this.Required;
      p.DefaultValue = this.DefaultValue;
      p.Value = this.Value;
      p.Options = this.Options;
      p.AllowMultiple = this.AllowMultiple;
      p.ParmLiteral = this.ParmLiteral;
      return p;
    }

    public void OptionAdd(string val)
    {
      Options.Add(val);
    }

    public override string ToString()
    {
      return Value.ToString();
    }

    public override void SetValue(object value)
    {
      Value = (string)value;
    }

    public override object GetValue()
    {
      return this.Value;
    }
  }
}
