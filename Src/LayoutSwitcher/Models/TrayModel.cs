using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using Hardcodet.Wpf.TaskbarNotification;
using LayoutSwitcher.Windows;

namespace LayoutSwitcher.Models;

public partial class TrayModel : IDisposable
{
    private readonly TaskbarIcon _taskbarIcon;
    private readonly SettingsWindow _settingsWindow;

    public TrayModel(SettingsWindow settingsWindow)
    {
        _settingsWindow = settingsWindow;
        _taskbarIcon = new TaskbarIcon
        {
            Icon = Resources.Keyboard,
            ContextMenu = Application.Current.Resources["TrayContextMenu"] as ContextMenu
        };
        if (_taskbarIcon.ContextMenu != null) _taskbarIcon.ContextMenu.DataContext = this;
        _taskbarIcon.DoubleClickCommand = SettingsCommand;
        _taskbarIcon.Visibility = Visibility.Visible;
    }

    public Action? ShutdownCommand { get; set; }
    
    [RelayCommand]
    private void Exit()
    {
        ShutdownCommand?.Invoke();
    }

    [RelayCommand]
    private void Settings()
    {
        if (!_settingsWindow.IsVisible)
        {
            _settingsWindow.Show();
        }
    }

    #region Dispose

    private bool _disposed;

    ~TrayModel()
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
            _taskbarIcon.Visibility = Visibility.Collapsed;
            _taskbarIcon.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}