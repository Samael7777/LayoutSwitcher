using CommunityToolkit.Mvvm.Input;

namespace LayoutSwitcher.Models.ViewModels;

public partial class TrayViewModel 
{
    private readonly Action? _showSettingsWindow;
    private readonly Action? _shutdownAction;

    public TrayViewModel(Action? showSettingsWindow, Action? shutdownAction)
    {
        _showSettingsWindow = showSettingsWindow;
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
        _showSettingsWindow?.Invoke();
    }
}