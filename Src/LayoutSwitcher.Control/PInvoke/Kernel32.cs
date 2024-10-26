// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Runtime.InteropServices;

namespace LayoutSwitcher.Control.PInvoke;

internal static class Kernel32
{
    // ReSharper disable once InconsistentNaming
    public const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern SafeLibraryHandle LoadLibraryExW([In] [MarshalAs(UnmanagedType.LPWStr)] string libFilename,
        IntPtr hFile, [MarshalAs(UnmanagedType.I4)] int dwFlags);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern  bool FreeLibrary(IntPtr hLibModule);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr handle);
}