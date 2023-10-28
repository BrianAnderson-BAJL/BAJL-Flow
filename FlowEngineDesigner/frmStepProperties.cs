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
  public partial class frmStepProperties : Form
  {
    private Core.FunctionStep mStep;
    private cFlowWrapper mFlow;
    private List<UserControl> userControls;
    private const int StartX = 10;
    private const int StartY = 20;
    public frmStepProperties(Core.FunctionStep step, cFlowWrapper flow)
    {
      InitializeComponent();
      mStep = step;
      mFlow = flow;
      userControls = new List<UserControl>(mStep.ParmVars.Count);

    }

    private void frmStepProperties_Load(object sender, EventArgs e)
    {
      txtSaveResponseName.Text = mStep.RespNames.Name;
      chkSaveResponse.Checked = mStep.SaveResponseVariable;
      txtType.Text = mStep.Name;
      if (mStep.ParmVars.Count == 0)
      {
        btnAddParameter.Visible = false;
      }
      for (int x = 0; x < mStep.ParmVars.Count; x++)
      {

        PARM_VAR pv = mStep.ParmVars[x];

        ucParameter? uc = CreateParameterInput(pv);
        if (uc != null)
        {
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          uc.Size = new Size(gbParameters.Width - 20, uc.Size.Height);
          gbParameters.Controls.Add(uc);
          userControls.Add(uc);
        }

        if (uc is not null && pv.Parm.AllowMultiple == PARM.PARM_ALLOW_MULTIPLE.Multiple && x == mStep.ParmVars.Count - 1)
        {
          btnAddParameter.Visible = true;
          btnAddParameter.Location = new Point(StartX, StartY + (uc.Size.Height * (x + 1)) + 5);
        }
        else
        {
          btnAddParameter.Visible = false;
        }
      }
    }

    private ucParameter? CreateParameterInput(PARM_VAR pv)
    {
      ucParameter? uc = null;
      if (pv.Parm.DataType == DATA_TYPE.Integer)
      {
        uc = new ucParameterInteger(pv, mStep, mFlow);
      }
      else if (pv.Parm.DataType == DATA_TYPE.String || pv.Parm.DataType == DATA_TYPE.Object)
      {
        uc = new ucParameterString(pv, mStep, mFlow);
      }
      else if (pv.Parm.DataType == DATA_TYPE.Decimal)
      {

      }
      else if (pv.Parm.DataType == DATA_TYPE.DropDownList)
      {
        uc = new ucParameterDropDownList(pv);
      }
      return uc;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      for (int x = 0; x < userControls.Count; x++)
      {
        ucParameter? uc = userControls[x] as ucParameter;
        if (uc != null)
        {
          uc.UpdateValues();
        }
      }
      this.Close();
    }

    private void chkSaveResponse_CheckedChanged(object sender, EventArgs e)
    {
      txtSaveResponseName.Enabled = chkSaveResponse.Checked;
      if (txtSaveResponseName.Enabled && txtSaveResponseName.Text == "")
      {
        txtSaveResponseName.Text = mFlow.GetUnusedVariableName(mStep.Function.RespNames.Name);
      }
      mStep.RespNames.Name = txtSaveResponseName.Text;
      mStep.SaveResponseVariable = chkSaveResponse.Checked;
    }

    private void txtSaveResponseName_TextChanged(object sender, EventArgs e)
    {
      mStep.RespNames.Name = txtSaveResponseName.Text;
    }

    private void btnAddParameter_Click(object sender, EventArgs e)
    {
      PARM_VAR p = mStep.ParmVars[mStep.ParmVars.Count - 1]; //Get the last parameter
      p = p.Clone();
      mStep.ParmVars.Add(p);
      ucParameter? uc = CreateParameterInput(p);
      if (uc != null)
      {
        uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        uc.Location = new Point(StartX, StartY + (uc.Size.Height * userControls.Count) + 5);
        uc.Size = new Size(gbParameters.Width - 20, uc.Size.Height);
        gbParameters.Controls.Add(uc);
        userControls.Add(uc);
        btnAddParameter.Location = new Point(StartX, StartY + (uc.Size.Height * userControls.Count) + 5);
      }

    }
  }
}
