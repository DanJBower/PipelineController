using CommonWpf.Attributes;
using ModernWpf;
using System.Diagnostics;
using System.Windows;

namespace ControllerPassthroughClient;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        Debug.WriteLine(typeof(AutoDependencyPropertyAttribute<>).FullName!);

    }

    private void ToggleTheme(object sender, RoutedEventArgs e)
    {
        ThemeManager.Current.ApplicationTheme =
            ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark ?
            ApplicationTheme.Light :
            ApplicationTheme.Dark;
    }
}
