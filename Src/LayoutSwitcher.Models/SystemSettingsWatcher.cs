using PhoenixTools.Watchers;

namespace LayoutSwitcher.Models;

// \HKEY_CURRENT_USER\Control Panel\Input Method\Hot Keys - hotkey to every layout

public class SystemSettingsWatcher : IDisposable
{
    private readonly RegistryWatcher _hotkeyWatcher;
    private readonly RegistryWatcher _settingsWatcher;

    public event EventHandler? HotKeyChanged;
    public event EventHandler? SystemLayoutsChanged;

    public SystemSettingsWatcher()
    {
        _hotkeyWatcher = new RegistryWatcher(RegistryRootKey.HKEY_CURRENT_USER, 
            @"\Keyboard Layout\Toggle", true);
        _hotkeyWatcher.RegistryChanged += (_, e) => HotKeyChanged?.Invoke(this, e);

        _settingsWatcher = new RegistryWatcher(RegistryRootKey.HKEY_CURRENT_USER,
            @"\Keyboard Layout\Preload", true);
        _settingsWatcher.RegistryChanged += (_, e) => SystemLayoutsChanged?.Invoke(this, e);
    }

    #region Dispose

    private bool _disposed;

    ~SystemSettingsWatcher()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            //dispose managed state (managed objects)
            _hotkeyWatcher.Dispose();
            _settingsWatcher.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}