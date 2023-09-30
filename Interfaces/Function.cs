﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core
{
  public class Function
  {
    public Delegate Fun;
    public PARMS Parms;
    public RESP Resps;
    private string mName;
    private string mToolTip;
    private string mDescription;
    private Plugin mPlugin;
    public Vector2 Pos;
    public Input? Input;
    private List<Output> mOutputs = new List<Output>(2);
    public bool OutputsModifiable = false; //This is for a switch statement the flow programmer needs to be able to modify the number of outputs (case val = 36, case val = 0, case default, ...) NOT SURE IF THIS WILL BE SUPPORTED!
    public bool DefaultSaveResponseVariable = false;

    /// <summary>
    /// The names of the return variables, these are needed for the designer, the author can change the name of the base variable if needed in the flow. (could have two connection_handle varialbes)
    /// </summary>
    public Variable RespNames = new Variable();

    public ReadOnlyCollection<Output> Outputs => mOutputs.AsReadOnly();


    public string Name
    { get { return mName; }
      private set { mName = value; }
    }

    public string ToolTip
    {
      get { return mToolTip; }
      private set { mToolTip = value; }
    }

    public string Description
    {
      get { return mDescription; }
      private set { mDescription = value; }
    }

    public Plugin Plugin
    {
      get { return mPlugin; }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Function(string name, Plugin plugin, Delegate function)
    {
      Initialize(name, plugin, function, "", "");
    }
    public Function(string name, Plugin plugin, Delegate function, string tooltip, string description)
    {
      Initialize(name, plugin, function, tooltip, description);
    }
    
    /// <summary>
    /// This constructor is only used when cloning the function
    /// </summary>
    /// <param name="name"></param>
    private Function(string name)
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private void Initialize(string name, Plugin plugin, Delegate function, string tooltip, string description)
    {
      mName = name;
      Fun = function;
      Parms = new PARMS();
      Resps = new RESP();
      mToolTip = "";
      mDescription = "";
      mPlugin = plugin;
      Input = new Input("Input", new Vector2(10, 50));
      OutputAdd("Success", Output.SUCCESS_POS);
      OutputAdd("Error", Output.ERROR_POS);
    }

    /// <summary>
    /// Wrapper for adding outputs, need to make sure that the 'OutputIndex' is set correctly
    /// </summary>
    /// <param name="label"></param>
    /// <param name="pos"></param>
    public void OutputAdd(string label, Vector2 pos)
    {
      mOutputs.Add(new Output(label, pos, Outputs.Count));
    }

    public void OutputClear()
    {
      mOutputs.Clear();
    }
    public Output? FindOutput(string label)
    {
      for (int x = 0; x < Outputs.Count; x++)
      {
        if (Outputs[x].Label == label)
          return Outputs[x];
      }
      return null;
    }

    public RESP Execute(PARMS parms)
    {
      //All plugin functions are required to return a RESP object in the response
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
      RESP resp = Fun.DynamicInvoke(parms) as RESP;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
      return resp!;
    }

  }
}