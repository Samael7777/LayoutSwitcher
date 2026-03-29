using LayoutSwitcher.Control;
// ReSharper disable InconsistentNaming

namespace LayoutSwitcher.Models.Settings;

public abstract class BaseSettings
{
    protected IEnumerable<KeyboardLayout> _cycledLayout = [];
    protected int _layoutToggleHotKeyIndex;
    protected bool _isChanged;

    public IEnumerable<KeyboardLayout> CycledLayout
    {
        get => _cycledLayout;
        set
        {
            if (_cycledLayout.SequenceEqual(value)) return;
            
            _cycledLayout = value;
            _isChanged = true;
        }
    }

    public int LayoutToggleHotKeyIndex
    {
        get => _layoutToggleHotKeyIndex;
        set
        {
            if (_layoutToggleHotKeyIndex == value) return;

            _layoutToggleHotKeyIndex = value;
            _isChanged = true;
        }
    }

    public abstract void Load();
    public abstract void Save();
}