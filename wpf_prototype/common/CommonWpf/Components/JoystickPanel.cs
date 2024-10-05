using SimpleSourceGenerators;
using System.Windows;
using System.Windows.Controls;

namespace CommonWpf.Components;

[AutoDependencyProperty<string>(Name = "Title", DefaultValue = "")]
/*[AutoDependencyProperty<float>(Name = "X")]
[AutoDependencyProperty<float>(Name = "Y")]
[AutoDependencyProperty<bool>(Name = "Pressed")]*/
public partial class JoystickPanel : UserControl
{
    static JoystickPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(JoystickPanel), new FrameworkPropertyMetadata(typeof(JoystickPanel)));
    }
}
