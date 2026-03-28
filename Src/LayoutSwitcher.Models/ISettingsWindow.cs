namespace LayoutSwitcher.Models;

public interface ISettingsWindow
{
    public event EventHandler? Closed;
    public object? DataContext { get; set; }
    public void Show();
}