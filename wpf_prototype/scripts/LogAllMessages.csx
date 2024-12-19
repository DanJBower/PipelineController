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
    await using var client = await ClientUtilities.FindAndConnectToClient();

    var mqttFactory = new MqttFactory();

    client.MqttClient.ApplicationMessageReceivedAsync += async e =>
    {
        var (timestamp, data) = ServerDataConverter.ExtractData(e.ApplicationMessage.PayloadSegment.Array);
        WriteLine($"{timestamp:dd/MM/yyyy hh:mm:ss.fffffff} - {e.ApplicationMessage.Topic} ({e.ApplicationMessage.TopicAlias}) - {data}");
    };

    var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
        .WithTopicFilter("#")
        .Build();
    await client.MqttClient.SubscribeAsync(subscribeOptions);

    WriteLine("Client connected");
    WriteLine("Press Enter to exit");
    ReadLine();
}
