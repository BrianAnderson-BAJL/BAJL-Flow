﻿using Core.Administration;
using Core.Administration.Messages;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class Flow : IToJson
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
    public Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
    public Plugin? StartPlugin;
    public PARM_VARS StartCommands = new PARM_VARS(); //Used so the flow engine knows when to start this flow.
    public string SampleData = "";
    public DATA_FORMAT SampleDataFormat = DATA_FORMAT._None;
    public List<Comment> Comments = new List<Comment>(4);

    public FlowRequest.START_TYPE DebugStartType;
    public TcpClientBase? DebugTcpClient;
    public Packet? DebugPacket;
    public TimeElapsed DebugFlowStartTime = new TimeElapsed();
    public TimeElapsed DebugStepTime = new TimeElapsed();

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
      if (var is not null)
      {
        name = name.ToLower(); //The flow engine is case insensitive
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

    /// <summary>
    /// Will insert Trace steps between each normal step to allow debug results to be sent to the designer.
    /// Will only be called if the Options.ServerType == Development
    /// </summary>
    private void PrepareFlowForTracing()
    {
      if (start is null)
        return;

      //Find all the links between steps, we need to insert a trace step between each of these links
      List<Link> links = new List<Link>(functionSteps.Count * 3); //About how many outbound links are in each step, good starting point
      for (int x = 0; x < functionSteps.Count; x++)
      {
        //if (functionSteps[x] != start) //We don't want to trace the start step
          links.AddRange(functionSteps[x].LinkOutputs);
      }

      //Insert a FlowCore.Trace step inbetween each existing step link.
      for (int x = 0; x < links.Count; x++)
      {
        Link link = links[x];
        FunctionStep? oldInput = link.Input.Step;
        if (oldInput is null || oldInput.Function.Input is null)
          continue; //Not linked to another step, skip it

        FunctionStep stepTrace = new FunctionStep(this, getNextId(), "FlowCore.Trace", Vector2.Zero);
        link.Input.Step = stepTrace;
        Output outputNew = stepTrace.Function.Outputs[0].Clone(stepTrace);
        Input inputNew = oldInput.Function.Input.Clone(oldInput);
        stepTrace.LinkOutputs.Add(new Link(getNextId(), outputNew, inputNew));
        stepTrace.ParmVars.Add(new PARM_VAR(stepTrace.Function.Parms[0], new VarRef(VAR_NAME_PREVIOUS_STEP)));
        this.functionSteps.Add(stepTrace);
      }
    }

    public virtual RESP? Execute()
    {
      if (start is null)
      {
        start = FindStepByName("flowcore", "start");
        if (start is null)
        {
          return RESP.SetError(1, "Flow is missing Start step"); ;
        }
        if (Options.ServerType == Options.SERVER_TYPE.Development)
        {
          PrepareFlowForTracing();
        }
      }

      start.Execute(this);
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

      if (Options.ServerType == Options.SERVER_TYPE.Development)
        SendFlowDebugResponse();

      return this.resp;
    }

    /// <summary>
    /// ONLY USED WHEN ServerType == Development
    /// Will send flow results back to the designer when a flow finishes executing
    /// </summary>
    private void SendFlowDebugResponse()
    {
      if (Options.ServerType != Options.SERVER_TYPE.Development)
        return;

      if (this.DebugTcpClient is null || this.DebugPacket is null)
        return;

      Xml xml = new Xml();
      xml.WriteMemoryNew();
      xml.WriteTagStart("DebugResults");
      for (int x = 0; x < this.functionSteps.Count; x++)
      {
        xml.WriteXml(this.functionSteps[x].DebugTraceXml);
      }
      xml.WriteTagEnd("DebugResults");

      FlowDebugResponse flowDebugResponse = new FlowDebugResponse(this.DebugPacket.PacketId, BaseResponse.RESPONSE_CODE.Success, this.FileName, this.DebugFlowStartTime.End().Ticks, xml.ReadMemory());
      this.DebugTcpClient.Send(flowDebugResponse.GetPacket());
    }

    public void SendFlowDebugTraceStep(RESP resp, FunctionStep previousStep, long ticks)
    {
      if (this.DebugTcpClient is null || this.DebugPacket is null)
        return;

      Xml xml = new Xml();
      xml.WriteMemoryNew();
      xml.WriteTagStart("Trace");
      xml.WriteTagAndContents("StepId", previousStep.Id);
      xml.WriteTagAndContents("StepName", previousStep.Name);
      xml.WriteTagAndContents("Success", resp.Success);
      xml.WriteTagAndContents("ErrorNumber", resp.ErrorNumber);
      xml.WriteTagAndContents("ErrorDescription", resp.ErrorDescription);
      xml.WriteTagAndContents("OutputIndex", resp.OutputIndex);
      xml.WriteTagStart("FlowVariables");
      for (int x = 0; x < this.Variables.Values.Count; x++)
      {
        xml.WriteTagAndContents("Variable", this.Variables.Values.ElementAt(x).ToJson(), Xml.BASE_64_ENCODE.Encoded);
      }
      xml.WriteTagEnd("FlowVariables");
      if (resp.Variable is not null)
        xml.WriteTagAndContents("Variables", resp.Variable.ToJson(), Xml.BASE_64_ENCODE.Encoded); //Let's cram some JSON in the XML, everybody loves mixing data schemas!!! The XML parser isn't 100% yet, so this is a temp hack
      xml.WriteTagEnd("Trace");
      string xmlStr = xml.ReadMemory();
      previousStep.DebugTraceXml = xmlStr;
      Core.Administration.Messages.TraceResponse trace = new Core.Administration.Messages.TraceResponse(previousStep.Id, previousStep.Name, xmlStr, ticks);
      this.DebugTcpClient.Send(trace.GetPacket());
    }

    protected virtual List<FunctionStep> ExecuteSteps(List<FunctionStep> steps)
    {
      List<FunctionStep> nextSteps = new List<FunctionStep>(16);
      for (int x = 0; x < steps.Count; x++)
      {
        FunctionStep step = steps[x];
        RESP resp = step.Execute(this);

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
        if (s is not null)
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
          if (varSplit.Length == 1)
            return baseVar;
        }
      }
      if (baseVar is not null)
        return FindVariable(baseVar, varSplit);
      else
        return null;
    }


    public bool DeleteVariable(string name)
    {
      string[] varSplit = name.Split('.');
      Variable? baseVar = null;

      if (varSplit.Length > 1)
      {
        if (Variables.ContainsKey(varSplit[0]) == true)
        {
          baseVar = Variables[varSplit[0]];
        }
      }
      else if (varSplit.Length == 1)
      {
        Variables.Remove(varSplit[0]);
      }

      if (baseVar is not null)
        return DeleteVariable(baseVar, varSplit);
      else
        return true;
    }

    public bool DeleteVariable(Variable var, params string[] varNames)
    {
      Variable? varTemp = var;
      int x = 0;
      if (varNames.Length <= 1)
        return true;

      if (var.Name == varNames[0]) //If the first name is the base name, then lets go to the next level
        x++;

      while (x < varNames.Length)
      {
        if (x == varNames.Length - 1)
        {
          return varTemp.DeleteSubVariableByName(varNames[x]); //Increment before to get the next variable name to delete it.
        }
        else
        {
          varTemp = varTemp.FindSubVariableByName(varNames[x]);
        }
        if (varTemp is null)
          return false;

        x++;
      };

      return true;
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

    public string FormatStartCommands()
    {
      string startCommands = "";
      for (int x = 0; x < StartCommands.Count; x++)
      {
        startCommands += StartCommands[x].Parm.Name;
        startCommands += $" [{StartCommands[x].ToString()}]";
        if (x < StartCommands.Count - 1)
          startCommands += " : ";
      }
      return startCommands;
    }

    public virtual void XmlReadFile(string fileName, READ_TIL til = READ_TIL.All)
    {
      functionSteps.Clear();
      FileName = fileName;
      Xml xml = new Xml();
      string content = xml.FileRead(FileName);
      XmlRead(ref content, til);
    }
    public enum READ_TIL
    {
      All,
      TilSteps, //Only read the header info for the flow, this is for when opening flows.
    }
    public virtual void XmlRead(ref string content, READ_TIL til = READ_TIL.All)
    {
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
      }

      if (StartPlugin is not null && StartPlugin.FlowStartCommands.Count > 0)
      {
        ParseVariables(StartPlugin.FlowStartCommands, StartCommands, ref startCommands);
      }

      if (til == READ_TIL.TilSteps)
        return;

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
          string parameters = Xml.GetXMLChunk(ref step, "Variables");
          FunctionStep fs = new FunctionStep(this, stepId, pluginName, functionName, stepPos, links); //Store the links XML with the step for now so we can link the steps together below
          ParseVariables(fs.Function.Parms, fs.ParmVars, ref parameters);
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

    private void ParseVariables(PARMS parms, PARM_VARS parmVars, ref string variables)
    {
      string var = "";
      do
      {
        var = Xml.GetXMLChunk(ref variables, "Variable");
        if (var.Length <= 0)
          break; //We have reached the end, lets exit

        PARM? p = null;
        string paramName = Xml.GetXMLChunk(ref var, "ParamName");
        string name = Xml.GetXMLChunk(ref var, "Name");
        if (paramName.Length <= 0) //If ParamName isn't in the XML, the parameter name wasn't changed, so just use the Name element
          paramName = name;
        p = parms.FindParmByName(paramName);
        if (p is null)
          throw new ExceptionFlowLoad($"Bad Parameter name [{name}], could not find parameter with that name for flow [{this.FileName}]");

        PARM_VAR? pv;
        string temp = Xml.GetXMLChunk(ref var, "Literal");
        PARM_VAR.PARM_L_OR_V lOrV = Enum.Parse<PARM_VAR.PARM_L_OR_V>(temp, true);
        if (lOrV == PARM_VAR.PARM_L_OR_V.Literal)
        {
          string dataType = Xml.GetXMLChunk(ref var, "DataType");
          if (dataType == "Integer" && (p.DataType == DATA_TYPE.Integer || p.DataType == DATA_TYPE.Various))
          {
            long value = Xml.GetXMLChunkAsLong(ref var, "Value");
            pv = new PARM_VAR(p, value);
          }
          else if (dataType == "Decimal" && (p.DataType == DATA_TYPE.Decimal || p.DataType == DATA_TYPE.Various))
          {
            decimal value = Xml.GetXMLChunkAsDecimal(ref var, "Value");
            pv = new PARM_VAR(p, value);
          }
          else if (dataType == "String" && (p.DataType == DATA_TYPE.String || p.DataType == DATA_TYPE.Various))
          {
            string value = Xml.GetXMLChunk(ref var, "Value", Xml.BASE_64_ENCODE.Encoded); //Need to decode the string, strings could have weird data in it like '<', '>', whatever
            pv = new PARM_VAR(p, value);
          }
          else if (dataType == "Boolean" && (p.DataType == DATA_TYPE.Boolean || p.DataType == DATA_TYPE.Various))
          {
            bool value = Xml.GetXMLChunkAsBool(ref var, "Value");
            pv = new PARM_VAR(p, value);
          }
          else if (dataType == "Object" && p.DataType == DATA_TYPE.Object)
          {
            string value = Xml.GetXMLChunk(ref var, "Value", Xml.BASE_64_ENCODE.Encoded); //Need to decode the string, strings could have weird data in it like '<', '>', whatever
            pv = new PARM_VAR(p, value);
          }
          else
          {
            throw new ExceptionFlowLoad($"Unknown Datatype [{dataType}] for parameter [{name}] in flow [{this.FileName}]");
          }
        }
        else //Variable
        {
          string value = Xml.GetXMLChunk(ref var, "Value", Xml.BASE_64_ENCODE.Encoded); //Need to decode the string, strings could have weird data in it like '<', '>', whatever
          pv = new PARM_VAR(p, new VarRef(value));
        }
        pv.ParmName = name;
        parmVars.Add(pv);
      } while (var.Length > 0) ;

      //TODO: Check that all required parameters have been assigned to
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
          if (output is not null)
          {
            output = output.Clone(function);
            string input = Xml.GetXMLChunk(ref link, "Input");
            if (input.Length > 0 && output is not null)
            {
              output.Step = function;
              int stepId = Xml.GetXMLChunkAsInt(ref input, "StepId");
              string inputLabel = Xml.GetXMLChunk(ref input, "Label");
              FunctionStep? inputStep = this.FindStepById(stepId);
              if (inputStep is not null)
              {
                if (inputStep.Function.Input is not null)
                {
                  Core.Input? iw = inputStep.Function.Input.Clone(inputStep);
                  function.LinkAdd(this, output, iw);
                }
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

    public string ToJson(JSON_ROOT_BLOCK_OPTIONS options = JSON_ROOT_BLOCK_OPTIONS.None, int TabIndents = 0)
    {
      string json = new string('\t', TabIndents);

      json += "\"flow\"";
      json += ": {";
      json += "\"filename\":\"" + this.FileName + "\",";
      json += "}";
      return json;
    }
  }
}
