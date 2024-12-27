#r "nuget: MQTTnet, 4.3.7.1207"
#r "nuget: ppy.SDL3-CS, 2024.1128.0"
#r "nuget: SharpHook, 5.3.8"
#r "..\release\ServerInfo.dll"
#r "..\release\Controller.dll"
#r "..\release\CommonClient.dll"

using CommonClient;
using Controller;
using SDL; // Xbox One/PS5 controller
using SharpHook; // Keyboard State
using SharpHook.Native;
using System.Collections.Concurrent;
using System.Threading;
using Timer = System.Timers.Timer;

// Config
const double DefaultMaxSearchForServerSeconds = 10;
const double DefaultControllerUpdateRateHz = 60;

// Parse arguments passed to script to override defaults.
// To specify, use format "-[Name] [Val]"
// Not case sensitive
var configArgs = ParseArgs();
readonly var MaxSearchForServerSeconds = configArgs.GetDoubleArg("SearchTimeout") ?? DefaultMaxSearchForServerSeconds;
readonly var ControllerUpdateRateHz = configArgs.GetDoubleArg("UpdateRate") ?? DefaultControllerUpdateRateHz;

// Program
OutputEncoding = Encoding.UTF8;
await Run().ConfigureAwait(false);

Dictionary<string, string> ParseArgs()
{
    Dictionary<string, string> result = [];

    for (int i = 0; i < Args.Count - 1; i++)
    {
        var arg = Args[i];

        if (arg.StartsWith("-"))
        {
            result[arg[1..].ToUpperInvariant()] = Args[i + 1].ToUpperInvariant();
        }
    }

    return result;
}

static double? GetDoubleArg(this Dictionary<string, string> args, string arg)
{
    arg = arg.ToUpperInvariant();
    if (!args.TryGetValue(arg, out var argRepresentation))
    {
        return null;
    }

    if (double.TryParse(argRepresentation, out var doubleRepresentation))
    {
        return doubleRepresentation;
    }

    return null;
}

