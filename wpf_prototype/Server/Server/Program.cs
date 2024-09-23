using Makaretu.Dns;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet.Server;
using ServerInfo;
using System.Net;
using System.Net.NetworkInformation;

using var mqttServer = await StartServer();

try
{
    await RegisterAliases();
    var (mdns, serviceDiscovery) = AdvertiseServer();
    using var _1 = mdns;
    using var _2 = serviceDiscovery;
    Console.WriteLine("Press Enter to exit");
    Console.ReadLine();
    mdns.Stop();
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
    var localIp = GetLocalIpAddress();
    Log($"Started server on {localIp}:{ServerConstants.Port}");
    await Task.CompletedTask;
}

static string GetLocalIpAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    var ip = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
    return ip?.ToString() ?? throw new NetworkInformationException();
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

static (MulticastService, ServiceDiscovery) AdvertiseServer()
{
    var mdns = new MulticastService();
    var service = new ServiceProfile(ServerConstants.Name, ServerConstants.ServiceName, ServerConstants.Port);
    var serviceDiscovery = new ServiceDiscovery(mdns);
    serviceDiscovery.Advertise(service);
    mdns.Start();
    return (mdns, serviceDiscovery);
}

static void Log(string message = "")
{
    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fffffff}: {message}");
}

static async Task RegisterAliases()
{
    Console.WriteLine("Registering Topic Aliases");
    var defaultValues = new Dictionary<int, byte[]>
    {
        {Topics.StartTopicAlias, BitConverter.GetBytes(false)},
        {Topics.SelectTopicAlias, BitConverter.GetBytes(false)},
        {Topics.HomeTopicAlias, BitConverter.GetBytes(false)},
        {Topics.BigHomeTopicAlias, BitConverter.GetBytes(false)},
        {Topics.XTopicAlias, BitConverter.GetBytes(false)},
        {Topics.YTopicAlias, BitConverter.GetBytes(false)},
        {Topics.ATopicAlias, BitConverter.GetBytes(false)},
        {Topics.BTopicAlias, BitConverter.GetBytes(false)},
        {Topics.UpTopicAlias, BitConverter.GetBytes(false)},
        {Topics.RightTopicAlias, BitConverter.GetBytes(false)},
        {Topics.DownTopicAlias, BitConverter.GetBytes(false)},
        {Topics.LeftTopicAlias, BitConverter.GetBytes(false)},
        {Topics.LeftStickXTopicAlias, BitConverter.GetBytes(0f)},
        {Topics.LeftStickYTopicAlias, BitConverter.GetBytes(0f)},
        {Topics.LeftStickInTopicAlias, BitConverter.GetBytes(false)},
        {Topics.RightStickXTopicAlias, BitConverter.GetBytes(0f)},
        {Topics.RightStickYTopicAlias, BitConverter.GetBytes(0f)},
        {Topics.RightStickInTopicAlias, BitConverter.GetBytes(false)},
        {Topics.LeftBumperTopicAlias, BitConverter.GetBytes(false)},
        {Topics.LeftTriggerTopicAlias, BitConverter.GetBytes(0f)},
        {Topics.RightBumperTopicAlias, BitConverter.GetBytes(false)},
        {Topics.RightTriggerTopicAlias, BitConverter.GetBytes(0f)},
    };

    var mqttFactory = new MqttFactory();
    using var client = mqttFactory.CreateMqttClient();
    var clientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer(GetLocalIpAddress(), ServerConstants.Port)
        .WithProtocolVersion(MqttProtocolVersion.V500)
        .Build();
    await client.ConnectAsync(clientOptions);

    foreach (var (alias, topic) in Topics.AliasedTopics)
    {
        await RegisterAlias(client, alias, topic, defaultValues[alias]);
    }
}

static async Task RegisterAlias(IMqttClient client, ushort alias, string topic, byte[] defaultValue)
{
    var message = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload(defaultValue)
        .WithUserProperty("timestamp", $"{DateTime.Now.Ticks}")
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag()
        .WithTopicAlias(alias)
        .Build();

    await client.PublishAsync(message);
}
