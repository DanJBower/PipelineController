namespace Controller;

public record ControllerState(
    bool Start = false,
    bool Select = false,
    bool Home = false,
    bool BigHome = false,
    bool X = false,
    bool Y = false,
    bool A = false,
    bool B = false,
    bool Up = false,
    bool Right = false,
    bool Down = false,
    bool Left = false,
    float LeftStickX = 0,
    float LeftStickY = 0,
    bool LeftStickIn = false,
    float RightStickX = 0,
    float RightStickY = 0,
    bool RightStickIn = false,
    bool LeftBumper = false,
    float LeftTrigger = 0,
    bool RightBumper = false,
    float RightTrigger = 0
);

/*

Sample init for copy and paste if needed

new ControllerState(
   Start: false,
   Select: false,
   Home: false,
   BigHome: false,
   X: false,
   Y: false,
   A: false,
   B: false,
   Up: false,
   Right: false,
   Down: false,
   Left: false,
   LeftStickX: 0,
   LeftStickY: 0,
   LeftStickIn: false,
   RightStickX: 0,
   RightStickY: 0,
   RightStickIn: false,
   LeftBumper: false,
   LeftTrigger: 0,
   RightBumper: false,
   RightTrigger: 0
);

Sample Copy

var t1 = new ControllerState();
var t2 = t1 with { B = true };

Sample Deconstruct

var t1 = new ControllerState();
var (start, select, home, bigHome,
     x, y, a, b,
     up, right, down, left,
     leftStickX, leftStickY, leftStickIn,
     rightStickX, rightStickY, rightStickIn,
     leftBumper, leftTrigger,
     rightBumper, rightTrigger) = t1;

*/
