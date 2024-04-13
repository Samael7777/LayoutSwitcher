using System.Windows;
using System.Windows.Threading;
using LayoutControl.ShellHookTools;
using LayoutSwitcher.Adapters;
using LayoutSwitcher.MessageWindow;
using LayoutSwitcher.Models;
using LayoutSwitcher.ViewModels;
using LayoutSwitcher.Windows;

namespace LayoutSwitcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private const int AppAlreadyRunningExitCode = -2;

    private readonly Mutex _appMutex;
    private readonly TrayModel _trayModel;
    private readonly HotKeyHook.HotKeyHook _hotKeyHook;
    private readonly HotkeysModel _hotkeysModel;
    private readonly SettingsChangesWatcher _settingsChangesWatcher;
    private readonly ShellHook _shellHook;
    private readonly MsgOnlyWindow _msgWindow;
    private readonly HotKeyHookMsgWindowAdapter _hotkeyHookMsgWindowAdapter;
    private readonly ChangeLayoutHookMsgWindowAdapter _changeLayoutHookMsgWindowAdapter;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public App()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        InitializeComponent();
        Current.DispatcherUnhandledException += OnException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainException;

        //Check for single application instance
        _appMutex = new Mutex(true, "LayoutSwitcherMutex");
        if (!_appMutex.WaitOne(500, false))
        {
            MessageBox.Show("Application already running!", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown(AppAlreadyRunningExitCode);
            return;
        }
        
        //Load settings
        var settings = Settings.LoadSettingsFromFile();
        
        //Init message capture window
        _msgWindow = new MsgOnlyWindow();

        //Init layout hook
        _changeLayoutHookMsgWindowAdapter = new ChangeLayoutHookMsgWindowAdapter(_msgWindow);
        _shellHook = new ShellHook(_changeLayoutHookMsgWindowAdapter);

        //Init main app model
        var layoutsSwitchModel = new LayoutsSwitchModel(settings, _shellHook);
        _shellHook.LayoutChanged += (_, a) =>
            layoutsSwitchModel.SetCurrentLayout(a.NewLayout);

        //Init HotKey
        _hotkeyHookMsgWindowAdapter = new HotKeyHookMsgWindowAdapter(_msgWindow);
        _hotKeyHook = new HotKeyHook.HotKeyHook(_hotkeyHookMsgWindowAdapter);

        _hotkeysModel = new HotkeysModel(settings, _hotKeyHook);
        _hotkeysModel.LayoutToggleRequested += (_, _) 
            => layoutsSwitchModel.SwitchToNextLayout();
       
        //Init system settings watcher
        _settingsChangesWatcher = new SettingsChangesWatcher();
        _settingsChangesWatcher.SystemLayoutsChanged += (_, _) => layoutsSwitchModel.OnSystemLayoutsChanged();
        _settingsChangesWatcher.HotKeyChanged += (_, _) => _hotkeysModel.OnSystemToggleHotKeyChanged();

        //Init visual elements
        var settingsVm = new SettingsVm(settings, layoutsSwitchModel, _hotkeysModel);

        var settingsWindow = new SettingsWindow()
        {
            DataContext = settingsVm
        };

        _trayModel = new TrayModel(settingsWindow)
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
        if (e.ApplicationExitCode == AppAlreadyRunningExitCode) return;

        DisposeComponents();
    }

    private void DisposeComponents()
    {
        _trayModel.Dispose();
        _settingsChangesWatcher.Dispose();
        _hotkeysModel.Dispose();
        _hotKeyHook.Dispose();
        _hotkeyHookMsgWindowAdapter.Dispose();
        _shellHook.Dispose();
        _changeLayoutHookMsgWindowAdapter.Dispose();
        _msgWindow.Close();
        _appMutex.ReleaseMutex();
        _appMutex.Dispose();
    }
}