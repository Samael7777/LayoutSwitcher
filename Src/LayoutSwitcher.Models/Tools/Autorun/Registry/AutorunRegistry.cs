using Microsoft.Win32;

namespace LayoutSwitcher.Models.Tools.Autorun.Registry;

internal class AutorunRegistry : IAutorun
{
    private const string AutorunSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public bool IsAutorunEnabled(string appId)
    {
        using var autorunSubKey = GetAutorunSubKey(false);
        return autorunSubKey.GetValue(appId) is string;
    }

    public void AddToAutoRun(string appId, string appPath)
    {
        using var autorunSubKey = GetAutorunSubKey(true);
        autorunSubKey.SetValue(appId, appPath);
    }

    public void RemoveFromAutorun(string appId)
    {
        using var autorunSubKey = GetAutorunSubKey(true);
        if (autorunSubKey.GetValue(appId) is string)
            autorunSubKey.DeleteValue(appId);
    }

    private static RegistryKey GetAutorunSubKey(bool writable)
    {
        return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(AutorunSubKey, writable)
               ?? throw new ApplicationException("Can't open autorun registry key.");
    }
}