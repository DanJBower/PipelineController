using Makaretu.Dns;
using MQTTnet;
using MQTTnet.Server;
using ServerInfo;

using var mqttServer = await StartServer();

try
{
    using var serviceDiscovery = AdvertiseServer();
    Console.WriteLine("Press Enter to exit.");
    Console.ReadLine();
}
finally
{
    await mqttServer.StopAsync();
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
    await mqttServer.StartAsync();
    return mqttServer;
}

static ServiceDiscovery AdvertiseServer()
{
    var service = new ServiceProfile(ServerConstants.Name, ServerConstants.ServiceName, ServerConstants.Port);
    var serviceDiscovery = new ServiceDiscovery();
    serviceDiscovery.Advertise(service);
    return serviceDiscovery;
}
