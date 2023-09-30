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
      userControls = new List<UserControl>(mStep.parms.Count);

    }

    private void frmStepProperties_Load(object sender, EventArgs e)
    {
      txtSaveResponseName.Text = mStep.RespNames.Name;
      chkSaveResponse.Checked = mStep.SaveResponseVariable;
      txtType.Text = mStep.Name;
      for (int x = 0; x < mStep.parms.Count; x++)
      {

        PARM p = mStep.parms[x];

        ucParameter? uc = CreateParameterInput(p);
        if (uc != null)
        {
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + 5);
          uc.Size = new Size(gbParameters.Width - 20, uc.Size.Height);
          gbParameters.Controls.Add(uc);
          userControls.Add(uc);
        }

        if (uc is not null && p.AllowMultiple == PARM.PARM_ALLOW_MULTIPLE.Multiple && x == mStep.parms.Count - 1)
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

    private ucParameter? CreateParameterInput(PARM p)
    {
      PARM_Integer? ParmInt = p as PARM_Integer;
      ucParameter? uc = null;
      if (ParmInt != null)
      {
        uc = new ucParameterInteger(ParmInt);
      }
      PARM_Various? ParmStr = p as PARM_Various;
      if (ParmStr != null)
      {
        uc = new ucParameterString(ParmStr, mStep, mFlow);
      }
      PARM_Decimal? ParmDec = p as PARM_Decimal;
      if (ParmDec != null)
      {

      }
      PARM_DropDownList? ParmDdl = p as PARM_DropDownList;
      if (ParmDdl != null)
      {
        uc = new ucParameterDropDownList(ParmDdl);
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
      PARM p = mStep.parms[mStep.parms.Count - 1]; //Get the last parameter
      p = p.Clone();
      mStep.parms.Add(p);
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
