using CommonClient;
using Controller;

// await ConsoleControllerPassthrough.RunApproach4().ConfigureAwait(false);
// return;

/*var startTime = Stopwatch.GetTimestamp();

var timerAction1 = new TimerEvent(async () => Log("Task 1"), TimeSpan.FromMilliseconds(800));
var timerAction2 = new TimerEvent(async () => Log("Task 2"), TimeSpan.FromMilliseconds(1500));
var timerAction3 = new TimerEvent(async () =>
{
    Log("Task 3 start");
    await Task.Delay(500);
    Log("Task 3 finished");
}, TimeSpan.FromSeconds(2), NumberOfActivations: 2);
var timerAction4 = new TimerEvent(async () => Log("Task 4"), TimeSpan.FromMilliseconds(100));

using var timer = new HighAccuracyTimer();
// timer.Start(timerAction2);
timer.Start(timerAction3);

await Task.Delay(5000);

timer.Start(timerAction3);
await Task.Delay(5000);

// timer.Start(timerAction1);
timer.Start(timerAction4);

await Task.Delay(TimeSpan.FromSeconds(5));

return;

void Log(string message = "")
{
    if (string.IsNullOrWhiteSpace(message))
    {
        return;
    }

    Console.WriteLine($"{Stopwatch.GetElapsedTime(startTime).TotalMilliseconds:00000.00}: {message}");
}*/

/*var startTime = Stopwatch.GetTimestamp();
SimpleProcessingQueue<(int, int)>? simpleProcessingQueueSample = null;
simpleProcessingQueueSample = new(ProcessItem);

Log("Starting add run 1");
Log();

var taskId = 0;

for (var i = 0; i < 11; i++)
{
    var x = i - 5;
    var delayMs = ((x * x) * 250) + 1000;
    Log($"Adding: {taskId}: Delay {delayMs}");
    simpleProcessingQueueSample.Enqueue((taskId, delayMs));
    taskId++;
}

Log();
Log("5 second delay");
Log();
await Task.Delay(5000);

Log();
Log("Starting add run 2");
Log();

for (var i = 0; i < 11; i++)
{
    var x = i / 2.0;
    var delayMs = (int)((x * x) * 250) + 1000;
    Log($"Adding: {taskId}: Delay {delayMs}");
    simpleProcessingQueueSample.Enqueue((taskId, delayMs));
    taskId++;
}

Log();
Log("Waiting till all current tasks complete");
Log();
await simpleProcessingQueueSample.WaitForQueueToEmpty();

Log();
Log("Starting add run 3");
Log();

for (var i = 0; i < 11; i++)
{
    var x = i - 5;
    var delayMs = ((x * x) * 250) + 1000;
    Log($"Adding: {taskId}: Delay {delayMs}");
    simpleProcessingQueueSample.Enqueue((taskId, delayMs));
    taskId++;
    await Task.Delay(1000);
}

simpleProcessingQueueSample.CompleteAdding();

Log();
Log("Waiting till all tasks complete");
Log();
await simpleProcessingQueueSample.WaitForCompletion();

async Task ProcessItem((int id, int delay) item, CancellationToken cancellationToken)
{
    Log($"Starting to process task {item.id}: {item.delay:0000}ms");
    await Task.Delay(item.delay, cancellationToken);
    Log($"Finished processing {item.id}: {item.delay:0000}ms");
}

void Log(string message = "")
{
    if (string.IsNullOrWhiteSpace(message))
    {
        Console.WriteLine();
        return;
    }

    var duration = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
    Console.WriteLine($"{duration:00000}ms: {message}. Current queue count: {simpleProcessingQueueSample.Count}");
}

return;*/

// Used for monitoring all events happening in the client
Console.WriteLine("Attempting to connect to client");
using var cancellationTokenSource = new CancellationTokenSource();
cancellationTokenSource.CancelAfter(20000);

try
{
    await using var client = await ClientUtilities.FindAndConnectToClient(cancellationTokenSource.Token);

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
    SubscribeToEvent<bool>(client, nameof(client.DebugLightUpdated));
    await client.EnableControllerChangeMonitoring();

    Console.WriteLine("Client connected");

    await SendBool(client, client.SetDebugLight, true);
    await SendBool(client, client.SetStart, true);
    await SendBool(client, client.SetSelect, true);
    await SendBool(client, client.SetHome, true);
    await SendBool(client, client.SetBigHome, true);
    await SendBool(client, client.SetX, true);
    await SendBool(client, client.SetY, true);
    await SendBool(client, client.SetA, true);
    await SendBool(client, client.SetB, true);
    await SendBool(client, client.SetDebugLight, false);
    await SendBool(client, client.SetUp, true);
    await SendBool(client, client.SetRight, true);
    await SendBool(client, client.SetDown, true);
    await SendBool(client, client.SetLeft, true);
    await SendFloat(client, client.SetLeftStickX, 1);
    await SendFloat(client, client.SetLeftStickY, -0.5f);
    await SendBool(client, client.SetLeftStickIn, true);
    await SendFloat(client, client.SetRightStickX, -0.2f);
    await SendFloat(client, client.SetRightStickY, 0.4f);
    await SendBool(client, client.SetRightStickIn, true);
    await SendBool(client, client.SetLeftBumper, true);
    await SendFloat(client, client.SetLeftTrigger, 0.6f);
    await SendBool(client, client.SetRightBumper, true);
    await SendFloat(client, client.SetRightTrigger, 0.8f);
    await SendBool(client, client.SetDebugLight, true);


    await Task.Delay(1000);
    await client.SetLeftStick((0.1f, -0.2f));

    await Task.Delay(1000);
    await client.SetRightStick((0.8f, -1));

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
    ), true);
}
catch (OperationCanceledException)
{
    Console.Error.WriteLine("Could not connect to client after 20s");
}

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

static async Task SendBool(PrototypeClient client, Func<bool, bool, Task> sendAction, bool newValue)
{
    await Task.Delay(1000);
    await sendAction(newValue, true);
}

static async Task SendFloat(PrototypeClient client, Func<float, bool, Task> sendAction, float newValue)
{
    await Task.Delay(1000);
    await sendAction(newValue, true);
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
