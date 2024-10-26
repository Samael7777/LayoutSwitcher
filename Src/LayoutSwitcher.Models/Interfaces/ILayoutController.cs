using LayoutSwitcher.Control;

namespace LayoutSwitcher.Models.Interfaces;

public interface ILayoutController
{
    public KeyboardLayout GetForegroundWindowLayout();
    public IEnumerable<KeyboardLayout> GetSystemLayouts();
    public void ChangeLayoutOnForegroundWindow(KeyboardLayout target);
}