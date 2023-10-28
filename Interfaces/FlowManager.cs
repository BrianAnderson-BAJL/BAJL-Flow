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
    public static void LoadFlows(string fullPath)
    {
      string[] files = Directory.GetFiles(fullPath, "*.flow");
      List<Flow> Flows = new List<Flow>(files.Length);
      for (int x = 0; x < files.Length; x++)
      {
        Flow flow = new Flow();
        try
        {
          flow.XmlReadFile(files[x]);
        }
        catch (Exception ex)
        {
          Global.Write($"Failed to Load Flow [{files[x]}] - {ex.Message}", DEBUG_TYPE.Warning);
        }
        if (flow.StartPlugin != null) //If there is no StartPlugin assigned to the flow, then the flow isn't kept in memory, it gets dropped, Flow engine doesn't want flows with no plugin, if you can't start it, no point to keep it
        {
          flow.StartPlugin.FlowAdd(flow);
          Global.Write("Loaded Flow - " + flow.ToString());
        }
        else
        {
          Global.Write("Unable to load Flow, no start plugin defined - " + flow.ToString(), DEBUG_TYPE.Error);
        }
      }

      if (Options.FlowPathAllowSubDirectories == true)
      {
        string[] dirs = Directory.GetDirectories(fullPath);
        for (int x = 0; x < dirs.Length; x++)
        {
          LoadFlows(dirs[x]);
        }
      }
    }

    public static void LoadSingleFlow(string fullPath)
    {
      Flow flow = new Flow();
      flow.XmlReadFile(fullPath);
      if (flow.StartPlugin != null) //If there is no StartPlugin assigned to the flow, then the flow isn't kept in memory, it gets dropped, Flow engine doesn't want flows with no plugin, if you can't start it, no point to keep it
      {
        flow.StartPlugin.FlowAdd(flow);
        Global.Write("Loaded Flow - " + flow.ToString());
      }
      else
      {
        Global.Write("Unable to load Flow, no start plugin defined - " + flow.ToString(), DEBUG_TYPE.Error);
      }
    }

    public static void LoadSingleFlowFromXml(string xml)
    {
      Flow flow = new Flow();
      flow.XmlRead(ref xml); //just parse the passed XML, no need to load from file
      if (flow.StartPlugin != null) //If there is no StartPlugin assigned to the flow, then the flow isn't kept in memory, it gets dropped, Flow engine doesn't want flows with no plugin, if you can't start it, no point to keep it
      {
        flow.StartPlugin.FlowAdd(flow);
        Global.Write("Loaded Flow - " + flow.ToString());
      }
      else
      {
        Global.Write("Unable to load Flow, no start plugin defined - " + flow.ToString(), DEBUG_TYPE.Error);
      }
    }
  }
}
