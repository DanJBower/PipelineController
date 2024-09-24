using CommonClient;
using MQTTnet;
using ServerInfo;

// Used for monitoring all logs being sent

Console.WriteLine("Attempting to connect to client");
await using var client = await ClientUtilities.ConnectToClient();

var mqttFactory = new MqttFactory();
var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
    .WithTopicFilter("#")
    .Build();

client.MqttClient.ApplicationMessageReceivedAsync += async e =>
{
    var (timestamp, data) = ServerDataConverter.ExtractData(e.ApplicationMessage.PayloadSegment.Array);
    Console.WriteLine($"{timestamp:dd/MM/yyyy hh:mm:ss.fffffff} - {e.ApplicationMessage.Topic} - {data}");
};

await client.MqttClient.SubscribeAsync(subscribeOptions);

Console.WriteLine("Client connected");
Console.WriteLine("Press Enter to exit");
Console.ReadLine();
