using LayoutSwitcher.Control;

namespace LayoutSwitcher.Models.Interfaces;

public interface ISettings
{
    public IEnumerable<KeyboardLayout> CycledLayout { get; set; }
    public int LayoutToggleHotKeyIndex { get; set; }
    public void Load();
    public void Save();
}