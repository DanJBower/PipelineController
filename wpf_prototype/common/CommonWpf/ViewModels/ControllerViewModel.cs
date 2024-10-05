using CommonWpf.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Timer = System.Timers.Timer;

namespace CommonWpf.ViewModels;

public partial class ControllerViewModel : ViewModel, IControllerViewModel
{
    public ControllerViewModel()
    {
        Timer timer = new();
        timer.Elapsed += (sender, args) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StartPressed = !StartPressed;
            });
        };
        timer.Interval = 1000;
        timer.Enabled = true;
    }

    [ObservableProperty]
    private bool _startPressed;

    partial void OnStartPressedChanged(bool oldValue, bool newValue)
    {
        Debug.WriteLine($"{nameof(StartPressed)}: {oldValue} -> {newValue}");
    }

    /*[ObservableProperty]
    private bool _select;

    [ObservableProperty]
    private bool _home;

    [ObservableProperty]
    private bool _bigHome;

    [ObservableProperty]
    private bool _x;

    [ObservableProperty]
    private bool _y;

    [ObservableProperty]
    private bool _a;

    [ObservableProperty]
    private bool _b;

    [ObservableProperty]
    private bool _up;

    [ObservableProperty]
    private bool _right;

    [ObservableProperty]
    private bool _down;

    [ObservableProperty]
    private bool _left;

    [ObservableProperty]
    private float _leftStickX;

    [ObservableProperty]
    private float _leftStickY;

    [ObservableProperty]
    private bool _leftStickIn;

    [ObservableProperty]
    private float _rightStickX;

    [ObservableProperty]
    private float _rightStickY;

    [ObservableProperty]
    private bool _rightStickIn;

    [ObservableProperty]
    private bool _leftBumper;

    [ObservableProperty]
    private float _leftTrigger;

    [ObservableProperty]
    private bool _rightBumper;

    [ObservableProperty]
    private float _rightTrigger;*/


}
