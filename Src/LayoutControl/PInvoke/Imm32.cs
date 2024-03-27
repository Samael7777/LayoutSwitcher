using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

internal static class Imm32
{
	[DllImport("imm32.dll", EntryPoint="ImmGetDefaultIMEWnd")]
	public static extern  IntPtr ImmGetDefaultIMEWnd(IntPtr param0) ;
}