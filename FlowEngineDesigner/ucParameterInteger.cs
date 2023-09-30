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
  public partial class ucParameterInteger : ucParameter
  {
    private PARM_Integer mParm;
    public ucParameterInteger(PARM_Integer parm)
    {
      InitializeComponent();
      mParm = parm;
      txtKey.Text = mParm.Name;
      txtDataType.Text = mParm.DataType.ToString();
      nudValue.Minimum = mParm.Min;
      nudValue.Maximum = mParm.Max;
      nudValue.Increment = mParm.Increment;
      nudValue.Value = mParm.Value;
    }

    private void ucParameterInteger_Load(object sender, EventArgs e)
    {

    }
    public override void UpdateValues()
    {
      mParm.Value = (long)nudValue.Value;
    }
  }
}
