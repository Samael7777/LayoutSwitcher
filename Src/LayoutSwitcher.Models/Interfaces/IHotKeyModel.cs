namespace LayoutSwitcher.Models.Interfaces;

public interface IHotKeyModel
{
    public event EventHandler? HotKeyAlreadyUsed;
    public event EventHandler? HotKeyPressed;
    public IReadOnlyList<string> AvailableCombinations { get; }
    public int HotKeyIndex { get; set; }
}