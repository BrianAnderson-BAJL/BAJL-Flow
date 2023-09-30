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
  public partial class ucParameterString : ucParameter
  {
    private PARM_Various mParm;
    private cFlowWrapper mFlow;
    private FunctionStep? mStep;
    public ucParameterString(PARM_Various parm, FunctionStep? step, cFlowWrapper flow)
    {
      InitializeComponent();
      mParm = parm;
      mFlow = flow;
      mStep = step;
      txtKey.Text = mParm.Name;
      txtDataType.Text = mParm.DataType.ToString();
      txtValue.Text = mParm.Value;
      if (mParm.ParmLiteral == PARM.PARM_L_OR_V.Variable)
      {
        chkVariable.Checked = true;
      }
    }


    public override void UpdateValues()
    {
      mParm.Value = txtValue.Text;
    }

    private void ucParameterString_Load(object sender, EventArgs e)
    {

    }

    private void btnSelectVariable_Click(object sender, EventArgs e)
    {
      frmVariableSelection f = new frmVariableSelection(mParm, mStep, mFlow);
      if (f.ShowDialog() == DialogResult.OK)
      {
        txtValue.Text = mParm.Value;
        chkVariable.Checked = true;
      }
    }

    private void chkVariable_CheckedChanged(object sender, EventArgs e)
    {
      if (chkVariable.Checked)
      {
        mParm.ParmLiteral = PARM.PARM_L_OR_V.Variable;
      }
      else
      {
        mParm.ParmLiteral = PARM.PARM_L_OR_V.Literal;
      }
    }
  }
}
