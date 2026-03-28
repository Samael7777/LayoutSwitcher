using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using LayoutSwitcher.Models.Interfaces;
using NHotkey;
using NHotkey.Avalonia;

namespace LayoutSwitcher.GUI.Avalonia.Models;

public class HotKeyModel : IHotKeyModel
{
    private const string GestureName = "SwitchLayout";
    private readonly List<KeyGesture> _availableCombinations;
    private int _hotKeyIndex;

    public event EventHandler? HotKeyAlreadyUsed;
    public event EventHandler? HotKeyPressed;
    
    public IReadOnlyList<string> AvailableCombinations { get; }

    public int HotKeyIndex
    {
        get => _hotKeyIndex;
        set
        {
            if (value < 0 || value >= _availableCombinations.Count)
                throw new ArgumentOutOfRangeException(nameof(HotKeyIndex));

            if (HotKeyIndex != value)
                _hotKeyIndex = RegisterHotKeySetZeroOnError(value);
        }
    }
    
    public HotKeyModel(IEnumerable<KeyGesture> availableCombinations)
    {
        _availableCombinations = [new KeyGesture(Key.None)];
        _availableCombinations.AddRange(availableCombinations);

        AvailableCombinations = _availableCombinations
            .Select(k=>k.ToString()).ToList();

    }

    private void OnHotKeyPressed(object? sender, HotkeyEventArgs e)
    {
        HotKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    private int RegisterHotKeySetZeroOnError(int value)
    {
        if (value == _hotKeyIndex) return value;

        HotkeyManager.Current.Remove(GestureName);
        if (value == 0) return value;

        var newGesture = _availableCombinations[value];
        try
        {
            HotkeyManager.Current.AddOrReplace(GestureName, newGesture, true, OnHotKeyPressed);
        }
        catch (HotkeyAlreadyRegisteredException)
        {
            value = 0;
            HotKeyAlreadyUsed?.Invoke(this, EventArgs.Empty);
        }

        return value;
    }
}