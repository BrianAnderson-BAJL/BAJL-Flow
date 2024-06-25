using FlowEngineCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public class FlowManager
  {
    private static ILog? mLog = null;
    private Dictionary<string, List<Flow>> FlowsByPlugin = new();

    /// <summary>
    /// Loading flows must be performed after the plugins are loaded
    /// </summary>
    /// <param name="fullPath"></param>
    public static void Load(string fullPath, Dictionary<string, object> GlobalPluginValues)
    {
      if (GlobalPluginValues.ContainsKey("log") == true)
      {
        mLog = GlobalPluginValues["log"] as ILog;
      }


      List<string> errorMessages = new List<string>(32);
      string[] files = Directory.GetFiles(fullPath, "*.flow");
      List<Flow> Flows = new List<Flow>(files.Length);
      for (int x = 0; x < files.Length; x++)
      {
        Flow flow = new Flow();
        try
        {
          flow.XmlReadFile(files[x]);
          flow.PrepareFlowForTracing(); //If it isn't development, it will just return

        }
        catch (Exception ex)
        {
          mLog?.Write($"Failed to Load Flow [{files[x]}] - {ex.Message}", LOG_TYPE.WAR);
        }
        if (flow.StartPlugin is not null) //If there is no StartPlugin assigned to the flow, then the flow isn't kept in memory, it gets dropped, Flow engine doesn't want flows with no plugin, if you can't start it, no point to keep it
        {
          flow.StartPlugin.FlowAdd(flow);
        }
        else
        {
          errorMessages.Add($"Bad Flow, no start plugin defined [{flow.ToString()}]");
        }
      }

      if (Options.GetSettings.SettingGetAsBoolean("FlowPathAllowSubDirectories") == true)
      {
        string[] dirs = Directory.GetDirectories(fullPath);
        for (int x = 0; x < dirs.Length; x++)
        {
          Load(dirs[x], GlobalPluginValues);
        }
      }
      for (int x = 0; x < errorMessages.Count; x++)
      {
        mLog?.Write(errorMessages[x], LOG_TYPE.WAR);
      }
    }

    public static void LoadSingleFlow(string fullPath)
    {
      Flow flow = new Flow();
      flow.XmlReadFile(fullPath);
      if (flow.StartPlugin is not null) //If there is no StartPlugin assigned to the flow, then the flow isn't kept in memory, it gets dropped, Flow engine doesn't want flows with no plugin, if you can't start it, no point to keep it
      {
        flow.PrepareFlowForTracing(); //If it isn't development, it will just return
        flow.StartPlugin.FlowAdd(flow);
        mLog?.Write("Loaded Flow - " + flow.ToString());
      }
      else
      {
        mLog?.Write("Unable to load Flow, no start plugin defined - " + flow.ToString(), LOG_TYPE.WAR);
      }
    }

    //public static void LoadSingleFlowFromXml(string xml)
    //{
    //  Flow flow = new Flow();
    //  flow.XmlRead(ref xml); //just parse the passed XML, no need to load from file
    //  if (flow.StartPlugin is not null) //If there is no StartPlugin assigned to the flow, then the flow isn't kept in memory, it gets dropped, Flow engine doesn't want flows with no plugin, if you can't start it, no point to keep it
    //  {
    //    flow.StartPlugin.FlowAdd(flow);
    //    Global.Write("Loaded Flow - " + flow.ToString());
    //  }
    //  else
    //  {
    //    Global.Write("Unable to load Flow, no start plugin defined - " + flow.ToString(), DEBUG_TYPE.Error);
    //  }
    //}
  }
}
