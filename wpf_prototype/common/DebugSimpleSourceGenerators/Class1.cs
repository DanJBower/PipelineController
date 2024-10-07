using CommonWpf.Attributes;
using System.Windows.Controls;

namespace DebugSimpleSourceGenerators;

[AutoDependencyProperty<string>(Name = "TestProp", DefaultValue = "Hi")]
public partial class SampleControl : UserControl
{

}
