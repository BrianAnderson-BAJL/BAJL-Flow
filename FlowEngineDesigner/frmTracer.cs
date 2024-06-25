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
  public partial class frmTracer : Form
  {
    public frmTracer()
    {
      InitializeComponent();
    }

    private void frmTracer_Load(object sender, EventArgs e)
    {
      cEventManager.TracerHandler += CEventManager_Tracer;
    }

    private void CEventManager_Tracer(object? sender, TracerEventArgs e)
    {
      if (sender is null)
      {
      }
      else
      {
        string name = sender.ToString()!;
        cEventManager.SENDER senderType = (cEventManager.SENDER)sender;
        if (senderType == cEventManager.SENDER.FlowDebug)
        {
          string tempXml = e.XmlData;
          tempXml = Xml.GetXMLChunk(ref tempXml, "Trace");
          name += " - " + Xml.GetXMLChunk(ref tempXml, "FileName");
        }
        ListViewItem lvi = lvTracer.Items.Add(name);
        lvi.SubItems.Add(e.Trace);
        lvi.SubItems.Add(FlowEngineCore.Global.ConvertToString(e.Ticks));
        lvi.Tag = e.XmlData;
        if (e.TracerType == cEventManager.TRACER_TYPE.Error)
        {
          lvi.ForeColor = Color.Red;
        }
        else if (e.TracerType == cEventManager.TRACER_TYPE.Warning)
        {
          lvi.ForeColor = Color.Purple;
        }
      }
    }

    private void tsbClear_Click(object sender, EventArgs e)
    {
      lvTracer.Items.Clear();
    }




    /// <summary>
    /// I use this to activate the form if it isn't the active window. If this isn't there it will take 2 clicks to activate a menu or toolstrip button, makes it rather annoying.
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
      const int WM_PARENTNOTIFY = 0x0210;
      if (this.Focused == false && m.Msg == WM_PARENTNOTIFY)
      {
        // Make this form auto-grab the focus when menu/controls are clicked
        this.Activate();
      }
      base.WndProc(ref m);
    }

    private void lvTracer_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (lvTracer.SelectedItems.Count == 0)
        return;

      if (lvTracer.SelectedItems[0].Text.StartsWith("FlowDebug") == true)
      {
        frmTraceDetails f = new frmTraceDetails();
        f.txtXml.Text = lvTracer.SelectedItems[0].Tag.ToString();
        f.Show();
      }
    }

  }
}
