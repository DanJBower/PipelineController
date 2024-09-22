using CommonClient;

Console.WriteLine("Trying to find client");
var (clientIp, clientPort) = await ClientBasics.FindClient();

Console.WriteLine($"Connecting to client - {clientIp}:{clientPort}");
await ClientBasics.ConnectToClient(clientIp, clientPort);

Console.WriteLine("Press Enter to exit");
Console.ReadLine();
