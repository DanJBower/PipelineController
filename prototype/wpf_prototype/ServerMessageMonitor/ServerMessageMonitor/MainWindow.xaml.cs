using ModernWpf;
using System.Windows;

namespace ServerMessageMonitor;

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
}
