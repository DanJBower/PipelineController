using Controller;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using ServerInfo;

namespace CommonClient;

public class PrototypeClient : IAsyncDisposable
{
    private static readonly MqttFactory MqttFactory = new();
    public IMqttClient MqttClient { get; }

    public PrototypeClient(IMqttClient client)
    {
        MqttClient = client;

        TriggerControllerEventActions = new()
        {
            {Topics.StartTopicAlias, (timestamp, data) => StartUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.SelectTopicAlias, (timestamp, data) => SelectUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.HomeTopicAlias, (timestamp, data) => HomeUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.BigHomeTopicAlias, (timestamp, data) => BigHomeUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.XTopicAlias, (timestamp, data) => XUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.YTopicAlias, (timestamp, data) => YUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.ATopicAlias, (timestamp, data) => AUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.BTopicAlias, (timestamp, data) => BUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.UpTopicAlias, (timestamp, data) => UpUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.RightTopicAlias, (timestamp, data) => RightUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.DownTopicAlias, (timestamp, data) => DownUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.LeftTopicAlias, (timestamp, data) => LeftUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.LeftStickXTopicAlias, (timestamp, data) => LeftStickXUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.LeftStickYTopicAlias, (timestamp, data) => LeftStickYUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.LeftStickInTopicAlias, (timestamp, data) => LeftStickInUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.RightStickXTopicAlias, (timestamp, data) => RightStickXUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.RightStickYTopicAlias, (timestamp, data) => RightStickYUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.RightStickInTopicAlias, (timestamp, data) => RightStickInUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.LeftBumperTopicAlias, (timestamp, data) => LeftBumperUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.LeftTriggerTopicAlias, (timestamp, data) => LeftTriggerUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.RightBumperTopicAlias, (timestamp, data) => RightBumperUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
            {Topics.RightTriggerTopicAlias, (timestamp, data) => RightTriggerUpdated?.Invoke(this, new() {NewValue = data, TimeStamp = timestamp})},
        };
    }

    public async Task SetController(ControllerState controllerState)
    {
        await SetStart(controllerState.Start);
        await SetSelect(controllerState.Select);
        await SetHome(controllerState.Home);
        await SetBigHome(controllerState.BigHome);
        await SetX(controllerState.X);
        await SetY(controllerState.Y);
        await SetA(controllerState.A);
        await SetB(controllerState.B);
        await SetUp(controllerState.Up);
        await SetRight(controllerState.Right);
        await SetDown(controllerState.Down);
        await SetLeft(controllerState.Left);
        await SetLeftStickX(controllerState.LeftStickX);
        await SetLeftStickY(controllerState.LeftStickY);
        await SetLeftStickIn(controllerState.LeftStickIn);
        await SetRightStickX(controllerState.RightStickX);
        await SetRightStickY(controllerState.RightStickY);
        await SetRightStickIn(controllerState.RightStickIn);
        await SetLeftBumper(controllerState.LeftBumper);
        await SetLeftTrigger(controllerState.LeftTrigger);
        await SetRightBumper(controllerState.RightBumper);
        await SetRightTrigger(controllerState.RightTrigger);
    }

    public async Task SetController(
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
            await SetStart(start.Value);
        }

        if (select.HasValue)
        {
            await SetSelect(select.Value);
        }

        if (home.HasValue)
        {
            await SetHome(home.Value);
        }

        if (bigHome.HasValue)
        {
            await SetBigHome(bigHome.Value);
        }

        if (x.HasValue)
        {
            await SetX(x.Value);
        }

        if (y.HasValue)
        {
            await SetY(y.Value);
        }

        if (a.HasValue)
        {
            await SetA(a.Value);
        }

        if (b.HasValue)
        {
            await SetB(b.Value);
        }

        if (up.HasValue)
        {
            await SetUp(up.Value);
        }

        if (right.HasValue)
        {
            await SetRight(right.Value);
        }

        if (down.HasValue)
        {
            await SetDown(down.Value);
        }

        if (left.HasValue)
        {
            await SetLeft(left.Value);
        }

        if (leftStickX.HasValue)
        {
            await SetLeftStickX(leftStickX.Value);
        }

        if (leftStickY.HasValue)
        {
            await SetLeftStickY(leftStickY.Value);
        }

        if (leftStickIn.HasValue)
        {
            await SetLeftStickIn(leftStickIn.Value);
        }

        if (rightStickX.HasValue)
        {
            await SetRightStickX(rightStickX.Value);
        }

        if (rightStickY.HasValue)
        {
            await SetRightStickY(rightStickY.Value);
        }

        if (rightStickIn.HasValue)
        {
            await SetRightStickIn(rightStickIn.Value);
        }

        if (leftBumper.HasValue)
        {
            await SetLeftBumper(leftBumper.Value);
        }

        if (leftTrigger.HasValue)
        {
            await SetLeftTrigger(leftTrigger.Value);
        }

        if (rightBumper.HasValue)
        {
            await SetRightBumper(rightBumper.Value);
        }

        if (rightTrigger.HasValue)
        {
            await SetRightTrigger(rightTrigger.Value);
        }
    }

