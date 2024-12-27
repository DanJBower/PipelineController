using CommonWpf.ViewModels.Interfaces;

namespace CommonWpf.ViewModels.Mocks;

public class MockControllerViewModel : IControllerViewModel
{
    public bool Start => true;
    public bool Select => true;
    public bool Home => true;
    public bool BigHome => true;
    public bool X => true;
    public bool Y => true;
    public bool A => true;
    public bool B => true;
    public bool Up => true;
    public bool Right => true;
    public bool Down => true;
    public bool Left => true;
    public float LeftStickX => 0;
    public float LeftStickY => 0;
    public bool LeftStickIn => true;
    public float RightStickX => 0.34f;
    public float RightStickY => -0.2f;
    public bool RightStickIn => false;
    public bool LeftBumper => true;
    public float LeftTrigger => 0;
    public bool RightBumper => true;
    public float RightTrigger => 0.8f;

    public string StartTitle => "Start";
    public string SelectTitle => "Select";
    public string HomeTitle => "Home";
    public string BigHomeTitle => "Touch\nPad";
    public string XTitle => "X";
    public string YTitle => "Y";
    public string ATitle => "A";
    public string BTitle => "B";
    public string UpTitle => "Up";
    public string RightTitle => "Right";
    public string DownTitle => "Down";
    public string LeftTitle => "Left";
    public string LeftStickTitle => "Left Stick";
    public string LeftStickInTitle => "L3";
    public string RightStickTitle => "Right Stick";
    public string RightStickInTitle => "R3";
    public string LeftBumperTitle => "LB";
    public string LeftTriggerTitle => "LT";
    public string RightBumperTitle => "RB";
    public string RightTriggerTitle => "RT";
}
