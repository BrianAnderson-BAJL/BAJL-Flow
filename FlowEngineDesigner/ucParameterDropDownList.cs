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
  public partial class ucParameterDropDownList : ucParameter
  {
    Core.FunctionStep? Step;
    Core.Flow? Flow;
    Core.PARM_DropDownList Parm;
    public ucParameterDropDownList(Core.PARM_DropDownList parm)
    {
      Parm = parm;
      InitializeComponent();
    }
    public ucParameterDropDownList(Core.PARM_DropDownList parm, Core.FunctionStep step, Core.Flow flow)
    {
      Parm = parm;
      Step = step;
      Flow = flow;
      InitializeComponent();
    }

    private void ucParameterDropDownList_Load(object sender, EventArgs e)
    {
      txtKey.Text = Parm.Name;
      txtDataType.Text = Parm.DataType.ToString();
      cmbItems.Items.Clear();
      for (int x = 0; x < Parm.Options.Count; x++)
      {
        cmbItems.Items.Add(Parm.Options[x]);
        if (Parm.Value == Parm.Options[x])
        {
          cmbItems.SelectedIndex = x;
        }
      }
    }

    public override void UpdateValues()
    {
      if (cmbItems.SelectedItem != null)
      {
#pragma warning disable CS8601 // Possible null reference assignment. It seems like Visual studio is fucking up here, there is zero chance of it being null, but the warning won't go away, so I'm supressing it
        Parm.Value = cmbItems.SelectedItem.ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
      }
    }

  }
}
