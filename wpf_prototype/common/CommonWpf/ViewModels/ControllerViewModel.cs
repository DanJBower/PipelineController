using CommonWpf.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace CommonWpf.ViewModels;

public partial class ControllerViewModel : ViewModel, IControllerViewModel
{
    private readonly Dispatcher _uiDispatcher = Application.Current.Dispatcher;
    private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var result = "";

        for (var i = 0; i < length; i++)
        {
            result += chars[Random.Next(chars.Length)];
        }

        return result;
    }

    public ControllerViewModel()
    {
        Timer timer = new();
        timer.Elapsed += (_, _) =>
        {
            _uiDispatcher.Invoke(() =>
            {
                Start = !Start;
                ATitle = RandomString(3);
            }, DispatcherPriority.Send);
        };
        timer.Interval = 1000;
        timer.Enabled = true;
    }

    partial void OnStartChanged(bool oldValue, bool newValue)
    {
        Debug.WriteLine($"{nameof(Start)}: {oldValue} -> {newValue}");
    }

    [ObservableProperty]
    private bool _start;

    [ObservableProperty]
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
    private float _rightTrigger;

    [ObservableProperty]
    private string _startTitle = "Start";

    [ObservableProperty]
    private string _selectTitle = "Select";

    [ObservableProperty]
    private string _homeTitle = "Home";

    [ObservableProperty]
    private string _bigHomeTitle = "Touch\nPad";

    [ObservableProperty]
    private string _xTitle = "X";

    [ObservableProperty]
    private string _yTitle = "Y";

    [ObservableProperty]
    private string _aTitle = "A";

    [ObservableProperty]
    private string _bTitle = "B";

    [ObservableProperty]
    private string _upTitle = "Up";

    [ObservableProperty]
    private string _rightTitle = "Right";

    [ObservableProperty]
    private string _downTitle = "Down";

    [ObservableProperty]
    private string _leftTitle = "Left";

    [ObservableProperty]
    private string _leftStickTitle = "Left Stick";

    [ObservableProperty]
    private string _leftStickInTitle = "L3";

    [ObservableProperty]
    private string _rightStickTitle = "Right Stick";

    [ObservableProperty]
    private string _rightStickInTitle = "R3";

    [ObservableProperty]
    private string _leftBumperTitle = "LB";

    [ObservableProperty]
    private string _leftTriggerTitle = "LT";

    [ObservableProperty]
    private string _rightBumperTitle = "RB";

    [ObservableProperty]
    private string _rightTriggerTitle = "RT";
}
