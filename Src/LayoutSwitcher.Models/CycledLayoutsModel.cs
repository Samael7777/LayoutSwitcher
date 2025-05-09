﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LayoutSwitcher.Control;

namespace LayoutSwitcher.Models;

public partial class CycledLayoutsModel : ObservableObject
{
    private readonly ReaderWriterLockSlim _lock;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(AvailableLayouts))]
    private ObservableCollection<KeyboardLayout> _cycledLayouts;

    public KeyboardLayout[] AvailableLayouts =>
        LayoutController.GetSystemLayouts()
            .Where(l => !CycledLayouts.Contains(l))
            .ToArray();

    public CycledLayoutsModel(IEnumerable<KeyboardLayout> cycledLayouts)
    {   
        _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        _cycledLayouts = new ObservableCollection<KeyboardLayout>(cycledLayouts);
        _cycledLayouts.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CycledLayouts));
    }

    public void SwitchToNextLayout()
    {
        var currentLayout = LayoutController.GetForegroundWindowLayout();
        var nextLayout = GetNextLayout(currentLayout);
        LayoutController.ChangeLayoutOnForegroundWindow(nextLayout);
    }
    
    public void AddToCycling(KeyboardLayout layout)
    {
        _lock.EnterWriteLock();
        try
        {
            CycledLayouts.Add(layout);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void RemoveFromCycling(KeyboardLayout layout)
    {
        _lock.EnterWriteLock();
        try
        {   if (CycledLayouts.Contains(layout))
                CycledLayouts.Remove(layout);
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
            
            CycledLayouts.Move(index - 1, index);
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
        var systemLayouts = LayoutController.GetSystemLayouts();
        var cleaned = CycledLayouts.Where(l => systemLayouts.Contains(l));
        CycledLayouts = new ObservableCollection<KeyboardLayout>(cleaned);
    }

    private KeyboardLayout GetNextLayout(KeyboardLayout current)
    {
        _lock.EnterReadLock();
        try
        {
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
}