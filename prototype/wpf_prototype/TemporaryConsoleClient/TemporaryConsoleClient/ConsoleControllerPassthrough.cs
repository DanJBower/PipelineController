using CommonClient;
using Controller;
using SDL;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace TemporaryConsoleClient;

public static class ConsoleControllerPassthrough
{
    private static readonly Lock Lock = new();
    private static PrototypeClient Client;
    private static long StartTime;
    private static readonly List<double> Times = [];

    public static async Task RunApproach1()
    {
        Console.WriteLine("Finding and connecting to server");
        await using var client = await ClientUtilities.FindAndConnectToClient();
        Client = client;
        Console.WriteLine("Server connected");
        StartSdl();
        Console.WriteLine("Listening for PS5 controller input");
        Console.WriteLine($"Stopwatch resolution: {Stopwatch.IsHighResolution}");
        Console.WriteLine("Press enter to exit");

        StartTime = Stopwatch.GetTimestamp();

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            await PollPs5Controller();
        }

        StopSdl();
        var totalDuration = Stopwatch.GetElapsedTime(StartTime).TotalSeconds;
        var mps = Times.Count / totalDuration;
        File.WriteAllText("Stats1.log", $"""
                               Duration: {totalDuration:0.00}s
                               Total messages: {Times.Count}
                               Messages per second: {mps:0.00}
                               Times:
                                * {string.Join("\n * ", Times)}
                               """);
    }

    public static async Task RunApproach2()
    {
        Console.WriteLine("Finding and connecting to server");
        await using var client = await ClientUtilities.FindAndConnectToClient();
        Client = client;
        Console.WriteLine("Server connected");
        StartSdl();
        Console.WriteLine("Listening for PS5 controller input");
        Console.WriteLine("Press enter to exit");
        using var timer = new Timer();
        timer.Elapsed += async (_, _) => await PollPs5Controller();
        timer.Interval = 1;
        StartTime = Stopwatch.GetTimestamp();
        timer.Enabled = true;

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
        }

        timer.Enabled = false;
        await Task.Delay(200); // Allow timer to fully stop

        StopSdl();
        var totalDuration = Stopwatch.GetElapsedTime(StartTime).TotalSeconds;
        var mps = Times.Count / totalDuration;
        File.WriteAllText("Stats2.log", $"""
                               Duration: {totalDuration:0.00}s
                               Total messages: {Times.Count}
                               Messages per second: {mps:0.00}
                               Times:
                                * {string.Join("\n * ", Times)}
                               """);
    }

    public static async Task RunApproach3()
    {
        Console.WriteLine("Finding and connecting to server");
        await using var client = await ClientUtilities.FindAndConnectToClient();
        Client = client;
        Console.WriteLine("Server connected");
        StartSdl();
        Console.WriteLine("Listening for PS5 controller input");
        Console.WriteLine("Press enter to exit");
        CancellationTokenSource cts = new();
        StartTime = Stopwatch.GetTimestamp();
        var approach3 = Approach3PollController(cts.Token);

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
        }

        await cts.CancelAsync();
        await Task.Delay(200); // Allow timer to fully stop

        StopSdl();
        var totalDuration = Stopwatch.GetElapsedTime(StartTime).TotalSeconds;
        var mps = Times.Count / totalDuration;
        File.WriteAllText("Stats3.log", $"""
                               Duration: {totalDuration:0.00}s
                               Total messages: {Times.Count}
                               Messages per second: {mps:0.00}
                               Times:
                                * {string.Join("\n * ", Times)}
                               """);
    }

    private static async Task Approach3PollController(CancellationToken token)
    {
        var targetDelay = new TimeSpan(0, 0, 0, 0, 1);
        while (!token.IsCancellationRequested)
        {
            await PollPs5Controller();
            var delay = targetDelay - Stopwatch.GetElapsedTime(StartTime);
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay);
            }
        }
    }

    private static unsafe SDL_Gamepad* _gamepad;

    private static void StartSdl()
    {
        SDL3.SDL_SetHint(SDL3.SDL_HINT_JOYSTICK_THREAD, "1");

        if (!SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_JOYSTICK | SDL_InitFlags.SDL_INIT_GAMEPAD))
        {
            throw new Exception("Couldn't initialise SDL");
        }

        var gamepadId = GetFirstValidControllerId(SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5);

        unsafe
        {
            _gamepad = SDL3.SDL_OpenGamepad(gamepadId);
        }
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

    private static void StopSdl()
    {
        unsafe
        {
            SDL3.SDL_CloseGamepad(_gamepad);
        }

        SDL3.SDL_Quit();
    }

    private static async Task PollPs5Controller()
    {
        var state = GetPs5ControllerState();

        await Client.SetController(state);

        using (Lock.EnterScope())
        {
            Times.Add(Stopwatch.GetElapsedTime(StartTime).TotalMilliseconds);
        }
    }

    private static ControllerState GetPs5ControllerState()
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

    private static float ScaleJoystickInput(double input)
    {
        return (float)Scale(input,
            short.MinValue, short.MaxValue + 1,
            -1, 1);
    }

    private static float ScaleTriggerInput(double input)
    {
        return (float)Scale(input,
            0, short.MaxValue + 1,
            0, 1);
    }

    private static double Scale(double input,
        double oldMin, double oldMax,
        double newMin, double newMax)
    {
        var oldRange = oldMax - oldMin;
        var newRange = newMax - newMin;
        return (((input - oldMin) * newRange) / oldRange) + newMin;
    }
}
