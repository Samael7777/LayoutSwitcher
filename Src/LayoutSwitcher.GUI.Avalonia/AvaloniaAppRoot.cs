using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Threading;
using LayoutSwitcher.GUI.Avalonia.Models;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Interfaces;


namespace LayoutSwitcher.GUI.Avalonia;

public class AvaloniaAppRoot<T>() : AppRootBase<T>(Dispatcher.UIThread.Invoke)
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