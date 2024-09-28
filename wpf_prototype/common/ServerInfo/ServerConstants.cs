namespace ServerInfo;

public static class ServerConstants
{
    public const string Name = "PrototypeMqttServer";
    public const string ServiceName = "_mqtt._tcp";
    public const int Port = 1883; // 1883 is the MQTT standard port

    /// <summary>
    /// Whether clients get the last good message automatically when they
    /// subscribe to the topics.
    /// </summary>
    public const bool RetainLastGoodMessages = false;
}
