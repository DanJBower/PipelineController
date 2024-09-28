using Makaretu.Dns;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using ServerInfo;
using Timer = System.Timers.Timer;

namespace CommonClient;

// TODO
// * Make it so find client retries every 2 seconds
// * Pass cancellation token to connect to client so searching for client
//   can be cancelled if wanted

public static class ClientUtilities
{
    public static async Task<PrototypeClient> ConnectToClient(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var (ip, port) = await FindClient(cancellationToken);
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ip, port)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        await mqttClient.ConnectAsync(options, cancellationToken);
        return new(mqttClient);
    }

    private static async Task<(string, int)> FindClient(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var port = 0;
        var tcs = new TaskCompletionSource<(string Address, int Port)>();
        using var mdns = new MulticastService();
        using var serviceDiscovery = new ServiceDiscovery(mdns);

        serviceDiscovery.ServiceInstanceDiscovered += (_, e) =>
        {
            // ReSharper disable once AccessToDisposedClosure
            mdns.SendQuery(e.ServiceInstanceName, type: DnsType.SRV);
        };

        mdns.AnswerReceived += (_, e) =>
        {
            var servers = e.Message.Answers.OfType<SRVRecord>();
            foreach (var server in servers)
            {
                if (server.Name.Labels[0].Equals(ServerConstants.Name))
                {
                    port = server.Port;
                    // ReSharper disable once AccessToDisposedClosure
                    mdns.SendQuery(server.Target, type: DnsType.A);
                }
            }

            var address = e.Message.Answers.OfType<AddressRecord>().FirstOrDefault();
            if (address is not null)
            {
                var ip = address.Address.ToString();
                tcs.SetResult((ip, port));
            }
        };

        mdns.Start();

        using Timer timer = new();
        timer.Elapsed += (_, _) =>
        {
            serviceDiscovery.QueryServiceInstances(ServerConstants.ServiceName);
        };
        timer.Interval = 1000;
        timer.Enabled = true;

        serviceDiscovery.QueryServiceInstances(ServerConstants.ServiceName);

        await using (cancellationToken.Register(() => { tcs.TrySetCanceled(); }))
        {
            return await tcs.Task;
        }
    }
}
