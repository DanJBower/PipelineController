namespace Controller;

public static class ControllerStateSerialiser
{
    private const int FloatSize = sizeof(float);
    private const int BoolSize = sizeof(bool);
    private const int BytesRequired = (6 * FloatSize) + (16 * BoolSize);

    private const int StartPosition = 0;
    private const int SelectPosition = StartPosition + BoolSize;
    private const int HomePosition = SelectPosition + BoolSize;
    private const int BigHomePosition = HomePosition + BoolSize;
    private const int XPosition = BigHomePosition + BoolSize;
    private const int YPosition = XPosition + BoolSize;
    private const int APosition = YPosition + BoolSize;
    private const int BPosition = APosition + BoolSize;
    private const int UpPosition = BPosition + BoolSize;
    private const int RightPosition = UpPosition + BoolSize;
    private const int DownPosition = RightPosition + BoolSize;
    private const int LeftPosition = DownPosition + BoolSize;
    private const int LeftStickXPosition = LeftPosition + BoolSize;
    private const int LeftStickYPosition = LeftStickXPosition + FloatSize;
    private const int LeftStickInPosition = LeftStickYPosition + FloatSize;
    private const int RightStickXPosition = LeftStickInPosition + BoolSize;
    private const int RightStickYPosition = RightStickXPosition + FloatSize;
    private const int RightStickInPosition = RightStickYPosition + FloatSize;
    private const int LeftBumperPosition = RightStickInPosition + BoolSize;
    private const int LeftTriggerPosition = LeftBumperPosition + BoolSize;
    private const int RightBumperPosition = LeftTriggerPosition + FloatSize;
    private const int RightTriggerPosition = RightBumperPosition + BoolSize;

    public static ControllerState DeserialiseControllerState(ReadOnlySpan<byte> data)
    {
        return new ControllerState(
            Start: BitConverter.ToBoolean(data[..SelectPosition]),
            Select: BitConverter.ToBoolean(data[SelectPosition..HomePosition]),
            Home: BitConverter.ToBoolean(data[HomePosition..BigHomePosition]),
            BigHome: BitConverter.ToBoolean(data[BigHomePosition..XPosition]),
            X: BitConverter.ToBoolean(data[XPosition..YPosition]),
            Y: BitConverter.ToBoolean(data[YPosition..APosition]),
            A: BitConverter.ToBoolean(data[APosition..BPosition]),
            B: BitConverter.ToBoolean(data[BPosition..UpPosition]),
            Up: BitConverter.ToBoolean(data[UpPosition..RightPosition]),
            Right: BitConverter.ToBoolean(data[RightPosition..DownPosition]),
            Down: BitConverter.ToBoolean(data[DownPosition..LeftPosition]),
            Left: BitConverter.ToBoolean(data[LeftPosition..LeftStickXPosition]),
            LeftStickX: BitConverter.ToSingle(data[LeftStickXPosition..LeftStickYPosition]),
            LeftStickY: BitConverter.ToSingle(data[LeftStickYPosition..LeftStickInPosition]),
            LeftStickIn: BitConverter.ToBoolean(data[LeftStickInPosition..RightStickXPosition]),
            RightStickX: BitConverter.ToSingle(data[RightStickXPosition..RightStickYPosition]),
            RightStickY: BitConverter.ToSingle(data[RightStickYPosition..RightStickInPosition]),
            RightStickIn: BitConverter.ToBoolean(data[RightStickInPosition..LeftBumperPosition]),
            LeftBumper: BitConverter.ToBoolean(data[LeftBumperPosition..LeftTriggerPosition]),
            LeftTrigger: BitConverter.ToSingle(data[LeftTriggerPosition..RightBumperPosition]),
            RightBumper: BitConverter.ToBoolean(data[RightBumperPosition..RightTriggerPosition]),
            RightTrigger: BitConverter.ToSingle(data[RightTriggerPosition..])
        );
    }

    public static byte[] SerialiseControllerState(ControllerState controllerState)
    {
        var result = new byte[BytesRequired];
        Array.Copy(BitConverter.GetBytes(controllerState.Start), 0, result, StartPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.Select), 0, result, SelectPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.Home), 0, result, HomePosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.BigHome), 0, result, BigHomePosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.X), 0, result, XPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.Y), 0, result, YPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.A), 0, result, APosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.B), 0, result, BPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.Up), 0, result, UpPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.Right), 0, result, RightPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.Down), 0, result, DownPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.Left), 0, result, LeftPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.LeftStickX), 0, result, LeftStickXPosition, FloatSize);
        Array.Copy(BitConverter.GetBytes(controllerState.LeftStickY), 0, result, LeftStickYPosition, FloatSize);
        Array.Copy(BitConverter.GetBytes(controllerState.LeftStickIn), 0, result, LeftStickInPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.RightStickX), 0, result, RightStickXPosition, FloatSize);
        Array.Copy(BitConverter.GetBytes(controllerState.RightStickY), 0, result, RightStickYPosition, FloatSize);
        Array.Copy(BitConverter.GetBytes(controllerState.RightStickIn), 0, result, RightStickInPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.LeftBumper), 0, result, LeftBumperPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.LeftTrigger), 0, result, LeftTriggerPosition, FloatSize);
        Array.Copy(BitConverter.GetBytes(controllerState.RightBumper), 0, result, RightBumperPosition, BoolSize);
        Array.Copy(BitConverter.GetBytes(controllerState.RightTrigger), 0, result, RightTriggerPosition, FloatSize);
        return result;
    }
}
