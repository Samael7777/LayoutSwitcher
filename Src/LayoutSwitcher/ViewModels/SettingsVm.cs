using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LayoutControl;
using LayoutSwitcher.Extensions;
using LayoutSwitcher.Models;
using VirtualKeys;

namespace LayoutSwitcher.ViewModels;

public partial class SettingsVm : ObservableObject
{
    private readonly LayoutsSwitchModel _layoutsSwitchModel;
    private readonly Settings _settings;
    private readonly HotkeysModel _hotkeysModel;

    [ObservableProperty] private ObservableCollection<string> _availableLayouts = new();
    [ObservableProperty] private ObservableCollection<string> _cyclingLayouts = new();
    [ObservableProperty] private ObservableCollection<string> _availableHotkeys;
    [ObservableProperty] private string _systemLayoutSwitchHotkey;
    [ObservableProperty] private int _appHotkeyIndex;

    public SettingsVm(Settings settings, LayoutsSwitchModel layoutsSwitchModel, HotkeysModel hotkeysModel)
    {
        _settings = settings;
        _hotkeysModel = hotkeysModel;
        _hotkeysModel.PropertyChanged += OnHotkeysChanged;
        _settings.PropertyChanged += OnHotkeysChanged;
        
        _layoutsSwitchModel = layoutsSwitchModel;
        _layoutsSwitchModel.PropertyChanged += OnSwitchModelChanged;

        GetLayoutsNames(AvailableLayouts, _layoutsSwitchModel.AvailableLayouts);
        GetLayoutsNames(CyclingLayouts, _layoutsSwitchModel.CyclingLayouts);

        _systemLayoutSwitchHotkey = GetHotkeyString(_hotkeysModel.SystemToggleHotkey);
        _availableHotkeys = new ObservableCollection<string>(_hotkeysModel.AvailableCombinations
            .Select(c => c.ToString()));

        _appHotkeyIndex = GetCurrentAppHotkeyIndex();
    }

    public bool Autorun
    {
        get => _settings.Autorun;
        set => _settings.Autorun = value;
    }

    [RelayCommand]
    private void AddToCycling(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _layoutsSwitchModel.AddToCycling(index);
    }

    [RelayCommand]
    private void RemoveFromCycling(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _layoutsSwitchModel.RemoveFromCycling(index);
    }

    [RelayCommand]
    private void MoveUp(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _layoutsSwitchModel.MoveCyclingLayoutUp(index);
    }

    [RelayCommand]
    private void MoveDown(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _layoutsSwitchModel.MoveCyclingLayoutDown(index);
    }

    [RelayCommand]
    private void SetNewAppHotkey(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _hotkeysModel.SetAppToggleHotKey(index);
    }

    //todo
    [RelayCommand]
    private Task FormClosing()
    {
        return _settings.IsChanged
            ? _settings.SaveSettingsToFileAsync()
            : Task.CompletedTask;
    }
    
    private void OnSwitchModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(LayoutsSwitchModel.CyclingLayouts):
                    GetLayoutsNames(AvailableLayouts, _layoutsSwitchModel.AvailableLayouts);
                    GetLayoutsNames(CyclingLayouts, _layoutsSwitchModel.CyclingLayouts);
                    break;
            }
        });
    }

    private void OnHotkeysChanged(object? sender, PropertyChangedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(HotkeysModel.SystemToggleHotkey):
                    SystemLayoutSwitchHotkey = GetHotkeyString(_hotkeysModel.SystemToggleHotkey);
                    break;
                case nameof(Settings.AppToggleHotkey):
                    AppHotkeyIndex = GetCurrentAppHotkeyIndex();
                    break;
            }
        });
    }

    private int GetCurrentAppHotkeyIndex()
    {
        return _hotkeysModel.AvailableCombinations.FirstIndexOf(_settings.AppToggleHotkey);
    }
    
    private static void GetLayoutsNames(ICollection<string> collection, IEnumerable<KeyboardLayout> list)
    {
        var data = list.Select(l=>l.LayoutDisplayName);
        collection.Clear();
        foreach (var layout in data)
        {
            collection.Add(layout);
        }
    }

    private static string GetHotkeyString(HotKey hotkey)
    {
        return hotkey.IsEmpty ? "(none)" : hotkey.ToString();
    }
}