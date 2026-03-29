namespace LayoutSwitcher.Models.Settings;

public interface ISettingsWindow
{
    public event EventHandler? Closed;
    public object? DataContext { get; set; }
    public void Show();
}