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
    Core.PARM_VAR ParmVar;
    public ucParameterDropDownList(Core.PARM_VAR parmVar)
    {
      ParmVar = parmVar;
      InitializeComponent();
    }
    public ucParameterDropDownList(Core.PARM_VAR parmVar, Core.FunctionStep step, Core.Flow flow)
    {
      ParmVar = parmVar;
      Step = step;
      Flow = flow;
      InitializeComponent();
    }

    private void ucParameterDropDownList_Load(object sender, EventArgs e)
    {
      txtKey.Text = ParmVar.Parm.Name;
      txtDataType.Text = ParmVar.Parm.DataType.ToString();
      cmbItems.Items.Clear();
      if (ParmVar.Parm.Options is not null)
      {
        ParmVar.GetValue(out string currentVal);
        for (int x = 0; x < ParmVar.Parm.Options.Count; x++)
        {
          cmbItems.Items.Add(ParmVar.Parm.Options[x]);
          if (currentVal == ParmVar.Parm.Options[x])
          {
            cmbItems.SelectedIndex = x;
          }
        }
      }
    }

    public override void UpdateValues()
    {
      if (cmbItems.SelectedItem is not null)
      {
        ParmVar.SetVariableLiteral(cmbItems.SelectedItem.ToString()!);
      }
    }

  }
}
