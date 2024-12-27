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
    WriteLine("Attempting to connect to server");
    await using var client = await ClientUtilities.FindAndConnectToClient();

    bool nextDebugLightValue = true;
    using Timer debugTimer = new();
    debugTimer.Elapsed += async (_, _) =>
    {
        WriteLine($"Setting debug light to {nextDebugLightValue}");
        await client.SetDebugLight(nextDebugLightValue);
        nextDebugLightValue = !nextDebugLightValue;
    };
    debugTimer.Interval = 1500;
    debugTimer.Enabled = true;

    bool nextBVal = true;
    using Timer bTimer = new();
    bTimer.Elapsed += async (_, _) =>
    {
        WriteLine($"Setting B to {nextBVal}");
        await client.SetB(nextBVal);
        nextBVal = !nextBVal;
    };
    bTimer.Interval = 1000;
    bTimer.Enabled = true;

    WriteLine("Press enter to exit");
    ReadLine();
}
