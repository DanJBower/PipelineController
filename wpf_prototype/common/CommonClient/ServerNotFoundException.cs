namespace CommonClient;

public class ServerNotFoundException : Exception
{
    public ServerNotFoundException() : base("Failed to find the controller server") { }

    public ServerNotFoundException(string message) : base(message) { }
}
