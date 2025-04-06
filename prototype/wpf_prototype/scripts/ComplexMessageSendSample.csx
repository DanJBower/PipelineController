#r "nuget: Makaretu.Dns.Multicast, 0.27.0"
#r "nuget: MQTTnet, 4.3.7.1207"
#r "..\release\ServerInfo.dll"
#r "..\release\Controller.dll"
#r "..\release\CommonClient.dll"

using System.Timers;
using CommonClient;
using MQTTnet;
using ServerInfo;
using Controller;

static readonly Random random = new Random();

{
    WriteLine("Attempting to connect to server");
    await using var client = await ClientUtilities.FindAndConnectToClient();

    await client.SetRandomDebugLight();
    await client.SetRandomLeftStick();
    await client.SetRandomRightStick();
    // await client.SetRandomFullController();
}

static async Task SetRandomDebugLight(this PrototypeClient client)
{
    var nextValue = random.NextBool();
    WriteLine($"Setting debug light to {nextValue}");
    await client.SetDebugLight(nextValue);
}

static async Task SetRandomLeftStick(this PrototypeClient client)
{
    var nextValue = GetRandomStickPosition();
    WriteLine($"Setting left stick to {nextValue.x:0.00} {nextValue.y:0.00}");
    await client.SetLeftStick(nextValue);
}

static async Task SetRandomRightStick(this PrototypeClient client)
{
    var nextValue = GetRandomStickPosition();
    WriteLine($"Setting right stick to {nextValue.x:0.00} {nextValue.y:0.00}");
    await client.SetRightStick(nextValue);
}

static async Task SetRandomFullController(this PrototypeClient client)
{
    var nextValue = GetRandomController();
    WriteLine($"Setting controller to {nextValue}");
    await client.SetController(nextValue);
}

static (float x, float y) GetRandomStickPosition()
{
    return (random.NextFloat(-1, 1), random.NextFloat(-1, 1));
}

static ControllerState GetRandomController()
{
    return new ControllerState(
        Start: random.NextBool(),
        Select: random.NextBool(),
        Home: random.NextBool(),
        BigHome: random.NextBool(),
        X: random.NextBool(),
        Y: random.NextBool(),
        A: random.NextBool(),
        B: random.NextBool(),
        Up: random.NextBool(),
        Right: random.NextBool(),
        Down: random.NextBool(),
        Left: random.NextBool(),
        LeftStickX: random.NextFloat(-1, 1),
        LeftStickY: random.NextFloat(-1, 1),
        LeftStickIn: random.NextBool(),
        RightStickX: random.NextFloat(-1, 1),
        RightStickY: random.NextFloat(-1, 1),
        RightStickIn: random.NextBool(),
        LeftBumper: random.NextBool(),
        LeftTrigger: random.NextFloat(0, 1),
        RightBumper: random.NextBool(),
        RightTrigger: random.NextFloat(0, 1)
    );
}

static float NextFloat(this Random random, double minimum, double maximum)
{
    return (float)(random.NextDouble() * (maximum - minimum) + minimum);
}

static bool NextBool(this Random random)
{
    return random.Next(0, 2) == 1;
}
