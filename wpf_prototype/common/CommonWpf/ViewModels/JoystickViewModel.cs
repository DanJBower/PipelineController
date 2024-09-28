using CommunityToolkit.Mvvm.ComponentModel;

namespace CommonWpf.ViewModels;

public partial class JoystickViewModel : ViewModel
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private float _xPosition;

    [ObservableProperty]
    private float _yPosition;

    [ObservableProperty]
    private bool _pressedIn;
}
