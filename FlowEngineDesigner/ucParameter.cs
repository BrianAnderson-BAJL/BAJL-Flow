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
  public partial class ucParameter : UserControl
  {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.  IS SET ALWAYS IN CHILD CLASSES
    protected PARM_VAR mParmVar;
    protected cFlowWrapper mFlow;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected FunctionStep mStep;


    public ucParameter()
    {
      InitializeComponent();
    }

    public ucParameter(PARM_VAR parmVar, FunctionStep step, cFlowWrapper flow)
    {
      InitializeComponent();
      mParmVar = parmVar;
      mFlow = flow;
      mStep = step;
    }

    private void ucParameter_Load(object sender, EventArgs e)
    {

    }

    public virtual void UpdateValues()
    {
    }

    public PARM_VAR PARM_VAR
    {
      get { return mParmVar; }
    }

  }
}
