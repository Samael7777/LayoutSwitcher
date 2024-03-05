using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

public class SafeProcHandle : SafeHandle
{
    public SafeProcHandle() : base(IntPtr.Zero, true)
    { }

    protected override bool ReleaseHandle()
    {
        return IsInvalid || Kernel32.CloseHandle(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;
}