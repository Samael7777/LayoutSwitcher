using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo

namespace LayoutSwitcher.Control.PInvoke;

[StructLayout(LayoutKind.Sequential)]
internal struct GuiThreadInfo
{
	public int cbSize;
	public uint flags;
	public IntPtr hwndActive;
	public IntPtr hwndFocus;
	public IntPtr hwndCapture;
	public IntPtr hwndMenuOwner;
	public IntPtr hwndMoveSize;
	public IntPtr hwndCaret;
	public Rect rcCaret;
};