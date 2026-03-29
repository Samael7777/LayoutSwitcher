using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using LayoutSwitcher.Gui.WPF.Windows;
using LayoutSwitcher.Models.Exceptions;
using LayoutSwitcher.Models.ViewModels;


namespace LayoutSwitcher.Gui.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly TaskbarIcon _taskbarIcon;
    private readonly WpfAppRoot<SettingsWindow> _wpfAppRoot;
    
    public App()
    {
        Current.DispatcherUnhandledException += OnException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainException;

        InitializeComponent();

        _wpfAppRoot = new WpfAppRoot<SettingsWindow>();
        var trayViewModel = new TrayViewModel(_wpfAppRoot.ShowSettingsWindow, Current.Shutdown);
        _taskbarIcon = new TaskbarIcon
        {
            Icon = WPF.Resources.Keyboard,
            ContextMenu = Current.Resources["TrayContextMenu"] as ContextMenu
        };
        if (_taskbarIcon.ContextMenu != null)
            _taskbarIcon.ContextMenu.DataContext = trayViewModel;
        _taskbarIcon.DoubleClickCommand = trayViewModel.SettingsCommand;
        _taskbarIcon.Visibility = Visibility.Visible;
    }

    private static void OnDomainException(object sender, UnhandledExceptionEventArgs e)
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
        _wpfAppRoot.Dispose();
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