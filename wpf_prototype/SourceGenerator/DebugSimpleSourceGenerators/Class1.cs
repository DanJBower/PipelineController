using SimpleSourceGenerators;
using System.Windows.Controls;

namespace DebugSimpleSourceGenerators;

[AutoDependencyProperty<string>(Name = "TestProp", DefaultValue = "Hi")]
[AutoDependencyProperty<string>(Name = "TestPropTwo", DefaultValueLiteral = "\"Test\"")]
public partial class SampleControl : UserControl
{

}
