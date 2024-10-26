using LayoutSwitcher.Control;
using LayoutSwitcher.Models.Interfaces;

namespace LayoutSwitcher.Gui.WPF.Adapters;

public class LayoutControllerAdapter : ILayoutController
{
    public KeyboardLayout GetForegroundWindowLayout()
    {
        return LayoutController.GetForegroundWindowLayout();
    }

    public IEnumerable<KeyboardLayout> GetSystemLayouts()
    {
        return LayoutController.GetSystemLayouts();
    }

    public void ChangeLayoutOnForegroundWindow(KeyboardLayout target)
    {
        LayoutController.ChangeLayoutOnForegroundWindow(target);
    }
}