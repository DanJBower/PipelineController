﻿using CommonClient;
using CommonWpf.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controller;
using MQTTnet.Exceptions;
using SDL;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Gaming.Input;

namespace ControllerPassthroughClient;

public partial class MainViewModel : ViewModel
{
    private readonly Dispatcher _uiDispatcher = Application.Current.Dispatcher;
    private CancellationTokenSource? _serverConnectionCancellationTokenSource;
    private readonly TimerEvent _updateFromXboxControllerEvent;
    private readonly TimerEvent _updateFromPs5ControllerEvent;
    private readonly HighAccuracyTimer _highAccuracyTimer = new();

    private readonly Lock _controllerStateLock = new();
    private ControllerState _controllerState = new();

    public MainViewModel()
    {
        const double controllerUpdateRateHz = 1000;
        Gamepad.GamepadAdded += OnXboxControllerAdded;
        _updateFromXboxControllerEvent = new TimerEvent(UpdateFromXboxController, controllerUpdateRateHz.Hz());
        _updateFromPs5ControllerEvent = new TimerEvent(UpdateFromPs5Controller, controllerUpdateRateHz.Hz());
        _highAccuracyTimer.Start(new TimerEvent(UpdateControllerUi, 60.0.Hz()));
    }

    private void DispatchPropertyChange(Action changeProperties)
    {
        _uiDispatcher.Invoke(changeProperties, DispatcherPriority.Send);
    }

    private PrototypeClient? _client;

    private const string ConnectToServerDefaultText = "Connect to Server";

    [ObservableProperty]
    private string _serverConnectionButtonText = ConnectToServerDefaultText;

