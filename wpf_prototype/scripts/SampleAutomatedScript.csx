#r "nuget: Makaretu.Dns.Multicast, 0.27.0"
#r "nuget: MQTTnet, 4.3.7.1207"
#r "..\release\ServerInfo.dll"
#r "..\release\Controller.dll"
#r "..\release\CommonClient.dll"

using System.Timers;
using CommonClient;
using MQTTnet;
using ServerInfo;

{
    WriteLine("Attempting to connect to client");
    await using var client = await ClientUtilities.ConnectToClient();
    bool nextBVal = true;

    Timer timer = new();
    timer.Elapsed += async (_, _) =>
    {
        WriteLine($"Setting B to {nextBVal}");
        await client.SetB(nextBVal);
        nextBVal = !nextBVal;
    };
    timer.Interval = 1000;
    timer.Enabled = true;

    WriteLine("Press enter to exit");
    ReadLine();
}
