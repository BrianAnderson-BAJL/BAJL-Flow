using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class cLayoutForm
  {
    internal enum LAYOUT_FORM
    {
      Main,
      Toolbox,
      Flow,
      Tracer,
    }

    internal LAYOUT_FORM layoutForm;
    internal bool open;
    internal System.Windows.Forms.FormWindowState state = FormWindowState.Normal;
    internal Vector2 size;
    internal Vector2 position;
  }
}
