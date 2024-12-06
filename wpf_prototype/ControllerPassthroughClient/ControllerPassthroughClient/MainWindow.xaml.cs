using ModernWpf;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ControllerPassthroughClient;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void ToggleTheme(object sender, RoutedEventArgs e)
    {
        ThemeManager.Current.ApplicationTheme =
            ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark ?
            ApplicationTheme.Light :
            ApplicationTheme.Dark;
    }

    private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        Debug.WriteLine($"Down: {e.Key}");
        e.Handled = true; // Prevent keyboard focus on any elements
    }

    private void MainWindow_OnPreviewKeyUp(object sender, KeyEventArgs e)
    {
        Debug.WriteLine($"Up: {e.Key}");
    }
}
