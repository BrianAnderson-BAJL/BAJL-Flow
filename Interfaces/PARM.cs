using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
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


    /// <summary>
    /// The parameter name
    /// </summary>
    public string Name = "";

    /// <summary>
    /// Is the parameter name changeable? Normally the parameter name is fixed, but in certain conditions you can change the name to something else. This is usefull
    /// for the Database plugin where the parameter name will be used in the SQL when passing values in. (SELECT * FROM Table WHERE Name = @UserName) @UserName will be the parameter name
    /// </summary>
    public bool NameChangeable = false;

    /// <summary>
    /// Should the parameter name be incremented when adding multiple parameters, NameChangeable needs to be true, and AllowMultiple needs to be PARM_ALLOW_MULTIPLE.Multiple for this to be used
    /// @Parm0, @Parm1, @Parm2, the 0, 1, 2, ... will be added to each parameter name to make them unique
    /// </summary>
    public bool NameChangeIncrement = false;
    public string Description = "";
    public DATA_TYPE DataType = DATA_TYPE.String;
    public PARM_REQUIRED Required = PARM_REQUIRED.Yes;
    public PARM_ALLOW_MULTIPLE AllowMultiple = PARM_ALLOW_MULTIPLE.Single; //If Multiple, this parameter can be duplicated to allow multiple values to be passed in.
    public PARM_RESOLVE_VARIABLES ResolveVariables = PARM_RESOLVE_VARIABLES.Yes;
    public STRING_SUB_TYPE StringSubType = STRING_SUB_TYPE._None;
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
    public PARM(string parmName, STRING_SUB_TYPE stringSubType, PARM_REQUIRED required = PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE multiple = PARM_ALLOW_MULTIPLE.Single, PARM_RESOLVE_VARIABLES resolveVariables = PARM_RESOLVE_VARIABLES.Yes)
    {
      Name = parmName;
      DataType = DATA_TYPE.String;
      StringSubType = stringSubType;
      Required = required;
      AllowMultiple = multiple;
      ResolveVariables = resolveVariables;
    }

    public PARM_VAR ToParmVar()
    {
      return new PARM_VAR(this);
    }

    public void OptionAdd(string option)
    {
      if (DataType != DATA_TYPE.String && StringSubType != STRING_SUB_TYPE.DropDownList)
        throw new Exception($"DataType [{DataType}] is not a sub type of 'DropDownList'");
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
