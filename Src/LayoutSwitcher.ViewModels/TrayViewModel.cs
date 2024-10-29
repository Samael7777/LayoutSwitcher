using CommunityToolkit.Mvvm.Input;
using LayoutSwitcher.ViewModels.Interfaces;

namespace LayoutSwitcher.ViewModels;

public partial class TrayViewModel 
{
    private readonly ISettingsWindow _settingsWindow;
    private readonly Action? _shutdownAction;

    public TrayViewModel(ISettingsWindow settingsWindow, Action? shutdownAction)
    {
        _settingsWindow = settingsWindow;
        _shutdownAction = shutdownAction;
    }

    [RelayCommand]
    private void Exit()
    {
        _shutdownAction?.Invoke();
    }

    [RelayCommand]
    private void Settings()
    {
        if (!_settingsWindow.IsVisible)
        {
            _settingsWindow.Show();
        }
    }
}