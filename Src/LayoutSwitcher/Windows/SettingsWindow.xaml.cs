﻿using System.ComponentModel;
using System.Windows;

namespace LayoutSwitcher.Windows;

/// <summary>
/// Логика взаимодействия для SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow 
{
    public SettingsWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MinHeight = Height;
        MinWidth = Width;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
        base.OnClosing(e);
    }
}