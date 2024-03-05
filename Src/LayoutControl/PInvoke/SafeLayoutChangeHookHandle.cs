using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

[DebuggerDisplay("{handle.ToInt64()}")]
internal class SafeLayoutChangeHookHandle : SafeHandle
{
    public SafeLayoutChangeHookHandle() : base(IntPtr.Zero, true)
    { }

    protected override bool ReleaseHandle()
    {
        return IsInvalid || User32.UnhookWindowsHookEx(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;
}