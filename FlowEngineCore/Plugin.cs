using FlowEngineCore.Interfaces;
using System.Drawing;
using System.Reflection;
using System.Web;


namespace FlowEngineCore
{

//#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

  /// <summary>
  /// New Plugin creation notes
  ///   1. Don't forget to use the mFlowsCriticalSection lock when finding a flow in the 'Flows' list This list could also be accessed by a user saving a flow with the 'FlowGoLive' option set
  /// </summary>


  public abstract class Plugin
  {
    protected ILog? mLog = null;

    /// <summary>
    /// A List of functions, the functions all have a dictionary of parameters, and a dictionary of results
    /// </summary>
    public List<Function> Functions = new(16);
    
    /// <summary>
    /// Any constants the plugin would want to include in the flow
    /// </summary>
    public Dictionary<string, Variable> Constants = new(16);

    /// <summary>
    /// These are for global values for the plugin
    /// </summary>
    protected Settings mSettings = new();

    /// <summary>
    /// The actual DLL that is loaded at runtime
    /// </summary>
    public Assembly? PluginAssembly;

    /// <summary>
    /// The name of the plugin, defaults to the name of the class, but can be overriden to name it something different
    /// </summary>
    public string Name = "";

    /// <summary>
    /// Defines which order plugins should be started, lower number starts earlier, multiple plugins with the same StartPriority the start up order is undefined (random) All plugins by default have the latest start order. int.MaxValue
    /// I put 1000 between each plugin's priority which will give other plugins some room to set a priority higher or lower than other plugins
    /// </summary>
    public int StartPriority = int.MaxValue;
    /// <summary>
    /// Information so the flow engine knows what flow to start based on the plugin that is requesting a flow to start
    /// </summary>
    public PARMS FlowStartCommands = new PARMS();
    public PARM_VARS FlowStartCommandsParmVars = new PARM_VARS();

    /// <summary>
    /// The flows that this plugin could start, this is used only by the Flow Engine
    /// </summary>
    protected List<FlowEngineCore.Flow> Flows = new();

    /// <summary>
    /// Used in the Flow Engine Designer, allows users to see what variables will be in the flow when it starts.
    /// Not all variables will be showable, like in the Http plugin if it is a POST with JSON the designer won't know what data will appear.
    /// The designer will have the ability to include sample data when designing a flow, a JSON sample packet can be included
    /// </summary>
    public Variable? SampleStartData;

    protected object mFlowsCriticalSection = new object();


    public virtual void FlowAdd(FlowEngineCore.Flow flow)
    {
      lock (mFlowsCriticalSection)
      {

        flow.FileName = flow.FileName.Replace('\\', '/'); //Doesn't seem to be any way to get C# to use only forward slashes everywhere, makes it difficult to do file name string compares.
        FlowRemove(flow.FileName); //If a flow with the same file name is loaded, remove it so we can add the new one.

        Flows.Add(flow);
        FlowEngine.Log?.Write("Flow Added - " + flow.FileName);
      }
    }

    public virtual void FlowRemove(string fileName)
    {
      lock (mFlowsCriticalSection)
      {
        for (int x = 0; x < Flows.Count; x++)
        {
          if (Flows[x].FileName == fileName)
          {
            FlowEngine.Log?.Write("Flow Removed - " + fileName);
            Flows.RemoveAt(x);
            break;
          }
        }
      }
    }


    public Settings GetSettings
    {
      get { return mSettings; }
    }

    public virtual void LoadSettings(string path, Dictionary<string, object> GlobalPluginValues)
    {

      mSettings.LoadSettingsFromFile(path, GlobalPluginValues);
    }

    /// <summary>
    /// Will initialize the plugin, read the settings, define the functions.
    /// </summary>
    public virtual void Init()
    {
      Name = GetType().Name;
    }

    /// <summary>
    /// Will start up the plugin for runtime use, this is called after Init, will only be called once when the Flow Engine first starts running
    /// </summary>
    public virtual void StartPlugin(Dictionary<string, object> GlobalPluginValues)
    {
      if (GlobalPluginValues.ContainsKey("log") == true)
      {
        mLog = GlobalPluginValues["log"] as ILog;
      }
    }

    /// <summary>
    /// The Designer will use this function to start some designing functionality, it was added specificly for the Database plugin so the SQL editor could get the table and field names.
    /// Didn't want to use the normal 'StartPlugin()' function since I don't want to have the HTTP plugin start listening on ports blocking the server from doing the same.
    /// </summary>
    /// <param name="GlobalPluginValues"></param>
    public virtual void StartPluginDesigner(Dictionary<string, object> GlobalPluginValues)
    {
    }

    public virtual void StopPlugin()
    {
    }



    public virtual void Dispose()
    {
    }

  }
}