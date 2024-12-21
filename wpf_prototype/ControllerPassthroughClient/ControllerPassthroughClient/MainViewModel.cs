using CommonClient;
using CommonWpf.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controller;
using MQTTnet.Exceptions;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ControllerPassthroughClient;

public partial class MainViewModel : ViewModel
{
    private readonly Dispatcher _uiDispatcher = Application.Current.Dispatcher;
    private CancellationTokenSource? _serverConnectionCancellationTokenSource;

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

        if (InputMode is not InputMode.Keyboard &&
            (InputMode is not InputMode.XboxController || keyEventArgs.Key is not Key.Space))
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

        if (InputMode is not InputMode.Keyboard &&
            (InputMode is not InputMode.XboxController || keyEventArgs.Key is not Key.Space))
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
            _ => "",
        };

        UpdateKeyboardPressLabels(value);

        switch (value)
        {
            case InputMode.Zero:
                Zero().ConfigureAwait(false);
                break;
            case InputMode.Keyboard:
                break;
            case InputMode.XboxController:
                break;
            case InputMode.PlaystationController:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
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
            ControllerViewModel.HomeTitle = ControllerViewModel.DefaultHomeTitle;

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

    private async Task UpdateStart(bool start)
    {
        DispatchPropertyChange(() => ControllerViewModel.Start = start);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetStart(start);
        }
    }

    private async Task UpdateSelect(bool select)
    {
        DispatchPropertyChange(() => ControllerViewModel.Select = select);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetSelect(select);
        }
    }

    private async Task UpdateHome(bool home)
    {
        DispatchPropertyChange(() => ControllerViewModel.Home = home);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetHome(home);
        }
    }

    private async Task UpdateBigHome(bool bigHome)
    {
        DispatchPropertyChange(() => ControllerViewModel.BigHome = bigHome);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetBigHome(bigHome);
        }
    }

    private async Task UpdateX(bool x)
    {
        DispatchPropertyChange(() => ControllerViewModel.X = x);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetX(x);
        }
    }

    private async Task UpdateY(bool y)
    {
        DispatchPropertyChange(() => ControllerViewModel.Y = y);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetY(y);
        }
    }

    private async Task UpdateA(bool a)
    {
        DispatchPropertyChange(() => ControllerViewModel.A = a);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetA(a);
        }
    }

    private async Task UpdateB(bool b)
    {
        DispatchPropertyChange(() => ControllerViewModel.B = b);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetB(b);
        }
    }

    private async Task UpdateUp(bool up)
    {
        DispatchPropertyChange(() => ControllerViewModel.Up = up);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetUp(up);
        }
    }

    private async Task UpdateRight(bool right)
    {
        DispatchPropertyChange(() => ControllerViewModel.Right = right);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRight(right);
        }
    }

    private async Task UpdateDown(bool down)
    {
        DispatchPropertyChange(() => ControllerViewModel.Down = down);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetDown(down);
        }
    }

    private async Task UpdateLeft(bool left)
    {
        DispatchPropertyChange(() => ControllerViewModel.Left = left);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeft(left);
        }
    }

    private async Task UpdateLeftStickX(float leftStickX)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftStickX = leftStickX);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftStickX(leftStickX);
        }
    }

    private async Task UpdateLeftStickY(float leftStickY)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftStickY = leftStickY);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftStickY(leftStickY);
        }
    }

    private async Task UpdateLeftStickIn(bool leftStickIn)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftStickIn = leftStickIn);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftStickIn(leftStickIn);
        }
    }

    private async Task UpdateRightStickX(float rightStickX)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightStickX = rightStickX);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightStickX(rightStickX);
        }
    }

    private async Task UpdateRightStickY(float rightStickY)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightStickY = rightStickY);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightStickY(rightStickY);
        }
    }

    private async Task UpdateRightStickIn(bool rightStickIn)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightStickIn = rightStickIn);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightStickIn(rightStickIn);
        }
    }

    private async Task UpdateLeftBumper(bool leftBumper)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftBumper = leftBumper);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftBumper(leftBumper);
        }
    }

    private async Task UpdateLeftTrigger(float leftTrigger)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftTrigger = leftTrigger);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetLeftTrigger(leftTrigger);
        }
    }

    private async Task UpdateRightBumper(bool rightBumper)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightBumper = rightBumper);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightBumper(rightBumper);
        }
    }

    private async Task UpdateRightTrigger(float rightTrigger)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightTrigger = rightTrigger);

        if (_client is not null && ServerConnectionStatus is ConnectionStatus.Connected)
        {
            await _client.SetRightTrigger(rightTrigger);
        }
    }
}
