//Used information from https://www.autohotkey.com/boards/viewtopic.php?t=84140


using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace LayoutSwitcher.Control;

public static unsafe class LayoutController
{
    public static void ChangeLayoutOnForegroundWindow(KeyboardLayout target)
	{
        //_profiles->ChangeCurrentLanguage(target.LanguageId);
        
        var focusWindow = GetFocusedWindow();
        
        var currentThreadId = WinApi.GetCurrentThreadId();
        var foregroundThread = WinApi.GetWindowThreadProcessId(focusWindow, out _);

        var attached = currentThreadId != foregroundThread
            && WinApi.AttachThreadInput(currentThreadId, foregroundThread, true);
        
        try
        {
            WinApi.ActivateKeyboardLayout((HKL)target.Hkl, ACTIVATE_KEYBOARD_LAYOUT_FLAGS.KLF_SETFORPROCESS);
            if(!focusWindow.IsNull)
                WinApi.SendMessage(focusWindow, WinApi.WM_INPUTLANGCHANGEREQUEST, 0, (nint)target.Hkl);
        }
        finally
        {
            if (attached)
                WinApi.AttachThreadInput(currentThreadId, foregroundThread, false);
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
}