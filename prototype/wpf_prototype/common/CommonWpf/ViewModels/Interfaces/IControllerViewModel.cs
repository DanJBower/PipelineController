namespace CommonWpf.ViewModels.Interfaces;

public interface IControllerViewModel
{
    bool Start { get; }
    bool Select { get; }
    bool Home { get; }
    bool BigHome { get; }
    bool X { get; }
    bool Y { get; }
    bool A { get; }
    bool B { get; }
    bool Up { get; }
    bool Right { get; }
    bool Down { get; }
    bool Left { get; }
    float LeftStickX { get; }
    float LeftStickY { get; }
    bool LeftStickIn { get; }
    float RightStickX { get; }
    float RightStickY { get; }
    bool RightStickIn { get; }
    bool LeftBumper { get; }
    float LeftTrigger { get; }
    bool RightBumper { get; }
    float RightTrigger { get; }

    string StartTitle { get; }
    string SelectTitle { get; }
    string HomeTitle { get; }
    string BigHomeTitle { get; }
    string XTitle { get; }
    string YTitle { get; }
    string ATitle { get; }
    string BTitle { get; }
    string UpTitle { get; }
    string RightTitle { get; }
    string DownTitle { get; }
    string LeftTitle { get; }
    string LeftStickTitle { get; }
    string LeftStickInTitle { get; }
    string RightStickTitle { get; }
    string RightStickInTitle { get; }
    string LeftBumperTitle { get; }
    string LeftTriggerTitle { get; }
    string RightBumperTitle { get; }
    string RightTriggerTitle { get; }
}
