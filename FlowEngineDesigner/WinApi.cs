using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineDesigner
{
  internal class WinApi
  {
    private const int EM_SETTABSTOPS = 0x00CB;

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

    public static void SetTabWidth(TextBox textbox, int tabWidth)
    {
      SendMessage(textbox.Handle, EM_SETTABSTOPS, 1, new int[] { tabWidth * 4 });
    }
  }
}
