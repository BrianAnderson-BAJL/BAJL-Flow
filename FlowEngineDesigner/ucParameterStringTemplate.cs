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
  public partial class ucParameterStringTemplate : ucParameter
  {

    public ucParameterStringTemplate(PARM_VAR parmVar, FunctionStep step, cFlowWrapper flow) : base(parmVar, step, flow)
    {
      InitializeComponent();
      txtKey.Text = mParmVar.Parm.Name;
      txtDataType.Text = mParmVar.Parm.DataType.ToString() + "-" + mParmVar.Parm.StringSubType.ToString();
      mParmVar.GetValue(out string val);
      txtValue.Text = val;
    }

    private void btnFile_Click(object sender, EventArgs e)
    {
      frmAdministrationFile f = new frmAdministrationFile(FILE_MODE.Open, mFlow, File_Callback, frmAdministrationFile.FILE_TYPE.Template);
      f.Show();

    }

    private void File_Callback(cFlowWrapper flowWrapper, string filename)
    {
      txtValue.Text = filename;
    }


    public override void UpdateValues()
    {
      if (mIsVariable)
      {
        mParmVar.SetVarRef(txtValue.Text);
      }
      else
      {
        mParmVar.SetVariableLiteral(txtValue.Text);
      }
    }

    private void btnSelectVariable_Click(object sender, EventArgs e)
    {
      frmVariableSelection f = new frmVariableSelection(mParmVar, mStep, mFlow);
      f.SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
      if (f.ShowDialog() == DialogResult.OK)
      {
        mParmVar.GetValue(out string val);
        txtValue.Text = val;
        mIsVariable = true;
      }

    }
  }
}
