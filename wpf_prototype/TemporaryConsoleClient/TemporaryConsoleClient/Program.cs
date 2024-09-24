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
    string message = $"{e.ApplicationMessage.Topic} - {ServerDataConverter.ExtractData(e.ApplicationMessage.PayloadSegment.Array)}";

    var timeStampTickString = e.ApplicationMessage.UserProperties.FirstOrDefault(p => p.Name.Equals(AdditionalPropertyNames.TimeStamp))?.Value;

    if (timeStampTickString is not null)
    {
        var timeStamp = new DateTime(long.Parse(timeStampTickString));
        message += $" - {timeStamp:dd/MM/yyyy hh:mm:ss.fffffff}";
    }

    Console.WriteLine(message);
};

await client.MqttClient.SubscribeAsync(subscribeOptions);

Console.WriteLine("Client connected");
Console.WriteLine("Press Enter to exit");
Console.ReadLine();
