﻿using ModernWpf;
using System.Windows;

namespace ControllerPassthroughClient;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ToggleTheme(object sender, RoutedEventArgs e)
    {
        ThemeManager.Current.ApplicationTheme =
            ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark ?
            ApplicationTheme.Light :
            ApplicationTheme.Dark;
    }
}
