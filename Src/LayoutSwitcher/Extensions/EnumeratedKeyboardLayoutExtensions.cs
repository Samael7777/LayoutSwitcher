using LayoutControl;

namespace LayoutSwitcher.Extensions;

public static class EnumeratedKeyboardLayoutExtensions
{
    public static IEnumerable<KeyboardLayout> FilterFromOrphaned(this IEnumerable<KeyboardLayout> source)
    {
        var systemLayouts = KeyboardLayoutInfo.GetSystemLayouts();
        return source.Where(l => systemLayouts.Contains(l));
    }
}