static bool? GetBoolArg(this Dictionary<string, string> args, string arg)
{
    arg = arg.ToUpperInvariant();
    if (!args.TryGetValue(arg, out var argRepresentation))
    {
        return null;
    }

    if (argRepresentation.Equals("T", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("True", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("Y", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("1", StringComparison.InvariantCultureIgnoreCase))
    {
        return true;
    }
    else if (argRepresentation.Equals("F", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("False", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("N", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("No", StringComparison.InvariantCultureIgnoreCase) ||
        argRepresentation.Equals("0", StringComparison.InvariantCultureIgnoreCase))
    {
        return false;
    }

    return null;
}

enum InputMode
{
    Zero,
    Keyboard,
    XInput,
    PS5,
}

private static readonly SemaphoreLocker _locker = new();
static InputMode _inputMode = InputMode.Zero;
static readonly ConcurrentDictionary<KeyCode, bool> _keyPressedLookup = [];

async Task Run()
{
    WriteLine("Searching and attempting to connect to server");
    CancellationTokenSource cancellationTokenSource = new();
    cancellationTokenSource.CancelAfter((int)(MaxSearchForServerSeconds * 1000));
    await using var client = await ClientUtilities.FindAndConnectToClient(cancellationTokenSource.Token);

    WriteLine("Server connected");

    using var keyboardListener = StartKeyListener();

    WriteLine();
    WriteLine("Instructions:");
    WriteLine("Press 1 to toggle the debug light");
    WriteLine("Press 2 to set the input mode to zeroed");
    WriteLine("Press 3 to set the input mode to keyboard");
    WriteLine("Press 4 to set the input mode to Xbox One");
    WriteLine("Press 5 to set the input mode to PS5");

    WriteLine();
    WriteLine("When in keyboard mode:");
    WriteLine("Start = B");
    WriteLine("Select = N");
    WriteLine("Home = M");
    WriteLine("BigHome = Space");
    WriteLine("X = T");
    WriteLine("Y = H");
    WriteLine("A = G");
    WriteLine("B = F");
    WriteLine("Up = I");
    WriteLine("Right = L");
    WriteLine("Down = K");
    WriteLine("Left = J");
    WriteLine("Left Stick Position = WASD");
    WriteLine("Left Stick In = Q");
    WriteLine("RightStick Position = \u2191\u2192\u2193\u2190");
    WriteLine("Right Stick In = E");
    WriteLine("Left Bumper = Z");
    WriteLine("Left Trigger 100% = X");
    WriteLine("Right Bumper = C");
    WriteLine("Right Trigger 100% = V");

    WriteLine();
    WriteLine("When in XInput mode");
    WriteLine("Home = M");
    WriteLine("BigHome = Space");
    WriteLine("Rest are done through controller");

    WriteLine();
    WriteLine("When in PS5 mode, it's just the controller");

    WriteLine();
    WriteLine($"Current input mode: Zero");
    WriteLine();
    WriteLine("Press enter to exit");
    WriteLine();

    var controllerUpdateRateMs = 1000.0 / ControllerUpdateRateHz;

    using Timer _controllerPassthroughTimer = new();
    _controllerPassthroughTimer.Interval = controllerUpdateRateMs;
    _controllerPassthroughTimer.Elapsed += async (_, _) =>
    {
        await _locker.LockAsync(async () =>
        {
            ControllerState state = _inputMode switch
            {
                InputMode.Keyboard => GetKeyboardState(),
                InputMode.XInput => GetXboxControllerState(),
                InputMode.PS5 => GetPs5ControllerState(),
                _ => new(),
            };

            await client.SetController(state);
        });
    };
    _controllerPassthroughTimer.Enabled = true;

    var debugLightOn = false;

    // Wait for enter key without displaying hit keys as user might
    // be using keyboard to pass input.
    while (true)
    {
        var keyInfo = ReadKey(intercept: true);

        if (keyInfo.Key == ConsoleKey.Enter)
        {
            _controllerPassthroughTimer.Enabled = false;
            break;
        }

        if (keyInfo.Key == ConsoleKey.D1)
        {
            debugLightOn = !debugLightOn;
            await client.SetDebugLight(debugLightOn).ConfigureAwait(false);
        }
        else if (keyInfo.Key == ConsoleKey.D2 ||
            keyInfo.Key == ConsoleKey.D3 ||
            keyInfo.Key == ConsoleKey.D4 ||
            keyInfo.Key == ConsoleKey.D5)
        {
            await _locker.LockAsync(async () =>
            {
                InputMode newMode = keyInfo.Key switch
                {
                    ConsoleKey.D3 => InputMode.Keyboard,
                    ConsoleKey.D4 => InputMode.XInput,
                    ConsoleKey.D5 => InputMode.PS5,
                    _ => InputMode.Zero,
                };

                if (_inputMode == newMode)
                {
                    return;
                }

                _inputMode = newMode;
                WriteLine($"Changed input mode to {_inputMode}");
                StopSdlController();

                if (keyInfo.Key == ConsoleKey.D4)
                {
                    InitialiseSdlController(SDL_GamepadType.SDL_GAMEPAD_TYPE_XBOXONE);
                }
                else if (keyInfo.Key == ConsoleKey.D5)
                {
                    InitialiseSdlController(SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5);
                }

                await Task.FromResult(() => { });
            });
        }
    }
}

private SimpleGlobalHook StartKeyListener()
{
    var eventSource = new SimpleGlobalHook();

    eventSource.KeyPressed += (_, args) =>
    {
        _keyPressedLookup.AddOrUpdate(args.Data.KeyCode, true, (_, _) => true);
    };

    eventSource.KeyReleased += (_, args) =>
    {
        _keyPressedLookup.AddOrUpdate(args.Data.KeyCode, false, (_, _) => false);
    };

    eventSource.RunAsync();

    return eventSource;
}

ControllerState GetKeyboardState()
{
    var (lX, lY) = HandleKeyboardStick(KeyCode.VcW, KeyCode.VcS, KeyCode.VcD, KeyCode.VcA);
    var (rX, rY) = HandleKeyboardStick(KeyCode.VcUp, KeyCode.VcDown, KeyCode.VcRight, KeyCode.VcLeft);

    return new ControllerState(
        Start: _keyPressedLookup.GetOrAdd(KeyCode.VcB, false),
        Select: _keyPressedLookup.GetOrAdd(KeyCode.VcN, false),
        Home: _keyPressedLookup.GetOrAdd(KeyCode.VcM, false),
        BigHome: _keyPressedLookup.GetOrAdd(KeyCode.VcSpace, false),
        X: _keyPressedLookup.GetOrAdd(KeyCode.VcT, false),
        Y: _keyPressedLookup.GetOrAdd(KeyCode.VcH, false),
        A: _keyPressedLookup.GetOrAdd(KeyCode.VcG, false),
        B: _keyPressedLookup.GetOrAdd(KeyCode.VcF, false),
        Up: _keyPressedLookup.GetOrAdd(KeyCode.VcI, false),
        Right: _keyPressedLookup.GetOrAdd(KeyCode.VcL, false),
        Down: _keyPressedLookup.GetOrAdd(KeyCode.VcK, false),
        Left: _keyPressedLookup.GetOrAdd(KeyCode.VcJ, false),
        LeftStickX: (float)lX,
        LeftStickY: (float)lY,
        LeftStickIn: _keyPressedLookup.GetOrAdd(KeyCode.VcQ, false),
        RightStickX: (float)rX,
        RightStickY: (float)rY,
        RightStickIn: _keyPressedLookup.GetOrAdd(KeyCode.VcE, false),
        LeftBumper: _keyPressedLookup.GetOrAdd(KeyCode.VcZ, false),
        LeftTrigger: _keyPressedLookup.GetOrAdd(KeyCode.VcX, false) ? 1 : 0,
        RightBumper: _keyPressedLookup.GetOrAdd(KeyCode.VcC, false),
        RightTrigger: _keyPressedLookup.GetOrAdd(KeyCode.VcV, false) ? 1 : 0
    );
}

private (double, double) HandleKeyboardStick(KeyCode upKey, KeyCode downKey,
        KeyCode rightKey, KeyCode leftKey)
{
    var x = HandleStickAxis(rightKey, leftKey);
    var y = HandleStickAxis(upKey, downKey);
    var r = Math.Sqrt((x * x) + (y * y));

    if (r > 1)
    {
        x /= r;
        y /= r;
    }

    return (x, y);
}

private unsafe SDL_Gamepad* _gamepad;

private void InitialiseSdlController(SDL_GamepadType searchForGamepadType)
{
    SDL3.SDL_SetHint(SDL3.SDL_HINT_JOYSTICK_THREAD, "1");

    if (!SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_JOYSTICK | SDL_InitFlags.SDL_INIT_GAMEPAD))
    {
        throw new Exception("Couldn't initialise SDL");
    }

    var gamepadId = GetFirstValidControllerId(searchForGamepadType);

    unsafe
    {
        _gamepad = SDL3.SDL_OpenGamepad(gamepadId);
        var name = SDL3.SDL_GetGamepadName(_gamepad);
    }
}

private void StopSdlController()
{
    unsafe
    {
        SDL3.SDL_CloseGamepad(_gamepad);
    }

    SDL3.SDL_Quit();
}

private static SDL_JoystickID GetFirstValidControllerId(SDL_GamepadType searchForGamepadType)
{
    var joystickIds = SDL3.SDL_GetJoysticks();

    if (joystickIds is null || joystickIds.Count == 0)
    {
        throw new Exception("Couldn't find any joysticks");
    }

    for (var i = 0; i < joystickIds.Count; i++)
    {
        var joystickId = joystickIds[i];

        if (!SDL3.SDL_IsGamepad(joystickId))
        {
            continue;
        }

        bool isPs5Joystick;

        unsafe
        {
            var gamepad = SDL3.SDL_OpenGamepad(joystickId);
            var gamepadType = SDL3.SDL_GetGamepadType(gamepad);
            isPs5Joystick = gamepadType == searchForGamepadType;
            SDL3.SDL_CloseGamepad(gamepad);
        }

        if (isPs5Joystick)
        {
            return joystickId;
        }
    }

    throw new Exception("Couldn't find any gamepads from joystick list");
}

ControllerState GetXboxControllerState()
{
    SDL3.SDL_PumpEvents();
    ControllerState state;

    unsafe
    {
        state = new ControllerState(
            Start: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START),
            Select: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK),
            Home: _keyPressedLookup.GetOrAdd(KeyCode.VcM, false) || SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_GUIDE),
            BigHome: _keyPressedLookup.GetOrAdd(KeyCode.VcSpace, false),
            X: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST),
            Y: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH),
            A: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH),
            B: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST),
            Up: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP),
            Right: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT),
            Down: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN),
            Left: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT),
            LeftStickX: ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX)),
            LeftStickY: -ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY)),
            LeftStickIn: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK),
            RightStickX: ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX)),
            RightStickY: -ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY)),
            RightStickIn: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK),
            LeftBumper: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER),
            LeftTrigger: ScaleTriggerInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER)),
            RightBumper: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER),
            RightTrigger: ScaleTriggerInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER))
        );
    }

    return state;
}

