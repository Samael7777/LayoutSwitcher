using System.ComponentModel;
using System.Windows;
using LayoutSwitcher.Models.Settings;


namespace LayoutSwitcher.Gui.WPF.Windows;

/// <summary>
/// Логика взаимодействия для SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : ISettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MinHeight = Height;
        MinWidth = Width;
    }
}