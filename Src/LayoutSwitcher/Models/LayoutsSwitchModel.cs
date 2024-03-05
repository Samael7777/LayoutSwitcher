using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using LayoutControl;
using LayoutSwitcher.Extensions;

namespace LayoutSwitcher.Models;

public partial class LayoutsSwitchModel : ObservableObject
{
    private readonly ReaderWriterLockSlim _lock;

    private readonly Settings _settings;
    private readonly ChangeLayoutHook _changeLayoutHook;

    [ObservableProperty] 
    private KeyboardLayout _currentLayout;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvailableLayouts))] 
    private KeyboardLayout[] _cyclingLayouts;
    
    public LayoutsSwitchModel(Settings settings, ChangeLayoutHook changeLayoutHook)
    {
        _settings = settings;
        _changeLayoutHook = changeLayoutHook;
        
        _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        _currentLayout = KeyboardLayoutInfo.GetForegroundWindowLayout();
        _cyclingLayouts = _settings.CycledLayouts.FilterFromOrphaned().ToArray();
    }

    public KeyboardLayout[] AvailableLayouts
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return KeyboardLayoutInfo.GetSystemLayouts()
                    .Where(l => !CyclingLayouts.Contains(l)).ToArray();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
    
    public void SetCurrentLayout(KeyboardLayout currentLayout)
    {
        CurrentLayout = currentLayout;
    }

    public void SwitchToNextLayout()
    {
        _lock.EnterReadLock();
        try
        {
            if (CyclingLayouts.Length == 0) return;

            var index = CyclingLayouts.FirstIndexOf(CurrentLayout);
            var nextIndex = index == -1 || index == CyclingLayouts.Length - 1
                ? 0
                : index + 1;
            var target = CyclingLayouts[nextIndex];

            _changeLayoutHook.ChangeLayoutRequest(target);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void AddToCycling(int index)
    {
        _lock.EnterWriteLock();
        try
        {
            var layout = AvailableLayouts[index];
            CyclingLayouts = CyclingLayouts.Insert(layout, CyclingLayouts.Length);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void RemoveFromCycling(int index)
    {
        _lock.EnterWriteLock();
        try
        {
            CyclingLayouts = CyclingLayouts.Remove(index);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void MoveCyclingLayoutUp(int index)
    {
        _lock.EnterWriteLock();
        try
        {
            if (index == 0) return;
            CyclingLayouts = CyclingLayouts.Swap(index - 1, index);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void MoveCyclingLayoutDown(int index)
    {
        _lock.EnterWriteLock();
        try
        {
            if (index >= CyclingLayouts.Length - 1) return;
            CyclingLayouts = CyclingLayouts.Swap(index, index + 1);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void OnSystemLayoutsChanged()
    {
        _lock.EnterWriteLock();
        try
        {
            CyclingLayouts = CyclingLayouts.FilterFromOrphaned().ToArray();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    partial void OnCyclingLayoutsChanged(KeyboardLayout[] value)
    {
        _settings.CycledLayouts = value.ToImmutableList();
    }
}