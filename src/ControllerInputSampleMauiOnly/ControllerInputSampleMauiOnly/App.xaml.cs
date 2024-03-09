namespace ControllerInputSampleMauiOnly;

public partial class App
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return base.CreateWindow(activationState).MemoizeWindow();
    }
#endif
}
