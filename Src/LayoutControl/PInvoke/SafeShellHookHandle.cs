using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

[DebuggerDisplay("{handle.ToInt32()}")]
internal class SafeShellHookHandle : SafeHandle
{
    public SafeShellHookHandle() : base(IntPtr.Zero, true)
    { }
	
    protected override bool ReleaseHandle()
    {
        return IsInvalid || User32.UnhookWindowsHookEx(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

}