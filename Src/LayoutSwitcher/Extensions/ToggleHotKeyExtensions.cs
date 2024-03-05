using LayoutControl;
using VirtualKeys;

namespace LayoutSwitcher.Extensions;

public static class ToggleHotKeyExtensions
{
    public static HotKey ToHotkey(this ToggleHotKey toggleHotKey)
    {
        return toggleHotKey switch
        {
            ToggleHotKey.AltShift => new HotKey(Modifiers.Alt | Modifiers.Shift | Modifiers.NoRepeat, Key.NOKEY),
            ToggleHotKey.CtrShift => new HotKey(Modifiers.Control | Modifiers.Shift | Modifiers.NoRepeat, Key.NOKEY),
            ToggleHotKey.None => HotKey.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(toggleHotKey), toggleHotKey, null)
        };
    }
}