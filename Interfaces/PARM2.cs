using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class PARM2
  {

    public enum PARM_REQUIRED
    {
      Yes,
      No,
    }

    public enum PARM_ALLOW_MULTIPLE
    {
      Single,
      Multiple,
    }


    public string Name = "";
    public string DefaultValue = "";
    public DATA_TYPE DataType = DATA_TYPE.String;
    public PARM_REQUIRED Required = PARM_REQUIRED.Yes;
    public PARM_ALLOW_MULTIPLE AllowMultiple = PARM_ALLOW_MULTIPLE.Single; //If Multiple, this parameter can be duplicated to allow multiple values to be passed in.
    public List<string>? Options = null; //For drop down list


    public PARM2(string parmName, DATA_TYPE dataType, string defaultValue)
    {
      Name = parmName;
      DataType = dataType;
      Required = PARM_REQUIRED.Yes;
      DefaultValue = defaultValue;
    }
    public PARM2(string parmName, DATA_TYPE dataType, string defaultValue, PARM_REQUIRED required = PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE multiple = PARM_ALLOW_MULTIPLE.Single)
    {
      Name = parmName;
      DataType = dataType;
      Required = PARM_REQUIRED.Yes;
      DefaultValue = defaultValue;
    }
    public PARM2(string parmName, DATA_TYPE dataType, PARM_REQUIRED required = PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE multiple = PARM_ALLOW_MULTIPLE.Single)
    {
      Name = parmName;
      DataType = dataType;
      Required = required;
      AllowMultiple = multiple;
    }


    public virtual PARM2 Clone()
    {
      PARM2 parm = new PARM2(Name, DataType);
      parm.Options = Options;
      parm.DefaultValue = DefaultValue;
      parm.Required = Required;
      parm.AllowMultiple = AllowMultiple;
      return parm;
    }

    public void OptionAdd(string option)
    {
      if (DataType != DATA_TYPE.DropDownList)
        throw new Exception(String.Format("DataType [{0}] is not 'DropDownList'", DataType.ToString()));
      if (Options is null)
        Options = new List<string>();
      Options.Add(option);
    }


   

  }

}
