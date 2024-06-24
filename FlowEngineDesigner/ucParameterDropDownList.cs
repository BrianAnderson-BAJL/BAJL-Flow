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
  public partial class ucParameterDropDownList : ucParameter
  {
    //public ucParameterDropDownList(FlowEngineCore.PARM_VAR parmVar)
    //{
    //  ParmVar = parmVar;
    //  InitializeComponent();
    //}
    public ucParameterDropDownList(FlowEngineCore.PARM_VAR parmVar, FlowEngineCore.FunctionStep step, cFlowWrapper flow) : base(parmVar, step, flow)
    {
      InitializeComponent();
    }

    private void ucParameterDropDownList_Load(object sender, EventArgs e)
    {
      txtKey.Text = mParmVar.Parm.Name;
      txtDataType.Text = mParmVar.Parm.DataType.ToString();
      cmbItems.Items.Clear();
      if (mParmVar.Parm.Options is not null)
      {
        mParmVar.GetValue(out string currentVal);
        if (currentVal is null || currentVal == "")
          mParmVar.Parm.ValidatorGetValue(PARM.PARM_VALIDATION.StringDefaultValue, out currentVal);

        for (int x = 0; x < mParmVar.Parm.Options.Count; x++)
        {
          cmbItems.Items.Add(mParmVar.Parm.Options[x]);
          if (currentVal == mParmVar.Parm.Options[x])
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
        mParmVar.SetVariableLiteral(cmbItems.SelectedItem.ToString()!);
      }
    }

  }
}
