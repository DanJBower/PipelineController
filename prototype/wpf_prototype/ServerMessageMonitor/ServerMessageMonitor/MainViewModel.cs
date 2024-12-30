using CommonClient;
using CommonWpf.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controller;
using MQTTnet.Exceptions;
using System.Windows;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace ServerMessageMonitor;

public partial class MainViewModel : ViewModel
{
    private readonly Dispatcher _uiDispatcher = Application.Current.Dispatcher;
    private CancellationTokenSource? _serverConnectionCancellationTokenSource;

    private readonly Timer _updateScreenValues;

    public MainViewModel()
    {
        _updateScreenValues = new();
        _updateScreenValues.Elapsed += (_, _) => UpdateValues();
        _updateScreenValues.Interval = 1000.0 / 60.0;
        _updateScreenValues.Enabled = true;
    }

    [RelayCommand]
    private void OnWindowClosing()
    {
        _updateScreenValues.Enabled = false;
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

    [ObservableProperty]
    private ControllerViewModel _controllerViewModel = new();

    [RelayCommand(CanExecute = nameof(CanToggleServerConnection))]
    private async Task ToggleServerConnection()
    {
        if (ServerConnectionStatus is ConnectionStatus.Disconnected or ConnectionStatus.ServerNotFound or ConnectionStatus.Error)
        {
            _serverConnectionCancellationTokenSource?.Dispose();
            _serverConnectionCancellationTokenSource = new();

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

            await EnableMessageMonitoring(_client);

            ServerConnectionButtonText = "Disconnect from Server";
            ServerConnectionStatus = ConnectionStatus.Connected;
        }
        else if (ServerConnectionStatus is ConnectionStatus.Connected)
        {
            ServerConnectionStatus = ConnectionStatus.Disconnecting;
            await DisableMessageMonitoring(_client!);
            await _client!.DisposeAsync();
            Zero();
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

    private async Task EnableMessageMonitoring(PrototypeClient client)
    {
        client.DebugLightUpdated += OnDebugLightUpdated;
        await client.EnableControllerChangeMonitoring();
    }

    private async Task DisableMessageMonitoring(PrototypeClient client)
    {
        await client.DisableControllerChangeMonitoring();
        client.DebugLightUpdated -= OnDebugLightUpdated;
    }

    private void OnDebugLightUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => DebugLight = e.NewValue);
    }

    private void UpdateValues()
    {
        if (_client is null)
        {
            return;
        }

        DispatchPropertyChange(() =>
        {
            ControllerViewModel.Start = _client.ControllerState.Start;
            ControllerViewModel.Select = _client.ControllerState.Select;
            ControllerViewModel.Home = _client.ControllerState.Home;
            ControllerViewModel.BigHome = _client.ControllerState.BigHome;
            ControllerViewModel.X = _client.ControllerState.X;
            ControllerViewModel.Y = _client.ControllerState.Y;
            ControllerViewModel.A = _client.ControllerState.A;
            ControllerViewModel.B = _client.ControllerState.B;
            ControllerViewModel.Up = _client.ControllerState.Up;
            ControllerViewModel.Right = _client.ControllerState.Right;
            ControllerViewModel.Down = _client.ControllerState.Down;
            ControllerViewModel.Left = _client.ControllerState.Left;
            ControllerViewModel.LeftStickX = _client.ControllerState.LeftStickX;
            ControllerViewModel.LeftStickY = _client.ControllerState.LeftStickY;
            ControllerViewModel.LeftStickIn = _client.ControllerState.LeftStickIn;
            ControllerViewModel.RightStickX = _client.ControllerState.RightStickX;
            ControllerViewModel.RightStickY = _client.ControllerState.RightStickY;
            ControllerViewModel.RightStickIn = _client.ControllerState.RightStickIn;
            ControllerViewModel.LeftBumper = _client.ControllerState.LeftBumper;
            ControllerViewModel.LeftTrigger = _client.ControllerState.LeftTrigger;
            ControllerViewModel.RightBumper = _client.ControllerState.RightBumper;
            ControllerViewModel.RightTrigger = _client.ControllerState.RightTrigger;
        });
    }

    private bool CanToggleServerConnection()
    {
        return ServerConnectionStatus is not ConnectionStatus.Disconnecting;
    }

    private void Zero()
    {
        DebugLight = false;
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
