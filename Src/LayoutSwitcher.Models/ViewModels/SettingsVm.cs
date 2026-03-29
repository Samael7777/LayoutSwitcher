using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LayoutSwitcher.Models.Interfaces;
using LayoutSwitcher.Models.Models;
using LayoutSwitcher.Models.Tools;

namespace LayoutSwitcher.Models.ViewModels;

public partial class SettingsVm : ObservableObject
{
    private readonly AutorunModel _autorunModel;
    private readonly IHotKeyModel _hotkeyModel;
    private readonly CycledLayoutsModel _cycledLayoutsModel;
    private readonly Action<Action>? _uiThreadInvokeAction;

    [ObservableProperty] private int _appHotkeyIndex;
    [ObservableProperty] private IReadOnlyList<string> _availableLayouts = [];
    [ObservableProperty] private IReadOnlyList<string> _cycledLayouts = [];
    
    public IReadOnlyList<string> AvailableHotkeys => _hotkeyModel.AvailableCombinations;

    public bool Autorun
    {
        get => _autorunModel.Autorun;
        set => _autorunModel.Autorun = value;
    }

    public bool AutorunControlEnabled => AccountHelper.IsAdministrator();
    
    public SettingsVm(AppModel appModel, Action<Action>? uiThreadInvokeAction)
    {
        _autorunModel = appModel.AutorunModel;
        _cycledLayoutsModel = appModel.CycledLayoutsModel;
        _hotkeyModel = appModel.HotKeyModel;
        _uiThreadInvokeAction = uiThreadInvokeAction;
        AppHotkeyIndex =_hotkeyModel.HotKeyIndex;
        _cycledLayoutsModel.CycledLayouts.CollectionChanged += 
            (_, _) => UpdateLayoutsLists();

        UpdateLayoutsLists();
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
        _cycledLayoutsModel.AddToCyclingFromAvailableIndex(index);
    }

    [RelayCommand]
    private void RemoveFromCycling(int index)
    {
        _cycledLayoutsModel.RemoveFromCycling(index);
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

    private void UpdateLayoutsLists()
    {
        if (_uiThreadInvokeAction == null)
        {
            UpdateLayoutsListsInner();
            return;
        }
        _uiThreadInvokeAction.Invoke(UpdateLayoutsListsInner);
    }

    private void UpdateLayoutsListsInner()
    {
        AvailableLayouts = _cycledLayoutsModel.AvailableLayouts
            .Select(l => l.LayoutDisplayName).ToImmutableList();

        CycledLayouts = _cycledLayoutsModel.CycledLayouts
            .Select(l => l.LayoutDisplayName).ToImmutableList();

    }
}
