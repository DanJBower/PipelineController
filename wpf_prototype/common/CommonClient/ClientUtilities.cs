using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using ServerInfo;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonClient;

// TODO
// * Make it so find client retries every 2 seconds
// * Pass cancellation token to connect to client so searching for client
//   can be cancelled if wanted

public static partial class ClientUtilities
{
    public static async Task<PrototypeClient> FindAndConnectToClient(CancellationToken cancellationToken = default)
    {
        var (ip, port) = await FindClient(cancellationToken);
        var mqttClient = await ConnectToClient(ip, port, cancellationToken);
        return mqttClient;
    }

    public static async Task<PrototypeClient> ConnectToClient(
        string ip,
        int port,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ip, port)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();

        cancellationToken.ThrowIfCancellationRequested();
        await mqttClient.ConnectAsync(options, cancellationToken);
        return new(mqttClient);
    }

    [GeneratedRegex("^" + ServerConstants.Name + ServerConstants.ServiceName + @"@((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}:(\d+)" + "$")]
    private static partial Regex ServerIpRegex();

    /// <summary>
    /// Please note, this likely requires a rule to be set up on windows firewall for both inbound and outbound traffic
    /// Set a rule up for domain + private networks to allow port <see cref="ServerConstants.BroadcastPort"/> on UDP
    /// I chose "Allow the connection" when setting up mine
    /// Really important! Make sure your wifi is set to private and not public... I lost hours to this
    /// </summary>
    public static async Task<(string, int)> FindClient(CancellationToken cancellationToken = default)
    {
        using UdpClient udpClient = new(ServerConstants.BroadcastPort);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await udpClient.ReceiveAsync(cancellationToken);
            string message;

            try
            {
                message = Encoding.UTF8.GetString(result.Buffer);
            }
            catch
            {
                continue;
            }

            var matchInfo = ServerIpRegex().Match(message);

            if (matchInfo.Success)
            {
                var ip = string.Join("", matchInfo.Groups[1].Captures.Select(x => x.Value));
                var port = int.Parse(matchInfo.Groups[4].Value);

                return (ip, port);
            }
        }
    }
}
