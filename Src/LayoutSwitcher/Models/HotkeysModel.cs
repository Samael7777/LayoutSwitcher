using CommunityToolkit.Mvvm.ComponentModel;
using HotKeyHook;
using LayoutControl;
using LayoutSwitcher.Extensions;
using VirtualKeys;

namespace LayoutSwitcher.Models;

public partial class HotkeysModel : ObservableObject, IDisposable
{
    private readonly Settings _settings;
    private readonly HotKeyHook.HotKeyHook _hotKeyHook;

    [ObservableProperty] private HotKey _systemToggleHotkey;
    [ObservableProperty] private HotKey[] _availableCombinations;

    public event EventHandler? LayoutToggleRequested;

    public HotkeysModel(Settings settings, HotKeyHook.HotKeyHook hotKeyHook)
    {
        _settings = settings;
        _hotKeyHook = hotKeyHook;
        _hotKeyHook.HotKeyCaptured += OnHotkeyCaptured;
        _systemToggleHotkey = LayoutsHotkeys.GetLayoutToggleHotkey().ToHotkey();

        _availableCombinations = new[]
        {
            ToggleHotKey.None.ToHotkey(),
            ToggleHotKey.CtrShift.ToHotkey(),
            ToggleHotKey.AltShift.ToHotkey()
        };
        if (_settings.AppToggleHotkey != HotKey.Empty)
            _hotKeyHook.AddHotKey(_settings.AppToggleHotkey);
    }

    public void SetAppToggleHotKey(int index)
    {
        if (index < 0 || index >= AvailableCombinations.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        var oldHotkey = _settings.AppToggleHotkey;
        _hotKeyHook.RemoveHotKey(oldHotkey);
        
        var newHotkey = AvailableCombinations[index];
        _hotKeyHook.AddHotKey(newHotkey);
        _settings.AppToggleHotkey = newHotkey;
    }
    
    public void OnSystemToggleHotKeyChanged()
    {
        SystemToggleHotkey = LayoutsHotkeys.GetLayoutToggleHotkey().ToHotkey();
    }

    private void OnHotkeyCaptured(object? sender, HotKeyEventArgs e)
    {
        if (e.HotKey == null || e.HotKey != _settings.AppToggleHotkey) return;
        LayoutToggleRequested?.Invoke(this, EventArgs.Empty);
    }

    #region Dispose

    private bool _disposed;

    ~HotkeysModel()
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
            _hotKeyHook.HotKeyCaptured -= OnHotkeyCaptured;
        }
        //free unmanaged resources (unmanaged objects) and override finalizer
        //set large fields to null

        _disposed = true;
    }

    #endregion
}