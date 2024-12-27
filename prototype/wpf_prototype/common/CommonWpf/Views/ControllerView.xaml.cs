using SimpleSourceGeneratorAttributes;

namespace CommonWpf.Views;


[AutoDependencyProperty<string>(Name = "UpTitle", DefaultValue = "Up")]
[AutoDependencyProperty<string>(Name = "RightTitle", DefaultValue = "Right")]
[AutoDependencyProperty<string>(Name = "DownTitle", DefaultValue = "Down")]
[AutoDependencyProperty<string>(Name = "LeftTitle", DefaultValue = "Left")]
public partial class ControllerView
{
    public ControllerView()
    {
        InitializeComponent();
    }
}
