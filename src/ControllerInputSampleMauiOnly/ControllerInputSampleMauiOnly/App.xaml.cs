namespace ControllerInputSampleMauiOnly;

public partial class App
{
    public App(Page mainPage)
    {
        InitializeComponent();
        MainPage = mainPage;
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return base.CreateWindow(activationState).MemoizeWindow();
    }
#endif
}
