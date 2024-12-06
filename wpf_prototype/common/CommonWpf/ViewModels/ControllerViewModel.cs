using CommonWpf.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CommonWpf.ViewModels;

public partial class ControllerViewModel : ViewModel, IControllerViewModel
{
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
    private string _startTitle = DefaultStartTitle;

    [ObservableProperty]
    private string _selectTitle = DefaultSelectTitle;

    [ObservableProperty]
    private string _homeTitle = DefaultHomeTitle;

    [ObservableProperty]
    private string _bigHomeTitle = DefaultBigHomeTitle;

    [ObservableProperty]
    private string _xTitle = DefaultXTitle;

    [ObservableProperty]
    private string _yTitle = DefaultYTitle;

    [ObservableProperty]
    private string _aTitle = DefaultATitle;

    [ObservableProperty]
    private string _bTitle = DefaultBTitle;

    [ObservableProperty]
    private string _upTitle = DefaultUpTitle;

    [ObservableProperty]
    private string _rightTitle = DefaultRightTitle;

    [ObservableProperty]
    private string _downTitle = DefaultDownTitle;

    [ObservableProperty]
    private string _leftTitle = DefaultLeftTitle;

    [ObservableProperty]
    private string _leftStickTitle = DefaultLeftStickTitle;

    [ObservableProperty]
    private string _leftStickInTitle = DefaultLeftStickInTitle;

    [ObservableProperty]
    private string _rightStickTitle = DefaultRightStickTitle;

    [ObservableProperty]
    private string _rightStickInTitle = DefaultRightStickInTitle;

    [ObservableProperty]
    private string _leftBumperTitle = DefaultLeftBumperTitle;

    [ObservableProperty]
    private string _leftTriggerTitle = DefaultLeftTriggerTitle;

    [ObservableProperty]
    private string _rightBumperTitle = DefaultRightBumperTitle;

    [ObservableProperty]
    private string _rightTriggerTitle = DefaultRightTriggerTitle;

    public const string DefaultStartTitle = "Start";
    public const string DefaultSelectTitle = "Select";
    public const string DefaultHomeTitle = "Home";
    public const string DefaultBigHomeTitle = "Touch\nPad";
    public const string DefaultXTitle = "X";
    public const string DefaultYTitle = "Y";
    public const string DefaultATitle = "A";
    public const string DefaultBTitle = "B";
    public const string DefaultUpTitle = "Up";
    public const string DefaultRightTitle = "Right";
    public const string DefaultDownTitle = "Down";
    public const string DefaultLeftTitle = "Left";
    public const string DefaultLeftStickTitle = "Left Stick";
    public const string DefaultLeftStickInTitle = "L3";
    public const string DefaultRightStickTitle = "Right Stick";
    public const string DefaultRightStickInTitle = "R3";
    public const string DefaultLeftBumperTitle = "LB";
    public const string DefaultLeftTriggerTitle = "LT";
    public const string DefaultRightBumperTitle = "RB";
    public const string DefaultRightTriggerTitle = "RT";
}
