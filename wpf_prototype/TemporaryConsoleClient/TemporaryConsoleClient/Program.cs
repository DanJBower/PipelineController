using CommonClient;

Console.WriteLine("Attempting to connect to client");
using var client = await ClientBasics.ConnectToClient();

Console.WriteLine("Client connected");
Console.WriteLine("Press Enter to exit");
Console.ReadLine();
