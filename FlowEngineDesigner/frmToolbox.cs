using Core;
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
      for (int x = 0; x < Core.PluginManager.Plugins.Count; x++)
      {
        TreeNode tn = tvToolbox.Nodes.Add(Core.PluginManager.Plugins[x].Name);
        tn.Tag = Core.PluginManager.Plugins[x];
        for (int y = 0; y < Core.PluginManager.Plugins[x].Functions.Count; y++)
        {
          TreeNode SubNode = tn.Nodes.Add(Core.PluginManager.Plugins[x].Functions[y].Name);
          SubNode.Tag = Core.PluginManager.Plugins[x].Functions[y];

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
          Core.Plugin? Pin = e.Node.Tag as Core.Plugin;
          if (Pin is not null && Pin.Settings.Count > 0)
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
        Core.Plugin? plugin = tvToolbox.SelectedNode.Tag as Core.Plugin;
        if (plugin is not null)
        {
          frmSettings f = new frmSettings(plugin);
          f.Show();
        }
      }
    }

    private void tvToolbox_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        TreeViewHitTestInfo tvi = tvToolbox.HitTest(new Point(e.X, e.Y));
        if (tvi.Node is not null && tvi.Node.Tag is Core.Function)
        {
          tvToolbox.SelectedNode = tvi.Node;
          DoDragDrop(tvi.Node.Tag, DragDropEffects.Copy);
        }
      }
    }


  }
}
