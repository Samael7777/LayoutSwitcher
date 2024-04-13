using System.Runtime.InteropServices;

namespace LayoutControl.PInvoke;

internal static class ShellHookNative
{
	private const string LibName = "ShellHook";
	private const string PostfixX64 = "_x64";
	private const string PostfixX86 = "_x86";

	[DllImport($"{LibName}{PostfixX64}.dll", SetLastError = true, EntryPoint = "RegisterShellHook")]
	private static extern SafeShellHookHandle RegisterShellHook_x64(IntPtr messageWindow);

	[DllImport($"{LibName}{PostfixX86}.dll", SetLastError = true, EntryPoint = "RegisterShellHook")]
	private static extern SafeShellHookHandle RegisterShellHook_x86(IntPtr messageWindow);

	[DllImport($"{LibName}{PostfixX64}.dll", SetLastError = true, EntryPoint = "GetShellHookMessageCode")]
	private static extern uint GetShellHookMessageCode_x64();

	[DllImport($"{LibName}{PostfixX86}.dll", SetLastError = true, EntryPoint = "GetShellHookMessageCode")]
	private static extern uint GetShellHookMessageCode_x86();


	public static SafeShellHookHandle RegisterShellHook(IntPtr messageWindow)
	{
		var hook = IsX86Environment()
			? RegisterShellHook_x86(messageWindow)
			: RegisterShellHook_x64(messageWindow);

		if (!hook.IsInvalid) return hook;

		var error = Marshal.GetLastWin32Error();
		throw new ApplicationException($"Error registering shell hook: {error:x8}");
	}

	public static uint GetShellHookMessageCode() => IsX86Environment()
		? GetShellHookMessageCode_x86()
		: GetShellHookMessageCode_x64();

	private static bool IsX86Environment() => IntPtr.Size == 4;
}