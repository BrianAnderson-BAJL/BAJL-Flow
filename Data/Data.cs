using FlowEngineCore;
using System.Drawing;
using static FlowEngineCore.PARM;

namespace Data
{
  public class Data : FlowEngineCore.Plugin
  {
    private const int ERROR_VARIABLE_MISSING = (int)STEP_ERROR_NUMBERS.DataErrorMin;
    private const int ERROR_SOURCE_IS_NULL = (int)STEP_ERROR_NUMBERS.DataErrorMin + 1;
    private const int ERROR_SPLIT_VALUE_IS_NULL = (int)STEP_ERROR_NUMBERS.DataErrorMin + 2;
    private const int ERROR_SEEK_VALUE_IS_NOT_FOUND = (int)STEP_ERROR_NUMBERS.DataErrorMin + 3;

    public override void Init()
    {
      base.Init();

      Function function;
      function = new Function("Variables Exists", this, VariablesExists);
      function.UpdateErrorOutputLabel("Var Missing");
      function.Parms.Add("Variable Name", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes, PARM.PARM_ALLOW_MULTIPLE.Multiple, PARM.PARM_RESOLVE_VARIABLES.No);
      Functions.Add(function);

      function = new Function("Variable Create", this, VariableCreate);
      PARM parm = new PARM("Value", DATA_TYPE.Various);
      parm.ValidatorAdd(PARM_VALIDATION.NumberDecimalPlaces, 5);
      function.Parms.Add(parm);
      PARM pddl = new PARM("Sub block format", STRING_SUB_TYPE.DropDownList, PARM.PARM_REQUIRED.Yes);
      pddl.OptionAdd(DATA_FORMAT_SUB_VARIABLES.Block.ToString());
      pddl.OptionAdd(DATA_FORMAT_SUB_VARIABLES.Array.ToString());
      pddl.ValidatorAdd(PARM_VALIDATION.StringDefaultValue, DATA_FORMAT_SUB_VARIABLES.Block.ToString());
      function.Parms.Add(pddl);
      function.RespNames.Name = "newVariable";
      function.DefaultSaveResponseVariable = true;
      function.OutputAddSuccess("Continue");
      Functions.Add(function);

      function = new Function("Sub Variable Add", this, VariableSubVariableAdd);
      function.Parms.Add("Variable to add sub variables to", DATA_TYPE.Various);
      parm = new PARM("Sub variable", DATA_TYPE.Various, PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE.Multiple);
      parm.NameChangeable = true;
      function.Parms.Add(parm);
      function.OutputAddSuccess("Continue");
      Functions.Add(function);

      function = new Function("Variables Delete", this, VariablesDelete);
      function.Parms.Add("Variable Name to delete", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes, PARM.PARM_ALLOW_MULTIPLE.Multiple, PARM.PARM_RESOLVE_VARIABLES.No); //We want raw variable names in this function so we can delete the actual variable objects
      function.OutputAddSuccess("Continue");
      Functions.Add(function);

      function = new Function("Variable Rename", this, VariableRename);
      function.Parms.Add("Variable to rename", DATA_TYPE.String);
      function.Parms.Add("New Variable Name", DATA_TYPE.String);
      function.OutputAddSuccess("Continue");
      Functions.Add(function);

      function = new Function("Variable Set Value", this, VariableSetValue);
      function.Parms.Add("Variable to set", DATA_TYPE.String);
      function.Parms.Add("New Variable Value", DATA_TYPE.Various);
      function.OutputAddSuccess("Continue");
      Functions.Add(function);

      function = new Function("Flatten to Array", this, FlattenToArray, "", "Take all the sub variables from the parameters and flatten them all to a single array");
      parm = new PARM("Sub variable", DATA_TYPE.Various, PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE.Multiple);
      function.Parms.Add(parm);
      function.OutputAddSuccess("Continue");
      function.RespNames.Name = "flattenedArray";
      function.DefaultSaveResponseVariable = true;
      Functions.Add(function);

      function = new Function("Group Array data", this, GroupArrayData, "", "Loop through an array of data and group it by the variable names");
      parm = new PARM("Source array", DATA_TYPE.Various);
      function.Parms.Add(parm);
      parm = new PARM("New sub variable name", DATA_TYPE.String);
      function.Parms.Add(parm);
      parm = new PARM("Group by variable", DATA_TYPE.Various, PARM_REQUIRED.Yes, PARM_ALLOW_MULTIPLE.Multiple);
      parm.NameChangeIncrement = true;
      function.Parms.Add(parm);
      function.OutputAddSuccess("Continue");
      function.RespNames.Name = "groupedArray";
      function.DefaultSaveResponseVariable = true;
      Functions.Add(function);



      function = new Function("Contains", this, VariableContains);
      function.Parms.Add("Source string to check", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      parm = function.Parms.Add("Value to seek", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      parm.ValidatorAdd(PARM.PARM_VALIDATION.StringMinLength, 1);
      function.Parms.Add("Case sensitive?", DATA_TYPE.Boolean, PARM.PARM_REQUIRED.Yes);
      Functions.Add(function);

      function = new Function("Split", this, VariableSplit);
      function.Parms.Add("Source string to be split", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      parm = function.Parms.Add("Value to split with", DATA_TYPE.String, PARM.PARM_REQUIRED.Yes);
      parm.ValidatorAdd(PARM.PARM_VALIDATION.StringMinLength, 1);
      Functions.Add(function);


      // SETTINGS
      mSettings.SettingAdd(new Setting("", "Designer", "BackgroundColor", Color.Transparent));
      mSettings.SettingAdd(new Setting("", "Designer", "BorderColor", Color.CadetBlue));
      mSettings.SettingAdd(new Setting("", "Designer", "FontColor", Color.Black));

    }

    public override void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      base.StartPlugin(GlobalPluginValues);
    }

    public override void StopPlugin()
    {
      base.StopPlugin();
    }



    /// <summary>
    /// Create a new variable in a flow. Might want to send a custom JSON/XML format back to the caller.
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    private RESP VariableCreate(FlowEngineCore.Flow flow, Variable[] vars)
    {
      Variable newVar = new Variable();
      newVar.DataType = vars[0].DataType;
      newVar.Value = vars[0].Value;
      Enum.TryParse<DATA_FORMAT_SUB_VARIABLES>(vars[1].GetValueAsString(), out DATA_FORMAT_SUB_VARIABLES subFormat);
      newVar.SubVariablesFormat = subFormat;
      return RESP.SetSuccess(newVar);
    }

    /// <summary>
    /// Create a variable and add sub variables to it.
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    private RESP VariableSubVariableAdd(FlowEngineCore.Flow flow, Variable[] vars)
    {
      Variable newVar = new Variable(vars[0]);

      for (int x = 1; x < vars.Length; x++)
      {
        newVar.SubVariableAdd(vars[x]);
      }
      return RESP.SetSuccess(newVar);
    }

    /// <summary>
    /// Check if variables exist, is good for checking / validating JSON or XML incoming
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    private RESP VariablesExists(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableExists", LOG_TYPE.DBG);

      for (int x = 0; x < vars.Length; x++)
      {
        Variable? v = flow.FindVariable(vars[x].Value);
        if (v is null)
          return RESP.SetError(ERROR_VARIABLE_MISSING, $"Variable missing [{vars[x].Value}]");
      }
      return RESP.SetSuccess();
    }

    /// <summary>
    /// Delete a variable and all sub variables of the supplied 
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    private RESP VariablesDelete(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariablesDelete", LOG_TYPE.DBG);

      for (int x = 0; x < vars.Length; x++)
      {
        vars[x].GetValue(out string varName);
        mLog?.Write($"Deleting variable [{varName}]", LOG_TYPE.DBG);
        flow.DeleteVariable(varName);
      }
      //If we got into this function, then all the variables exist (FunctionStep.Execute validates all the variables before calling the function), just return success. Easist function ever!

      return RESP.SetSuccess();
    }

    /// <summary>
    /// Take any variables with sub varialbes and flatten them to a single array
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    public RESP FlattenToArray(Flow flow, Variable[] vars)
    {
      Variable newVar = new Variable("flattenedToArray");
      newVar.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;

      for (int x = 0; x < vars.Length; x++)
      {
        Variable temp = vars[x];
        AddToArray(ref newVar, temp);
      }

      return RESP.SetSuccess(newVar);
    }

    /// <summary>
    /// Recursive AddToArray function used by 'FlattenToArray'
    /// </summary>
    /// <param name="arrayAddingTo"></param>
    /// <param name="var"></param>
    private void AddToArray(ref Variable arrayAddingTo, Variable var, bool preserveName = false)
    {
      if (var.DataType != DATA_TYPE._None)
      {
        if (preserveName)
          arrayAddingTo.SubVariableAdd(new Variable() { Name = var.Name, DataType = var.DataType, Value = var.Value });
        else
          arrayAddingTo.SubVariableAdd(new Variable() { DataType = var.DataType, Value = var.Value });
      }
      for (int x = 0; x < var.Count; x++)
      {
        Variable temp = var[x];
        if (temp.DataType != DATA_TYPE._None)
        {
          if (preserveName)
            arrayAddingTo.SubVariableAdd(new Variable() { Name = temp.Name, DataType = temp.DataType, Value = temp.Value });
          else
            arrayAddingTo.SubVariableAdd(new Variable() { DataType = temp.DataType, Value = temp.Value });
        }
        if (temp.Count > 0)
          AddToArray(ref arrayAddingTo, temp);
      }
    }

    /// <summary>
    /// Pulls out duplicate data (maybe from an SQL recordset) and pushes the unique data into an array
    /// </summary>
    /// <param name="flow"></param>
    /// <param name="vars"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public RESP GroupArrayData(Flow flow, Variable[] vars)
    {
      Variable newRoot = new Variable();
      newRoot.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;
      Variable newItem = new Variable();
      Variable newSubItem = new Variable(vars[1].GetValueAsString());

      Dictionary<string, Variable> uniqueItems = new Dictionary<string, Variable>();
      string uniqueKey = "";
      Variable source = vars[0];

      List<Variable> uniqueItemsToReadd = new List<Variable>(32);

      //Find unique keys
      for (int x = 0; x < source.Count; x++)
      {
        Variable tmpSource = source[x];
        tmpSource.Name = "";
        uniqueKey = "";
        uniqueItemsToReadd.Clear();
        for (int y = 2; y < vars.Length; y++)
        {
          Variable? tmpSub = tmpSource.SubVariableFindByName(vars[y].GetValueAsString());
          if (tmpSub is null)
            throw new ArgumentException($"Sub variable not found {vars[y].GetValueAsString()}");
          uniqueKey += tmpSub.GetValueAsString();
          tmpSource.SubVariableDeleteByName(vars[y].GetValueAsString());
          uniqueItemsToReadd.Add(tmpSub);
        }
        if (uniqueItems.ContainsKey(uniqueKey) == true)
        {
          newItem = uniqueItems[uniqueKey];
          newSubItem = newItem.SubVariableFindByName(vars[1].GetValueAsString())!;
          newSubItem.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;
        }
        else
        {
          newItem = new Variable();
          newSubItem = new Variable(vars[1].GetValueAsString());
          if (tmpSource.Count < 2)
            newSubItem.SubVariablesFormat = DATA_FORMAT_SUB_VARIABLES.Array;
          for (int i = 0; i < uniqueItemsToReadd.Count; i++)
          {
            newItem.SubVariableAdd(uniqueItemsToReadd[i]);
          }
          newItem.SubVariableAdd(newSubItem);
          uniqueItems.Add(uniqueKey, newItem);
        }
        if (tmpSource.Count == 1)
        {
          AddToArray(ref newSubItem, tmpSource, false);
        }
        else
        {
          //AddToArray(ref newSubItem, tmpSource, true);
          newSubItem.SubVariableAdd(tmpSource);
        }
      }

      foreach (var item in uniqueItems.Values)
      {
        newRoot.SubVariableAdd(item);
      }

      return RESP.SetSuccess(newRoot);
    }


    private RESP VariableSplit(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableSplit", LOG_TYPE.DBG);
      vars[0].GetValue(out string source);
      vars[1].GetValue(out string splitOn);

      if (source is null)
        return RESP.SetError(ERROR_SOURCE_IS_NULL, "Source is null");
      if (splitOn is null)
        return RESP.SetError(ERROR_SPLIT_VALUE_IS_NULL, "Split on value is null");


      string[] splitStr = source.Split(splitOn, StringSplitOptions.RemoveEmptyEntries);
      Variable var = new Variable();
      for (int x = 0; x < splitStr.Length; x++)
      {
        var.SubVariableAdd(new Variable(x.ToString(), splitStr[x]));
      }
      return RESP.SetSuccess(var);
    }


    private RESP VariableContains(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableContains", LOG_TYPE.DBG);
      vars[0].GetValue(out string source);
      vars[1].GetValue(out string seekVal);
      vars[2].GetValue(out bool caseSensitive);

      if (source is null)
        return RESP.SetError(ERROR_SOURCE_IS_NULL, "Source is null");
      if (seekVal is null)
        return RESP.SetError(1, "Seek value is null");

      if (caseSensitive == false)
      {
        source = source.ToLower();
        seekVal = seekVal.ToLower();
      }

      if (source.Contains(seekVal) == true)
        return RESP.SetSuccess();
      else
        return RESP.SetError(ERROR_SEEK_VALUE_IS_NOT_FOUND, "Seek value is not contained in Source value");
    }

    private RESP VariableRename(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableRename", LOG_TYPE.DBG);

      vars[0].Name = vars[1].Value;

      return RESP.SetSuccess();
    }

    private RESP VariableSetValue(FlowEngineCore.Flow flow, Variable[] vars)
    {
      mLog?.Write("FlowCore.VariableRename", LOG_TYPE.DBG);

      vars[0].Value = vars[1].Value;
      vars[0].DataType = vars[1].DataType;

      return RESP.SetSuccess();
    }

  }
}
