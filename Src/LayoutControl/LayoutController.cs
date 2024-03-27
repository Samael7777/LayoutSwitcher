//Used information from https://www.autohotkey.com/boards/viewtopic.php?t=84140


using LayoutControl.PInvoke;

namespace LayoutControl;

public static class LayoutController
{
	// ReSharper disable once InconsistentNaming
	// ReSharper disable once IdentifierTypo
	private const uint WM_INPUTLANGCHANGEREQUEST = 0x50;
	private const int MaxTries = 6;

	public static KeyboardLayout GetForegroundWindowKeyboardLayout()
	{
		var foregroundWindow = WindowHelper.GetForegroundFocusedWnd();
		var currentLayout = WindowHelper.GetWindowKeyboardLayout(foregroundWindow);
		return currentLayout;
	}

	public static void ChangeLayoutOnForegroundWindow(KeyboardLayout targetLayout)
	{
		var focusWindow = WindowHelper.GetForegroundFocusedWnd();
		var targetHkl = (IntPtr)unchecked((int)targetLayout.Hkl);
		var tries = 0;
		var targetWindowEnumerator = WindowHelper.GetFocusedImeAncestorWindow(focusWindow);

		while (tries < MaxTries)
		{
			if (!targetWindowEnumerator.MoveNext())
			{
				targetWindowEnumerator = WindowHelper.GetFocusedImeAncestorWindow(focusWindow);
				continue;
			}

			if (targetLayout == WindowHelper.GetWindowKeyboardLayout(focusWindow)) 
				break;

			var targetWindow = targetWindowEnumerator.Current;
			PostChangeLayoutRequest(targetWindow, targetHkl);
			Thread.Sleep(10); //Pause to set new layout
			tries++;
		}
	}

	private static void PostChangeLayoutRequest(IntPtr window, IntPtr hkl)
	{
		User32.PostMessage(window, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, (IntPtr)hkl);
	}
}