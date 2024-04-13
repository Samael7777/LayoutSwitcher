//Used information from https://www.autohotkey.com/boards/viewtopic.php?t=84140

#if DEBUG
using System.Diagnostics;
#endif

namespace LayoutControl;

public static class LayoutController
{
	public static KeyboardLayout GetForegroundWindowKeyboardLayout()
	{
		var foregroundWindow = WindowHelper.GetForegroundFocusedWnd();
		var currentLayout = WindowHelper.GetWindowKeyboardLayout(foregroundWindow);
		return currentLayout;
	}

	public static void ChangeLayoutOnForegroundWindow(KeyboardLayout targetLayout)
	{
		var focusWindow = WindowHelper.GetForegroundFocusedWnd();
		var targetWindowEnumerator = WindowHelper.GetFocusedImeAncestorWindow(focusWindow);
#if DEBUG
		Debug.WriteLine($"Try to set layout {targetLayout.LayoutDisplayName}");
#endif
		while (targetWindowEnumerator.MoveNext() 
		       && targetLayout != WindowHelper.GetWindowKeyboardLayout(focusWindow))
		{
			var current = WindowHelper.GetWindowKeyboardLayout(focusWindow);
#if DEBUG
			Debug.WriteLine($"\tFor window {focusWindow:x8}, current: {current.LayoutDisplayName}");
#endif
			var targetWindow = targetWindowEnumerator.Current;
			WindowHelper.PostChangeLayoutRequest(targetWindow, targetLayout);
			Thread.Sleep(10); //Pause to set new layout
		}
	}
}