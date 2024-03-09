namespace ControllerInputSampleMauiOnly;

public partial class MainPage
{
    private int _count;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;
        CounterBtn.Text = $"Clicked {_count} time{(_count == 1 ? "" : "s")}";
    }
}
