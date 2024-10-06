using CommonWpf.Attributes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonWpf.Components;

[AutoDependencyProperty<string>(Name = "GroupTitle", DefaultValue = "")]
[AutoDependencyProperty<string>(Name = "ButtonOneTitle", DefaultValue = "")]
[AutoDependencyProperty<bool>(Name = "ButtonOnePressed", DefaultValue = false)]
[AutoDependencyProperty<SolidColorBrush>(Name = "ButtonOnePressedColor", DefaultValueLiteral = "System.Windows.Media.Brushes.DarkMagenta")]
[AutoDependencyProperty<string>(Name = "ButtonTwoTitle", DefaultValue = "")]
[AutoDependencyProperty<bool>(Name = "ButtonTwoPressed", DefaultValue = false)]
[AutoDependencyProperty<SolidColorBrush>(Name = "ButtonTwoPressedColor", DefaultValueLiteral = "System.Windows.Media.Brushes.DarkMagenta")]
[AutoDependencyProperty<string>(Name = "ButtonThreeTitle", DefaultValue = "")]
[AutoDependencyProperty<bool>(Name = "ButtonThreePressed", DefaultValue = false)]
[AutoDependencyProperty<SolidColorBrush>(Name = "ButtonThreePressedColor", DefaultValueLiteral = "System.Windows.Media.Brushes.DarkMagenta")]
[AutoDependencyProperty<string>(Name = "ButtonFourTitle", DefaultValue = "")]
[AutoDependencyProperty<bool>(Name = "ButtonFourPressed", DefaultValue = false)]
[AutoDependencyProperty<SolidColorBrush>(Name = "ButtonFourPressedColor", DefaultValueLiteral = "System.Windows.Media.Brushes.DarkMagenta")]
public partial class FourButtonPanel : UserControl
{
    static FourButtonPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FourButtonPanel), new FrameworkPropertyMetadata(typeof(FourButtonPanel)));
    }

    partial void TestGen(bool value)
    {
        Debug.WriteLine($"{nameof(FourButtonPanel)}: {value}");
    }
}
