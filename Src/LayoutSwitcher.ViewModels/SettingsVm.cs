using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Interfaces;

namespace LayoutSwitcher.ViewModels;

public partial class SettingsVm : ObservableObject
{
    private readonly CycledLayoutsModel _cycledLayoutsModel;
    private readonly AutorunModel _autorunModel;
    private readonly IHotKeyModel _hotkeyModel;

    [ObservableProperty] private int _appHotkeyIndex;

    public List<string> AvailableLayouts => _cycledLayoutsModel.AvailableLayouts
        .Select(l=>l.LayoutDisplayName)
        .ToList();

    public List<string> CycledLayouts => _cycledLayoutsModel.CycledLayouts
        .Select(l=>l.LayoutDisplayName)
        .ToList();

    public List<string> AvailableHotkeys => _hotkeyModel.AvailableCombinations.ToList();

    public bool Autorun
    {
        get => _autorunModel.Autorun;
        set => _autorunModel.Autorun = value;
    }

    public SettingsVm(AutorunModel autorunModel, CycledLayoutsModel cycledLayoutsModel,
        IHotKeyModel hotkeyModel)
    {
        _autorunModel = autorunModel;
        _cycledLayoutsModel = cycledLayoutsModel;
        _hotkeyModel = hotkeyModel;
        AppHotkeyIndex = _hotkeyModel.HotKeyIndex;
        _cycledLayoutsModel.PropertyChanged += OnLayoutModelChanged;
    }

    #region Relay commands

    [RelayCommand]
    private void SetNewAppHotkey(int index)
    {
        if (_hotkeyModel.HotKeyIndex == index) return;

        _hotkeyModel.HotKeyIndex = index;
        if (_hotkeyModel.HotKeyIndex != index)
            AppHotkeyIndex = _hotkeyModel.HotKeyIndex;
    }

    [RelayCommand]
    private void AddToCycling(int index)
    {
        var layout = _cycledLayoutsModel.AvailableLayouts[index];
        _cycledLayoutsModel.AddToCycling(layout);
    }

    [RelayCommand]
    private void RemoveFromCycling(int index)
    {
        var layout = _cycledLayoutsModel.CycledLayouts[index];
        _cycledLayoutsModel.RemoveFromCycling(layout);
    }

    [RelayCommand]
    private void MoveUp(int index)
    {
       _cycledLayoutsModel.MoveCyclingLayoutUp(index);
    }

    [RelayCommand]
    private void MoveDown(int index)
    {
        _cycledLayoutsModel.MoveCyclingLayoutDown(index);
    }

    [RelayCommand]
    private void FormClosing()
    {
        //todo ???
    }


    #endregion
    
    private void OnLayoutModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        var property = e.PropertyName;
        
        if (property is not (nameof(_cycledLayoutsModel.CycledLayouts)
            or nameof(_cycledLayoutsModel.AvailableLayouts))) return;

        OnPropertyChanged(nameof(AvailableLayouts));
        OnPropertyChanged(nameof(CycledLayouts));
    }
}