    [ObservableProperty]
    private bool _debugLight;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleServerConnectionCommand))]
    private ConnectionStatus _serverConnectionStatus;

    // The preview key down event fires a lot. This dictionary is
    // used to prevent callback behaviour running multiple times
    private readonly ConcurrentDictionary<Key, bool> _keyPressedLookup = [];

    [RelayCommand]
    private void OnWindowClosing()
    {
        Debug.WriteLine("Shutting down");
        _highAccuracyTimer.Dispose();
        StopPs5Controller();
    }

    [RelayCommand]
    private async Task OnPreviewKeyDown(KeyEventArgs? keyEventArgs)
    {
        if (keyEventArgs is null)
        {
            return;
        }

        // Debug.WriteLine($"Down: {keyEventArgs.Key}");
        keyEventArgs.Handled = true;

        // Stop here if keydown has already been triggered
        if (_keyPressedLookup.GetOrAdd(keyEventArgs.Key, false))
        {
            return;
        }
        _keyPressedLookup.AddOrUpdate(keyEventArgs.Key, true, (_, _) => true);

        if (InputMode is not InputMode.Keyboard)
        {
            return;
        }

        await HandleKey(keyEventArgs.Key, true);
    }

    private async Task HandleKey(Key key, bool pressed)
    {
        switch (key)
        {
            case Key.B:
                await UpdateStart(pressed);
                break;

            case Key.N:
                await UpdateSelect(pressed);
                break;

            case Key.M:
                await UpdateHome(pressed);
                break;

            case Key.Space:
                await UpdateBigHome(pressed);
                break;

            case Key.T:
                await UpdateX(pressed);
                break;

            case Key.H:
                await UpdateY(pressed);
                break;

            case Key.G:
                await UpdateA(pressed);
                break;

            case Key.F:
                await UpdateB(pressed);
                break;

            case Key.I:
                await UpdateUp(pressed);
                break;

            case Key.L:
                await UpdateRight(pressed);
                break;

            case Key.K:
                await UpdateDown(pressed);
                break;

            case Key.J:
                await UpdateLeft(pressed);
                break;

            case Key.W or Key.A or Key.S or Key.D:
                await HandleKeyboardStick(Key.W, Key.S, Key.D, Key.A, UpdateLeftStickX, UpdateLeftStickY);
                break;

            case Key.Q:
                await UpdateLeftStickIn(pressed);
                break;

            case Key.Up or Key.Down or Key.Right or Key.Left:
                await HandleKeyboardStick(Key.Up, Key.Down, Key.Right, Key.Left, UpdateRightStickX, UpdateRightStickY);
                break;

            case Key.E:
                await UpdateRightStickIn(pressed);
                break;

            case Key.Z:
                await UpdateLeftBumper(pressed);
                break;

            case Key.X:
                await UpdateLeftTrigger(pressed ? 1 : 0);
                break;

            case Key.C:
                await UpdateRightBumper(pressed);
                break;

            case Key.V:
                await UpdateRightTrigger(pressed ? 1 : 0);
                break;
        }
    }

    private async Task HandleKeyboardStick(Key upKey, Key downKey,
        Key rightKey, Key leftKey,
        Func<float, Task> setX,
        Func<float, Task> setY)
    {
        var x = HandleStickAxis(rightKey, leftKey);
        var y = HandleStickAxis(upKey, downKey);
        var r = Math.Sqrt((x * x) + (y * y));

        if (r > 1)
        {
            x /= r;
            y /= r;
        }

        await setX((float)x);
        await setY((float)y);
    }

    private double HandleStickAxis(Key positiveKey, Key negativeKey)
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

    [RelayCommand]
    private async Task OnPreviewKeyUp(KeyEventArgs? keyEventArgs)
    {
        if (keyEventArgs is null)
        {
            return;
        }

        // Debug.WriteLine($"Up: {keyEventArgs.Key}");
        _keyPressedLookup.AddOrUpdate(keyEventArgs.Key, false, (_, _) => false);

        if (InputMode is not InputMode.Keyboard)
        {
            return;
        }

        await HandleKey(keyEventArgs.Key, false);
    }

    [RelayCommand(CanExecute = nameof(CanToggleServerConnection))]
    private async Task ToggleServerConnection()
    {
        if (ServerConnectionStatus is ConnectionStatus.Disconnected or ConnectionStatus.ServerNotFound or ConnectionStatus.Error)
        {
            _serverConnectionCancellationTokenSource?.Dispose();
            _serverConnectionCancellationTokenSource = new();
            _serverConnectionCancellationTokenSource.CancelAfter(10000);

            ServerConnectionButtonText = "Cancel Server Connection";
            ServerConnectionStatus = ConnectionStatus.Searching;

            string ip;
            int port;

            try
            {
                (ip, port) = await ClientUtilities.FindClient(_serverConnectionCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                ServerConnectionStatus = ConnectionStatus.ServerNotFound;
                ServerConnectionButtonText = ConnectToServerDefaultText;
                return;
            }

            ServerConnectionStatus = ConnectionStatus.Connecting;

            try
            {
                if (!(await ClientUtilities.CheckServerReachable(ip, cancellationToken: _serverConnectionCancellationTokenSource.Token)))
                {
                    ServerConnectionStatus = ConnectionStatus.ServerUnreachable;
                    ServerConnectionButtonText = ConnectToServerDefaultText;
                    return;
                }

                _client = await ClientUtilities.ConnectToClient(ip, port, _serverConnectionCancellationTokenSource.Token);
                await _client.RegisterAliases();
            }
            catch (OperationCanceledException)
            {
                ServerConnectionStatus = ConnectionStatus.Disconnected;
                ServerConnectionButtonText = ConnectToServerDefaultText;
                return;
            }
            catch (MqttCommunicationException)
            {
                ServerConnectionStatus = ConnectionStatus.Error;
                ServerConnectionButtonText = ConnectToServerDefaultText;
                return;
            }

            ServerConnectionButtonText = "Disconnect from Server";
            ServerConnectionStatus = ConnectionStatus.Connected;
        }
        else if (ServerConnectionStatus is ConnectionStatus.Connected)
        {
            ServerConnectionStatus = ConnectionStatus.Disconnecting;
            await _client!.DisposeAsync();
            ServerConnectionStatus = ConnectionStatus.Disconnected;
            ServerConnectionButtonText = ConnectToServerDefaultText;
        }
        else
        {
            if (_serverConnectionCancellationTokenSource is not null)
            {
                await _serverConnectionCancellationTokenSource.CancelAsync().ConfigureAwait(false);
            }
        }
    }

    private bool CanToggleServerConnection()
    {
        return ServerConnectionStatus is not ConnectionStatus.Disconnecting;
    }

    [ObservableProperty]
    private InputMode _inputMode = InputMode.Zero;

    [ObservableProperty]
    private string _inputStatus = "Zeroed";

    partial void OnInputModeChanged(InputMode value)
    {
        InputStatus = value switch
        {
            InputMode.Zero => "Zeroed",
            InputMode.Keyboard => "Keyboard",
            InputMode.XboxController => "XInput",
            InputMode.PlaystationController => "PS5",
            _ => "",
        };

        UpdateKeyboardPressLabels(value);
        _highAccuracyTimer.Stop(_updateFromXboxControllerEvent); // Disable the xbox controller polling
        StopPs5Controller();
        Zero().ConfigureAwait(false); // Zero commands to prevent left over inputs

        switch (value)
        {
            case InputMode.Zero:
            case InputMode.Keyboard:
                break;
            case InputMode.XboxController:
                _xboxController = Gamepad.Gamepads.FirstOrDefault();
                _highAccuracyTimer.Start(_updateFromXboxControllerEvent);
                break;
            case InputMode.PlaystationController:
                InitialisePs5Controller();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }

    private Gamepad? _xboxController;

    private void OnXboxControllerAdded(object? sender, Gamepad e)
    {
        _xboxController = e;

        if (InputMode is InputMode.XboxController)
        {
            _highAccuracyTimer.Start(_updateFromXboxControllerEvent);
        }
    }

    private async Task UpdateFromXboxController()
    {
        if (_xboxController is null)
        {
            return;
        }

        var reading = _xboxController.GetCurrentReading();

        var state = new ControllerState(
            Start: reading.Buttons.HasFlag(GamepadButtons.Menu),
            Select: reading.Buttons.HasFlag(GamepadButtons.View),
            Home: _keyPressedLookup.GetOrAdd(Key.M, false),
            BigHome: _keyPressedLookup.GetOrAdd(Key.Space, false),
            X: reading.Buttons.HasFlag(GamepadButtons.X),
            Y: reading.Buttons.HasFlag(GamepadButtons.Y),
            A: reading.Buttons.HasFlag(GamepadButtons.A),
            B: reading.Buttons.HasFlag(GamepadButtons.B),
            Up: reading.Buttons.HasFlag(GamepadButtons.DPadUp),
            Right: reading.Buttons.HasFlag(GamepadButtons.DPadRight),
            Down: reading.Buttons.HasFlag(GamepadButtons.DPadDown),
            Left: reading.Buttons.HasFlag(GamepadButtons.DPadLeft),
            LeftStickX: (float)reading.LeftThumbstickX,
            LeftStickY: (float)reading.LeftThumbstickY,
            LeftStickIn: reading.Buttons.HasFlag(GamepadButtons.LeftThumbstick),
            RightStickX: (float)reading.RightThumbstickX,
            RightStickY: (float)reading.RightThumbstickY,
            RightStickIn: reading.Buttons.HasFlag(GamepadButtons.RightThumbstick),
            LeftBumper: reading.Buttons.HasFlag(GamepadButtons.LeftShoulder),
            LeftTrigger: (float)reading.LeftTrigger,
            RightBumper: reading.Buttons.HasFlag(GamepadButtons.RightShoulder),
            RightTrigger: (float)reading.RightTrigger
        );

        await UpdateFullController(state);
    }

    [ObservableProperty]
    private ControllerViewModel _controllerViewModel = new();

    private void UpdateKeyboardPressLabels(InputMode inputMode)
    {
        if (inputMode is InputMode.Keyboard)
        {
            ControllerViewModel.StartTitle = $"{ControllerViewModel.DefaultStartTitle} (B)";
            ControllerViewModel.SelectTitle = $"{ControllerViewModel.DefaultSelectTitle} (N)";
            ControllerViewModel.HomeTitle = $"{ControllerViewModel.DefaultHomeTitle} (M)";
            ControllerViewModel.BigHomeTitle = $"{ControllerViewModel.DefaultBigHomeTitle}\n(SPACE)";
            ControllerViewModel.XTitle = $"{ControllerViewModel.DefaultXTitle} (T)";
            ControllerViewModel.YTitle = $"{ControllerViewModel.DefaultYTitle} (H)";
            ControllerViewModel.ATitle = $"{ControllerViewModel.DefaultATitle} (G)";
            ControllerViewModel.BTitle = $"{ControllerViewModel.DefaultBTitle} (F)";
            ControllerViewModel.UpTitle = $"{ControllerViewModel.DefaultUpTitle} (I)";
            ControllerViewModel.RightTitle = $"{ControllerViewModel.DefaultRightTitle} (L)";
            ControllerViewModel.DownTitle = $"{ControllerViewModel.DefaultDownTitle} (K)";
            ControllerViewModel.LeftTitle = $"{ControllerViewModel.DefaultLeftTitle} (J)";
            ControllerViewModel.LeftStickTitle = $"{ControllerViewModel.DefaultLeftStickTitle} (WASD)";
            ControllerViewModel.LeftStickInTitle = $"{ControllerViewModel.DefaultLeftStickInTitle} (Q)";
            ControllerViewModel.RightStickTitle = $"{ControllerViewModel.DefaultRightStickTitle} (\u2191\u2192\u2193\u2190)";
            ControllerViewModel.RightStickInTitle = $"{ControllerViewModel.DefaultRightStickInTitle} (E)";
            ControllerViewModel.LeftBumperTitle = $"{ControllerViewModel.DefaultLeftBumperTitle} (Z)";
            ControllerViewModel.LeftTriggerTitle = $"{ControllerViewModel.DefaultLeftTriggerTitle} (X)";
            ControllerViewModel.RightBumperTitle = $"{ControllerViewModel.DefaultRightBumperTitle} (C)";
            ControllerViewModel.RightTriggerTitle = $"{ControllerViewModel.DefaultRightTriggerTitle} (V)";
        }
        else
        {
            ControllerViewModel.StartTitle = ControllerViewModel.DefaultStartTitle;
            ControllerViewModel.SelectTitle = ControllerViewModel.DefaultSelectTitle;

            ControllerViewModel.HomeTitle = inputMode is InputMode.XboxController ?
                $"{ControllerViewModel.DefaultHomeTitle} (M)" :
                ControllerViewModel.DefaultHomeTitle;

            ControllerViewModel.BigHomeTitle = inputMode is InputMode.XboxController ?
                $"{ControllerViewModel.DefaultBigHomeTitle}\n(SPACE)" :
                ControllerViewModel.DefaultBigHomeTitle;

            ControllerViewModel.XTitle = ControllerViewModel.DefaultXTitle;
            ControllerViewModel.YTitle = ControllerViewModel.DefaultYTitle;
            ControllerViewModel.ATitle = ControllerViewModel.DefaultATitle;
            ControllerViewModel.BTitle = ControllerViewModel.DefaultBTitle;
            ControllerViewModel.UpTitle = ControllerViewModel.DefaultUpTitle;
            ControllerViewModel.RightTitle = ControllerViewModel.DefaultRightTitle;
            ControllerViewModel.DownTitle = ControllerViewModel.DefaultDownTitle;
            ControllerViewModel.LeftTitle = ControllerViewModel.DefaultLeftTitle;
            ControllerViewModel.LeftStickTitle = ControllerViewModel.DefaultLeftStickTitle;
            ControllerViewModel.LeftStickInTitle = ControllerViewModel.DefaultLeftStickInTitle;
            ControllerViewModel.RightStickTitle = ControllerViewModel.DefaultRightStickTitle;
            ControllerViewModel.RightStickInTitle = ControllerViewModel.DefaultRightStickInTitle;
            ControllerViewModel.LeftBumperTitle = ControllerViewModel.DefaultLeftBumperTitle;
            ControllerViewModel.LeftTriggerTitle = ControllerViewModel.DefaultLeftTriggerTitle;
            ControllerViewModel.RightBumperTitle = ControllerViewModel.DefaultRightBumperTitle;
            ControllerViewModel.RightTriggerTitle = ControllerViewModel.DefaultRightTriggerTitle;
        }
    }

    private async Task Zero()
    {
        await UpdateStart(false);
        await UpdateSelect(false);
        await UpdateHome(false);
        await UpdateBigHome(false);
        await UpdateX(false);
        await UpdateY(false);
        await UpdateA(false);
        await UpdateB(false);
        await UpdateUp(false);
        await UpdateRight(false);
        await UpdateDown(false);
        await UpdateLeft(false);
        await UpdateLeftStickX(0);
        await UpdateLeftStickY(0);
        await UpdateLeftStickIn(false);
        await UpdateRightStickX(0);
        await UpdateRightStickY(0);
        await UpdateRightStickIn(false);
        await UpdateLeftBumper(false);
        await UpdateLeftTrigger(0);
        await UpdateRightBumper(false);
        await UpdateRightTrigger(0);
    }

    [RelayCommand]
    private async Task OnDebugLightUpdate(bool debugLight)
    {
        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetDebugLight(debugLight);
        }
    }

    private void WithControllerLock(Action a)
    {
        using (_controllerStateLock.EnterScope())
        {
            a();
        }
    }

    private async Task UpdateFullController(ControllerState state)
    {
        WithControllerLock(() => _controllerState = state);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetController(state, addToMessageQueue: true);
        }
    }

    private async Task UpdateStart(bool start)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Start = start });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetStart(start, addToMessageQueue: true);
        }
    }

    private async Task UpdateSelect(bool select)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Select = select });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetSelect(select, addToMessageQueue: true);
        }
    }

    private async Task UpdateHome(bool home)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Home = home });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetHome(home, addToMessageQueue: true);
        }
    }

    private async Task UpdateBigHome(bool bigHome)
    {
        WithControllerLock(() => _controllerState = _controllerState with { BigHome = bigHome });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetBigHome(bigHome, addToMessageQueue: true);
        }
    }

    private async Task UpdateX(bool x)
    {
        WithControllerLock(() => _controllerState = _controllerState with { X = x });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetX(x, addToMessageQueue: true);
        }
    }

    private async Task UpdateY(bool y)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Y = y });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetY(y, addToMessageQueue: true);
        }
    }

    private async Task UpdateA(bool a)
    {
        WithControllerLock(() => _controllerState = _controllerState with { A = a });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetA(a, addToMessageQueue: true);
        }
    }

    private async Task UpdateB(bool b)
    {
        WithControllerLock(() => _controllerState = _controllerState with { B = b });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetB(b, addToMessageQueue: true);
        }
    }

    private async Task UpdateUp(bool up)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Up = up });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetUp(up, addToMessageQueue: true);
        }
    }

    private async Task UpdateRight(bool right)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Right = right });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRight(right, addToMessageQueue: true);
        }
    }

    private async Task UpdateDown(bool down)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Down = down });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetDown(down, addToMessageQueue: true);
        }
    }

    private async Task UpdateLeft(bool left)
    {
        WithControllerLock(() => _controllerState = _controllerState with { Left = left });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeft(left, addToMessageQueue: true);
        }
    }

    private async Task UpdateLeftStickX(float leftStickX)
    {
        WithControllerLock(() => _controllerState = _controllerState with { LeftStickX = leftStickX });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftStickX(leftStickX, addToMessageQueue: true);
        }
    }

    private async Task UpdateLeftStickY(float leftStickY)
    {
        WithControllerLock(() => _controllerState = _controllerState with { LeftStickY = leftStickY });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftStickY(leftStickY, addToMessageQueue: true);
        }
    }

    private async Task UpdateLeftStickIn(bool leftStickIn)
    {
        WithControllerLock(() => _controllerState = _controllerState with { LeftStickIn = leftStickIn });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftStickIn(leftStickIn, addToMessageQueue: true);
        }
    }

    private async Task UpdateRightStickX(float rightStickX)
    {
        WithControllerLock(() => _controllerState = _controllerState with { RightStickX = rightStickX });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightStickX(rightStickX, addToMessageQueue: true);
        }
    }

    private async Task UpdateRightStickY(float rightStickY)
    {
        WithControllerLock(() => _controllerState = _controllerState with { RightStickY = rightStickY });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightStickY(rightStickY, addToMessageQueue: true);
        }
    }

    private async Task UpdateRightStickIn(bool rightStickIn)
    {
        WithControllerLock(() => _controllerState = _controllerState with { RightStickIn = rightStickIn });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightStickIn(rightStickIn, addToMessageQueue: true);
        }
    }

    private async Task UpdateLeftBumper(bool leftBumper)
    {
        WithControllerLock(() => _controllerState = _controllerState with { LeftBumper = leftBumper });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftBumper(leftBumper, addToMessageQueue: true);
        }
    }

    private async Task UpdateLeftTrigger(float leftTrigger)
    {
        WithControllerLock(() => _controllerState = _controllerState with { LeftTrigger = leftTrigger });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftTrigger(leftTrigger, addToMessageQueue: true);
        }
    }

    private async Task UpdateRightBumper(bool rightBumper)
    {
        WithControllerLock(() => _controllerState = _controllerState with { RightBumper = rightBumper });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightBumper(rightBumper, addToMessageQueue: true);
        }
    }

    private async Task UpdateRightTrigger(float rightTrigger)
    {
        WithControllerLock(() => _controllerState = _controllerState with { RightTrigger = rightTrigger });

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightTrigger(rightTrigger, addToMessageQueue: true);
        }
    }

    private unsafe SDL_Gamepad* _gamepad;

    private void InitialisePs5Controller()
    {
        SDL3.SDL_SetHint(SDL3.SDL_HINT_JOYSTICK_HIDAPI_PS5, "1");
        if (!SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_JOYSTICK | SDL_InitFlags.SDL_INIT_GAMEPAD))
        {
            throw new Exception("Couldn't initialise SDL");
        }

        var gamepadId = GetFirstValidControllerId();

        unsafe
        {
            _gamepad = SDL3.SDL_OpenGamepad(gamepadId);
        }

        _highAccuracyTimer.Start(_updateFromPs5ControllerEvent);
    }

    private void StopPs5Controller()
    {
        _highAccuracyTimer.Stop(_updateFromPs5ControllerEvent);

        unsafe
        {
            SDL3.SDL_CloseGamepad(_gamepad);
        }

        SDL3.SDL_Quit();
    }

    private static SDL_JoystickID GetFirstValidControllerId()
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
                isPs5Joystick = gamepadType is SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5;
                SDL3.SDL_CloseGamepad(gamepad);
            }

            if (isPs5Joystick)
            {
                return joystickId;
            }
        }

        throw new Exception("Couldn't find any gamepads from joystick list");
    }

    private async Task UpdateFromPs5Controller()
    {
        SDL3.SDL_PumpEvents();
        ControllerState state;
        unsafe
        {
            // LeftStickX: (float)ScalePs5JoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX))
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
                LeftStickX: ScalePs5JoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX)),
                LeftStickY: -ScalePs5JoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY)),
                LeftStickIn: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK),
                RightStickX: ScalePs5JoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX)),
                RightStickY: -ScalePs5JoystickInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY)),
                RightStickIn: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK),
                LeftBumper: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER),
                LeftTrigger: ScalePs5TriggerInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER)),
                RightBumper: SDL3.SDL_GetGamepadButton(_gamepad, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER),
                RightTrigger: ScalePs5TriggerInput(SDL3.SDL_GetGamepadAxis(_gamepad, SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER))
            );
        }

        await UpdateFullController(state);
    }

    private float ScalePs5JoystickInput(double input)
    {
        return (float)Scale(input,
            short.MinValue, short.MaxValue + 1,
            -1, 1);
    }

    private float ScalePs5TriggerInput(double input)
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

    private Task UpdateControllerUi()
    {
        ControllerState state;

        using (_controllerStateLock.EnterScope())
        {
            state = _controllerState;
        }

        DispatchPropertyChange(() =>
        {

            var (start, select, home, bigHome,
                x, y, a, b,
                up, right, down, left,
                leftStickX, leftStickY, leftStickIn,
                rightStickX, rightStickY, rightStickIn,
                leftBumper, leftTrigger,
                rightBumper, rightTrigger) = state;

            ControllerViewModel.Start = start;
            ControllerViewModel.Select = select;
            ControllerViewModel.Home = home;
            ControllerViewModel.BigHome = bigHome;
            ControllerViewModel.X = x;
            ControllerViewModel.Y = y;
            ControllerViewModel.A = a;
            ControllerViewModel.B = b;
            ControllerViewModel.Up = up;
            ControllerViewModel.Right = right;
            ControllerViewModel.Down = down;
            ControllerViewModel.Left = left;
            ControllerViewModel.LeftStickX = leftStickX;
            ControllerViewModel.LeftStickY = leftStickY;
            ControllerViewModel.LeftStickIn = leftStickIn;
            ControllerViewModel.RightStickX = rightStickX;
            ControllerViewModel.RightStickY = rightStickY;
            ControllerViewModel.RightStickIn = rightStickIn;
            ControllerViewModel.LeftBumper = leftBumper;
            ControllerViewModel.LeftTrigger = leftTrigger;
            ControllerViewModel.RightBumper = rightBumper;
            ControllerViewModel.RightTrigger = rightTrigger;
        });

        return Task.CompletedTask;
    }
}
