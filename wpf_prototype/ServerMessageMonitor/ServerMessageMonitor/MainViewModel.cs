using CommonClient;
using CommonWpf.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controller;
using System.Windows;
using System.Windows.Threading;

namespace ServerMessageMonitor;

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
                _client = await ClientUtilities.ConnectToClient(ip, port, _serverConnectionCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                ServerConnectionStatus = ConnectionStatus.Disconnected;
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
        client.StartUpdated += OnStartUpdated;
        client.SelectUpdated += OnSelectUpdated;
        client.HomeUpdated += OnHomeUpdated;
        client.BigHomeUpdated += OnBigHomeUpdated;
        client.XUpdated += OnXUpdated;
        client.YUpdated += OnYUpdated;
        client.AUpdated += OnAUpdated;
        client.BUpdated += OnBUpdated;
        client.UpUpdated += OnUpUpdated;
        client.RightUpdated += OnRightUpdated;
        client.DownUpdated += OnDownUpdated;
        client.LeftUpdated += OnLeftUpdated;
        client.LeftStickXUpdated += OnLeftStickXUpdated;
        client.LeftStickYUpdated += OnLeftStickYUpdated;
        client.LeftStickInUpdated += OnLeftStickInUpdated;
        client.RightStickXUpdated += OnRightStickXUpdated;
        client.RightStickYUpdated += OnRightStickYUpdated;
        client.RightStickInUpdated += OnRightStickInUpdated;
        client.LeftBumperUpdated += OnLeftBumperUpdated;
        client.LeftTriggerUpdated += OnLeftTriggerUpdated;
        client.RightBumperUpdated += OnRightBumperUpdated;
        client.RightTriggerUpdated += OnRightTriggerUpdated;
        await client.EnableControllerChangeMonitoring();
    }

    private async Task DisableMessageMonitoring(PrototypeClient client)
    {
        await client.DisableControllerChangeMonitoring();
        client.DebugLightUpdated -= OnDebugLightUpdated;
        client.StartUpdated -= OnStartUpdated;
        client.SelectUpdated -= OnSelectUpdated;
        client.HomeUpdated -= OnHomeUpdated;
        client.BigHomeUpdated -= OnBigHomeUpdated;
        client.XUpdated -= OnXUpdated;
        client.YUpdated -= OnYUpdated;
        client.AUpdated -= OnAUpdated;
        client.BUpdated -= OnBUpdated;
        client.UpUpdated -= OnUpUpdated;
        client.RightUpdated -= OnRightUpdated;
        client.DownUpdated -= OnDownUpdated;
        client.LeftUpdated -= OnLeftUpdated;
        client.LeftStickXUpdated -= OnLeftStickXUpdated;
        client.LeftStickYUpdated -= OnLeftStickYUpdated;
        client.LeftStickInUpdated -= OnLeftStickInUpdated;
        client.RightStickXUpdated -= OnRightStickXUpdated;
        client.RightStickYUpdated -= OnRightStickYUpdated;
        client.RightStickInUpdated -= OnRightStickInUpdated;
        client.LeftBumperUpdated -= OnLeftBumperUpdated;
        client.LeftTriggerUpdated -= OnLeftTriggerUpdated;
        client.RightBumperUpdated -= OnRightBumperUpdated;
        client.RightTriggerUpdated -= OnRightTriggerUpdated;
    }

    private void OnDebugLightUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => DebugLight = e.NewValue);
    }

    private void OnStartUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Start = e.NewValue);
    }

    private void OnSelectUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Select = e.NewValue);
    }

    private void OnHomeUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Home = e.NewValue);
    }

    private void OnBigHomeUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.BigHome = e.NewValue);
    }

    private void OnXUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.X = e.NewValue);
    }

    private void OnYUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Y = e.NewValue);
    }

    private void OnAUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.A = e.NewValue);
    }

    private void OnBUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.B = e.NewValue);
    }

    private void OnUpUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Up = e.NewValue);
    }

    private void OnRightUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Right = e.NewValue);
    }

    private void OnDownUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Down = e.NewValue);
    }

    private void OnLeftUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.Left = e.NewValue);
    }

    private void OnLeftStickXUpdated(object? sender, ValueUpdatedEventArgs<float> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftStickX = e.NewValue);
    }

    private void OnLeftStickYUpdated(object? sender, ValueUpdatedEventArgs<float> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftStickY = e.NewValue);
    }

    private void OnLeftStickInUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftStickIn = e.NewValue);
    }

    private void OnRightStickXUpdated(object? sender, ValueUpdatedEventArgs<float> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightStickX = e.NewValue);
    }

    private void OnRightStickYUpdated(object? sender, ValueUpdatedEventArgs<float> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightStickY = e.NewValue);
    }

    private void OnRightStickInUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightStickIn = e.NewValue);
    }

    private void OnLeftBumperUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftBumper = e.NewValue);
    }

    private void OnLeftTriggerUpdated(object? sender, ValueUpdatedEventArgs<float> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.LeftTrigger = e.NewValue);
    }

    private void OnRightBumperUpdated(object? sender, ValueUpdatedEventArgs<bool> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightBumper = e.NewValue);
    }

    private void OnRightTriggerUpdated(object? sender, ValueUpdatedEventArgs<float> e)
    {
        DispatchPropertyChange(() => ControllerViewModel.RightTrigger = e.NewValue);
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
