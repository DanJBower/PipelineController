using CommonWpf.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CommonWpf.ViewModels;

public partial class JoystickViewModel : ViewModel, IJoystickViewModel
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private float _xPosition;

    [ObservableProperty]
    private float _yPosition;

    [ObservableProperty]
    private bool _pressed;
}
