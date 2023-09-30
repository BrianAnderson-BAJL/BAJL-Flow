using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static Core.PARM;

namespace Core
{
  public class Flow
  {
    public const string VAR_NAME_FLOW_START = "flow_start";
    public const string VAR_REQUEST = "request";
    public const string VAR_NAME_PREVIOUS_STEP = "previous_step_resp";
    public const string VAR_DATA = "data";
    protected int FileVersion = 0;
    protected RESP? resp;
    protected List<FunctionStep> functionSteps = new List<FunctionStep>(32);
    protected FunctionStep? start = null;
    public string FileName = "";
    protected DateTime mCreatedDateTime = DateTime.MinValue;
    protected DateTime mModifiedLastDateTime = DateTime.MinValue;
    protected List<User> previousUsers = new List<User>(10);
    protected Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
    public Plugin? StartPlugin;
    public PARMS StartCommands = new PARMS(); //Used so the flow engine knows when to start this flow.
    public string SampleData = "";
    public DATA_FORMAT SampleDataFormat = DATA_FORMAT._None;
    public List<Comment> Comments = new List<Comment>(4);

    private int currentId = 1;

    public RESP? Resp
    {
      get { return resp; }
      set { resp = value; }
    }

    public Flow Clone()
    {
      Flow flow = new Flow();
      flow.FileName = FileName; //Don't need to clone, it is only read, not writen to
      flow.FileVersion = FileVersion; //Don't need to clone, it is only read, not writen to
      flow.StartPlugin = StartPlugin; //Don't need to clone, it is only read, not writen to
      flow.StartCommands = StartCommands; //Don't need to clone, it is only read, not writen to
      
      if (functionSteps.Count > flow.functionSteps.Capacity) //If the default size isn't big enough for the steps, lets allocate a big enough list only once.
      {
        flow.functionSteps = new List<FunctionStep>(functionSteps.Count);
      }
      for (int x = 0; x < functionSteps.Count; x++) 
      {
        flow.functionSteps.Add(functionSteps[x].Clone(this));
      }

      return flow;
    }

    /// <summary>
    /// Will add a variable to the flows variables. If the name already exists, it will overwrite the old variable with the new variable
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="var">The actual Variable object</param>
    public void VariableAdd(string name, Variable? var)
    {
      if (var != null)
      {
        name = name.ToLower(); //The flow engine will be case insensitive
        if (Variables.ContainsKey(name))
        {
          Variables[name] = var; //If the key exists, just overwrite it
        }
        else
        {
          Variables.Add(name, var);
        }
      }
    }

    public void VariableDelete(string name)
    {
      if (Variables.ContainsKey(name) == true)
      {
        Variables.Remove(name);
      }
    }

    public DateTime CreateDateTime
    {
      get {return mCreatedDateTime; }
    }

    public DateTime ModifiedLastDateTime
    {
      get { return mModifiedLastDateTime; }
    }


    public int getNextId()
    {
      return currentId++;
    }

    public virtual RESP? Execute()
    {
      if (start == null)
      {
        start = FindStepByName("flowcore", "start");
        if (start == null)
        {
          return RESP.SetError(1, "Flow is missing Start step"); ;
        }
      }


      //start.RuntimeParms = start.parms.Clone();
      List<FunctionStep> nextSteps = GetNextSteps(start);
      List<FunctionStep> nextNextSteps = new List<FunctionStep>(16);
      do
      {
        nextNextSteps.AddRange(ExecuteSteps(nextSteps));
        nextSteps.Clear();
        nextSteps.AddRange(nextNextSteps);
        nextNextSteps.Clear();
      } while (nextSteps.Count > 0);
      return this.resp;
    }

    protected virtual List<FunctionStep> ExecuteSteps(List<FunctionStep> steps)
    {
      List<FunctionStep> nextSteps = new List<FunctionStep>(16);
      for (int x = 0; x < steps.Count; x++)
      {
        FunctionStep step = steps[x];
        RESP resp = step.Execute(this);
        this.VariableAdd(VAR_NAME_PREVIOUS_STEP, resp.Variable);  //Previous step variable always contains the last steps values
        if (step.SaveResponseVariable == true)
        {
          this.VariableAdd(step.RespNames.Name, resp.Variable);  //If the flow author wants/needs the response later, they can store it in a new flow variable
        }
        nextSteps.AddRange(step.GetNextStepsBasedOnResp());
      }


      return nextSteps;
    }


