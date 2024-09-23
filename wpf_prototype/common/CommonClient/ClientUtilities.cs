﻿using Makaretu.Dns;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using ServerInfo;

namespace CommonClient;

// TODO
// * Make it so find client retries every second
// * Pass cancelation token to connect to client so searching for client
//   can be cancelled if wanted

public static class ClientUtilities
{
    public static async Task<PrototypeClient> ConnectToClient()
    {
        var (ip, port) = await FindClient();
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ip, port)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        await mqttClient.ConnectAsync(options);
        return new(mqttClient);
    }

    private static async Task<(string, int)> FindClient()
    {
        var port = 0;
        var tcs = new TaskCompletionSource<(string Address, int Port)>();
        using var mdns = new MulticastService();
        using var serviceDiscovery = new ServiceDiscovery(mdns);

        serviceDiscovery.ServiceInstanceDiscovered += (_, e) =>
        {
            //Console.WriteLine($"service instance '{e.ServiceInstanceName}'");
            mdns.SendQuery(e.ServiceInstanceName, type: DnsType.SRV);
        };

        mdns.AnswerReceived += (_, e) =>
        {
            var servers = e.Message.Answers.OfType<SRVRecord>();
            foreach (var server in servers)
            {
                //Console.WriteLine($"service instance host '{server.Target}' for '{server.Name} is available on port {server.Port}'");
                port = server.Port;
                mdns.SendQuery(server.Target, type: DnsType.A);
            }

            var address = e.Message.Answers.OfType<AddressRecord>().FirstOrDefault();
            if (address is not null)
            {
                var ip = address.Address.ToString();
                tcs.SetResult((ip, port));
            }
        };

        mdns.Start();
        serviceDiscovery.QueryServiceInstances(ServerConstants.ServiceName);

        return await tcs.Task;
    }
}