    public async Task SetLeftStick(float x, float y)
    {
        await SetLeftStickX(x);
        await SetLeftStickY(y);
    }

    public async Task SetRightStick(float x, float y)
    {
        await SetRightStickX(x);
        await SetRightStickY(y);
    }

    public DateTime ControllerStateLastUpdated { get; private set; }
    public event EventHandler<ValueUpdatedEventArgs<ControllerState>>? ControllerUpdated;

    private async Task SendMessage<T>(MqttApplicationMessageBuilder messageBuilder,
        T messagePayload)
    {
        var message = messageBuilder
            .WithPayload(ServerDataConverter.ExtractBytes(messagePayload))
            .Build();

        await MqttClient.PublishAsync(message);
    }

    private static readonly MqttApplicationMessageBuilder InitialStartMessageBuilder = new MqttApplicationMessageBuilder()
    .WithTopic(Topics.StartTopic)
    .WithTopicAlias(Topics.StartTopicAlias)
    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
    .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedStartMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.StartTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _startMessageBuilder = InitialStartMessageBuilder;

    public async Task SetStart(bool start)
    {
        await SendMessage(_startMessageBuilder, start);
        _startMessageBuilder = AliasedStartMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? StartUpdated;

    public DateTime StartLastUpdated { get; private set; }

    private void OnStartUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > StartLastUpdated)
            {
                ControllerState = ControllerState with { Start = newValue.NewValue };
                StartLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialSelectMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.SelectTopic)
        .WithTopicAlias(Topics.SelectTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedSelectMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.SelectTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _selectMessageBuilder = InitialSelectMessageBuilder;

    public async Task SetSelect(bool select)
    {
        await SendMessage(_selectMessageBuilder, select);
        _selectMessageBuilder = AliasedSelectMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? SelectUpdated;

    public DateTime SelectLastUpdated { get; private set; }

    private void OnSelectUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > SelectLastUpdated)
            {
                ControllerState = ControllerState with { Select = newValue.NewValue };
                SelectLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.HomeTopic)
        .WithTopicAlias(Topics.HomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.HomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _homeMessageBuilder = InitialHomeMessageBuilder;

    public async Task SetHome(bool home)
    {
        await SendMessage(_homeMessageBuilder, home);
        _homeMessageBuilder = AliasedHomeMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? HomeUpdated;

    public DateTime HomeLastUpdated { get; private set; }

    private void OnHomeUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > HomeLastUpdated)
            {
                ControllerState = ControllerState with { Home = newValue.NewValue };
                HomeLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialBigHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.BigHomeTopic)
        .WithTopicAlias(Topics.BigHomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedBigHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.BigHomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _bigHomeMessageBuilder = InitialBigHomeMessageBuilder;

    public async Task SetBigHome(bool bigHome)
    {
        await SendMessage(_bigHomeMessageBuilder, bigHome);
        _bigHomeMessageBuilder = AliasedBigHomeMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? BigHomeUpdated;

    public DateTime BigHomeLastUpdated { get; private set; }

    private void OnBigHomeUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > BigHomeLastUpdated)
            {
                ControllerState = ControllerState with { BigHome = newValue.NewValue };
                BigHomeLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.XTopic)
        .WithTopicAlias(Topics.XTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.XTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _xMessageBuilder = InitialXMessageBuilder;

    public async Task SetX(bool x)
    {
        await SendMessage(_xMessageBuilder, x);
        _xMessageBuilder = AliasedXMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? XUpdated;

    public DateTime XLastUpdated { get; private set; }

    private void OnXUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > XLastUpdated)
            {
                ControllerState = ControllerState with { X = newValue.NewValue };
                XLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.YTopic)
        .WithTopicAlias(Topics.YTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.YTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _yMessageBuilder = InitialYMessageBuilder;

    public async Task SetY(bool y)
    {
        await SendMessage(_yMessageBuilder, y);
        _yMessageBuilder = AliasedYMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? YUpdated;

    public DateTime YLastUpdated { get; private set; }

    private void OnYUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > YLastUpdated)
            {
                ControllerState = ControllerState with { Y = newValue.NewValue };
                YLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialAMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.ATopic)
        .WithTopicAlias(Topics.ATopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedAMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.ATopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _aMessageBuilder = InitialAMessageBuilder;

    public async Task SetA(bool a)
    {
        await SendMessage(_aMessageBuilder, a);
        _aMessageBuilder = AliasedAMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? AUpdated;

    public DateTime ALastUpdated { get; private set; }

    private void OnAUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > ALastUpdated)
            {
                ControllerState = ControllerState with { A = newValue.NewValue };
                ALastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialBMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.BTopic)
        .WithTopicAlias(Topics.BTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedBMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.BTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _bMessageBuilder = InitialBMessageBuilder;

    public async Task SetB(bool b)
    {
        await SendMessage(_bMessageBuilder, b);
        _bMessageBuilder = AliasedBMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? BUpdated;

    public DateTime BLastUpdated { get; private set; }

    private void OnBUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > BLastUpdated)
            {
                ControllerState = ControllerState with { B = newValue.NewValue };
                BLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialUpMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.UpTopic)
        .WithTopicAlias(Topics.UpTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedUpMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.UpTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _upMessageBuilder = InitialUpMessageBuilder;

    public async Task SetUp(bool up)
    {
        await SendMessage(_upMessageBuilder, up);
        _upMessageBuilder = AliasedUpMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? UpUpdated;

    public DateTime UpLastUpdated { get; private set; }

    private void OnUpUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > UpLastUpdated)
            {
                ControllerState = ControllerState with { Up = newValue.NewValue };
                UpLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightTopic)
        .WithTopicAlias(Topics.RightTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedRightMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _rightMessageBuilder = InitialRightMessageBuilder;

    public async Task SetRight(bool right)
    {
        await SendMessage(_rightMessageBuilder, right);
        _rightMessageBuilder = AliasedRightMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? RightUpdated;

    public DateTime RightLastUpdated { get; private set; }

    private void OnRightUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > RightLastUpdated)
            {
                ControllerState = ControllerState with { Right = newValue.NewValue };
                RightLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialDownMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.DownTopic)
        .WithTopicAlias(Topics.DownTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedDownMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.DownTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _downMessageBuilder = InitialDownMessageBuilder;

    public async Task SetDown(bool down)
    {
        await SendMessage(_downMessageBuilder, down);
        _downMessageBuilder = AliasedDownMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? DownUpdated;

    public DateTime DownLastUpdated { get; private set; }

    private void OnDownUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > DownLastUpdated)
            {
                ControllerState = ControllerState with { Down = newValue.NewValue };
                DownLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftTopic)
        .WithTopicAlias(Topics.LeftTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedLeftMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _leftMessageBuilder = InitialLeftMessageBuilder;

    public async Task SetLeft(bool left)
    {
        await SendMessage(_leftMessageBuilder, left);
        _leftMessageBuilder = AliasedLeftMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? LeftUpdated;

    public DateTime LeftLastUpdated { get; private set; }

    private void OnLeftUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > LeftLastUpdated)
            {
                ControllerState = ControllerState with { Left = newValue.NewValue };
                LeftLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftStickXTopic)
        .WithTopicAlias(Topics.LeftStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedLeftStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _leftStickXMessageBuilder = InitialLeftStickXMessageBuilder;

    public async Task SetLeftStickX(float leftStickX)
    {
        await SendMessage(_leftStickXMessageBuilder, leftStickX);
        _leftStickXMessageBuilder = AliasedLeftStickXMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? LeftStickXUpdated;

    public DateTime LeftStickXLastUpdated { get; private set; }

    private void OnLeftStickXUpdated(object? sender, ValueUpdatedEventArgs<float> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > LeftStickXLastUpdated)
            {
                ControllerState = ControllerState with { LeftStickX = newValue.NewValue };
                LeftStickXLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftStickYTopic)
        .WithTopicAlias(Topics.LeftStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedLeftStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _leftStickYMessageBuilder = InitialLeftStickYMessageBuilder;

    public async Task SetLeftStickY(float leftStickY)
    {
        await SendMessage(_leftStickYMessageBuilder, leftStickY);
        _leftStickYMessageBuilder = AliasedLeftStickYMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? LeftStickYUpdated;

    public DateTime LeftStickYLastUpdated { get; private set; }

    private void OnLeftStickYUpdated(object? sender, ValueUpdatedEventArgs<float> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > LeftStickYLastUpdated)
            {
                ControllerState = ControllerState with { LeftStickY = newValue.NewValue };
                LeftStickYLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftStickInTopic)
        .WithTopicAlias(Topics.LeftStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedLeftStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _leftStickInMessageBuilder = InitialLeftStickInMessageBuilder;

    public async Task SetLeftStickIn(bool leftStickIn)
    {
        await SendMessage(_leftStickInMessageBuilder, leftStickIn);
        _leftStickInMessageBuilder = AliasedLeftStickInMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? LeftStickInUpdated;

    public DateTime LeftStickInLastUpdated { get; private set; }

    private void OnLeftStickInUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > LeftStickInLastUpdated)
            {
                ControllerState = ControllerState with { LeftStickIn = newValue.NewValue };
                LeftStickInLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightStickXTopic)
        .WithTopicAlias(Topics.RightStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedRightStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _rightStickXMessageBuilder = InitialRightStickXMessageBuilder;

    public async Task SetRightStickX(float rightStickX)
    {
        await SendMessage(_rightStickXMessageBuilder, rightStickX);
        _rightStickXMessageBuilder = AliasedRightStickXMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? RightStickXUpdated;

    public DateTime RightStickXLastUpdated { get; private set; }

    private void OnRightStickXUpdated(object? sender, ValueUpdatedEventArgs<float> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > RightStickXLastUpdated)
            {
                ControllerState = ControllerState with { RightStickX = newValue.NewValue };
                RightStickXLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightStickYTopic)
        .WithTopicAlias(Topics.RightStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedRightStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _rightStickYMessageBuilder = InitialRightStickYMessageBuilder;

    public async Task SetRightStickY(float rightStickY)
    {
        await SendMessage(_rightStickYMessageBuilder, rightStickY);
        _rightStickYMessageBuilder = AliasedRightStickYMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? RightStickYUpdated;

    public DateTime RightStickYLastUpdated { get; private set; }

    private void OnRightStickYUpdated(object? sender, ValueUpdatedEventArgs<float> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > RightStickYLastUpdated)
            {
                ControllerState = ControllerState with { RightStickY = newValue.NewValue };
                RightStickYLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightStickInTopic)
        .WithTopicAlias(Topics.RightStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedRightStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _rightStickInMessageBuilder = InitialRightStickInMessageBuilder;

    public async Task SetRightStickIn(bool rightStickIn)
    {
        await SendMessage(_rightStickInMessageBuilder, rightStickIn);
        _rightStickInMessageBuilder = AliasedRightStickInMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? RightStickInUpdated;

    public DateTime RightStickInLastUpdated { get; private set; }

    private void OnRightStickInUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > RightStickInLastUpdated)
            {
                ControllerState = ControllerState with { RightStickIn = newValue.NewValue };
                RightStickInLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftBumperTopic)
        .WithTopicAlias(Topics.LeftBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedLeftBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _leftBumperMessageBuilder = InitialLeftBumperMessageBuilder;

    public async Task SetLeftBumper(bool leftBumper)
    {
        await SendMessage(_leftBumperMessageBuilder, leftBumper);
        _leftBumperMessageBuilder = AliasedLeftBumperMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? LeftBumperUpdated;

    public DateTime LeftBumperLastUpdated { get; private set; }

    private void OnLeftBumperUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > LeftBumperLastUpdated)
            {
                ControllerState = ControllerState with { LeftBumper = newValue.NewValue };
                LeftBumperLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftTriggerTopic)
        .WithTopicAlias(Topics.LeftTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedLeftTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _leftTriggerMessageBuilder = InitialLeftTriggerMessageBuilder;

    public async Task SetLeftTrigger(float leftTrigger)
    {
        await SendMessage(_leftTriggerMessageBuilder, leftTrigger);
        _leftTriggerMessageBuilder = AliasedLeftTriggerMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? LeftTriggerUpdated;

    public DateTime LeftTriggerLastUpdated { get; private set; }

    private void OnLeftTriggerUpdated(object? sender, ValueUpdatedEventArgs<float> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > LeftTriggerLastUpdated)
            {
                ControllerState = ControllerState with { LeftTrigger = newValue.NewValue };
                LeftTriggerLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightBumperTopic)
        .WithTopicAlias(Topics.RightBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedRightBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _rightBumperMessageBuilder = InitialRightBumperMessageBuilder;

    public async Task SetRightBumper(bool rightBumper)
    {
        await SendMessage(_rightBumperMessageBuilder, rightBumper);
        _rightBumperMessageBuilder = AliasedRightBumperMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? RightBumperUpdated;

    public DateTime RightBumperLastUpdated { get; private set; }

    private void OnRightBumperUpdated(object? sender, ValueUpdatedEventArgs<bool> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > RightBumperLastUpdated)
            {
                ControllerState = ControllerState with { RightBumper = newValue.NewValue };
                RightBumperLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightTriggerTopic)
        .WithTopicAlias(Topics.RightTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedRightTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _rightTriggerMessageBuilder = InitialRightTriggerMessageBuilder;

    public async Task SetRightTrigger(float rightTrigger)
    {
        await SendMessage(_rightTriggerMessageBuilder, rightTrigger);
        _rightTriggerMessageBuilder = AliasedRightTriggerMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? RightTriggerUpdated;

    public DateTime RightTriggerLastUpdated { get; private set; }

    private void OnRightTriggerUpdated(object? sender, ValueUpdatedEventArgs<float> newValue)
    {
        lock (_stateLock)
        {
            if (newValue.TimeStamp > RightTriggerLastUpdated)
            {
                ControllerState = ControllerState with { RightTrigger = newValue.NewValue };
                RightTriggerLastUpdated = newValue.TimeStamp;

                ControllerUpdated?.Invoke(sender, new()
                {
                    NewValue = ControllerState,
                    TimeStamp = newValue.TimeStamp,
                });

                if (newValue.TimeStamp > ControllerStateLastUpdated)
                {
                    ControllerStateLastUpdated = newValue.TimeStamp;
                }
            }
        }
    }


    private readonly object _stateLock = new();

    public ControllerState ControllerState { get; private set; } = new();

    private bool _subscribed;

    private readonly Dictionary<ushort, Action<DateTime, dynamic>> TriggerControllerEventActions;

    private async Task MessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        if (e.ApplicationMessage.TopicAlias > 0)
        {
            var (timestamp, data) = ServerDataConverter.ExtractData(e.ApplicationMessage.PayloadSegment.Array);

            if (TriggerControllerEventActions.TryGetValue(e.ApplicationMessage.TopicAlias, out var updateAction))
            {
                updateAction(timestamp, data);
            }
        }
    }

    public async Task EnableControllerChangeMonitoring()
    {
        if (_subscribed)
        {
            return;
        }

        MqttClient.ApplicationMessageReceivedAsync += MessageReceived;
        StartUpdated += OnStartUpdated;
        SelectUpdated += OnSelectUpdated;
        HomeUpdated += OnHomeUpdated;
        BigHomeUpdated += OnBigHomeUpdated;
        XUpdated += OnXUpdated;
        YUpdated += OnYUpdated;
        AUpdated += OnAUpdated;
        BUpdated += OnBUpdated;
        UpUpdated += OnUpUpdated;
        RightUpdated += OnRightUpdated;
        DownUpdated += OnDownUpdated;
        LeftUpdated += OnLeftUpdated;
        LeftStickXUpdated += OnLeftStickXUpdated;
        LeftStickYUpdated += OnLeftStickYUpdated;
        LeftStickInUpdated += OnLeftStickInUpdated;
        RightStickXUpdated += OnRightStickXUpdated;
        RightStickYUpdated += OnRightStickYUpdated;
        RightStickInUpdated += OnRightStickInUpdated;
        LeftBumperUpdated += OnLeftBumperUpdated;
        LeftTriggerUpdated += OnLeftTriggerUpdated;
        RightBumperUpdated += OnRightBumperUpdated;
        RightTriggerUpdated += OnRightTriggerUpdated;

        foreach (var (_, topic) in Topics.AliasedTopics)
        {
            var subscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            await MqttClient.SubscribeAsync(subscribeOptions);
        }

        _subscribed = true;
    }

    public async Task DisableControllerChangeMonitoring()
    {
        if (!_subscribed)
        {
            return;
        }

        MqttClient.ApplicationMessageReceivedAsync -= MessageReceived;
        StartUpdated -= OnStartUpdated;
        SelectUpdated -= OnSelectUpdated;
        HomeUpdated -= OnHomeUpdated;
        BigHomeUpdated -= OnBigHomeUpdated;
        XUpdated -= OnXUpdated;
        YUpdated -= OnYUpdated;
        AUpdated -= OnAUpdated;
        BUpdated -= OnBUpdated;
        UpUpdated -= OnUpUpdated;
        RightUpdated -= OnRightUpdated;
        DownUpdated -= OnDownUpdated;
        LeftUpdated -= OnLeftUpdated;
        LeftStickXUpdated -= OnLeftStickXUpdated;
        LeftStickYUpdated -= OnLeftStickYUpdated;
        LeftStickInUpdated -= OnLeftStickInUpdated;
        RightStickXUpdated -= OnRightStickXUpdated;
        RightStickYUpdated -= OnRightStickYUpdated;
        RightStickInUpdated -= OnRightStickInUpdated;
        LeftBumperUpdated -= OnLeftBumperUpdated;
        LeftTriggerUpdated -= OnLeftTriggerUpdated;
        RightBumperUpdated -= OnRightBumperUpdated;
        RightTriggerUpdated -= OnRightTriggerUpdated;

        foreach (var (_, topic) in Topics.AliasedTopics)
        {
            var unsubscribeOptions = MqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            await MqttClient.UnsubscribeAsync(unsubscribeOptions);
        }

        _subscribed = false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisableControllerChangeMonitoring().RunSynchronously();
            MqttClient.DisconnectAsync().RunSynchronously();
            MqttClient.Dispose();
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await DisableControllerChangeMonitoring().ConfigureAwait(false);
        await MqttClient.DisconnectAsync().ConfigureAwait(false);
        MqttClient.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    ~PrototypeClient()
    {
        Dispose(false);
    }
}
