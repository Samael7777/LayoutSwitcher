using System.Windows;
using System.Windows.Threading;
using LayoutSwitcher.Gui.WPF.Models;

namespace LayoutSwitcher.Gui.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly AppMain _appMain;
    private readonly TrayModel _trayModel;
    
    public App()
    {
        InitializeComponent();
        Current.DispatcherUnhandledException += OnException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainException;

        _appMain = new AppMain();
        _trayModel = new TrayModel(_appMain.SettingsWindow)
        {
            ShutdownCommand = () => Current.Shutdown()
        };
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

    private void HandleException(Exception ex)
    {
        MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        Current?.Shutdown();
    }

    private void OnApplicationShutdown(object sender, ExitEventArgs e)
    {
        _trayModel.Dispose();
        _appMain.Dispose();
    }
}