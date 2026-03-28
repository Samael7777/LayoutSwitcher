using System.Collections.Generic;
using Avalonia.Input;
using LayoutSwitcher.GUI.Avalonia.Models;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Interfaces;


namespace LayoutSwitcher.GUI.Avalonia;

public class AppRoot<T> : AppRootBase<T>
    where T : ISettingsWindow, new()
{ 
    protected override void InitHotKeys(out IHotKeyModel hotKeyModel)
    {
        var availableCombinations = new List<KeyGesture>
        {
            new (Key.None, KeyModifiers.Alt | KeyModifiers.Shift),
            new (Key.None, KeyModifiers.Control | KeyModifiers.Shift)
        };

        hotKeyModel = new HotKeyModel(availableCombinations);
    }
}