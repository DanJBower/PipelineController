using CommonClient;
using CommonWpf.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ControllerPassthroughClient;

public partial class MainViewModel : ViewModel
{
    private readonly Dispatcher _uiDispatcher = Application.Current.Dispatcher;
    private CancellationTokenSource _serverConnectionCancellationTokenSource;

    private void DispatchPropertyChange(Action changeProperties)
    {
        _uiDispatcher.Invoke(changeProperties, DispatcherPriority.Send);
    }

    private PrototypeClient _client;

    private const string ConnectToServerDefaultText = "Connect to Server";

    [ObservableProperty]
    private string _serverConnectionButtonText = ConnectToServerDefaultText;

    [ObservableProperty]
    private bool _debugLight;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleServerConnectionCommand))]
    private ConnectionStatus _serverConnectionStatus;

    [RelayCommand]
    private void OnPreviewKeyDown(KeyEventArgs? keyEventArgs)
    {
        if (keyEventArgs is null)
        {
            return;
        }

        Debug.WriteLine($"Down: {keyEventArgs.Key}");
        keyEventArgs.Handled = true;
    }

    [RelayCommand]
    private void OnPreviewKeyUp(KeyEventArgs? keyEventArgs)
    {
        if (keyEventArgs is null)
        {
            return;
        }

        Debug.WriteLine($"Up: {keyEventArgs.Key}");
    }

    [RelayCommand(CanExecute = nameof(CanToggleServerConnection))]
    private async Task ToggleServerConnection()
    {
        if (ServerConnectionStatus is ConnectionStatus.Disconnected or ConnectionStatus.ServerNotFound or ConnectionStatus.Error)
        {
            if (_serverConnectionCancellationTokenSource is not null)
            {
                _serverConnectionCancellationTokenSource.Dispose();
            }

            _serverConnectionCancellationTokenSource = new();

            ServerConnectionButtonText = "Cancel Server Connection";
            ServerConnectionStatus = ConnectionStatus.Searching;

            string ip;
            int port;

            try
            {
                (ip, port) = await ClientUtilities.FindClient(_serverConnectionCancellationTokenSource.Token);
            }
            catch (ServerNotFoundException)
            {
                ServerConnectionStatus = ConnectionStatus.ServerNotFound;
                ServerConnectionButtonText = ConnectToServerDefaultText;
                return;
            }
            catch (OperationCanceledException)
            {
                ServerConnectionStatus = ConnectionStatus.Disconnected;
                ServerConnectionButtonText = ConnectToServerDefaultText;
                return;
            }

            ServerConnectionStatus = ConnectionStatus.Connecting;

            try
            {
                _client = await ClientUtilities.ConnectToClient(ip, port, _serverConnectionCancellationTokenSource.Token);
            }
            catch (OperationCanceledException e)
            {
                ServerConnectionStatus = ConnectionStatus.Disconnected;
                ServerConnectionButtonText = ConnectToServerDefaultText;
                return;
            }

            ServerConnectionButtonText = "Disconnect from Server";
            ServerConnectionStatus = ConnectionStatus.Connected;
        }
        else if (ServerConnectionStatus is ConnectionStatus.Connected)
        {
            ServerConnectionStatus = ConnectionStatus.Disconnecting;
            await _client.DisposeAsync();
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
                Zero();
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

    private void Zero()
    {
        ControllerViewModel.Start = false;
        ControllerViewModel.Select = false;
        ControllerViewModel.Home = false;
        ControllerViewModel.BigHome = false;
        ControllerViewModel.X = false;
        ControllerViewModel.Y = false;
        ControllerViewModel.A = false;
        ControllerViewModel.B = false;
        ControllerViewModel.Up = false;
        ControllerViewModel.Right = false;
        ControllerViewModel.Down = false;
        ControllerViewModel.Left = false;
        ControllerViewModel.LeftStickX = 0;
        ControllerViewModel.LeftStickY = 0;
        ControllerViewModel.LeftStickIn = false;
        ControllerViewModel.RightStickX = 0;
        ControllerViewModel.RightStickY = 0;
        ControllerViewModel.RightStickIn = false;
        ControllerViewModel.LeftBumper = false;
        ControllerViewModel.LeftTrigger = 0;
        ControllerViewModel.RightBumper = false;
        ControllerViewModel.RightTrigger = 0;
    }
}
