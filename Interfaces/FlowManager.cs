using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class FlowManager
  {
    private Dictionary<string, List<Flow>> FlowsByPlugin = new Dictionary<string, List<Flow>>();
    public static void LoadFlows(string FullPath)
    {
      string[] Files = Directory.GetFiles(FullPath, "*.flow");
      List<Flow> Flows = new List<Flow>(Files.Length);
      for (int x = 0; x < Files.Length; x++)
      {
        Flow F = new Flow();
        F.XmlRead(Files[x]);
        if (F.StartPlugin != null) //If there is no StartPlugin assigned to the flow, then the flow isn't kept in memory, it gets dropped, Flow engine doesn't want flows with no plugin, if you can't start it, no point to keep it
        {
          F.StartPlugin.Flows.Add(F);
          Global.Write("Loaded Flow - " + F.ToString());
        }
      }
    }
  }
}
