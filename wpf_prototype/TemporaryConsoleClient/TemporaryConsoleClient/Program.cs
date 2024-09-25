using CommonClient;
using Controller;

// Used for monitoring all events happening in the client

Console.WriteLine("Attempting to connect to client");
await using var client = await ClientUtilities.ConnectToClient();

SubscribeToEvent<bool>(client, nameof(client.StartUpdated));
SubscribeToEvent<bool>(client, nameof(client.SelectUpdated));
SubscribeToEvent<bool>(client, nameof(client.HomeUpdated));
SubscribeToEvent<bool>(client, nameof(client.BigHomeUpdated));
SubscribeToEvent<bool>(client, nameof(client.XUpdated));
SubscribeToEvent<bool>(client, nameof(client.YUpdated));
SubscribeToEvent<bool>(client, nameof(client.AUpdated));
SubscribeToEvent<bool>(client, nameof(client.BUpdated));
SubscribeToEvent<bool>(client, nameof(client.UpUpdated));
SubscribeToEvent<bool>(client, nameof(client.RightUpdated));
SubscribeToEvent<bool>(client, nameof(client.DownUpdated));
SubscribeToEvent<bool>(client, nameof(client.LeftUpdated));
SubscribeToEvent<float>(client, nameof(client.LeftStickXUpdated));
SubscribeToEvent<float>(client, nameof(client.LeftStickYUpdated));
SubscribeToEvent<bool>(client, nameof(client.LeftStickInUpdated));
SubscribeToEvent<float>(client, nameof(client.RightStickXUpdated));
SubscribeToEvent<float>(client, nameof(client.RightStickYUpdated));
SubscribeToEvent<bool>(client, nameof(client.RightStickInUpdated));
SubscribeToEvent<bool>(client, nameof(client.LeftBumperUpdated));
SubscribeToEvent<float>(client, nameof(client.LeftTriggerUpdated));
SubscribeToEvent<bool>(client, nameof(client.RightBumperUpdated));
SubscribeToEvent<float>(client, nameof(client.RightTriggerUpdated));
SubscribeToEvent<ControllerState>(client, nameof(client.ControllerUpdated));
await client.EnableControllerChangeMonitoring();

Console.WriteLine("Client connected");

await SendBool(client, client.SetStart, true);
await SendBool(client, client.SetSelect, true);
await SendBool(client, client.SetHome, true);
await SendBool(client, client.SetBigHome, true);
await SendBool(client, client.SetX, true);
await SendBool(client, client.SetY, true);
await SendBool(client, client.SetA, true);
await SendBool(client, client.SetB, true);
await SendBool(client, client.SetUp, true);
await SendBool(client, client.SetRight, true);
await SendBool(client, client.SetDown, true);
await SendBool(client, client.SetLeft, true);
await SendFloat(client, client.SetLeftStickX, 10);
await SendFloat(client, client.SetLeftStickY, 35);
await SendBool(client, client.SetLeftStickIn, true);
await SendFloat(client, client.SetRightStickX, 256);
await SendFloat(client, client.SetRightStickY, 346);
await SendBool(client, client.SetRightStickIn, true);
await SendBool(client, client.SetLeftBumper, true);
await SendFloat(client, client.SetLeftTrigger, 273);
await SendBool(client, client.SetRightBumper, true);
await SendFloat(client, client.SetRightTrigger, 378);


await Task.Delay(1000);
await client.SetLeftStick((15, 4));

await Task.Delay(1000);
await client.SetRightStick((234, 74));

await Task.Delay(1000);
await client.SetController(new(
    Start: false,
    Select: false,
    Home: false,
    BigHome: false,
    X: false,
    Y: false,
    A: false,
    B: false,
    Up: false,
    Right: false,
    Down: false,
    Left: false,
    LeftStickX: 0,
    LeftStickY: 0,
    LeftStickIn: false,
    RightStickX: 0,
    RightStickY: 0,
    RightStickIn: false,
    LeftBumper: false,
    LeftTrigger: 0,
    RightBumper: false,
    RightTrigger: 0
));

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

static async Task SendBool(PrototypeClient client, Func<bool, Task> sendAction, bool newValue)
{
    await Task.Delay(1000);
    await sendAction(newValue);
}

static async Task SendFloat(PrototypeClient client, Func<float, Task> sendAction, float newValue)
{
    await Task.Delay(1000);
    await sendAction(newValue);
}

static void SubscribeToEvent<T>(PrototypeClient client, string eventName)
{
    var eventInfo = typeof(PrototypeClient).GetEvent(eventName);
    var handler = new EventHandler<ValueUpdatedEventArgs<T>>((sender, args) => EventHandlerWithEventName(eventName, sender, args));
    eventInfo?.AddEventHandler(client, handler);
}

static void EventHandlerWithEventName<T>(string eventName, object? _, ValueUpdatedEventArgs<T> e)
{
    Console.WriteLine($"{e.TimeStamp:dd/MM/yyyy hh:mm:ss.fffffff} - '{eventName}' triggered - {e.NewValue}");
}