    public virtual List<FunctionStep> GetPreviousSteps(FunctionStep step)
    {
      List<FunctionStep> steps = new List<FunctionStep>(16);

      return steps;
    }

    protected virtual List<FunctionStep> GetNextSteps(FunctionStep step)
    {
      List<FunctionStep> nextSteps = new List<FunctionStep>(16);
      for (int x = 0; x < step.LinkOutputs.Count; x++)
      {
        FunctionStep? s = step.LinkOutputs[x].Input.Step;
        if (s != null)
        {
          //s.RuntimeParms = s.parms.Clone();
          nextSteps.Add(s);
        }
      }
      return nextSteps;
    }


    public FunctionStep? FindStepById(int id)
    {
      for (int x = 0; x < functionSteps.Count; x++)
      {
        if (functionSteps[x].Id == id)
          return functionSteps[x];
      }
      return null;
    }

    public FunctionStep? FindStepByName(string plugin, string function)
    {
      for (int x = 0; x < functionSteps.Count; x++)
      {
        if (functionSteps[x].Function.Plugin.Name.ToLower() == plugin.ToLower() && functionSteps[x].Function.Name.ToLower() == function.ToLower())
          return functionSteps[x];
      }
      return null;
    }

    public string FindVariableStringValue(string name)
    {
      string[] varSplit = name.Split('.');
      Variable? baseVar = null;

      if (varSplit.Length > 0)
      {
        if (Variables.ContainsKey(varSplit[0]) == true)
        {
          baseVar = Variables[varSplit[0]];
        }
      }
      if (baseVar is not null)
      {
        VariableString? var = FindVariable(baseVar, varSplit) as VariableString;
        if (var is not null)
        {
          return var.Value;
        }
      }

      return "";
    }


    public object? FindVariableObjectValue(string name)
    {
      string[] varSplit = name.Split('.');
      Variable? baseVar = null;

      if (varSplit.Length > 0)
      {
        if (Variables.ContainsKey(varSplit[0]) == true)
        {
          baseVar = Variables[varSplit[0]];
        }
      }
      if (baseVar is not null)
      {
        VariableObject? variableObject = FindVariable(baseVar, varSplit) as VariableObject;
        if (variableObject is not null)
        {
          return variableObject.Value;
        }
      }

      return null;
    }

