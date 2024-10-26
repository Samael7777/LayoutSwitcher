using System.Runtime.InteropServices;

namespace LayoutSwitcher.Control.PInvoke;

internal static class Imm32
{
	[DllImport("imm32.dll", EntryPoint="ImmGetDefaultIMEWnd")]
	public static extern  IntPtr ImmGetDefaultIMEWnd(IntPtr param0) ;
}