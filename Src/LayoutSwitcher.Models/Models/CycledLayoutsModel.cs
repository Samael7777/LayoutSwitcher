using CommunityToolkit.Mvvm.ComponentModel;
using LayoutSwitcher.Control;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable ComplexConditionExpression


namespace LayoutSwitcher.Models.Models;

public partial class CycledLayoutsModel : ObservableObject
{
    private readonly ReaderWriterLockSlim _lock;

    private KeyboardLayout[]? _systemLayoutsCache;

    [ObservableProperty] 
    private ObservableCollection<KeyboardLayout> _cycledLayouts;

    public KeyboardLayout[] AvailableLayouts
    {
        get
        {
            _lock.EnterReadLock();
            try
            { 
                return [.. _systemLayoutsCache?
                    .Where(l => !CycledLayouts.Contains(l)) ?? []];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    public CycledLayoutsModel(IEnumerable<KeyboardLayout> cycledLayouts)
    {
        _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        _cycledLayouts = new ObservableCollection<KeyboardLayout>(cycledLayouts);
        _cycledLayouts.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(CycledLayouts));
            OnPropertyChanged(nameof(AvailableLayouts));
        };

        RebuildSystemLayoutsCache();
    }

    public void SwitchToNextLayout()
    {
        var currentLayout = LayoutController.GetForegroundWindowLayout();
        var nextLayout = GetNextLayout(currentLayout);
        if (currentLayout == nextLayout)
            return;

        LayoutController.ChangeLayoutOnForegroundWindow(nextLayout);
    }
    
    public void AddToCyclingFromAvailableIndex(int index)
    {
        var available = AvailableLayouts;
        if (index < 0 || index >= available.Length)
            return;

        _lock.EnterWriteLock();
        try
        {
            var layout = available[index];
            if (!CycledLayouts.Contains(layout))
                CycledLayouts.Add(layout);
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
            if (index < 0 || index >= CycledLayouts.Count)
                return;

            CycledLayouts.RemoveAt(index);
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
            
            CycledLayouts.Move(index, index - 1);
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
            if (index >=  CycledLayouts.Count - 1) return;
            CycledLayouts.Move(index, index + 1);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void CleanFromOrphanedLayouts()
    {
        _lock.EnterWriteLock();
        try
        {
            RebuildSystemLayoutsCache();
            
            var orphaned = CycledLayouts
                .Where(l => !_systemLayoutsCache.Contains(l)).ToList();
            foreach (var layout in orphaned)
            {
                CycledLayouts.Remove(layout);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
    
    private KeyboardLayout GetNextLayout(KeyboardLayout current)
    {
        _lock.EnterReadLock();
        try
        {
            if (CycledLayouts.Count == 0)
                return current;

            var nextIndex = 0;
            var currentIndex = CycledLayouts.IndexOf(current);
            
            if (currentIndex >= 0)
                nextIndex = currentIndex + 1;

            if(nextIndex >= CycledLayouts.Count)
                nextIndex = 0;

            return CycledLayouts[nextIndex];
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    [MemberNotNull(nameof(_systemLayoutsCache))]
    private void RebuildSystemLayoutsCache()
    {
        _systemLayoutsCache = LayoutController.GetSystemLayouts().ToArray();
    }
}