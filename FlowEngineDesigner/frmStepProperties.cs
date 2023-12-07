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
    private const int StartX = 28;
    private const int StartY = 20;
    private const int YSpaceBetweenControls = 5;
    private const int WidthSubtract = 30;
    private int ScrollBarWidthSubtract = 0;
    private const int ucHeight = 29;
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
      if (mStep.Function.Validators.Validators.Count > 0)
      {
        cmbValidators.Items.Add(" - None -");
        for (int x = 0; x < mStep.Function.Validators.Validators.Count; x++)
        {
          cmbValidators.Items.Add(mStep.Function.Validators.Validators[x]);
        }
        cmbValidators.Enabled = true;
        if (mStep.Validator is not null)
          Global.ComboBoxSetIndex(cmbValidators, mStep.Validator.Name);
        else
          cmbValidators.SelectedIndex = 0;
      }
      else
      {
        lblValidator.Text = "Validator - None available";
      }
      if (mStep.Function.Parms.Count <= 0)
      {
        btnAddParameter.Visible = false;
      }
      
      for (int x = 0; x < mStep.ParmVars.Count; x++)
      {

        PARM_VAR pv = mStep.ParmVars[x];

        ucParameter? uc = CreateParameterInput(pv);
        if (uc is not null)
        {
          uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          uc.Location = new Point(StartX, StartY + (uc.Size.Height * x) + YSpaceBetweenControls);
          uc.Size = new Size(gbPanel.Width - WidthSubtract, uc.Size.Height);
          gbPanel.Controls.Add(uc);
          userControls.Add(uc);
        }
        if (uc is not null && pv.Parm.AllowMultiple == PARM.PARM_ALLOW_MULTIPLE.Multiple)
        {
          CreateDeleteButtonForMultiParam(uc, x);
        }

        if (uc is not null && pv.Parm.AllowMultiple == PARM.PARM_ALLOW_MULTIPLE.Multiple && x == mStep.ParmVars.Count - 1)
        {
          btnAddParameter.Visible = true;
          btnAddParameter.Location = new Point(StartX, StartY + (uc.Size.Height * (x + 1)) + YSpaceBetweenControls);
        }
        else
        {
          btnAddParameter.Visible = false;
        }
      }
      if (mStep.ParmVars.Count < mStep.Function.Parms.Count) //If a user has deleted the last 'multiple' parm_var then we need to have the 'Add Parameter' button visible
      {
        btnAddParameter.Visible = true;
        btnAddParameter.Location = new Point(StartX, StartY + (ucHeight * mStep.ParmVars.Count) + YSpaceBetweenControls);
      }

    }

    private void CreateDeleteButtonForMultiParam(ucParameter uc, int index)
    {
      Button btnDelete = new Button();
      btnDelete.Text = btnDeleteParameter.Text;
      btnDelete.Location = Global.Add(new Point(StartX - btnDeleteParameter.Width, StartY + (uc.Size.Height * index) + 8), gbPanel.AutoScrollPosition);
      btnDelete.Size = btnDeleteParameter.Size;
      btnDelete.Tag = uc;
      btnDelete.Click += btnDeleteParameter_Click!;
      gbPanel.Controls.Add(btnDelete);
    }

    private ucParameter? CreateParameterInput(PARM_VAR pv)
    {
      ucParameter? uc = null;
      if (pv.Parm.DataType == DATA_TYPE.Integer)
      {
        uc = new ucParameterInteger(pv, mStep, mFlow);
      }
      else if ((pv.Parm.DataType == DATA_TYPE.String || pv.Parm.DataType == DATA_TYPE.Object) && pv.Parm.StringSubType == STRING_SUB_TYPE._None)
      {
        uc = new ucParameterString(pv, mStep, mFlow);
      }
      else if (pv.Parm.DataType == DATA_TYPE.String && pv.Parm.StringSubType == STRING_SUB_TYPE.Sql)
      {
        uc = new ucParameterStringSql(pv, mStep, mFlow);
      }
      else if (pv.Parm.DataType == DATA_TYPE.String && pv.Parm.StringSubType == STRING_SUB_TYPE.DropDownList)
      {
        uc = new ucParameterDropDownList(pv);
      }
      else if (pv.Parm.DataType == DATA_TYPE.Various)
      {
        uc = new ucParameterVarious(pv, mStep, mFlow);
      }
      else if (pv.Parm.DataType == DATA_TYPE.Decimal)
      {

      }
      return uc;
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      for (int x = 0; x < userControls.Count; x++)
      {
        ucParameter? uc = userControls[x] as ucParameter;
        if (uc is not null)
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

    private int FindNextParmVarNumber()
    {
      int largest = 0;
      for (int x = 0; x < mStep.ParmVars.Count; x++)
      {
        if (mStep.ParmVars[x].Parm.AllowMultiple == PARM.PARM_ALLOW_MULTIPLE.Multiple)
        {
          int? val = mStep.ParmVars[x].ParmName.FindEndingNumber();
          if (val.HasValue && val > largest)
            largest = val.Value;
        }
      }
      return ++largest;
    }

    private int FindMultipleParmVarCount()
    {
      int count = 0;
      for (int x = 0; x < mStep.ParmVars.Count; x++)
      {
        if (mStep.ParmVars[x].Parm.AllowMultiple == PARM.PARM_ALLOW_MULTIPLE.Multiple)
        {
          count++;
        }
      }
      return count;
    }

    private void btnAddParameter_Click(object sender, EventArgs e)
    {
      PARM_VAR parmVar;
      if (mStep.Function.Parms.Count <= 0)
        return;
      PARM parm = mStep.Function.Parms[mStep.Function.Parms.Count - 1];
      parmVar = parm.ToParmVar();
      if (parmVar.Parm.NameChangeIncrement == true && parmVar.Parm.NameChangeable == true)
      {
        parmVar.ParmName = parmVar.Parm.Name + FindNextParmVarNumber();
      }
      mStep.ParmVars.Add(parmVar);
      ucParameter? uc = CreateParameterInput(parmVar);
      if (uc is not null)
      {

        uc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        uc.Location = Global.Add(new Point(StartX, StartY + (uc.Size.Height * userControls.Count) + YSpaceBetweenControls), gbPanel.AutoScrollPosition);
        uc.Size = new Size(gbPanel.Width - WidthSubtract - ScrollBarWidthSubtract, uc.Size.Height);
        CreateDeleteButtonForMultiParam(uc, userControls.Count);
        gbPanel.Controls.Add(uc);
        userControls.Add(uc);
        btnAddParameter.Location = Global.Add(new Point(StartX, StartY + (uc.Size.Height * userControls.Count) + YSpaceBetweenControls), gbPanel.AutoScrollPosition);
      }
    }

    private void cmbValidators_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cmbValidators.SelectedIndex > 0)
      {
        Core.FunctionValidator? validator = cmbValidators.SelectedItem as Core.FunctionValidator;
        mStep.Validator = validator;
      }
      else
      {
        mStep.Validator = null;
      }
    }

    private void btnDeleteParameter_Click(object sender, EventArgs e)
    {
      //if (FindMultipleParmVarCount() == 1)
      //{
        //MessageBox.Show("Are you sure you want to delete the last 'Multiple' parameter? You will not be able to add a new one afterwards
      //}

      Button? btn = sender as Button;
      if (btn is null)
        return;

      ucParameter? param = btn.Tag as ucParameter;
      if (param is null)
        return;

      PARM_VAR parmVar = param.PARM_VAR;
      gbPanel.Controls.Remove(param);
      gbPanel.Controls.Remove(btn);
      userControls.Remove(param);
      mStep.ParmVars.Remove(parmVar);

      for (int x = 0; x < gbPanel.Controls.Count; x++)
      {
        Control uc = gbPanel.Controls[x];
        if (uc.Location.Y > param.Location.Y)
        {
          uc.Location = new Point(uc.Location.X, uc.Location.Y - param.Size.Height); // - YSpaceBetweenControls
        }
      }
    }

    private void gbPanel_Paint(object sender, PaintEventArgs e)
    {
      if (gbPanel.VerticalScroll.Visible == true)
      {
        ScrollBarWidthSubtract = System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
      }
      else
      {
        ScrollBarWidthSubtract = 0;
      }
    }
  }
}
