using System.Runtime.InteropServices;

namespace LayoutSwitcher.Control.PInvoke;

public class SafeProcessHandle : SafeHandle
{
    public SafeProcessHandle() : base(IntPtr.Zero, true)
    { }

    protected override bool ReleaseHandle()
    {
        return IsInvalid || Kernel32.CloseHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;
}