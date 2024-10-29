namespace LayoutSwitcher.ViewModels.Interfaces;

public interface ISettingsWindow
{
    bool IsVisible { get; }
    void Show();
}