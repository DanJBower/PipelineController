#r "nuget: MQTTnet, 4.3.7.1207"
#r "..\release\ServerInfo.dll"
#r "..\release\Controller.dll"
#r "..\release\CommonClient.dll"

using System.Threading;
using System.Timers;
using CommonClient;
using MQTTnet;
using ServerInfo;

{
    WriteLine("Attempting to connect to client");
    await using var client = await ClientUtilities.FindAndConnectToClient();

    var mqttFactory = new MqttFactory();

    Queue<double> lastThousandMessageLatencies = [];
    double allMessageLatency = 0;
    long messageCount = 0;

    client.MqttClient.ApplicationMessageReceivedAsync += async e =>
    {
        var (sentTimestamp, _) = ServerDataConverter.ExtractData(e.ApplicationMessage.PayloadSegment.Array);
        var receivedTimestamp = DateTime.Now;
        var latency = receivedTimestamp - sentTimestamp;

        if (latency.TotalSeconds > 1)
        {
            return;
        }

        lastThousandMessageLatencies.Enqueue(latency.TotalMicroseconds);

        if (lastThousandMessageLatencies.Count > 1000)
        {
            lastThousandMessageLatencies.Dequeue();
        }

        allMessageLatency += latency.TotalMicroseconds;
        messageCount++;
    };

    Lock logLock = new();
    long lastMessageCount = 0;
    long checkCount = 0;

    var latencyLogAction = new TimerEvent(async () =>
    {
        using (logLock.EnterScope())
        {
            var messagesReceivedInLastSecond = messageCount - lastMessageCount;
            lastMessageCount = messageCount;
            checkCount++;
            WriteLine($"Num of messages received: {messageCount}");
            if (lastThousandMessageLatencies.Count > 0)
            {
                WriteLine($"Last 1000 messages latency (microseconds): {lastThousandMessageLatencies.Average():0.00}");
            }
            WriteLine($"All messages latency (microseconds): {(allMessageLatency / messageCount):0.00}");
            WriteLine($"Messages received in last second: {messagesReceivedInLastSecond}");
            WriteLine($"Total MPS: {messageCount / checkCount}");
            WriteLine();
        }
    }, TimeSpan.FromSeconds(1));
    using HighAccuracyTimer latencyTimer = new();
    latencyTimer.Start(latencyLogAction);

    var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
        .WithTopicFilter("#")
        .Build();
    await client.MqttClient.SubscribeAsync(subscribeOptions);

    WriteLine("Client connected");
    WriteLine("Press Enter to exit");
    ReadLine();
}
