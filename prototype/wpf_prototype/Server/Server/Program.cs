using Controller;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Server;
using ServerInfo;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Timer = System.Timers.Timer;

using var mqttServer = await StartServer();

try
{
    await SetInitialValues();
    var (udpClient, timer) = AdvertiseServer();
    using var _1 = udpClient;
    using var _3 = timer;
    Console.WriteLine("Press Enter to exit");
    Console.ReadLine();
    udpClient.Close();
    timer.Enabled = false;
}
finally
{
    await StopServer(mqttServer);
}

return;

static async Task<MqttServer> StartServer()
{
    Console.WriteLine("Starting Server");
    var mqttServerFactory = new MqttFactory();
    var mqttServerOptions = new MqttServerOptionsBuilder()
        .WithDefaultEndpoint()
        .WithDefaultEndpointBoundIPAddress(GetHostIpAddress())
        .WithDefaultEndpointPort(ServerConstants.Port)
        .Build();
    var mqttServer = mqttServerFactory.CreateMqttServer(mqttServerOptions);
    mqttServer.StartedAsync += MqttServerOnStartedAsync;
    mqttServer.StoppedAsync += MqttServerOnStoppedAsync;
    mqttServer.ClientConnectedAsync += MqttServerOnClientConnectedAsync;
    mqttServer.ClientDisconnectedAsync += MqttServerOnClientDisconnectedAsync;
    await mqttServer.StartAsync();
    return mqttServer;
}

static async Task StopServer(MqttServer server)
{
    await server.StopAsync();
    server.StartedAsync -= MqttServerOnStartedAsync;
    server.StoppedAsync -= MqttServerOnStoppedAsync;
    server.ClientConnectedAsync -= MqttServerOnClientConnectedAsync;
    server.ClientDisconnectedAsync -= MqttServerOnClientDisconnectedAsync;
}

static async Task MqttServerOnStartedAsync(EventArgs _)
{
    var localIp = GetHostIp();
    Log($"Started server on {localIp}:{ServerConstants.Port}");
    await Task.CompletedTask;
}

static async Task MqttServerOnStoppedAsync(EventArgs _)
{
    Log("Server Stopped");
    await Task.CompletedTask;
}

static async Task MqttServerOnClientConnectedAsync(ClientConnectedEventArgs args)
{
    Log($"Client connected: {args.ClientId}");
    await Task.CompletedTask;
}

static async Task MqttServerOnClientDisconnectedAsync(ClientDisconnectedEventArgs args)
{
    Log($"Client disconnected: {args.ClientId}");
    await Task.CompletedTask;
}

static (UdpClient, Timer) AdvertiseServer()
{
    var udpClient = new UdpClient(ServerConstants.BroadcastSourcePort);
    udpClient.EnableBroadcast = true;
    var localAddress = GetHostIpAddress().GetAddressBytes();

    if (localAddress.Length != 4)
    {
        throw new Exception("Didn't get an IPV4 address - Help!");
    }

    var broadcastAddress = localAddress[0] switch
    {
        192 when localAddress[1] == 168 => new IPAddress([192, 168, localAddress[2], 255]),
        // 172 when localAddress[1] >= 16 && localAddress[1] <= 31 => new IPAddress([172, 31, 255, 255]),
        // 10 => new IPAddress([10, 255, 255, 255]),
        _ => IPAddress.Broadcast
    };

    var broadcastEndPoint = new IPEndPoint(broadcastAddress, ServerConstants.BroadcastDestinationPort);
    var message = $"{ServerConstants.Name}{ServerConstants.ServiceName}@{GetHostIp()}:{ServerConstants.Port}";

    Timer timer = new();
    timer.Elapsed += (_, _) =>
    {
        var data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, broadcastEndPoint);
        // Console.WriteLine($"Broadcasted: {message}");
    };
    timer.Interval = ServerConstants.BroadcastIntervalMs;
    timer.Enabled = true;

    return (udpClient, timer);
}

static IPAddress GetHostIpAddress()
{
    // https://stackoverflow.com/a/27376368/4601149
    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
    socket.Connect("8.8.8.8", 65530);
    var endPoint = (IPEndPoint)socket.LocalEndPoint!;
    return endPoint.Address;
}

static string GetHostIp()
{
    return GetHostIpAddress().ToString();
}

/*static (MulticastService, ServiceDiscovery) AdvertiseServer()
{
    var mdns = new MulticastService();
    var service = new ServiceProfile(ServerConstants.Name, ServerConstants.ServiceName, ServerConstants.Port);
    var serviceDiscovery = new ServiceDiscovery(mdns);
    serviceDiscovery.Advertise(service);
    mdns.Start();
    return (mdns, serviceDiscovery);
}*/

static void Log(string message = "")
{
    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fffffff}: {message}");
}


// This was originally called set topic aliases, but it
// turns out topic aliases are client specific. The aliases
// are still included in the received message, so as long
// as both clients are using aliases, and using the same
// aliases, it is possible to use aliases to check what the
// topic is.
static async Task SetInitialValues()
{
    Console.WriteLine("Registering Topic Aliases");
    var defaultValues = new Dictionary<int, byte[]>
    {
        {Topics.StartTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.SelectTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.HomeTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.BigHomeTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.XTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.YTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.ATopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.BTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.UpTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.RightTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.DownTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.LeftTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.LeftStickXTopicAlias, ServerDataConverter.ExtractBytes(0f)},
        {Topics.LeftStickYTopicAlias, ServerDataConverter.ExtractBytes(0f)},
        {Topics.LeftStickInTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.RightStickXTopicAlias, ServerDataConverter.ExtractBytes(0f)},
        {Topics.RightStickYTopicAlias, ServerDataConverter.ExtractBytes(0f)},
        {Topics.RightStickInTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.LeftBumperTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.LeftTriggerTopicAlias, ServerDataConverter.ExtractBytes(0f)},
        {Topics.RightBumperTopicAlias, ServerDataConverter.ExtractBytes(false)},
        {Topics.RightTriggerTopicAlias, ServerDataConverter.ExtractBytes(0f)},
        {Topics.LeftStickTopicAlias, ServerDataConverter.ExtractBytes((0f, 0f))},
        {Topics.RightStickTopicAlias, ServerDataConverter.ExtractBytes((0f, 0f))},
        {Topics.FullTopicAlias, ServerDataConverter.ExtractBytes(new ControllerState())},
        {Topics.DebugLightTopicAlias, ServerDataConverter.ExtractBytes(false)},
    };

    var mqttFactory = new MqttFactory();
    using var client = mqttFactory.CreateMqttClient();
    var clientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer(GetHostIp(), ServerConstants.Port)
        .WithProtocolVersion(MqttProtocolVersion.V500)
        .Build();
    await client.ConnectAsync(clientOptions);

    foreach (var (alias, topic) in Topics.AliasedTopics)
    {
        await SetInitialValue(client, alias, topic, defaultValues[alias]);
    }
}

static async Task SetInitialValue(IMqttClient client, ushort alias, string topic, byte[] defaultValue)
{
    var message = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(defaultValue)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages || alias == Topics.DebugLightTopicAlias)
        .WithTopicAlias(alias)
        .Build();

    await client.PublishAsync(message);
}
