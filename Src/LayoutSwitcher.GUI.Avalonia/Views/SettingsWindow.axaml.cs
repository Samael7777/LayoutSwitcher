using System;
using Avalonia.Controls;
using LayoutSwitcher.GUI.Avalonia.CustomControls;
using LayoutSwitcher.ViewModels.Interfaces;

namespace LayoutSwitcher.GUI.Avalonia.Views;

public partial class SettingsWindow : Window, ISettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();

        HotkeySelector.DropDownClosed += OnDropDownClosed;
    }

    private static void OnDropDownClosed(object? sender, EventArgs e)
    {
        if (sender is not ComboBox cb) return;

        var index = cb.SelectedIndex;
        var command = cb.GetValue(ComboBoxCommand.DropDownClosedCommandProperty);
        if (command == null || !command.CanExecute(index)) return;

        command.Execute(index);
    }
}