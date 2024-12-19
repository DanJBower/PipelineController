#r "..\release\ServerInfo.dll"

using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerInfo;

private const int ListenPort = ServerConstants.BroadcastPort;
var IP = new IPEndPoint(IPAddress.Any, ListenPort);

using (UdpClient udpClient = new(ListenPort))
{
    await ListenForUdpBroadcasts(udpClient);

    WriteLine("Press Enter to exit");
    ReadLine();
}

async Task<(string, int)> ListenForUdpBroadcasts(UdpClient udpClient, CancellationToken token = default)
{
    while (true)
    {
        var result = await udpClient.ReceiveAsync(token);
        string message;

        try
        {
            message = $"From {result.RemoteEndPoint.Address} Received {Encoding.UTF8.GetString(result.Buffer)}";
        }
        catch
        {
            continue;
        }

        WriteLine(message);
    }
}
