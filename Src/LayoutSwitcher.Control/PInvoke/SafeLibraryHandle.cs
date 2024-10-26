using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LayoutSwitcher.Control.PInvoke;

[DebuggerDisplay("{handle.ToInt64()}")]
internal class SafeLibraryHandle : SafeHandle
{
    public SafeLibraryHandle() : base(IntPtr.Zero, true)
    { }

    protected override bool ReleaseHandle()
    {
        return IsInvalid || Kernel32.FreeLibrary(handle);
    }

    public override bool IsInvalid => handle == IntPtr.Zero;
}