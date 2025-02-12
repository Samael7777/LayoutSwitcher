using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using LayoutSwitcher.Gui.WPF.Models;
using LayoutSwitcher.Gui.WPF.Windows;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Tools;
using LayoutSwitcher.ViewModels;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace LayoutSwitcher.Gui.WPF;

public class AppRoot : IDisposable
{
    private const string AppId = "LayoutSwitcher";
    private const string AppRegistryKey = @"Vadim Kutin\Layout Switcher";

    private readonly SingleInstance _singleInstance;
    private readonly CycledLayoutsModel _cycledLayoutsModel;
    private readonly SystemSettingsWatcher _systemSettingsChangesWatcher;
    private readonly HotKeyModelWpf _hotKeyModel;
    private readonly SettingsInRegistry _settings;
    
    public SettingsWindow SettingsWindow { get; set; }

    public AppRoot()
    {
        InitCheckSingleAppInstance(out _singleInstance);

        var appPath = Process.GetCurrentProcess().MainModule?.FileName
                      ?? throw new ApplicationException("Can't get app path.");
        
        InitSettings(out _settings);
        _cycledLayoutsModel = new CycledLayoutsModel(_settings.CycledLayout);
        InitHotKeys(_settings, _cycledLayoutsModel, out _hotKeyModel);
        InitSystemSettingsWatcher(_cycledLayoutsModel, out _systemSettingsChangesWatcher);
        InitSettingsWindow(appPath, _cycledLayoutsModel, _hotKeyModel);
    }

    [MemberNotNull(nameof(SettingsWindow))]
    private void InitSettingsWindow(string appPath, CycledLayoutsModel cycledLayoutsModel, HotKeyModelWpf hotKeyModel)
    {
        var autorunModel = new AutorunModel(AppId, appPath);
        var settingsVm = new SettingsVm(autorunModel, cycledLayoutsModel, hotKeyModel);
        SettingsWindow = new SettingsWindow
        {
            DataContext = settingsVm
        };

        SettingsWindow.IsVisibleChanged += (_, args) =>
        {
            if (args.NewValue is false) SaveSettings();
        };
    }

    private static void InitSystemSettingsWatcher(CycledLayoutsModel cycledLayoutsModel, out SystemSettingsWatcher watcher)
    {
        watcher = new SystemSettingsWatcher();
        watcher.SystemLayoutsChanged += (_, _) => cycledLayoutsModel.CleanFromOrphanedLayouts();
    }

    private void InitHotKeys(SettingsInRegistry settings, CycledLayoutsModel cycledLayoutsModel, out HotKeyModelWpf hotKeyModel)
    {
        var availableCombinations = new List<KeyGesture>
        {
            new (Key.None, ModifierKeys.Alt | ModifierKeys.Shift, "Alt+Shift"),
            new (Key.None, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift")
        };

        hotKeyModel = new HotKeyModelWpf(availableCombinations, settings.LayoutToggleHotKeyIndex);
        hotKeyModel.HotKeyPressed += (_, _) => cycledLayoutsModel.SwitchToNextLayout();
        hotKeyModel.HotKeyAlreadyUsed += ShowAlreadyRegisteredHotKeyMessage;
    }

    private static void InitCheckSingleAppInstance(out SingleInstance instance)
    {
        instance = new SingleInstance(AppId);
        instance.CheckOtherInstancesThrowException();
    }
    private static void InitSettings(out SettingsInRegistry settings)
    {
        settings = new SettingsInRegistry(AppRegistryKey);
        settings.Load();
    }
    private static void ShowAlreadyRegisteredHotKeyMessage(object? sender, EventArgs e)
    {
        MessageBox.Show($@"Selected combination already used in other application.", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void SaveSettings()
    {
        _settings.CycledLayout = _cycledLayoutsModel.CycledLayouts;
        _settings.LayoutToggleHotKeyIndex = _hotKeyModel.HotKeyIndex;
        _settings.Save();
    }
    
    #region Dispose

    private bool _disposed;

    ~AppRoot()
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
            _systemSettingsChangesWatcher.Dispose();
            _singleInstance.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}