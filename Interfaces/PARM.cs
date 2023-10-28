using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class PARM
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

    public enum PARM_VALIDATION
    {
      //String Validators
      StringMaxLength,
      StringMinLength,
      StringDefaultValue,
      //Integer/Decimal Validators
      NumberMax,
      NumberMin,
      NumberDefaultValue,
      NumberDecimalPlaces,
      NumberIncrement,
    }

    public enum PARM_RESOLVE_VARIABLES
    {
      Yes,
      No,
    }


    public string Name = "";
    public DATA_TYPE DataType = DATA_TYPE.String;
    public PARM_REQUIRED Required = PARM_REQUIRED.Yes;
    public PARM_ALLOW_MULTIPLE AllowMultiple = PARM_ALLOW_MULTIPLE.Single; //If Multiple, this parameter can be duplicated to allow multiple values to be passed in.
    public PARM_RESOLVE_VARIABLES ResolveVariables = PARM_RESOLVE_VARIABLES.Yes;
    public List<string>? Options = null; //For drop down list
    public List<ParmValidator> Validators = new List<ParmValidator>();

    public PARM(string parmName, DATA_TYPE dataType)
    {
      Name = parmName;
      DataType = dataType;
      Required = PARM_REQUIRED.Yes;
    }
   
    public PARM(string parmName, DATA_TYPE dataType, PARM_REQUIRED required = PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE multiple = PARM_ALLOW_MULTIPLE.Single, PARM_RESOLVE_VARIABLES resolveVariables = PARM_RESOLVE_VARIABLES.Yes)
    {
      Name = parmName;
      DataType = dataType;
      Required = required;
      AllowMultiple = multiple;
      ResolveVariables = resolveVariables;
    }


    public void OptionAdd(string option)
    {
      if (DataType != DATA_TYPE.DropDownList)
        throw new Exception(String.Format("DataType [{0}] is not 'DropDownList'", DataType.ToString()));
      if (Options is null)
        Options = new List<string>();
      Options.Add(option);
    }

    public void ValidatorAdd(PARM_VALIDATION validator, decimal val)
    {
      if (validator == PARM_VALIDATION.NumberDecimalPlaces && (val > 30 || val < 0))
        throw new ArgumentException("ValidatorAdd - NumberDecimalPlaces must be between 0-30 (zero to thirty)");
      Validators.Add(new ParmValidator(validator, val));
    }

    public void ValidatorAdd(PARM_VALIDATION validator, string stringDefaultValue)
    {
      Validators.Add(new ParmValidator(stringDefaultValue));
    }

    public void ValidatorGetValue(PARM_VALIDATION validator, out decimal val)
    {
      if (validator == PARM_VALIDATION.StringMaxLength)
        val = int.MaxValue;
      else if (validator == PARM_VALIDATION.NumberMax)
        val = long.MaxValue;
      else if (validator == PARM_VALIDATION.NumberMin)
        val = long.MinValue;
      else if (validator == PARM_VALIDATION.NumberIncrement)
        val = 1;
      else
        val = 0; //NumberDecimalPlaces, NumberDefaultValue both default to zero
      ParmValidator? pv = FindValidator(validator);
      if (pv is not null)
        val = pv.Value;
    }

    public void ValidatorGetValue(PARM_VALIDATION validator, out string val)
    {
      val = "";

      ParmValidator? pv = FindValidator(PARM_VALIDATION.StringDefaultValue);
      if (pv is not null)
        val = pv.StringDefaultValue;
    }

    private ParmValidator? FindValidator(PARM_VALIDATION validator)
    {
      for (int x = 0; x < Validators.Count; x++)
      {
        if (Validators[x].Validator == validator)
          return Validators[x];
      }
      return null;
    }

  }

  public class ParmValidator
  {
    public PARM.PARM_VALIDATION Validator;
    public string StringDefaultValue = "";
    public decimal Value = 0;

    public ParmValidator(PARM.PARM_VALIDATION validator, decimal val)
    {
      this.Validator = validator;
      Value = val;
    }
    public ParmValidator(string defaultValue)
    {
      this.Validator = PARM.PARM_VALIDATION.StringDefaultValue;
      StringDefaultValue = defaultValue;
    }
  }

}
