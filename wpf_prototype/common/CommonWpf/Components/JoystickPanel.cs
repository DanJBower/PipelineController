using SimpleSourceGenerators;
using System.Windows;
using System.Windows.Controls;

namespace CommonWpf.Components;

[AutoDependencyProperty<string>(Name = "Title", DefaultValue = "")]
[AutoDependencyProperty<float>(Name = "X", DefaultValue = 0)]
[AutoDependencyProperty<float>(Name = "Y", DefaultValue = 0)]
[AutoDependencyProperty<bool>(Name = "Pressed", DefaultValue = false)]
public partial class JoystickPanel : UserControl
{
    static JoystickPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(JoystickPanel), new FrameworkPropertyMetadata(typeof(JoystickPanel)));
    }
}
