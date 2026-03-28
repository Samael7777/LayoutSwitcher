using LayoutSwitcher.Models.Interfaces;
using LayoutSwitcher.Models.Models;
using LayoutSwitcher.Models.Tools;
using System.Diagnostics;
using LayoutSwitcher.Models.ViewModels;

namespace LayoutSwitcher.Models;

public abstract class AppRootBase<T> : IDisposable
    where T : ISettingsWindow, new()
{
    protected readonly SingleInstance SingleInstance;
    protected readonly AppModel AppModel;
    protected ISettingsWindow? SettingsWindow;

    private bool _isSettingsShows;

    protected AppRootBase()
    {
        InitCheckSingleAppInstance(out SingleInstance);
        InitHotKeysBase(out var hotKeyModel);
        InitAppModel(hotKeyModel, out AppModel);
    }

    public void ShowSettingsWindow()
    {
        if (Interlocked.Exchange(ref _isSettingsShows, true))
            return;

        SettingsWindow = new T
        {
            DataContext = new SettingsVm(AppModel),
        };
        SettingsWindow.Show();
        SettingsWindow.Closed += OnSettingsClosed;
    }
    
    protected abstract void InitHotKeys(out IHotKeyModel hotKeyModel);
    
    private void InitHotKeysBase(out IHotKeyModel hotKeyModel)
    {
        InitHotKeys(out hotKeyModel);
    }

    private void OnSettingsClosed(object? sender, EventArgs args)
    {
        AppModel.SaveSettings();
        SettingsWindow?.Closed -= OnSettingsClosed;
        SettingsWindow = null;
        _isSettingsShows = false;
    }

    private static void InitCheckSingleAppInstance(out SingleInstance instance)
    {
        instance = new SingleInstance(AppModel.AppId);
        instance.CheckOtherInstancesThrowException();
    }
    
    private static void InitAppModel(IHotKeyModel hotKeyModel, out AppModel appModel)
    {
        var appPath = Process.GetCurrentProcess().MainModule?.FileName
                      ?? throw new ApplicationException("Can't get app path.");

        appModel = new AppModel(hotKeyModel, appPath);
    }

    #region Dispose

    private bool _disposed;

    ~AppRootBase()
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
            AppModel.Dispose();
            SingleInstance.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}