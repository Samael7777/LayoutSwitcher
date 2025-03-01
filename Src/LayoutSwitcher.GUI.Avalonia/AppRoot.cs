using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using LayoutSwitcher.GUI.Avalonia.Models;
using LayoutSwitcher.GUI.Avalonia.Views;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Interfaces;
using LayoutSwitcher.Models.Tools;
using LayoutSwitcher.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace LayoutSwitcher.GUI.Avalonia;

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

    private void InitHotKeys(out HotKeyModel hotKeyModel)
    {
        var availableCombinations = new List<KeyGesture>
        {
            new (Key.None, KeyModifiers.Alt | KeyModifiers.Shift),
            new (Key.None, KeyModifiers.Control | KeyModifiers.Shift)
        };

        hotKeyModel = new HotKeyModel(availableCombinations);
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

        SettingsWindow.Closing += (_, _) =>
        {
            appModel.SaveSettings(); //todo test
        };
    }

    private void ShowAlreadyRegisteredHotKeyMessage(object? sender, EventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Error",
            "Selected combination already used in other application.",
            ButtonEnum.Ok,
            Icon.Error);
        box.ShowAsPopupAsync(SettingsWindow);
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