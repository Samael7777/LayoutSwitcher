namespace LayoutControl.PInvoke;

internal static class ProcessHelper
{
    public static bool IsWow64ProcessWindow(IntPtr hWnd)
    {
        _ = User32.GetWindowThreadProcessId(hWnd, out var processId);

        using var procHandle = Kernel32.OpenProcess(ACCESS_MASK.GENERIC_READ, false, processId);
        if (procHandle.IsInvalid) return false;

        var success = Kernel32.IsWow64Process(procHandle, out var isWow64Process);
        return success && isWow64Process;
    }
}