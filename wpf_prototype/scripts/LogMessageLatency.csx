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

    using Timer latencyTimer = new();
    latencyTimer.Elapsed += (_, _) =>
    {
        WriteLine($"Num of messages received: {messageCount}");
        if (lastThousandMessageLatencies.Count > 0)
        {
            WriteLine($"Last 1000 messages latency (microseconds): {lastThousandMessageLatencies.Average():0.00}");
        }
        WriteLine($"All messages latency (microseconds): {(allMessageLatency / messageCount):0.00}");
        WriteLine();
    };
    latencyTimer.Interval = 1000;
    latencyTimer.Enabled = true;

    var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
        .WithTopicFilter("#")
        .Build();
    await client.MqttClient.SubscribeAsync(subscribeOptions);

    WriteLine("Client connected");
    WriteLine("Press Enter to exit");
    ReadLine();
}
