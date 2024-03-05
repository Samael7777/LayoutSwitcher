using System.IO;
using Microsoft.Win32;

namespace LayoutSwitcher.Models;

public class AppAutorun
{
    private const string AutorunSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private readonly string _appName;
    private readonly string _appPath;

    public AppAutorun()
    {
        _appName = AppDomain.CurrentDomain.FriendlyName;
        var directory = AppDomain.CurrentDomain.BaseDirectory;
        _appPath = Path.Combine(directory, _appName + ".exe");
    }

    public bool Autorun
    {
        get => IsAutorunEnabled();
        set
        {
            if (value == IsAutorunEnabled()) return;
            if (value) 
                AddAppToAutorun();
            else 
                RemoveAppFromAutorun();
        }
    }

    private void AddAppToAutorun()
    {
        var autorunSubKey = GetAutorunSubKey(true);
        if (IsAutorunEnabledInternal(autorunSubKey)) return;

        autorunSubKey.SetValue(_appName, _appPath);
        autorunSubKey.Close();
    }

    private void RemoveAppFromAutorun()
    {
        var autorunSubKey = GetAutorunSubKey(true);
        if(autorunSubKey.GetValue(_appName) is not null)
            autorunSubKey.DeleteValue(_appName);

        autorunSubKey.Close();
    }

    private bool IsAutorunEnabled()
    {
        var autorunSubKey = GetAutorunSubKey(false);
        return IsAutorunEnabledInternal(autorunSubKey);
    }


    private bool IsAutorunEnabledInternal(RegistryKey autorunSubKey)
    {
        return autorunSubKey.GetValue(_appName) is string appRecord
               && string.Equals(appRecord, _appPath, StringComparison.InvariantCultureIgnoreCase);
    }

    private static RegistryKey GetAutorunSubKey(bool writable)
    {
        return Registry.CurrentUser.OpenSubKey(AutorunSubKey, writable)
               ?? throw new ApplicationException("Can't open autorun registry key.");
    }
}