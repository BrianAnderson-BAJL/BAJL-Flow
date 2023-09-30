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
      cEventManager.Tracer += CEventManager_Tracer;
    }

    private void CEventManager_Tracer(object? sender, TracerEventArgs e)
    {
      if (sender == null)
      {
      }
      else
      {
        ListViewItem lvi = lvTracer.Items.Add(sender.ToString());
        lvi.SubItems.Add(e.Trace);
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

    private void frmTracer_MouseEnter(object sender, EventArgs e)
    {
      if (cOptions.FocusOnMouseEnter == true)
      {
        this.Focus();
      }
    }

    private void toolStrip1_MouseEnter(object sender, EventArgs e)
    {
      if (cOptions.FocusOnMouseEnter == true)
      {
        this.Focus();
      }
    }
  }
}
