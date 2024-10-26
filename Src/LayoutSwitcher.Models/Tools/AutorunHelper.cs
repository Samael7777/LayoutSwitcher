using Microsoft.Win32;

namespace LayoutSwitcher.Models.Tools;

internal class AutorunHelper
{
    private const string AutorunSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public static bool IsAutorunEnabled(string appId)
    {
        using var autorunSubKey = GetAutorunSubKey(false);
        return autorunSubKey.GetValue(appId) is string;
    }

    public static void AddToAutoRun(string appId, string appPath)
    {
        using var autorunSubKey = GetAutorunSubKey(true);
        autorunSubKey.SetValue(appId, appPath);
    }

    public static void RemoveFromAutorun(string appId)
    {
        using var autorunSubKey = GetAutorunSubKey(true);
        if (autorunSubKey.GetValue(appId) is string)
            autorunSubKey.DeleteValue(appId);
    }

    private static RegistryKey GetAutorunSubKey(bool writable)
    {
        return Registry.CurrentUser.OpenSubKey(AutorunSubKey, writable)
               ?? throw new ApplicationException("Can't open autorun registry key.");
    }
}