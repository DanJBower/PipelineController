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
            {Topics.StartTopicAlias, (timestamp, data) => OnStartUpdated(timestamp, data)},
            {Topics.SelectTopicAlias, (timestamp, data) => OnSelectUpdated(timestamp, data)},
            {Topics.HomeTopicAlias, (timestamp, data) => OnHomeUpdated(timestamp, data)},
            {Topics.BigHomeTopicAlias, (timestamp, data) => OnBigHomeUpdated(timestamp, data)},
            {Topics.XTopicAlias, (timestamp, data) => OnXUpdated(timestamp, data)},
            {Topics.YTopicAlias, (timestamp, data) => OnYUpdated(timestamp, data)},
            {Topics.ATopicAlias, (timestamp, data) => OnAUpdated(timestamp, data)},
            {Topics.BTopicAlias, (timestamp, data) => OnBUpdated(timestamp, data)},
            {Topics.UpTopicAlias, (timestamp, data) => OnUpUpdated(timestamp, data)},
            {Topics.RightTopicAlias, (timestamp, data) => OnRightUpdated(timestamp, data)},
            {Topics.DownTopicAlias, (timestamp, data) => OnDownUpdated(timestamp, data)},
            {Topics.LeftTopicAlias, (timestamp, data) => OnLeftUpdated(timestamp, data)},
            {Topics.LeftStickXTopicAlias, (timestamp, data) => OnLeftStickXUpdated(timestamp, data)},
            {Topics.LeftStickYTopicAlias, (timestamp, data) => OnLeftStickYUpdated(timestamp, data)},
            {Topics.LeftStickInTopicAlias, (timestamp, data) => OnLeftStickInUpdated(timestamp, data)},
            {Topics.RightStickXTopicAlias, (timestamp, data) => OnRightStickXUpdated(timestamp, data)},
            {Topics.RightStickYTopicAlias, (timestamp, data) => OnRightStickYUpdated(timestamp, data)},
            {Topics.RightStickInTopicAlias, (timestamp, data) => OnRightStickInUpdated(timestamp, data)},
            {Topics.LeftBumperTopicAlias, (timestamp, data) => OnLeftBumperUpdated(timestamp, data)},
            {Topics.LeftTriggerTopicAlias, (timestamp, data) => OnLeftTriggerUpdated(timestamp, data)},
            {Topics.RightBumperTopicAlias, (timestamp, data) => OnRightBumperUpdated(timestamp, data)},
            {Topics.RightTriggerTopicAlias, (timestamp, data) => OnRightTriggerUpdated(timestamp, data)},
            {Topics.FullTopicAlias, (timestamp, data) => OnFullControllerUpdated(timestamp, data)},
            {Topics.LeftStickTopicAlias, (timestamp, data) => OnLeftStickUpdated(timestamp, data)},
            {Topics.RightStickTopicAlias, (timestamp, data) => OnRightStickUpdated(timestamp, data)},
            {Topics.DebugLightTopicAlias, (timestamp, data) => OnDebugLightUpdated(timestamp, data)},
        };
    }

    private void OnFullControllerUpdated(DateTime timestamp, ControllerState state)
    {
        var updated = false;
        var startUpdated = false;
        var selectUpdated = false;
        var homeUpdated = false;
        var bigHomeUpdated = false;
        var xUpdated = false;
        var yUpdated = false;
        var aUpdated = false;
        var bUpdated = false;
        var upUpdated = false;
        var rightUpdated = false;
        var downUpdated = false;
        var leftUpdated = false;
        var leftStickXUpdated = false;
        var leftStickYUpdated = false;
        var leftStickInUpdated = false;
        var rightStickXUpdated = false;
        var rightStickYUpdated = false;
        var rightStickInUpdated = false;
        var leftBumperUpdated = false;
        var leftTriggerUpdated = false;
        var rightBumperUpdated = false;
        var rightTriggerUpdated = false;

        bool
            start,
            select,
            home,
            bigHome,
            x,
            y,
            a,
            b,
            up,
            right,
            down,
            left,
            leftStickIn,
            rightStickIn,
            leftBumper,
            rightBumper;

        float
            leftStickX,
            leftStickY,
            rightStickX,
            rightStickY,
            leftTrigger,
            rightTrigger;

        ControllerState newState;

        lock (_stateLock)
        {
            (start, select, home, bigHome,
                x, y, a, b,
                up, right, down, left,
                leftStickX, leftStickY, leftStickIn,
                rightStickX, rightStickY, rightStickIn,
                leftBumper, leftTrigger,
                rightBumper, rightTrigger) = ControllerState;

            if (timestamp > StartLastUpdated)
            {
                start = state.Start;
                StartLastUpdated = timestamp;
                updated = true;
                startUpdated = true;
            }

            if (timestamp > SelectLastUpdated)
            {
                select = state.Select;
                SelectLastUpdated = timestamp;
                updated = true;
                selectUpdated = true;
            }

            if (timestamp > HomeLastUpdated)
            {
                home = state.Home;
                HomeLastUpdated = timestamp;
                updated = true;
                homeUpdated = true;
            }

            if (timestamp > BigHomeLastUpdated)
            {
                bigHome = state.BigHome;
                BigHomeLastUpdated = timestamp;
                updated = true;
                bigHomeUpdated = true;
            }

            if (timestamp > XLastUpdated)
            {
                x = state.X;
                XLastUpdated = timestamp;
                updated = true;
                xUpdated = true;
            }

            if (timestamp > YLastUpdated)
            {
                y = state.Y;
                YLastUpdated = timestamp;
                updated = true;
                yUpdated = true;
            }

            if (timestamp > ALastUpdated)
            {
                a = state.A;
                ALastUpdated = timestamp;
                updated = true;
                aUpdated = true;
            }

            if (timestamp > BLastUpdated)
            {
                b = state.B;
                BLastUpdated = timestamp;
                updated = true;
                bUpdated = true;
            }

            if (timestamp > UpLastUpdated)
            {
                up = state.Up;
                UpLastUpdated = timestamp;
                updated = true;
                upUpdated = true;
            }

            if (timestamp > RightLastUpdated)
            {
                right = state.Right;
                RightLastUpdated = timestamp;
                updated = true;
                rightUpdated = true;
            }

            if (timestamp > DownLastUpdated)
            {
                down = state.Down;
                DownLastUpdated = timestamp;
                updated = true;
                downUpdated = true;
            }

            if (timestamp > LeftLastUpdated)
            {
                left = state.Left;
                LeftLastUpdated = timestamp;
                updated = true;
                leftUpdated = true;
            }

            if (timestamp > LeftStickXLastUpdated)
            {
                leftStickX = state.LeftStickX;
                LeftStickXLastUpdated = timestamp;
                updated = true;
                leftStickXUpdated = true;
            }

            if (timestamp > LeftStickYLastUpdated)
            {
                leftStickY = state.LeftStickY;
                LeftStickYLastUpdated = timestamp;
                updated = true;
                leftStickYUpdated = true;
            }

            if (timestamp > LeftStickInLastUpdated)
            {
                leftStickIn = state.LeftStickIn;
                LeftStickInLastUpdated = timestamp;
                updated = true;
                leftStickInUpdated = true;
            }

            if (timestamp > RightStickXLastUpdated)
            {
                rightStickX = state.RightStickX;
                RightStickXLastUpdated = timestamp;
                updated = true;
                rightStickXUpdated = true;
            }

            if (timestamp > RightStickYLastUpdated)
            {
                rightStickY = state.RightStickY;
                RightStickYLastUpdated = timestamp;
                updated = true;
                rightStickYUpdated = true;
            }

            if (timestamp > RightStickInLastUpdated)
            {
                rightStickIn = state.RightStickIn;
                RightStickInLastUpdated = timestamp;
                updated = true;
                rightStickInUpdated = true;
            }

            if (timestamp > LeftBumperLastUpdated)
            {
                leftBumper = state.LeftBumper;
                LeftBumperLastUpdated = timestamp;
                updated = true;
                leftBumperUpdated = true;
            }

            if (timestamp > LeftTriggerLastUpdated)
            {
                leftTrigger = state.LeftTrigger;
                LeftTriggerLastUpdated = timestamp;
                updated = true;
                leftTriggerUpdated = true;
            }

            if (timestamp > RightBumperLastUpdated)
            {
                rightBumper = state.RightBumper;
                RightBumperLastUpdated = timestamp;
                updated = true;
                rightBumperUpdated = true;
            }

            if (timestamp > RightTriggerLastUpdated)
            {
                rightTrigger = state.RightTrigger;
                RightTriggerLastUpdated = timestamp;
                updated = true;
                rightTriggerUpdated = true;
            }

            ControllerState = new(
                Start: start,
                Select: select,
                Home: home,
                BigHome: bigHome,
                X: x,
                Y: y,
                A: a,
                B: b,
                Up: up,
                Right: right,
                Down: down,
                Left: left,
                LeftStickX: leftStickX,
                LeftStickY: leftStickY,
                LeftStickIn: leftStickIn,
                RightStickX: rightStickX,
                RightStickY: rightStickY,
                RightStickIn: rightStickIn,
                LeftBumper: leftBumper,
                LeftTrigger: leftTrigger,
                RightBumper: rightBumper,
                RightTrigger: rightTrigger);
            newState = ControllerState;

            if (timestamp > ControllerStateLastUpdated)
            {
                ControllerStateLastUpdated = timestamp;
            }
        }

        if (updated)
        {
            if (startUpdated)
            {
                StartUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = start,
                });
            }

            if (selectUpdated)
            {
                SelectUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = select,
                });
            }

            if (homeUpdated)
            {
                HomeUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = home,
                });
            }

            if (bigHomeUpdated)
            {
                BigHomeUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = bigHome,
                });
            }

            if (xUpdated)
            {
                XUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = x,
                });
            }

            if (yUpdated)
            {
                YUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = y,
                });
            }

            if (aUpdated)
            {
                AUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = a,
                });
            }

            if (bUpdated)
            {
                BUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = b,
                });
            }

            if (upUpdated)
            {
                UpUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = up,
                });
            }

            if (rightUpdated)
            {
                RightUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = right,
                });
            }

            if (downUpdated)
            {
                DownUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = down,
                });
            }

            if (leftUpdated)
            {
                LeftUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = left,
                });
            }

            if (leftStickXUpdated)
            {
                LeftStickXUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = leftStickX,
                });
            }

            if (leftStickYUpdated)
            {
                LeftStickYUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = leftStickY,
                });
            }

            if (leftStickInUpdated)
            {
                LeftStickInUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = leftStickIn,
                });
            }

            if (rightStickXUpdated)
            {
                RightStickXUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = rightStickX,
                });
            }

            if (rightStickYUpdated)
            {
                RightStickYUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = rightStickY,
                });
            }

            if (rightStickInUpdated)
            {
                RightStickInUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = rightStickIn,
                });
            }

            if (leftBumperUpdated)
            {
                LeftBumperUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = leftBumper,
                });
            }

            if (leftTriggerUpdated)
            {
                LeftTriggerUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = leftTrigger,
                });
            }

            if (rightBumperUpdated)
            {
                RightBumperUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = rightBumper,
                });
            }

            if (rightTriggerUpdated)
            {
                RightTriggerUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = rightTrigger,
                });
            }

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = timestamp,
                NewValue = newState,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialFullMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.FullTopic)
        .WithTopicAlias(Topics.FullTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedFullMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.FullTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _fullMessageBuilder = InitialFullMessageBuilder;

    public async Task SetController(ControllerState controllerState)
    {
        await SendMessage(_fullMessageBuilder, controllerState);
        _fullMessageBuilder = AliasedFullMessageBuilder;
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

    private static readonly MqttApplicationMessageBuilder InitialLeftStickMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftStickTopic)
        .WithTopicAlias(Topics.LeftStickTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedLeftStickMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftStickTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _leftStickMessageBuilder = InitialLeftStickMessageBuilder;

    public async Task SetLeftStick((float x, float y) leftStick)
    {
        await SendMessage(_leftStickMessageBuilder, leftStick);
        _leftStickMessageBuilder = AliasedLeftStickMessageBuilder;
    }

    private void OnLeftStickUpdated(DateTime timestamp, ControllerState state)
    {
        var updated = false;
        var xUpdated = false;
        var yUpdated = false;

        float
            x,
            y;

        ControllerState newState;

        lock (_stateLock)
        {
            x = ControllerState.LeftStickX;
            y = ControllerState.LeftStickY;

            if (timestamp > LeftStickXLastUpdated)
            {
                x = state.LeftStickX;
                LeftStickXLastUpdated = timestamp;
                updated = true;
                xUpdated = true;
            }

            if (timestamp > LeftStickYLastUpdated)
            {
                y = state.LeftStickY;
                LeftStickYLastUpdated = timestamp;
                updated = true;
                yUpdated = true;
            }

            ControllerState = ControllerState with
            {
                LeftStickX = x,
                LeftStickY = y,
            };
            newState = ControllerState;

            if (timestamp > ControllerStateLastUpdated)
            {
                ControllerStateLastUpdated = timestamp;
            }
        }

        if (updated)
        {
            if (xUpdated)
            {
                LeftStickXUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = x,
                });
            }

            if (yUpdated)
            {
                LeftStickYUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = y,
                });
            }

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = timestamp,
                NewValue = newState,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightStickMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightStickTopic)
        .WithTopicAlias(Topics.RightStickTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedRightStickMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightStickTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _rightStickMessageBuilder = InitialRightStickMessageBuilder;

    public async Task SetRightStick((float x, float y) rightStick)
    {
        await SendMessage(_rightStickMessageBuilder, rightStick);
        _rightStickMessageBuilder = AliasedRightStickMessageBuilder;
    }

    private void OnRightStickUpdated(DateTime timestamp, ControllerState state)
    {
        var updated = false;
        var xUpdated = false;
        var yUpdated = false;

        float
            x,
            y;

        ControllerState newState;

        lock (_stateLock)
        {
            x = ControllerState.RightStickX;
            y = ControllerState.RightStickY;

            if (timestamp > RightStickXLastUpdated)
            {
                x = state.RightStickX;
                RightStickXLastUpdated = timestamp;
                updated = true;
                xUpdated = true;
            }

            if (timestamp > RightStickYLastUpdated)
            {
                y = state.RightStickY;
                RightStickYLastUpdated = timestamp;
                updated = true;
                yUpdated = true;
            }

            ControllerState = ControllerState with
            {
                RightStickX = x,
                RightStickY = y,
            };
            newState = ControllerState;

            if (timestamp > ControllerStateLastUpdated)
            {
                ControllerStateLastUpdated = timestamp;
            }
        }

        if (updated)
        {
            if (xUpdated)
            {
                RightStickXUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = x,
                });
            }

            if (yUpdated)
            {
                RightStickYUpdated?.Invoke(this, new()
                {
                    TimeStamp = timestamp,
                    NewValue = y,
                });
            }

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = timestamp,
                NewValue = newState,
            });
        }
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
    .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedStartMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.StartTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _startMessageBuilder = InitialStartMessageBuilder;

    public async Task SetStart(bool start)
    {
        await SendMessage(_startMessageBuilder, start);
        _startMessageBuilder = AliasedStartMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? StartUpdated;

    public DateTime StartLastUpdated { get; private set; }

    private void OnStartUpdated(DateTime timeStamp, bool start)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > StartLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Start = start };
                state = ControllerState;
                StartLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            StartUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = start,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialSelectMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.SelectTopic)
        .WithTopicAlias(Topics.SelectTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedSelectMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.SelectTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _selectMessageBuilder = InitialSelectMessageBuilder;

    public async Task SetSelect(bool select)
    {
        await SendMessage(_selectMessageBuilder, select);
        _selectMessageBuilder = AliasedSelectMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? SelectUpdated;

    public DateTime SelectLastUpdated { get; private set; }

    private void OnSelectUpdated(DateTime timeStamp, bool select)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > SelectLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Select = select };
                state = ControllerState;
                SelectLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            SelectUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = select,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.HomeTopic)
        .WithTopicAlias(Topics.HomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.HomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _homeMessageBuilder = InitialHomeMessageBuilder;

    public async Task SetHome(bool home)
    {
        await SendMessage(_homeMessageBuilder, home);
        _homeMessageBuilder = AliasedHomeMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? HomeUpdated;

    public DateTime HomeLastUpdated { get; private set; }

    private void OnHomeUpdated(DateTime timeStamp, bool home)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > HomeLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Home = home };
                state = ControllerState;
                HomeLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            HomeUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = home,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialBigHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.BigHomeTopic)
        .WithTopicAlias(Topics.BigHomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedBigHomeMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.BigHomeTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _bigHomeMessageBuilder = InitialBigHomeMessageBuilder;

    public async Task SetBigHome(bool bigHome)
    {
        await SendMessage(_bigHomeMessageBuilder, bigHome);
        _bigHomeMessageBuilder = AliasedBigHomeMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? BigHomeUpdated;

    public DateTime BigHomeLastUpdated { get; private set; }

    private void OnBigHomeUpdated(DateTime timeStamp, bool bigHome)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > BigHomeLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { BigHome = bigHome };
                state = ControllerState;
                BigHomeLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            BigHomeUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = bigHome,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.XTopic)
        .WithTopicAlias(Topics.XTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.XTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _xMessageBuilder = InitialXMessageBuilder;

    public async Task SetX(bool x)
    {
        await SendMessage(_xMessageBuilder, x);
        _xMessageBuilder = AliasedXMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? XUpdated;

    public DateTime XLastUpdated { get; private set; }

    private void OnXUpdated(DateTime timeStamp, bool x)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > XLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { X = x };
                state = ControllerState;
                XLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            XUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = x,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.YTopic)
        .WithTopicAlias(Topics.YTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.YTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _yMessageBuilder = InitialYMessageBuilder;

    public async Task SetY(bool y)
    {
        await SendMessage(_yMessageBuilder, y);
        _yMessageBuilder = AliasedYMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? YUpdated;

    public DateTime YLastUpdated { get; private set; }

    private void OnYUpdated(DateTime timeStamp, bool y)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > YLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Y = y };
                state = ControllerState;
                YLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            YUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = y,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialAMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.ATopic)
        .WithTopicAlias(Topics.ATopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedAMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.ATopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _aMessageBuilder = InitialAMessageBuilder;

    public async Task SetA(bool a)
    {
        await SendMessage(_aMessageBuilder, a);
        _aMessageBuilder = AliasedAMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? AUpdated;

    public DateTime ALastUpdated { get; private set; }

    private void OnAUpdated(DateTime timeStamp, bool a)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > ALastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { A = a };
                state = ControllerState;
                ALastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            AUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = a,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialBMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.BTopic)
        .WithTopicAlias(Topics.BTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedBMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.BTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _bMessageBuilder = InitialBMessageBuilder;

    public async Task SetB(bool b)
    {
        await SendMessage(_bMessageBuilder, b);
        _bMessageBuilder = AliasedBMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? BUpdated;

    public DateTime BLastUpdated { get; private set; }

    private void OnBUpdated(DateTime timeStamp, bool b)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > BLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { B = b };
                state = ControllerState;
                BLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            BUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = b,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialUpMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.UpTopic)
        .WithTopicAlias(Topics.UpTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedUpMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.UpTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _upMessageBuilder = InitialUpMessageBuilder;

    public async Task SetUp(bool up)
    {
        await SendMessage(_upMessageBuilder, up);
        _upMessageBuilder = AliasedUpMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? UpUpdated;

    public DateTime UpLastUpdated { get; private set; }

    private void OnUpUpdated(DateTime timeStamp, bool up)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > UpLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Up = up };
                state = ControllerState;
                UpLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            UpUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = up,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightTopic)
        .WithTopicAlias(Topics.RightTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedRightMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _rightMessageBuilder = InitialRightMessageBuilder;

    public async Task SetRight(bool right)
    {
        await SendMessage(_rightMessageBuilder, right);
        _rightMessageBuilder = AliasedRightMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? RightUpdated;

    public DateTime RightLastUpdated { get; private set; }

    private void OnRightUpdated(DateTime timeStamp, bool right)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > RightLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Right = right };
                state = ControllerState;
                RightLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            RightUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = right,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialDownMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.DownTopic)
        .WithTopicAlias(Topics.DownTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedDownMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.DownTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _downMessageBuilder = InitialDownMessageBuilder;

    public async Task SetDown(bool down)
    {
        await SendMessage(_downMessageBuilder, down);
        _downMessageBuilder = AliasedDownMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? DownUpdated;

    public DateTime DownLastUpdated { get; private set; }

    private void OnDownUpdated(DateTime timeStamp, bool down)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > DownLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Down = down };
                state = ControllerState;
                DownLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            DownUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = down,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftTopic)
        .WithTopicAlias(Topics.LeftTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedLeftMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _leftMessageBuilder = InitialLeftMessageBuilder;

    public async Task SetLeft(bool left)
    {
        await SendMessage(_leftMessageBuilder, left);
        _leftMessageBuilder = AliasedLeftMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? LeftUpdated;

    public DateTime LeftLastUpdated { get; private set; }

    private void OnLeftUpdated(DateTime timeStamp, bool left)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > LeftLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { Left = left };
                state = ControllerState;
                LeftLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            LeftUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = left,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftStickXTopic)
        .WithTopicAlias(Topics.LeftStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedLeftStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _leftStickXMessageBuilder = InitialLeftStickXMessageBuilder;

    public async Task SetLeftStickX(float leftStickX)
    {
        await SendMessage(_leftStickXMessageBuilder, leftStickX);
        _leftStickXMessageBuilder = AliasedLeftStickXMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? LeftStickXUpdated;

    public DateTime LeftStickXLastUpdated { get; private set; }

    private void OnLeftStickXUpdated(DateTime timeStamp, float leftStickX)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > LeftStickXLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { LeftStickX = leftStickX };
                state = ControllerState;
                LeftStickXLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            LeftStickXUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = leftStickX,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftStickYTopic)
        .WithTopicAlias(Topics.LeftStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedLeftStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _leftStickYMessageBuilder = InitialLeftStickYMessageBuilder;

    public async Task SetLeftStickY(float leftStickY)
    {
        await SendMessage(_leftStickYMessageBuilder, leftStickY);
        _leftStickYMessageBuilder = AliasedLeftStickYMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? LeftStickYUpdated;

    public DateTime LeftStickYLastUpdated { get; private set; }

    private void OnLeftStickYUpdated(DateTime timeStamp, float leftStickY)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > LeftStickYLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { LeftStickY = leftStickY };
                state = ControllerState;
                LeftStickYLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            LeftStickYUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = leftStickY,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftStickInTopic)
        .WithTopicAlias(Topics.LeftStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedLeftStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _leftStickInMessageBuilder = InitialLeftStickInMessageBuilder;

    public async Task SetLeftStickIn(bool leftStickIn)
    {
        await SendMessage(_leftStickInMessageBuilder, leftStickIn);
        _leftStickInMessageBuilder = AliasedLeftStickInMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? LeftStickInUpdated;

    public DateTime LeftStickInLastUpdated { get; private set; }

    private void OnLeftStickInUpdated(DateTime timeStamp, bool leftStickIn)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > LeftStickInLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { LeftStickIn = leftStickIn };
                state = ControllerState;
                LeftStickInLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            LeftStickInUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = leftStickIn,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightStickXTopic)
        .WithTopicAlias(Topics.RightStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedRightStickXMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightStickXTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _rightStickXMessageBuilder = InitialRightStickXMessageBuilder;

    public async Task SetRightStickX(float rightStickX)
    {
        await SendMessage(_rightStickXMessageBuilder, rightStickX);
        _rightStickXMessageBuilder = AliasedRightStickXMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? RightStickXUpdated;

    public DateTime RightStickXLastUpdated { get; private set; }

    private void OnRightStickXUpdated(DateTime timeStamp, float rightStickX)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > RightStickXLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { RightStickX = rightStickX };
                state = ControllerState;
                RightStickXLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            RightStickXUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = rightStickX,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightStickYTopic)
        .WithTopicAlias(Topics.RightStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedRightStickYMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightStickYTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _rightStickYMessageBuilder = InitialRightStickYMessageBuilder;

    public async Task SetRightStickY(float rightStickY)
    {
        await SendMessage(_rightStickYMessageBuilder, rightStickY);
        _rightStickYMessageBuilder = AliasedRightStickYMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? RightStickYUpdated;

    public DateTime RightStickYLastUpdated { get; private set; }

    private void OnRightStickYUpdated(DateTime timeStamp, float rightStickY)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > RightStickYLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { RightStickY = rightStickY };
                state = ControllerState;
                RightStickYLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            RightStickYUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = rightStickY,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightStickInTopic)
        .WithTopicAlias(Topics.RightStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedRightStickInMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightStickInTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _rightStickInMessageBuilder = InitialRightStickInMessageBuilder;

    public async Task SetRightStickIn(bool rightStickIn)
    {
        await SendMessage(_rightStickInMessageBuilder, rightStickIn);
        _rightStickInMessageBuilder = AliasedRightStickInMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? RightStickInUpdated;

    public DateTime RightStickInLastUpdated { get; private set; }

    private void OnRightStickInUpdated(DateTime timeStamp, bool rightStickIn)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > RightStickInLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { RightStickIn = rightStickIn };
                state = ControllerState;
                RightStickInLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            RightStickInUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = rightStickIn,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftBumperTopic)
        .WithTopicAlias(Topics.LeftBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedLeftBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _leftBumperMessageBuilder = InitialLeftBumperMessageBuilder;

    public async Task SetLeftBumper(bool leftBumper)
    {
        await SendMessage(_leftBumperMessageBuilder, leftBumper);
        _leftBumperMessageBuilder = AliasedLeftBumperMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? LeftBumperUpdated;

    public DateTime LeftBumperLastUpdated { get; private set; }

    private void OnLeftBumperUpdated(DateTime timeStamp, bool leftBumper)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > LeftBumperLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { LeftBumper = leftBumper };
                state = ControllerState;
                LeftBumperLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            LeftBumperUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = leftBumper,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialLeftTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.LeftTriggerTopic)
        .WithTopicAlias(Topics.LeftTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedLeftTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.LeftTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _leftTriggerMessageBuilder = InitialLeftTriggerMessageBuilder;

    public async Task SetLeftTrigger(float leftTrigger)
    {
        await SendMessage(_leftTriggerMessageBuilder, leftTrigger);
        _leftTriggerMessageBuilder = AliasedLeftTriggerMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? LeftTriggerUpdated;

    public DateTime LeftTriggerLastUpdated { get; private set; }

    private void OnLeftTriggerUpdated(DateTime timeStamp, float leftTrigger)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > LeftTriggerLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { LeftTrigger = leftTrigger };
                state = ControllerState;
                LeftTriggerLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            LeftTriggerUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = leftTrigger,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightBumperTopic)
        .WithTopicAlias(Topics.RightBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedRightBumperMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightBumperTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _rightBumperMessageBuilder = InitialRightBumperMessageBuilder;

    public async Task SetRightBumper(bool rightBumper)
    {
        await SendMessage(_rightBumperMessageBuilder, rightBumper);
        _rightBumperMessageBuilder = AliasedRightBumperMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? RightBumperUpdated;

    public DateTime RightBumperLastUpdated { get; private set; }

    private void OnRightBumperUpdated(DateTime timeStamp, bool rightBumper)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > RightBumperLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { RightBumper = rightBumper };
                state = ControllerState;
                RightBumperLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            RightBumperUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = rightBumper,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialRightTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.RightTriggerTopic)
        .WithTopicAlias(Topics.RightTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private static readonly MqttApplicationMessageBuilder AliasedRightTriggerMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.RightTriggerTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag(ServerConstants.RetainLastGoodMessages);

    private MqttApplicationMessageBuilder _rightTriggerMessageBuilder = InitialRightTriggerMessageBuilder;

    public async Task SetRightTrigger(float rightTrigger)
    {
        await SendMessage(_rightTriggerMessageBuilder, rightTrigger);
        _rightTriggerMessageBuilder = AliasedRightTriggerMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<float>>? RightTriggerUpdated;

    public DateTime RightTriggerLastUpdated { get; private set; }

    private void OnRightTriggerUpdated(DateTime timeStamp, float rightTrigger)
    {
        var updated = false;
        ControllerState state = new();
        DateTime controllerUpdateTime = default;

        lock (_stateLock)
        {
            if (timeStamp > RightTriggerLastUpdated)
            {
                controllerUpdateTime = ControllerStateLastUpdated;
                ControllerState = ControllerState with { RightTrigger = rightTrigger };
                state = ControllerState;
                RightTriggerLastUpdated = timeStamp;
                updated = true;

                if (timeStamp > controllerUpdateTime)
                {
                    controllerUpdateTime = timeStamp;
                }
            }
        }

        if (updated)
        {
            RightTriggerUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = rightTrigger,
            });

            ControllerUpdated?.Invoke(this, new()
            {
                TimeStamp = controllerUpdateTime,
                NewValue = state,
            });
        }
    }

    private static readonly MqttApplicationMessageBuilder InitialDebugLightMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopic(Topics.DebugLightTopic)
        .WithTopicAlias(Topics.DebugLightTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private static readonly MqttApplicationMessageBuilder AliasedDebugLightMessageBuilder = new MqttApplicationMessageBuilder()
        .WithTopicAlias(Topics.DebugLightTopicAlias)
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag();

    private MqttApplicationMessageBuilder _debugLightMessageBuilder = InitialDebugLightMessageBuilder;

    public async Task SetDebugLight(bool debugLight)
    {
        await SendMessage(_debugLightMessageBuilder, debugLight);
        _debugLightMessageBuilder = AliasedDebugLightMessageBuilder;
    }

    public event EventHandler<ValueUpdatedEventArgs<bool>>? DebugLightUpdated;

    public bool DebugLight { get; private set; }

    public DateTime DebugLightLastUpdated { get; private set; }

    private void OnDebugLightUpdated(DateTime timeStamp, bool debugLight)
    {
        var updated = false;

        lock (_stateLock)
        {
            if (timeStamp > DebugLightLastUpdated)
            {
                DebugLight = debugLight;
                DebugLightLastUpdated = timeStamp;
                updated = true;
            }
        }

        if (updated)
        {
            DebugLightUpdated?.Invoke(this, new()
            {
                TimeStamp = timeStamp,
                NewValue = debugLight,
            });
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
            TriggerControllerEventActions[e.ApplicationMessage.TopicAlias](timestamp, data);
        }
    }

    public async Task EnableControllerChangeMonitoring()
    {
        if (_subscribed)
        {
            return;
        }

        MqttClient.ApplicationMessageReceivedAsync += MessageReceived;

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
