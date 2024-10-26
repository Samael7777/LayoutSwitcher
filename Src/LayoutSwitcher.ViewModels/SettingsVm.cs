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

    public List<string> AvailableLayouts => _cycledLayoutsModel.AvailableLayouts
        .Select(l=>l.LayoutDisplayName)
        .ToList();

    public List<string> CycledLayouts => _cycledLayoutsModel.CycledLayouts
        .Select(l=>l.LayoutDisplayName)
        .ToList();

    public List<string> AvailableHotkeys => _hotkeyModel.AvailableCombinations.ToList();

    public int AppHotkeyIndex
    {
        get => _hotkeyModel.HotKeyIndex;
        set => _hotkeyModel.HotKeyIndex = value;
    }

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
       
        _cycledLayoutsModel.PropertyChanged += OnLayoutModelsChanged;
    }

    #region Relay commands

    [RelayCommand]
    private void AddToCycling(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        var layout = _cycledLayoutsModel.AvailableLayouts[index];
        _cycledLayoutsModel.AddToCycling(layout);
    }

    [RelayCommand]
    private void RemoveFromCycling(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        var layout = _cycledLayoutsModel.CycledLayouts[index];
        _cycledLayoutsModel.RemoveFromCycling(layout);
    }

    [RelayCommand]
    private void MoveUp(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _cycledLayoutsModel.MoveCyclingLayoutUp(index);
    }

    [RelayCommand]
    private void MoveDown(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _cycledLayoutsModel.MoveCyclingLayoutDown(index);
    }

    [RelayCommand]
    private void SetNewAppHotkey(object indexObj)
    {
        if (indexObj is not int index || index < 0) return;

        _hotkeyModel.HotKeyIndex = index;
    }

    [RelayCommand]
    private void FormClosing()
    {
        //todo ???
    }

    #endregion
    
    private void OnLayoutModelsChanged(object? sender, PropertyChangedEventArgs e)
    {
        var property = e.PropertyName;
        
        if (property is not (nameof(_cycledLayoutsModel.CycledLayouts)
            or nameof(_cycledLayoutsModel.AvailableLayouts))) return;

        OnPropertyChanged(nameof(AvailableLayouts));
        OnPropertyChanged(nameof(CycledLayouts));
    }
}