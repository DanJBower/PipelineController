using SimpleSourceGenerators;
using System.Windows;
using System.Windows.Controls;

namespace CommonWpf.Components;

[AutoDependencyProperty<string>(Name = "TriggerTitle", DefaultValue = "")]
[AutoDependencyProperty<float>(Name = "TriggerValue", DefaultValue = 0)]
public partial class TriggerPanel : UserControl
{
    static TriggerPanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TriggerPanel), new FrameworkPropertyMetadata(typeof(TriggerPanel)));
    }
}
