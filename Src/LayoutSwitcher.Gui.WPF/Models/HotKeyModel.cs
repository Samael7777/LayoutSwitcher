using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LayoutSwitcher.Models.Interfaces;
using NHotkey;
using NHotkey.Wpf;

namespace LayoutSwitcher.Gui.WPF.Models;

public class HotKeyModel : ObservableObject, IHotKeyModel
{
    private const string GestureName = "SwitchLayout";

    private readonly List<KeyGesture> _availableCombinations;

    private int _currentKeyIndex;

    public event EventHandler? HotKeyAlreadyUsed;
    public event EventHandler? HotKeyPressed;
    
    public IList<string> AvailableCombinations =>
        _availableCombinations.Select(k => k.GetDisplayStringForCulture(CultureInfo.InvariantCulture))
            .ToList();

    public int HotKeyIndex
    {
        get => _currentKeyIndex;
        set => SetNewHotKey(value);
    }
    
    public HotKeyModel(IEnumerable<KeyGesture> availableCombinations, int currentIndex)
    {
        _availableCombinations = new List<KeyGesture>
        {
            new (Key.None, ModifierKeys.None, "None") //No combination
        };

        _availableCombinations.AddRange(availableCombinations);

        if (currentIndex < 0 || currentIndex >= _availableCombinations.Count)
            throw new ArgumentOutOfRangeException(nameof(currentIndex));

        HotKeyIndex = currentIndex;
        HotkeyManager.HotkeyAlreadyRegistered += OnAlreadyRegistered;
    }

    private void OnAlreadyRegistered(object? sender, HotkeyAlreadyRegisteredEventArgs e)
    {
        _currentKeyIndex = 0;
        OnPropertyChanged(nameof(HotKeyIndex));

        HotKeyAlreadyUsed?.Invoke(this, EventArgs.Empty);
    }

    private void OnHotKeyPressed(object? sender, HotkeyEventArgs e)
    {
        HotKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    private void SetNewHotKey(int value)
    {
        if (value < 0 || value >= _availableCombinations.Count)
            throw new ArgumentOutOfRangeException(nameof(value));

        if (_currentKeyIndex == value) return;

        _currentKeyIndex = value;
        var gesture = _availableCombinations[value];
        HotkeyManager.Current.Remove(GestureName);
        try
        {
            HotkeyManager.Current.AddOrReplace(GestureName, gesture, true, OnHotKeyPressed);
        }
        catch (HotkeyAlreadyRegisteredException)
        {
            HotKeyIndex = 0;
        }
    }
}