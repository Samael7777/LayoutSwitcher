using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using LayoutSwitcher.Gui.WPF.Models;
using LayoutSwitcher.Gui.WPF.Windows;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Interfaces;
using LayoutSwitcher.Models.Tools;
using LayoutSwitcher.ViewModels;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace LayoutSwitcher.Gui.WPF;

public class AppMain : IDisposable
{
    private const string AppId = "LayoutSwitcher";
    private const string AppRegistryKey = @"Vadim Kutin\Layout Switcher";

    private readonly SingleInstanceChecker _singleInstanceChecker;
    private readonly CycledLayoutsModel _cycledLayoutsModel;
    private readonly SystemSettingsWatcher _systemSettingsChangesWatcher;
    private readonly IHotKeyModel _hotKeyModel;
    private readonly ISettings _settings;
    
    public SettingsWindow SettingsWindow { get; }

    public AppMain()
    {
        //Check for single application instance
        _singleInstanceChecker = new SingleInstanceChecker(AppId);
        _singleInstanceChecker.CheckOtherInstancesThrowException();
        
        var appPath = Process.GetCurrentProcess().MainModule?.FileName
                      ?? throw new ApplicationException("Can't get app path.");
        
        //Init app modules
       
        _settings = new SettingsInRegistry(AppRegistryKey);
        _settings.Load();
        
        _cycledLayoutsModel = new CycledLayoutsModel(_settings.CycledLayout);

        var availableCombinations = new List<KeyGesture>
        {
            new (Key.None, ModifierKeys.Alt | ModifierKeys.Shift, "Alt+Shift"),
            new (Key.None, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift")
        };

        _hotKeyModel = new HotKeyModel(availableCombinations, _settings.LayoutToggleHotKeyIndex);
        _hotKeyModel.HotKeyPressed += (_, _) => _cycledLayoutsModel.SwitchToNextLayout();
        _hotKeyModel.HotKeyAlreadyUsed += ShowAlreadyRegisteredHotKeyMessage;
     
        _systemSettingsChangesWatcher = new SystemSettingsWatcher();
        _systemSettingsChangesWatcher.SystemLayoutsChanged += (_, _) =>
            _cycledLayoutsModel.CleanFromOrphanedLayouts();

        var autorunModel = new AutorunModel(AppId, appPath);
        
        var settingsVm = new SettingsVm(autorunModel, _cycledLayoutsModel, _hotKeyModel);
        SettingsWindow = new SettingsWindow
        {
            DataContext = settingsVm
        };

        SettingsWindow.IsVisibleChanged += (_, args) =>
        {
            if (args.NewValue is false) SaveSettings();
        };
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

    ~AppMain()
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
            _singleInstanceChecker.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}