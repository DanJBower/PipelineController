namespace Controller;

public enum ConnectionStatus
{
    Disconnected,
    Disconnecting,
    Searching,
    ServerNotFound,
    ServerUnreachable,
    Connecting,
    Connected,
    Error,
}
