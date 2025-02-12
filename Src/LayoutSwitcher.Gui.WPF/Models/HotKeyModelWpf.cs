using System.Globalization;
using System.Windows.Input;
using LayoutSwitcher.Models.Interfaces;
using NHotkey;
using NHotkey.Wpf;

namespace LayoutSwitcher.Gui.WPF.Models;

public class HotKeyModelWpf : IHotKeyModel
{
    private const string GestureName = "SwitchLayout";

    private readonly List<KeyGesture> _availableCombinations;

    private int _hotKeyIndex;

    public event EventHandler? HotKeyAlreadyUsed;
    public event EventHandler? HotKeyPressed;
    
    public IList<string> AvailableCombinations =>
        _availableCombinations.Select(k => k.GetDisplayStringForCulture(CultureInfo.InvariantCulture))
            .ToList();

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
    
    public HotKeyModelWpf(IEnumerable<KeyGesture> availableCombinations, int currentIndex)
    {
        _availableCombinations = [new KeyGesture(Key.None, ModifierKeys.None, "None")];
        _availableCombinations.AddRange(availableCombinations);

        if (currentIndex < 0 || currentIndex >= _availableCombinations.Count)
            throw new ArgumentOutOfRangeException(nameof(currentIndex));

        HotKeyIndex = currentIndex;
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