using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using LayoutSwitcher.Models.Exceptions;
using LayoutSwitcher.ViewModels;

namespace LayoutSwitcher.Gui.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly TaskbarIcon _taskbarIcon;
    private readonly AppMain _appMain;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly TrayViewModel _trayViewModel;
    
    public App()
    {
        Current.DispatcherUnhandledException += OnException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainException;

        InitializeComponent();

        _appMain = new AppMain();
        _trayViewModel = new TrayViewModel(_appMain.SettingsWindow, Current.Shutdown);
        _taskbarIcon = new TaskbarIcon
        {
            Icon = WPF.Resources.Keyboard,
            ContextMenu = Current.Resources["TrayContextMenu"] as ContextMenu
        };
        if (_taskbarIcon.ContextMenu != null) 
            _taskbarIcon.ContextMenu.DataContext = _trayViewModel;
        _taskbarIcon.DoubleClickCommand = _trayViewModel.SettingsCommand;
        _taskbarIcon.Visibility = Visibility.Visible;
    }

    private void OnDomainException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception ex) return;
        HandleException(ex);
    }

    private void OnException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        HandleException(e.Exception);
    }
    
    private void OnApplicationShutdown(object sender, ExitEventArgs e)
    {
        _taskbarIcon.Visibility = Visibility.Collapsed;
        DisposeComponents();
    }

    private void DisposeComponents()
    {
        _taskbarIcon.Dispose();
        _appMain.Dispose();
    }

    private static void HandleException(Exception ex)
    {
        var message = GetErrorStringForException(ex);
        ShowErrorMessage(message);
        Current?.Shutdown();
    }

    private static string GetErrorStringForException(Exception ex)
    {
        var message = ex switch
        {
            ApplicationAlreadyRunningException => 
                "Other instance of application is already running.",
            _ => ex.Message
        };

        return message;
    }

    private static void ShowErrorMessage(string message)
    {
        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}