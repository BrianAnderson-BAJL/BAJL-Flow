using FlowEngineCore;
using FlowEngineCore.Interfaces;
using System.Drawing;

namespace Validation
{
  public class Validation : FlowEngineCore.Plugin
  {

    private const int ERROR_INVALID_EMAIL = (int)STEP_ERROR_NUMBERS.ValidationErrorMin;
    private const int ERROR_INVALID_PHONE = (int)STEP_ERROR_NUMBERS.ValidationErrorMin + 1;
    private const int ERROR_VARIABLE_NO_VALUE = (int)STEP_ERROR_NUMBERS.ValidationErrorMin + 2;

    public override void Init()
    {
      base.Init();

      Function function = new("Email", this, Email, "", "");
      function.Parms.Add("Email", DATA_TYPE.String);
      PARM parm = new("Format", STRING_SUB_TYPE.DropDownList);
      parm.OptionAdd("a@a");
      parm.OptionAdd("a@a.a");
      function.Parms.Add(parm);
      Functions.Add(function);

      function = new Function("Phone", this, Phone);
      //function.Description
      function.Parms.Add("Phone Number", DATA_TYPE.String);
      parm = new PARM("Format", STRING_SUB_TYPE.DropDownList);
      parm.OptionAdd("###-###-####");
      parm.OptionAdd("+1 ###-###-####");
      function.Parms.Add(parm);
      Functions.Add(function);

      function = new Function("VariableHasValue", this, VariableHasValue);
      function.Parms.Add("Variable", DATA_TYPE.Various, PARM.PARM_REQUIRED.Yes, PARM.PARM_ALLOW_MULTIPLE.Multiple, PARM.PARM_RESOLVE_VARIABLES.No); //Don't resolve the variables so that we can report which variable had the error.
      Functions.Add(function);

      //SETTINGS
      {
        mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.Crimson));
        mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));
      }
      //SETTINGS

    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);

      //Let's grab the log from the log plugin

    }

    public override void StopPlugin()
    {
    }

    public RESP Email(Flow flow, Variable[] vars)
    {
      mLog?.Write("Validation.Email", LOG_TYPE.DBG);

      vars[0].GetValue(out string email);
      vars[1].GetValue(out string format);

      if (email is null)
        return RESP.SetError(ERROR_INVALID_EMAIL, "Email - null");

      if (email.Contains('@') == false)
        return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - no @ symbol [{email}]");

      string[] emailSplit = email.Split('@');
      if (emailSplit.Length != 2)
        return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - wrong number of @ symbols [{email}]");

      if (format == "a@a")
      {
        if (emailSplit[0].Length < 1)
          return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - value before @ symbol is not valid [{email}]");

        if (emailSplit[1].Length < 1)
          return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - value after @ symbol is not valid [{email}]");
      }
      else if (format == "a@a.a")
      {
        if (emailSplit[0].Length < 1)
          return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - value before @ symbol is not valid [{email}]");

        if (emailSplit[1].Length < 3) // must be at least a.a (3 characters)
          return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - value after @ symbol is not valid [{email}]");

        if (emailSplit[1].Contains("..") == true)
          return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - invalid double dot detected '..' [{email}]");

        string[] afterAt = emailSplit[1].Split('.');
        if (afterAt.Length < 2)
          return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - value after @ symbol is not valid, invalid domain name [{email}]");
        for (int x = 0; x < afterAt.Length; x++)
        {
          if (afterAt[x].Length < 1)
            return RESP.SetError(ERROR_INVALID_EMAIL, $"Email - value after @ symbol is not valid [{email}]");
        }
      }

      return RESP.SetSuccess();
    }

    public RESP Phone(Flow flow, Variable[] vars)
    {
      mLog?.Write("Validation.Phone", LOG_TYPE.DBG);
      throw new NotImplementedException();
      //return RESP.SetSuccess();
    }

    public RESP VariableHasValue(Flow flow, Variable[] vars)
    {
      mLog?.Write("Validation.VariableHasValue", LOG_TYPE.DBG);

      for (int x = 0; x < vars.Length; x++)
      {
        Variable? var = flow.FindVariable(vars[x].Value);
        string temp = var.GetValueAsString();
        if (temp.Length == 0)
          return RESP.SetError(ERROR_VARIABLE_NO_VALUE, $"Variable [{vars[x].Name}] is empty [{vars[x].Value}]");
      }

      return RESP.SetSuccess();
    }
  }
}