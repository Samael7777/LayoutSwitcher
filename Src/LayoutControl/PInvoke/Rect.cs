﻿using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
	public int Left;
	public int Top;
	public int Right;
	public int Bottom;
}