using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class FlowManager
  {
    private Dictionary<string, List<Flow>> FlowsByPlugin = new Dictionary<string, List<Flow>>();
    public static void Load(string fullPath)
    {
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
          Global.Write($"Failed to Load Flow [{files[x]}] - {ex.Message}", LOG_TYPE.WAR);
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

      if (Options.FlowPathAllowSubDirectories == true)
      {
        string[] dirs = Directory.GetDirectories(fullPath);
        for (int x = 0; x < dirs.Length; x++)
        {
          Load(dirs[x]);
        }
      }
      for (int x = 0; x < errorMessages.Count; x++)
      {
        Global.Write(errorMessages[x], LOG_TYPE.WAR);
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
        Global.Write("Loaded Flow - " + flow.ToString());
      }
      else
      {
        Global.Write("Unable to load Flow, no start plugin defined - " + flow.ToString(), LOG_TYPE.WAR);
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
