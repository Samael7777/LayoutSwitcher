using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
// ReSharper disable ComplexConditionExpression

namespace LayoutSwitcher.Control;

// ReSharper disable once HollowTypeName
public static unsafe class LayoutController
{
    public static void ChangeLayoutOnForegroundWindow(KeyboardLayout target)
    {
        if (target.IsEmpty)
            return;

        var hkl = LoadLayout(target.KLID);
        ActivateLayout(hkl);
        
        var tries = 0;
        while (tries <= 10 && GetForegroundWindowLayout() != target)
        {
            var focusWindow = GetFocusedWindow();
            if (focusWindow.IsNull)
                break;

            SendChangeLayoutMessage(focusWindow, hkl);
            tries++;
        } 
    }

    public static IEnumerable<KeyboardLayout> GetSystemLayouts()
    {
        var count = WinApi.GetKeyboardLayoutList(0);
        var rawLayouts = new HKL[count];
        _ = WinApi.GetKeyboardLayoutList(rawLayouts);

        return rawLayouts.Select(hkl=> KeyboardLayout.GetLayout((uint)hkl.Value));
    }
    
    public static KeyboardLayout GetForegroundWindowLayout()
    {
        var foregroundWindow = GetFocusedWindow();
        var foregroundThreadId = WinApi.GetWindowThreadProcessId(foregroundWindow, out _);
        var hkl = WinApi.GetKeyboardLayout(foregroundThreadId);
        
        return KeyboardLayout.GetLayout((uint)hkl.Value);
    }

    private static HWND GetFocusedWindow()
    {
        var foregroundWindow = WinApi.GetForegroundWindow();
        var foregroundThreadId = WinApi.GetWindowThreadProcessId(foregroundWindow, out _);
        var gui = new GUITHREADINFO
        {
            cbSize = (uint)Marshal.SizeOf<GUITHREADINFO>(),
        };
        
        if (WinApi.GetGUIThreadInfo(foregroundThreadId, ref gui) && !gui.hwndFocus.IsNull)
            return gui.hwndFocus;
        
        return foregroundWindow;
    }

    private static HKL LoadLayout(uint klId)
    {
        var klIdStr = $"{klId:x8}";
        fixed(char* klIdPtr = klIdStr)
        {
            return WinApi.LoadKeyboardLayout(klIdPtr, ACTIVATE_KEYBOARD_LAYOUT_FLAGS.KLF_ACTIVATE);
        }
    }

    private static void ActivateLayout(HKL hkl)
    {
        WinApi.ActivateKeyboardLayout(hkl, ACTIVATE_KEYBOARD_LAYOUT_FLAGS.KLF_SETFORPROCESS);
    }

    private static void SendChangeLayoutMessage(HWND window, HKL hkl)
    {
        WinApi.PostMessage(window, WinApi.WM_INPUTLANGCHANGEREQUEST, 0, (nint)hkl);
        Thread.Sleep(10);
    }
}