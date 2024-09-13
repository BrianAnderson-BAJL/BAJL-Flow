using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore.Administration.Messages
{
  public class ServerPluginsGetResponse : BaseResponse
  {
    public class PluginData
    {
      public string Name = "";
      public string VersionAssembly = "";
      public string VersionFile = "";
    }

    private ReadOnlyCollection<Plugin> PluginsReadOnly;
    public List<PluginData> Plugins;
    /// <summary>
    /// Used when parsing a response from a client
    /// </summary>
    /// <param name="packet"></param>
    public ServerPluginsGetResponse(FlowEngineCore.Administration.Packet packet) : base(packet)
    {
      packet.GetData(out int Count);
      Plugins = new List<PluginData>(Count);
      for (int x = 0; x < Count; x++)
      {
        packet.GetData(out string name);
        packet.GetData(out string versionAssembly);
        packet.GetData(out string versionFile);

        PluginData plugin = new();
        plugin.Name = name;
        plugin.VersionAssembly = versionAssembly;
        plugin.VersionFile = versionFile;

        Plugins.Add(plugin);
      }
    }
    public ServerPluginsGetResponse(int packetId) : base(packetId, Packet.PACKET_TYPE.ServerPluginsGetResponse)
    {
      PluginsReadOnly = PluginManager.Plugins;
    }

    public override FlowEngineCore.Administration.Packet GetPacket()
    {
      Packet packet = base.GetPacket();
      packet.AddData(this.PluginsReadOnly.Count);  //Add the number of users
      for (int x = 0; x < PluginsReadOnly.Count; x++)
      {
        packet.AddData(PluginsReadOnly[x].Name);
        packet.AddData(PluginsReadOnly[x].VersionAssembly);
        packet.AddData(PluginsReadOnly[x].VersionFile);
      }
      return packet;
    }

  }
}
