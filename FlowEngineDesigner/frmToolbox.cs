using FlowEngineCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowEngineDesigner
{
  public partial class frmToolbox : Form
  {
    public frmToolbox()
    {
      InitializeComponent();
    }

    private void frmToolbox_Load(object sender, EventArgs e)
    {


      if (cPluginManagerWrapper.PluginsLoaded == true)
      {
        LoadToolbox();
      }
    }


    private void LoadToolbox()
    {
      tvToolbox.Nodes.Clear();
      for (int x = 0; x < FlowEngineCore.PluginManager.Plugins.Count; x++)
      {
        TreeNode tn = tvToolbox.Nodes.Add(FlowEngineCore.PluginManager.Plugins[x].Name);
        tn.Tag = FlowEngineCore.PluginManager.Plugins[x];
        for (int y = 0; y < FlowEngineCore.PluginManager.Plugins[x].Functions.Count; y++)
        {
          TreeNode SubNode = tn.Nodes.Add(FlowEngineCore.PluginManager.Plugins[x].Functions[y].Name);
          SubNode.Tag = FlowEngineCore.PluginManager.Plugins[x].Functions[y];

        }
      }

    }

    private void tvToolbox_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        tvToolbox.SelectedNode = e.Node;
        if (e.Node.Tag is not null)
        {
          FlowEngineCore.Plugin? Pin = e.Node.Tag as FlowEngineCore.Plugin;
          if (Pin is not null && Pin.GetSettings.SettingsList.Count > 0)
          {
            cmsPopup.Show(MousePosition);
          }
        }
      }
      else if (e.Button == MouseButtons.Left)
      {
        //tvToolbox.SelectedNode = e.Node;
        //DoDragDrop(e.Node.Tag, DragDropEffects.Copy);
      }
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (tvToolbox.SelectedNode is not null)
      {
        FlowEngineCore.Plugin? plugin = tvToolbox.SelectedNode.Tag as FlowEngineCore.Plugin;
        if (plugin is not null)
        {
          frmSettings f = new frmSettings(plugin.GetSettings, $"Plugin Settings [{plugin.Name}]");
          f.Show();
        }
      }
    }

    private void tvToolbox_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        TreeViewHitTestInfo tvi = tvToolbox.HitTest(new Point(e.X, e.Y));
        if (tvi.Node is not null && tvi.Node.Tag is FlowEngineCore.Function)
        {
          tvToolbox.SelectedNode = tvi.Node;
          DoDragDrop(tvi.Node.Tag, DragDropEffects.Copy);
        }
      }
    }


  }
}
