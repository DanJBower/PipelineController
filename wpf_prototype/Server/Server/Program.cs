using Makaretu.Dns;
using MQTTnet;
using MQTTnet.Server;
using ServerInfo;
using System.Net;
using System.Net.NetworkInformation;

using var mqttServer = await StartServer();

try
{
    using var serviceDiscovery = AdvertiseServer();
    Console.WriteLine("Press Enter to exit");
    Console.ReadLine();
}
finally
{
    await StopServer(mqttServer);
}

return;

static async Task<MqttServer> StartServer()
{
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

static ServiceDiscovery AdvertiseServer()
{
    var service = new ServiceProfile(ServerConstants.Name, ServerConstants.ServiceName, ServerConstants.Port);
    var serviceDiscovery = new ServiceDiscovery();
    serviceDiscovery.Advertise(service);
    return serviceDiscovery;
}

static void Log(string message = "")
{
    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fffffff}: {message}");
}