ControllerState GetPs5ControllerState()
{
    SDL3.SDL_PumpEvents();
    ControllerState state;

    unsafe
    {
        state = new ControllerState(
            Start: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START),
            Select: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK),
            Home: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_GUIDE),
            BigHome: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_TOUCHPAD),
            X: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST),
            Y: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH),
            A: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH),
            B: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST),
            Up: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP),
            Right: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT),
            Down: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN),
            Left: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT),
            LeftStickX: ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX)),
            LeftStickY: -ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY)),
            LeftStickIn: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK),
            RightStickX: ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX)),
            RightStickY: -ScaleJoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY)),
            RightStickIn: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK),
            LeftBumper: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER),
            LeftTrigger: ScaleTriggerInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER)),
            RightBumper: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER),
            RightTrigger: ScaleTriggerInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER))
        );
    }

    return state;
}

private float ScaleJoystickInput(double input)
{
    return (float)Scale(input,
        short.MinValue, short.MaxValue + 1,
        -1, 1);
}

private float ScaleTriggerInput(double input)
{
    return (float)Scale(input,
        0, short.MaxValue + 1,
        0, 1);
}

private double Scale(double input,
    double oldMin, double oldMax,
    double newMin, double newMax)
{
    var oldRange = oldMax - oldMin;
    var newRange = newMax - newMin;
    return (((input - oldMin) * newRange) / oldRange) + newMin;
}

private double HandleStickAxis(KeyCode positiveKey, KeyCode negativeKey)
{
    var up = _keyPressedLookup.GetOrAdd(positiveKey, false);
    var down = _keyPressedLookup.GetOrAdd(negativeKey, false);

    float x = up switch
    {
        true when !down => 1,
        false when down => -1,
        _ => 0
    };

    return x;
}

public class SemaphoreLocker
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task LockAsync(Func<Task> worker)
    {
        await _semaphore.WaitAsync();
        try
        {
            await worker();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // overloading variant for non-void methods with return type (generic T)
    public async Task<T> LockAsync<T>(Func<Task<T>> worker)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await worker();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
