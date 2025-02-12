//Used information from https://www.autohotkey.com/boards/viewtopic.php?t=84140


using LayoutSwitcher.Control.PInvoke;
#if DEBUG
using System.Diagnostics;
#endif

namespace LayoutSwitcher.Control;

public static class LayoutController
{
	public static void ChangeLayoutOnForegroundWindow(KeyboardLayout target)
	{
		var focusWindow = WindowHelper.GetForegroundFocusedWnd();
		var targetWindowEnumerator = WindowHelper.GetFocusedImeAncestorWindow(focusWindow);
#if DEBUG
		Debug.WriteLine($"Try to set layout {target.LayoutDisplayName}");
#endif
		while (targetWindowEnumerator.MoveNext() 
		       && target != WindowHelper.GetWindowKeyboardLayout(focusWindow))
		{
			var current = WindowHelper.GetWindowKeyboardLayout(focusWindow);
#if DEBUG
			Debug.WriteLine($"\tFor window {focusWindow:x8}, current: {current.LayoutDisplayName}");
#endif
			var targetWindow = targetWindowEnumerator.Current;
			WindowHelper.PostChangeLayoutRequest(targetWindow, target);

			Thread.Sleep(10); //Pause to set new layout
		}
	}

    public static IEnumerable<KeyboardLayout> GetSystemLayouts()
    {
        var count = User32.GetKeyboardLayoutList(0, null);
        var rawLayouts = new IntPtr[count];
        _ = User32.GetKeyboardLayoutList(count, rawLayouts);
        return rawLayouts.Select(hkl=> KeyboardLayout.GetLayout((uint)hkl));
    }
    
    public static KeyboardLayout GetForegroundWindowLayout()
    {
        var foregroundWindow = User32.GetForegroundWindow();
        var foregroundThreadId = User32.GetWindowThreadProcessId(foregroundWindow, out _);
        return GetThreadKeyboardLayout(foregroundThreadId);
    }

    public static KeyboardLayout GetThreadKeyboardLayout(uint threadId = 0)
    {
        var hkl = User32.GetKeyboardLayout(threadId);
        return KeyboardLayout.GetLayout(hkl);
    }
}