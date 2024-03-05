using LayoutControl.PInvoke;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
#pragma warning disable CA1806

namespace LayoutControl;

public static class KeyboardLayoutInfo
{
    public static IEnumerable<KeyboardLayout> GetSystemLayouts()
    {
        var count = User32.GetKeyboardLayoutList(0, null);
        var rawLayouts = new IntPtr[count];
        User32.GetKeyboardLayoutList(count, rawLayouts);
        return rawLayouts.Select((hkl)=> KeyboardLayout.GetLayout((uint)hkl));
    }
    
    public static KeyboardLayout GetThreadKeyboardLayout(uint threadId = 0)
    {
        var hkl = User32.GetKeyboardLayout(threadId);
        return KeyboardLayout.GetLayout(hkl);
    }

    public static KeyboardLayout GetForegroundWindowLayout()
    {
        var foregroungHWnd = User32.GetForegroundWindow();
        var foregrounThreadId = User32.GetWindowThreadProcessId(foregroungHWnd, out _);
        return GetThreadKeyboardLayout(foregrounThreadId);
    }
}