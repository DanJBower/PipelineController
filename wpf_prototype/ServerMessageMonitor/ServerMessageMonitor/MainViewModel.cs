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
