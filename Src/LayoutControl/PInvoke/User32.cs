using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable IdentifierTypo

namespace LayoutControl.PInvoke;

internal class User32
{
    [DllImport("user32.dll")]
    public static extern int GetKeyboardLayoutList(int bufferItemsCount, IntPtr[]? layouts);

    [DllImport("user32.dll")]
    public static extern uint GetKeyboardLayout(uint threadId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int LoadString(SafeLibraryHandle handle, int resourceId,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer, int bufferSizeInChars);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId) ;
    
    [DllImport("user32.dll")]
    public static extern bool GetGUIThreadInfo(uint tId, ref GuiThreadInfo threadInfo);
    
    [DllImport("user32", SetLastError = true)]
    public static extern bool UnhookWindowsHookEx(IntPtr hHk);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern  IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern  IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint="GetAncestor")]
    public static extern  IntPtr GetAncestor(IntPtr hwnd, int gaFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern  IntPtr LoadKeyboardLayout([In] [MarshalAs(UnmanagedType.LPWStr)] string pwszKLID, int Flags) ;
}