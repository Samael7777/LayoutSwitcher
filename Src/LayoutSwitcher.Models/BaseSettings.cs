using LayoutSwitcher.Control;
// ReSharper disable InconsistentNaming

namespace LayoutSwitcher.Models;

public abstract class BaseSettings
{
    protected IEnumerable<KeyboardLayout> _cycledLayout = [];
    protected int layoutToggleHotKeyIndex;
    protected bool isChanged;

    public IEnumerable<KeyboardLayout> CycledLayout
    {
        get => _cycledLayout;
        set
        {
            if (_cycledLayout.SequenceEqual(value)) return;
            
            _cycledLayout = value;
            isChanged = true;
        }
    }

    public int LayoutToggleHotKeyIndex
    {
        get => layoutToggleHotKeyIndex;
        set
        {
            if (layoutToggleHotKeyIndex == value) return;

            layoutToggleHotKeyIndex = value;
            isChanged = true;
        }
    }

    public abstract void Load();
    public abstract void Save();
}