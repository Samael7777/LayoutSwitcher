using System.Runtime.InteropServices;
using LayoutControl.PInvoke;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace LayoutControl;

internal static class WindowHelper
{
	
	private const uint WM_INPUTLANGCHANGEREQUEST = 0x50;
	private const int KLF_ACTIVATE = 1;
	private const int GA_ROOTOWNER = 3; //Retrieves the owned root window by walking the chain of parent and owner windows 

	public static IntPtr GetForegroundFocusedWnd()
	{
		var foregroundWnd = User32.GetForegroundWindow();
		var foregroundThread = User32.GetWindowThreadProcessId(foregroundWnd, out _);

		var info = new GuiThreadInfo()
		{
			cbSize = Marshal.SizeOf<GuiThreadInfo>(),
		};
		User32.GetGUIThreadInfo(foregroundThread, ref info);

		var focus = info.hwndCaret != IntPtr.Zero ? info.hwndCaret :info.hwndFocus;
		if (focus == IntPtr.Zero) focus = foregroundWnd;
		
		return focus;
	}

	public static KeyboardLayout GetWindowKeyboardLayout(IntPtr window)
	{
		var windowsEnumerator = GetFocusedImeAncestorWindow(window);
		
		var hkl = 0u;
		while (windowsEnumerator.MoveNext())
		{
			var checkingWindow = windowsEnumerator.Current;
			var windowThreadId = User32.GetWindowThreadProcessId(checkingWindow, out _);
			hkl = User32.GetKeyboardLayout(windowThreadId);
			if (hkl != 0) break;
		}

		if (hkl == 0)
			throw new ApplicationException("Can't get current layout.");

		var layout = KeyboardLayout.GetLayout(hkl);
		return layout;
	}

	public static IntPtr GetDefaultImeWindowForWindow(IntPtr window)
	{
		var imeWnd = Imm32.ImmGetDefaultIMEWnd(window);
		return imeWnd;
	}

	public static IEnumerator<IntPtr> GetFocusedImeAncestorWindow(IntPtr focusedWindow)
	{
		yield return focusedWindow;
		var ime = GetDefaultImeWindowForWindow(focusedWindow);
		yield return ime;
		var ancestor = User32.GetAncestor(focusedWindow, GA_ROOTOWNER);
		yield return ancestor;
	}

	public static void PostChangeLayoutRequest(IntPtr window, KeyboardLayout layout)
	{
		var hkl = layout.IsImeLayout 
			? User32.LoadKeyboardLayout($"{layout.KLID:x8}", KLF_ACTIVATE)
			: unchecked((IntPtr)(int)layout.Hkl);

		if (hkl == IntPtr.Zero)
			throw new ApplicationException($"Invalid layout.");

		User32.PostMessage(window, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, hkl);
	}
}