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

    [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, ref PARAFORMAT2 lParam);

    public static void SetTabWidth(TextBox textbox, int tabWidth)
    {
      SendMessage(textbox.Handle, EM_SETTABSTOPS, 1, new int[] { tabWidth * 4 });
    }


    public static void SetLineSpacing(RichTextBox rtb, int LineSpacing)
    {
      PARAFORMAT2 format = new PARAFORMAT2();
      format.cbSize = Marshal.SizeOf(format);
      format.dwMask = PFM_LINESPACING;
      format.dyLineSpacing = LineSpacing;
      format.bLineSpacingRule = 4;

      SendMessage(rtb.Handle, EM_SETPARAFORMAT, SCF_SELECTION, ref format);
    }


    private const int SCF_SELECTION = 1;
    public const int PFM_LINESPACING = 256;
    public const int EM_SETPARAFORMAT = 1095;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PARAFORMAT2
    {
      public int cbSize;
      public uint dwMask;
      public Int16 wNumbering;
      public Int16 wReserved;
      public int dxStartIndent;
      public int dxRightIndent;
      public int dxOffset;
      public Int16 wAlignment;
      public Int16 cTabCount;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
      public int[] rgxTabs;
      public int dySpaceBefore;
      public int dySpaceAfter;
      public int dyLineSpacing;
      public Int16 sStyle;
      public byte bLineSpacingRule;
      public byte bOutlineLevel;
      public Int16 wShadingWeight;
      public Int16 wShadingStyle;
      public Int16 wNumberingStart;
      public Int16 wNumberingStyle;
      public Int16 wNumberingTab;
      public Int16 wBorderSpace;
      public Int16 wBorderWidth;
      public Int16 wBorders;
    }
  }
}
