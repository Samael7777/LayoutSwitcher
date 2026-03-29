using System.Windows;
using System.Windows.Input;
using LayoutSwitcher.Gui.WPF.Models;
using LayoutSwitcher.Models;
using LayoutSwitcher.Models.Interfaces;

namespace LayoutSwitcher.Gui.WPF;

public class WpfAppRoot<T>() : AppRootBase<T>(Application.Current.Dispatcher.Invoke)
    where T : ISettingsWindow, new()
{
    protected override void InitHotKeys(out IHotKeyModel hotKeyModel)
    {
        var availableCombinations = new List<KeyGesture>
        {
            new (Key.None, ModifierKeys.Alt | ModifierKeys.Shift, "Alt+Shift"),
            new (Key.None, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift")
        };

        hotKeyModel = new HotKeyModelWpf(availableCombinations);
        hotKeyModel.HotKeyAlreadyUsed += ShowAlreadyRegisteredHotKeyMessage;
    }

    private static void ShowAlreadyRegisteredHotKeyMessage(object? sender, EventArgs e)
    {
        MessageBox.Show(@"Selected combination already used in other application.", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}