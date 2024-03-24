using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
  public class PluginManager
  {
    private static List<Plugin> mPlugins = new List<Plugin>(0);

    public delegate void PluginsChangedHandler(List<Plugin> Plugins);
    public static event PluginsChangedHandler? OnPluginsChanged;
    public static bool PluginsLoaded = false;

    /// <summary>
    /// If Plugins are sharing data or connections with other plugins they will be stored in here. The Database connection is shared here.
    /// </summary>
    public static Dictionary<string, object> GlobalPluginValues = new Dictionary<string, object>();

    public static ReadOnlyCollection<Plugin> Plugins => mPlugins.AsReadOnly();

    public static void Load(string FullPath)
    {
      string[] Files = Directory.GetFiles(FullPath, "*.dll");
      mPlugins = new List<Plugin>(Files.Length);
      for (int x = 0; x < Files.Length; x++)
      {
        Assembly a = Assembly.LoadFile(Files[x]);
        Type[] Types = a.GetTypes();
        for (int y = 0; y < Types.Length; y++)
        {
          if (Types[y] is not null && Types[y].BaseType is not null && Types[y].BaseType!.FullName == "Core.Plugin")
          {
            Core.Plugin? p = Activator.CreateInstance(Types[y]) as Core.Plugin;
            if (p is not null)
            {
              p.Init();
              mPlugins.Add(p);
            }
          }
        }
      }
      PluginsLoaded = true;
      if (OnPluginsChanged is not null)
      {
        OnPluginsChanged(mPlugins);
      }
    }

    public static void StartPlugins()
    {
      List<Plugin> sortedByStartPriority = mPlugins.OrderBy(p => p.StartPriority).ToList();
      for (int x = 0; x < sortedByStartPriority.Count; x++)
      {
        Plugin plugin = sortedByStartPriority[x];
        plugin.StartPlugin(GlobalPluginValues);
      }
    }

    public static void StartPluginsDesigner()
    {
      List<Plugin> sortedByStartPriority = mPlugins.OrderBy(p => p.StartPriority).ToList();
      for (int x = 0; x < sortedByStartPriority.Count; x++)
      {
        Plugin plugin = sortedByStartPriority[x];
        plugin.StartPluginDesigner(GlobalPluginValues);
      }
    }


    public static void StopPlugins()
    {
      for (int x = 0; x < mPlugins.Count; x++)
      {
        mPlugins[x].StopPlugin();
      }
    }


    public static Plugin FindPluginByName(string name)
    {
      for (int x = 0; x < mPlugins.Count; x++)
      {
        if (name == mPlugins[x].Name)
        {
           return mPlugins[x];
        }
      }
      throw new ArgumentException("[" + name + "] plugin name is not valid");
    }

    public static Function FindFunctionByName(string pluginName, string functionName)
    {
      for (int x = 0; x < mPlugins.Count; x++)
      {
        if (pluginName == mPlugins[x].Name)
        {
          for (int y = 0; y < mPlugins[x].Functions.Count; y++)
          {
            if (functionName == mPlugins[x].Functions[y].Name)
            {
              return mPlugins[x].Functions[y];
            }
          }
        }
      }
      throw new ArgumentException("[" + pluginName + "." + functionName + "] is not valid");
    }
    public static Function FindFunctionByName(string name)
    {
      string[] stringsplit = name.Split('.');
      if (stringsplit.Length == 2)
      {
        string pluginName = stringsplit[0];
        string functionName = stringsplit[1];
        return FindFunctionByName(pluginName, functionName);
      }
      throw new ArgumentException("[" + name + "] is not a valid plugin.function name");
    }



  }
}
