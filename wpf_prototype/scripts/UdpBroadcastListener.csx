#r "..\release\ServerInfo.dll"

using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerInfo;

using (UdpClient udpClient = new(ServerConstants.BroadcastPort))
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
            message = $"{DateTime.Now}: From {result.RemoteEndPoint.Address} Received {Encoding.UTF8.GetString(result.Buffer)}";
        }
        catch
        {
            continue;
        }

        WriteLine(message);
    }
}
