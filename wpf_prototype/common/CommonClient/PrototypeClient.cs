using Controller;
using MQTTnet.Client;

namespace CommonClient;

public class PrototypeClient : IDisposable
{
    private readonly IMqttClient _mqttClient;

    public PrototypeClient(IMqttClient client)
    {
        _mqttClient = client;
    }

    public void SetController(ControllerState controllerState)
    {
        SetStart(controllerState.Start);
        SetSelect(controllerState.Select);
        SetHome(controllerState.Home);
        SetBigHome(controllerState.BigHome);
        SetX(controllerState.X);
        SetY(controllerState.Y);
        SetA(controllerState.A);
        SetB(controllerState.B);
        SetUp(controllerState.Up);
        SetRight(controllerState.Right);
        SetDown(controllerState.Down);
        SetLeft(controllerState.Left);
        SetLeftStickX(controllerState.LeftStickX);
        SetLeftStickY(controllerState.LeftStickY);
        SetLeftStickIn(controllerState.LeftStickIn);
        SetRightStickX(controllerState.RightStickX);
        SetRightStickY(controllerState.RightStickY);
        SetRightStickIn(controllerState.RightStickIn);
        SetLeftBumper(controllerState.LeftBumper);
        SetLeftTrigger(controllerState.LeftTrigger);
        SetRightBumper(controllerState.RightBumper);
        SetRightTrigger(controllerState.RightTrigger);
    }

    public void SetController(
        bool? start = null,
        bool? select = null,
        bool? home = null,
        bool? bigHome = null,
        bool? x = null,
        bool? y = null,
        bool? a = null,
        bool? b = null,
        bool? up = null,
        bool? right = null,
        bool? down = null,
        bool? left = null,
        float? leftStickX = null,
        float? leftStickY = null,
        bool? leftStickIn = null,
        float? rightStickX = null,
        float? rightStickY = null,
        bool? rightStickIn = null,
        bool? leftBumper = null,
        float? leftTrigger = null,
        bool? rightBumper = null,
        float? rightTrigger = null)
    {
        if (start.HasValue)
        {
            SetStart(start.Value);
        }

        if (select.HasValue)
        {
            SetSelect(select.Value);
        }

        if (home.HasValue)
        {
            SetHome(home.Value);
        }

        if (bigHome.HasValue)
        {
            SetBigHome(bigHome.Value);
        }

        if (x.HasValue)
        {
            SetX(x.Value);
        }

        if (y.HasValue)
        {
            SetY(y.Value);
        }

        if (a.HasValue)
        {
            SetA(a.Value);
        }

        if (b.HasValue)
        {
            SetB(b.Value);
        }

        if (up.HasValue)
        {
            SetUp(up.Value);
        }

        if (right.HasValue)
        {
            SetRight(right.Value);
        }

        if (down.HasValue)
        {
            SetDown(down.Value);
        }

        if (left.HasValue)
        {
            SetLeft(left.Value);
        }

        if (leftStickX.HasValue)
        {
            SetLeftStickX(leftStickX.Value);
        }

        if (leftStickY.HasValue)
        {
            SetLeftStickY(leftStickY.Value);
        }

        if (leftStickIn.HasValue)
        {
            SetLeftStickIn(leftStickIn.Value);
        }

        if (rightStickX.HasValue)
        {
            SetRightStickX(rightStickX.Value);
        }

        if (rightStickY.HasValue)
        {
            SetRightStickY(rightStickY.Value);
        }

        if (rightStickIn.HasValue)
        {
            SetRightStickIn(rightStickIn.Value);
        }

        if (leftBumper.HasValue)
        {
            SetLeftBumper(leftBumper.Value);
        }

        if (leftTrigger.HasValue)
        {
            SetLeftTrigger(leftTrigger.Value);
        }

        if (rightBumper.HasValue)
        {
            SetRightBumper(rightBumper.Value);
        }

        if (rightTrigger.HasValue)
        {
            SetRightTrigger(rightTrigger.Value);
        }
    }

    public void SetLeftStick(float x, float y)
    {
        SetLeftStickX(x);
        SetLeftStickY(y);
    }

    public void SetRightStick(float x, float y)
    {
        SetRightStickX(x);
        SetRightStickY(y);
    }

    public void SetStart(bool start)
    {

    }

    public void SetSelect(bool select)
    {

    }

    public void SetHome(bool home)
    {

    }

    public void SetBigHome(bool bigHome)
    {

    }

    public void SetX(bool x)
    {

    }

    public void SetY(bool y)
    {

    }

    public void SetA(bool a)
    {

    }

    public void SetB(bool b)
    {

    }

    public void SetUp(bool up)
    {

    }

    public void SetRight(bool right)
    {

    }

    public void SetDown(bool down)
    {

    }

    public void SetLeft(bool left)
    {

    }

    public void SetLeftStickX(float leftStickX)
    {

    }

    public void SetLeftStickY(float leftStickY)
    {

    }

    public void SetLeftStickIn(bool leftStickIn)
    {

    }

    public void SetRightStickX(float rightStickX)
    {

    }

    public void SetRightStickY(float rightStickY)
    {

    }

    public void SetRightStickIn(bool rightStickIn)
    {

    }

    public void SetLeftBumper(bool leftBumper)
    {

    }

    public void SetLeftTrigger(float leftTrigger)
    {

    }

    public void SetRightBumper(bool rightBumper)
    {

    }

    public void SetRightTrigger(float rightTrigger)
    {

    }


    // TODO
    // * Get State (gets the last values)
    // * Set <button>
    // * Controller state changed (any value changed) event
    //   - Pass controller state as new value
    //   - Trigger at end of individual change events?
    // * Individual value changed events
    // Tie value changes to something like:
    // private void XUpdated()
    // {
    //     ControllerState newValue;
    //     lock (_stateLock)
    //     {
    //         State = State with { X = true };
    //         newValue = State;
    //     }
    //     TriggerControllerChangedEvent(newValue);
    // }

    private readonly object _stateLock = new();

    public ControllerState State { get; private set; } = new();

    private bool _subscribed;

    public void EnableControllerChangeMonitoring()
    {
        if (_subscribed)
        {
            return;
        }

        lock (_stateLock)
        {
            State = new();
        }

        // Put all the += here

        _subscribed = true;
    }

    public void DisableControllerChangeMonitoring()
    {
        if (!_subscribed)
        {
            return;
        }

        // Put all the -= here

        _subscribed = false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _mqttClient.DisconnectAsync().RunSynchronously();
            DisableControllerChangeMonitoring();
            _mqttClient.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~PrototypeClient()
    {
        Dispose(false);
    }
}
