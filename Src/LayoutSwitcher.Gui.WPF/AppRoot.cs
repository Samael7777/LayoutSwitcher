using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using LayoutSwitcher.Gui.WPF.Models;
using LayoutSwitcher.Gui.WPF.Windows;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Interfaces;
using LayoutSwitcher.Models.Tools;
using LayoutSwitcher.ViewModels;

namespace LayoutSwitcher.Gui.WPF;

public class AppRoot : IDisposable
{
    private readonly SingleInstance _singleInstance;
    private readonly AppModel _appModel;

    public SettingsWindow SettingsWindow { get; set; }

    public AppRoot()
    {
        InitCheckSingleAppInstance(out _singleInstance);

        InitHotKeys(out var hotKeyModel);
        InitAppModel(hotKeyModel, out _appModel);
        InitSettingsWindow(_appModel);
    }

    private static void InitCheckSingleAppInstance(out SingleInstance instance)
    {
        instance = new SingleInstance(AppModel.AppId);
        instance.CheckOtherInstancesThrowException();
    }

    private static void InitHotKeys(out HotKeyModelWpf hotKeyModel)
    {
        var availableCombinations = new List<KeyGesture>
        {
            new (Key.None, ModifierKeys.Alt | ModifierKeys.Shift, "Alt+Shift"),
            new (Key.None, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift")
        };

        hotKeyModel = new HotKeyModelWpf(availableCombinations);
        hotKeyModel.HotKeyAlreadyUsed += ShowAlreadyRegisteredHotKeyMessage;
    }

    private static void InitAppModel(IHotKeyModel hotKeyModel, out AppModel appModel)
    {
        var appPath = Process.GetCurrentProcess().MainModule?.FileName
                      ?? throw new ApplicationException("Can't get app path.");

        appModel = new AppModel(hotKeyModel, appPath);
    }

    [MemberNotNull(nameof(SettingsWindow))]
    private void InitSettingsWindow(AppModel appModel)
    {
        
        var settingsVm = new SettingsVm(appModel.AutorunModel, appModel.CycledLayoutsModel, appModel.HotKeyModel);
        SettingsWindow = new SettingsWindow
        {
            DataContext = settingsVm
        };

        SettingsWindow.IsVisibleChanged += (_, args) =>
        {
            if (args.NewValue is false) appModel.SaveSettings();
        };
    }

    private static void ShowAlreadyRegisteredHotKeyMessage(object? sender, EventArgs e)
    {
        MessageBox.Show(@"Selected combination already used in other application.", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            _appModel.Dispose();
            _singleInstance.Dispose();
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}