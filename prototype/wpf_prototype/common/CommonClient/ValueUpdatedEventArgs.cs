namespace CommonClient;

public class ValueUpdatedEventArgs<T> : EventArgs
{
    public ValueUpdatedEventArgs() { }

    public required T NewValue { get; init; }
    public required DateTime TimeStamp { get; init; }

    public override string ToString()
    {
        return $"{NewValue} {TimeStamp:dd/MM/yyyy hh:mm:ss.fffffff}";
    }
}
