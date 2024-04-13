// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

internal static class Kernel32
{
    // ReSharper disable once InconsistentNaming
    public const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern SafeLibraryHandle LoadLibraryExW([In] [MarshalAs(UnmanagedType.LPWStr)] string libFilename,
        IntPtr hFile, [MarshalAs(UnmanagedType.I4)] int dwFlags);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern SafeLibraryHandle LoadLibrary([In]string lpLibFileName) ;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, [In][MarshalAs(UnmanagedType.LPStr)]string lpProcName);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern  bool FreeLibrary(IntPtr hLibModule);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr handle);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern SafeProcessHandle OpenProcess(ACCESS_MASK dwDesiredAccess, 
        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool IsWow64Process2([In] SafeProcessHandle hProcess, 
        out IMAGE_FILE_MACHINE pProcessMachine, out IMAGE_FILE_MACHINE pNativeMachine);

    
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process(SafeProcessHandle hProcess, 
        [MarshalAs(UnmanagedType.Bool)] out bool isWow64Process) ;

}

[Flags]
internal enum ACCESS_MASK : uint
{
    DELETE = 0x00010000,
    READ_CONTROL = 0x00020000,
    WRITE_DAC = 0x00040000,
    WRITE_OWNER = 0x00080000,
    SYNCHRONIZE = 0x00100000,
    STANDARD_RIGHTS_REQUIRED = 0x000F0000,
    STANDARD_RIGHTS_READ = 0x00020000,
    STANDARD_RIGHTS_WRITE = 0x00020000,
    STANDARD_RIGHTS_EXECUTE = 0x00020000,
    STANDARD_RIGHTS_ALL = 0x001F0000,
    SPECIFIC_RIGHTS_ALL = 0x0000FFFF,
    ACCESS_SYSTEM_SECURITY = 0x01000000,
    MAXIMUM_ALLOWED = 0x02000000,
    GENERIC_READ = 0x80000000,
    GENERIC_WRITE = 0x40000000,
    GENERIC_EXECUTE = 0x20000000,
    GENERIC_ALL = 0x10000000
}

internal enum IMAGE_FILE_MACHINE : ushort
{
    /// <summary>Unknown</summary>
    IMAGE_FILE_MACHINE_UNKNOWN = 0,

    /// <summary>
    ///     Interacts with the host and not a WOW64 guest.
    ///     <note>
    ///         This constant is available starting with Windows 10, version 1607 and
    ///         Windows Server 2016.
    ///     </note>
    /// </summary>
    IMAGE_FILE_MACHINE_TARGET_HOST = 0x0001,

    /// <summary>Intel 386</summary>
    IMAGE_FILE_MACHINE_I386 = 0x014c,

    /// <summary>MIPS little-endian, 0x160 big-endian</summary>
    IMAGE_FILE_MACHINE_R3000 = 0x0162,

    /// <summary>MIPS little-endian</summary>
    IMAGE_FILE_MACHINE_R4000 = 0x0166,

    /// <summary>MIPS little-endian</summary>
    IMAGE_FILE_MACHINE_R10000 = 0x0168,

    /// <summary>MIPS little-endian WCE v2</summary>
    IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x0169,

    /// <summary>Alpha_AXP</summary>
    IMAGE_FILE_MACHINE_ALPHA = 0x0184,

    /// <summary>SH3 little-endian</summary>
    IMAGE_FILE_MACHINE_SH3 = 0x01a2,

    /// <summary>SH3DSP</summary>
    IMAGE_FILE_MACHINE_SH3DSP = 0x01a3,

    /// <summary>SH3E little-endian</summary>
    IMAGE_FILE_MACHINE_SH3E = 0x01a4,

    /// <summary>SH4 little-endian</summary>
    IMAGE_FILE_MACHINE_SH4 = 0x01a6,

    /// <summary>SH5</summary>
    IMAGE_FILE_MACHINE_SH5 = 0x01a8,

    /// <summary>ARM Little-Endian</summary>
    IMAGE_FILE_MACHINE_ARM = 0x01c0,

    /// <summary>ARM Thumb/Thumb-2 Little-Endian</summary>
    IMAGE_FILE_MACHINE_THUMB = 0x01c2,

    /// <summary>
    ///     ARM Thumb-2 Little-Endian
    ///     <note>This constant is available starting with Windows 7 and Windows Server 2008 R2.</note>
    /// </summary>
    IMAGE_FILE_MACHINE_ARMNT = 0x01c4,

    /// <summary>TAM33BD</summary>
    IMAGE_FILE_MACHINE_AM33 = 0x01d3,

    /// <summary>IBM PowerPC Little-Endian</summary>
    IMAGE_FILE_MACHINE_POWERPC = 0x01F0,

    /// <summary>POWERPCFP</summary>
    IMAGE_FILE_MACHINE_POWERPCFP = 0x01f1,

    /// <summary>Intel 64</summary>
    IMAGE_FILE_MACHINE_IA64 = 0x0200,

    /// <summary>MIPS</summary>
    IMAGE_FILE_MACHINE_MIPS16 = 0x0266,

    /// <summary>ALPHA64</summary>
    IMAGE_FILE_MACHINE_ALPHA64 = 0x0284,

    /// <summary>MIPS</summary>
    IMAGE_FILE_MACHINE_MIPSFPU = 0x0366,

    /// <summary>MIPS</summary>
    IMAGE_FILE_MACHINE_MIPSFPU16 = 0x0466,

    /// <summary>AXP64</summary>
    IMAGE_FILE_MACHINE_AXP64 = 0x0284,

    /// <summary>Infineon</summary>
    IMAGE_FILE_MACHINE_TRICORE = 0x0520,

    /// <summary>CEF</summary>
    IMAGE_FILE_MACHINE_CEF = 0x0CEF,

    /// <summary>EFI Byte Code</summary>
    IMAGE_FILE_MACHINE_EBC = 0x0EBC,

    /// <summary>AMD64 (K8)</summary>
    IMAGE_FILE_MACHINE_AMD64 = 0x8664,

    /// <summary>M32R little-endian</summary>
    IMAGE_FILE_MACHINE_M32R = 0x9041,

    /// <summary>
    ///     ARM64 Little-Endian
    ///     <note>This constant is available starting with Windows 8.1 and Windows Server 2012 R2.</note>
    /// </summary>
    IMAGE_FILE_MACHINE_ARM64 = 0xAA64,

    /// <summary>CEE</summary>
    IMAGE_FILE_MACHINE_CEE = 0xC0EE
}