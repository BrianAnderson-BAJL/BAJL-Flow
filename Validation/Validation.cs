using Core;
using Core.Interfaces;
using System.Drawing;

namespace Validation
{
  public class Validation : Core.Plugin
  {

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
        SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        SettingAdd(new Setting("", "Designer", "BorderColor", Color.Crimson));
        SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));
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

    public RESP Email(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("Validation.Email", LOG_TYPE.DBG);

      vars[0].GetValue(out string email);
      vars[1].GetValue(out string format);

      if (email is null)
        return RESP.SetError(1, "email is null");

      if (email.Contains('@') == false)
        return RESP.SetError(1, $"email no @ symbol [{email}]");

      string[] emailSplit = email.Split('@');
      if (emailSplit.Length != 2)
        return RESP.SetError(1, $"email wrong number of @ symbols [{email}]");

      if (format == "a@a")
      {
        if (emailSplit[0].Length < 1)
          return RESP.SetError(1, $"email value before @ symbol is not valid [{email}]");

        if (emailSplit[1].Length < 1)
          return RESP.SetError(1, $"email value after @ symbol is not valid [{email}]");
      }
      else if (format == "a@a.a")
      {
        if (emailSplit[0].Length < 1)
          return RESP.SetError(1, $"email value before @ symbol is not valid [{email}]");

        if (emailSplit[1].Length < 3) // must be at least a.a (3 characters)
          return RESP.SetError(1, $"email value after @ symbol is not valid [{email}]");

        if (emailSplit[1].Contains("..") == true)
          return RESP.SetError(1, $"email invalid double dot detected '..' [{email}]");

        string[] afterAt = emailSplit[1].Split('.');
        if (afterAt.Length < 2)
          return RESP.SetError(1, $"email value after @ symbol is not valid, invalid domain name [{email}]");
        for (int x = 0; x < afterAt.Length; x++)
        {
          if (afterAt[x].Length < 1)
            return RESP.SetError(1, $"email value after @ symbol is not valid [{email}]");
        }
      }

      return RESP.SetSuccess();
    }

    public RESP Phone(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("Validation.Phone", LOG_TYPE.DBG);

      return RESP.SetSuccess();
    }

    public RESP VariableHasValue(Core.Flow flow, Variable[] vars)
    {
      mLog?.Write("Validation.VariableHasValue", LOG_TYPE.DBG);

      for (int x = 0; x < vars.Length; x++)
      {
        Variable? var = flow.FindVariable(vars[x].Value);
        string temp = var.GetValueAsString();
        if (temp.Length == 0)
          return RESP.SetError(1, $"Variable [{vars[x].Name}] is empty [{vars[x].Value}]");
      }

      return RESP.SetSuccess();
    }
  }
}