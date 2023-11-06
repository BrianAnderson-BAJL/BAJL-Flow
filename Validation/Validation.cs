using Core;
using System.Drawing;

namespace Validation
{
  public class Validation : Core.Plugin
  {
    public override void Init()
    {
      base.Init();

      Function function = new Function("Email", this, Email, "", "");
      function.Parms.Add("Email", DATA_TYPE.String);
      PARM parm = new PARM("Format", DATA_TYPE.DropDownList);
      parm.OptionAdd("a@a");
      parm.OptionAdd("a@a.a");
      function.Parms.Add(parm);
      Functions.Add(function);

      function = new Function("Phone", this, Phone);
      //function.Description
      function.Parms.Add("Phone Number", DATA_TYPE.String);
      parm = new PARM("Format", DATA_TYPE.DropDownList);
      parm.OptionAdd("###-###-####");
      parm.OptionAdd("+1 ###-###-####");
      function.Parms.Add(parm);
      Functions.Add(function);
      //SETTINGS
      {
        SettingAddIfMissing(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
        SettingAddIfMissing(new Setting("", "Designer", "BorderColor", Color.Crimson));
        SettingAddIfMissing(new Setting("", "Designer", "FontColor", Color.Black));
      }
      //SETTINGS

    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
    }

    public override void StopPlugin()
    {
    }

    public RESP Email(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Validation.Email");
      vars[0].GetValue(out string email);
      vars[1].GetValue(out string format);

      if (email is null)
        return RESP.SetError(1, "email is null");

      if (email.Contains('@') == false)
        return RESP.SetError(1, "No @ symbol");

      string[] emailSplit = email.Split('@');
      if (emailSplit.Length != 2)
        return RESP.SetError(1, "Wrong number of @ symbols");

      if (format == "a@a")
      {
        if (emailSplit[0].Length < 1)
          return RESP.SetError(1, "Value before @ symbol is not valid");

        if (emailSplit[1].Length < 1)
          return RESP.SetError(1, "Value after @ symbol is not valid");
      }
      else if (format == "a@a.a")
      {
        if (emailSplit[0].Length < 1)
          return RESP.SetError(1, "Value before @ symbol is not valid");

        if (emailSplit[1].Length < 3) // must be at least a.a (3 characters)
          return RESP.SetError(1, "Value after @ symbol is not valid");

        if (emailSplit[1].Contains("..") == true)
          return RESP.SetError(1, "Invalid double dot detected '..'");

        string[] afterAt = emailSplit[1].Split('.');
        for (int x = 0; x < afterAt.Length; x++)
        {
          if (afterAt[x].Length < 1)
            return RESP.SetError(1, "Value after @ symbol is not valid");
        }
      }

      return RESP.SetSuccess();
    }

    public RESP Phone(Core.Flow flow, Variable[] vars)
    {
      Global.Write("Validation.Phone");

      return RESP.SetSuccess();
    }
  }
}