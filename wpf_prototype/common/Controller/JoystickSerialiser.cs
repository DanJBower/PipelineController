namespace Controller;

public static class JoystickSerialiser
{
    private const int FloatSize = sizeof(float);
    private const int BytesRequired = (2 * FloatSize);

    private const int XPosition = 0;
    private const int YPosition = XPosition + FloatSize;

    public static (float x, float y) DeserialiseJoystickState(ReadOnlySpan<byte> data)
    {
        return (
            BitConverter.ToSingle(data[..YPosition]),
            BitConverter.ToSingle(data[YPosition..])
        );
    }

    public static byte[] SerialiseJoystickState((float x, float y) joystickPosition)
    {
        var result = new byte[BytesRequired];
        Array.Copy(BitConverter.GetBytes(joystickPosition.x), 0, result, XPosition, FloatSize);
        Array.Copy(BitConverter.GetBytes(joystickPosition.y), 0, result, YPosition, FloatSize);
        return result;
    }
}
