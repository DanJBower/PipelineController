namespace ServerInfo;

public static class ServerConstants
{
    public const string Name = "PrototypeMqttServer";
    public const string ServiceName = "_mqtt._tcp";
    public const int Port = 1883; // 1883 is the MQTT standard port

    public const int BroadcastSourcePort = 8914; // Made up number
    public const int BroadcastDestinationPort = 8913; // Made up number
    public const int BroadcastIntervalMs = 1000; // Broadcast the server address over UDP interval in milliseconds

    /// <summary>
    /// Whether clients get the last good message automatically when they
    /// subscribe to the topics.
    /// </summary>
    public const bool RetainLastGoodMessages = false;
}