    public Variable? FindVariable(string name)
    {
      string[] varSplit = name.Split('.');
      Variable? baseVar = null;

      if (varSplit.Length > 0)
      {
        if (Variables.ContainsKey(varSplit[0]) == true)
        {
          baseVar = Variables[varSplit[0]];
        }
      }
      if (baseVar is not null)
        return FindVariable(baseVar, varSplit);
      else
        return null;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="var"></param>
    /// <param name="name">This will be in the format varName.SubVarName.SubSubVarName...</param>
    /// <returns></returns>
    public Variable? FindVariable(Variable var, string name)
    {
      string[] varSplit = name.Split('.');
      return FindVariable(var, varSplit);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="varNames"></param>
    /// <returns>The Variable if found</returns>
    public Variable? FindVariable(Variable var, params string[] varNames)
    {
      Variable? varTemp = var;
      int x = 0;
      if (varNames.Length == 0)
        return null;

      if (var.Name == varNames[0]) //If the first name is the base name, then lets go to the next level
        x++;

      while (x < varNames.Length)
      {
        varTemp = varTemp.FindSubVariableByName(varNames[x]);
        if (varTemp is null)
          return null;

        x++;
      };

      return varTemp;
    }

    public string CreateVariableName(params string[] names)
    {
      if (names.Length == 0)
        return "";

      string newName = names[0];
      for (int x = 1; x < names.Length; x++)
      {
        newName += "." + names[x];
      }
      return newName;
    }

    public virtual void XmlRead(string fileName)
    {
      functionSteps.Clear();
      FileName = fileName;
      Xml xml = new Xml();
      string content = xml.FileRead(FileName);
      content = Xml.GetXMLChunk(ref content, "Base"); //Strip off the base tag

      //Parse the meta data from the flow file
      string metaData = Xml.GetXMLChunk(ref content, "MetaData");
      int FileFormatVersion = Xml.GetXMLChunkAsInt(ref metaData, "FileFormatVersion"); //Not needed yet, but it will be if I ever create a v2 file format
      FileVersion = Xml.GetXMLChunkAsInt(ref metaData, "FileVersion");
      mCreatedDateTime = Xml.GetXMLChunkAsDateTime(ref metaData, "CreatedDateTime");
      mModifiedLastDateTime = Xml.GetXMLChunkAsDateTime(ref metaData, "ModifiedLastDateTime");
      string temp = Xml.GetXMLChunk(ref metaData, "SampleDataFormat"); //Could be JSON or XML data for testing the flow.
      if (temp == "Json")
        SampleDataFormat = DATA_FORMAT.Json;
      else if (temp == "Xml")
        SampleDataFormat = DATA_FORMAT.Xml;
      SampleData = Xml.GetXMLChunk(ref metaData, "SampleData", Xml.BASE_64_ENCODE.Encoded); //Could be JSON or XML data for testing the flow.
      string usersData = Xml.GetXMLChunk(ref metaData, "Users");
      string userXml = "";
      do
      {
        userXml = Xml.GetXMLChunk(ref usersData, "User");
        if (userXml != "")
        {
          User u = new User();
          u.FromXml(ref userXml);
          previousUsers.Add(u);
        }
      } while (userXml != "");

      string flow = Xml.GetXMLChunk(ref content, "Flow");
      //Load StartCommands
      string startCommands = Xml.GetXMLChunk(ref flow, "StartCommands");
      string pluginName = Xml.GetXMLChunk(ref startCommands, "StartPlugin");
      if (pluginName != "")
      {
        StartPlugin = PluginManager.FindPluginByName(pluginName);
        if (StartPlugin != null)
        {
          StartCommands = StartPlugin.FlowStartCommands.Clone();
        }
      }
      if (StartCommands.Count > 0) //This plugin doesn't have any start commands currently (probably there is no plugin assigned as the startPlugin)
      {
        ParseParameters(StartCommands, ref startCommands);
      }
      //Load all the steps
      string step = "";
      do
      {
        step = Xml.GetXMLChunk(ref flow, "Step");
        if (step.Length > 0)
        {
          int stepId = Xml.GetXMLChunkAsInt(ref step, "Id");
          if (stepId >= currentId)
            currentId = stepId + 1;
          pluginName = Xml.GetXMLChunk(ref step, "PluginName");
          string functionName = Xml.GetXMLChunk(ref step, "FunctionName");
          Vector2 stepPos = Xml.GetXMLChunkAsVector2(ref step, "Position");
          bool saveRespVar = Xml.GetXMLChunkAsBool(ref step, "SaveResponseVariable");
          string saveRespVarName = Xml.GetXMLChunk(ref step, "SaveResponseVariableName");
          string links = Xml.GetXMLChunk(ref step, "Links"); //Can't fully parse the links here since we haven't loaded all the steps yet
          string parameters = Xml.GetXMLChunk(ref step, "Parameters");
          FunctionStep fs = new FunctionStep(this, stepId, pluginName, functionName, stepPos, links); //Store the links XML with the step for now so we can link the steps together below
          fs.parms = fs.Function.Parms.Clone();
          ParseParameters(fs.parms, ref parameters);
          functionSteps.Add(fs);
        }
      } while (step.Length > 0);
      string comments = Xml.GetXMLChunk(ref flow, "Comments");
      string comment = "";
      do
      {
        comment = Xml.GetXMLChunk(ref comments, "Comment");
        if (comment.Length > 0)
        {
          Comment c = new Comment();
          c.Id = Xml.GetXMLChunkAsInt(ref comment, "Id");
          if (c.Id >= currentId)
            currentId = c.Id + 1;
          c.Position = Xml.GetXMLChunkAsVector2(ref comment, "Position");
          c.Size = Xml.GetXMLChunkAsSize(ref comment, "Size");
          c.Text = Xml.GetXMLChunk(ref comment, "Text");
          Comments.Add(c);
        }
      } while (comment.Length > 0);
      //Have to parse the links after all the steps are loaded, need to link steps to each other
      for (int x = 0; x < functionSteps.Count; x++)
      {
        FunctionStep fs = functionSteps[x];
        ParseLinks(fs);
      }
    }

    private void ParseParameters(PARMS parms, ref string parameters)
    {
      string lastParmName = "";
      string parm = "";
      do
      {
        parm = Xml.GetXMLChunk(ref parameters, "Parameter");
        if (parm.Length > 0)
        {
          PARM? p = null;
          string name = Xml.GetXMLChunk(ref parm, "Name");
          if (name == lastParmName) //Need to perform some special cloning if this is a multiple parameter
          {
            p = parms.FindParmByName(name);
            if (p is not null && p.AllowMultiple == PARM_ALLOW_MULTIPLE.Multiple)
            {
              p = p.Clone();
              parms.Add(p);
            }
          }
          else
          {
            p = parms.FindParmByName(name);
          }
          lastParmName = name;

          if (p != null)
          {
            string dataType = Xml.GetXMLChunk(ref parm, "DataType");
            if (dataType == "Integer" && p.DataType == DATA_TYPE.Integer)
            {
              long value = Xml.GetXMLChunkAsLong(ref parm, "Value");
              p.SetValue(value);
            }
            else if (dataType == "Decimal" && p.DataType == DATA_TYPE.Decimal)
            {
              decimal value = Xml.GetXMLChunkAsDecimal(ref parm, "Value");
              p.SetValue(value);
            }
            else if (dataType == "DropDownList" && p.DataType == DATA_TYPE.DropDownList)
            {
              string value = Xml.GetXMLChunk(ref parm, "Value");
              p.SetValue(value);
            }
            else if ((dataType == "String" && p.DataType == DATA_TYPE.String) || (dataType == "Object" && p.DataType == DATA_TYPE.Object) || (dataType == "Block" && p.DataType == DATA_TYPE.Block)) //Object data types are just strings under the covers, they just hold the varialble name
            {
              string value = Xml.GetXMLChunk(ref parm, "Value", Xml.BASE_64_ENCODE.Encoded); //Need to decode the string, strings could have weird data in it like '<', '>', whatever
              p.SetValue(value);
            }
            else if (dataType == "Boolean" && p.DataType == DATA_TYPE.Boolean)
            {
              bool value = Xml.GetXMLChunkAsBool(ref parm, "Value");
              p.SetValue(value);
            }
            string lit = Xml.GetXMLChunk(ref parm, "Literal");
            p.ParmLiteral = System.Enum.Parse<PARM_L_OR_V>(lit);
          }
        }
      } while (parm.Length > 0);
    }

    private void ParseLinks(FunctionStep function)
    {
      string link = "";
      string LinkStr = function.LinkStr;
      do
      {
        link = Xml.GetXMLChunk(ref LinkStr, "Link");
        if (link.Length > 0)
        {
          int id = Xml.GetXMLChunkAsInt(ref link, "Id");
          if (id >= currentId)
            currentId = id + 1;
          string outputLabel = Xml.GetXMLChunk(ref link, "OutputLabel");

          Output? output = function.Function.FindOutput(outputLabel);

          string input = Xml.GetXMLChunk(ref link, "Input");
          if (input.Length > 0 && output != null)
          {
            int stepId = Xml.GetXMLChunkAsInt(ref input, "StepId");
            string inputLabel = Xml.GetXMLChunk(ref input, "Label");
            FunctionStep? inputStep = this.FindStepById(stepId);
            if (inputStep != null)
            {
              if (inputStep.Function.Input != null)
              {
                Core.Input? iw = inputStep.Function.Input.Clone(inputStep);
                function.LinkAdd(this, output, iw);
              }
            }
          }
        }
      } while (link.Length > 0);

    }

    public override string ToString()
    {
      return FileName;
    }
  }
}
