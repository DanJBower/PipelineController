package com.danjbower.pipelinecontrollerviewer.services

import android.util.Log
import com.danjbower.pipelinecontrollerviewer.data.ControllerState
import com.danjbower.pipelinecontrollerviewer.data.IpPortPair
import com.danjbower.pipelinecontrollerviewer.data.ServerDataConverter
import com.danjbower.pipelinecontrollerviewer.data.TopicConstants
import com.hivemq.client.mqtt.MqttClient
import com.hivemq.client.mqtt.datatypes.MqttQos
import com.hivemq.client.mqtt.mqtt5.Mqtt5AsyncClient
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asSharedFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.future.await
import kotlinx.coroutines.launch
import kotlinx.coroutines.runBlocking
import java.io.Closeable
import java.time.Instant
import java.util.UUID

class ControllerClient(ipPort: IpPortPair,
    private val coroutineScope: CoroutineScope) : Closeable
{
    companion object {
        val TAG: String = ControllerClient::class.java.simpleName
    }

    private val _messages = MutableSharedFlow<String>()
    val messages = _messages.asSharedFlow()

    private val _controllerState = MutableStateFlow<ControllerState>(ControllerState())
    val controllerState : StateFlow<ControllerState> = _controllerState

    private val _debugLight = MutableStateFlow<Boolean>(false)
    val debugLight : StateFlow<Boolean> = _debugLight

    private val _stateLock = Any()

    private val _client: Mqtt5AsyncClient = MqttClient.builder()
        .useMqttVersion5()
        .serverHost(ipPort.ip)
        .serverPort(ipPort.port)
        .identifier(UUID.randomUUID().toString())
        .buildAsync()

    private val _callbackLookup : Map<Int, suspend (Instant, Any?)-> Unit> = mapOf(
        TopicConstants.DebugLightTopicAlias to ::onDebugLightMessage,
        TopicConstants.StartTopicAlias to ::onStartMessage,
        TopicConstants.SelectTopicAlias to ::onSelectMessage,
        TopicConstants.HomeTopicAlias to ::onHomeMessage,
        TopicConstants.BigHomeTopicAlias to ::onBigHomeMessage,
        TopicConstants.XTopicAlias to ::onXMessage,
        TopicConstants.YTopicAlias to ::onYMessage,
        TopicConstants.ATopicAlias to ::onAMessage,
        TopicConstants.BTopicAlias to ::onBMessage,
        TopicConstants.UpTopicAlias to ::onUpMessage,
        TopicConstants.RightTopicAlias to ::onRightMessage,
        TopicConstants.DownTopicAlias to ::onDownMessage,
        TopicConstants.LeftTopicAlias to ::onLeftMessage,
        TopicConstants.LeftStickXTopicAlias to ::onLeftStickXMessage,
        TopicConstants.LeftStickYTopicAlias to ::onLeftStickYMessage,
        TopicConstants.LeftStickInTopicAlias to ::onLeftStickInMessage,
        TopicConstants.RightStickXTopicAlias to ::onRightStickXMessage,
        TopicConstants.RightStickYTopicAlias to ::onRightStickYMessage,
        TopicConstants.RightStickInTopicAlias to ::onRightStickInMessage,
        TopicConstants.LeftBumperTopicAlias to ::onLeftBumperMessage,
        TopicConstants.LeftTriggerTopicAlias to ::onLeftTriggerMessage,
        TopicConstants.RightBumperTopicAlias to ::onRightBumperMessage,
        TopicConstants.RightTriggerTopicAlias to ::onRightTriggerMessage,
    )

    suspend fun connect() {
        _client.connectWith()
            .restrictions()
                .topicAliasMaximum(255)
                .applyRestrictions()
            .send()
            .await()

        TopicConstants.AliasedTopics.forEach { (alias, topic) ->
            val callback = _callbackLookup[alias]!!
            _client.subscribeWith()
                .topicFilter(topic)
                .qos(MqttQos.AT_LEAST_ONCE)
                .callback { p ->
                    coroutineScope.launch(Dispatchers.IO) {
                        var (timestamp, data) = ServerDataConverter.extractData(p.payloadAsBytes)
                        callback(timestamp, data)
                    }
                }
                .send()
                .await()
        }
    }

    private var _lastDebugLightUpdate = Instant.ofEpochSecond(0)

    suspend fun onDebugLightMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.DebugLightTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastDebugLightUpdate)
                {
                    _debugLight.update { _ -> value }
                }
                _lastDebugLightUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onDebugLightMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastStartUpdate = Instant.ofEpochSecond(0)

    suspend fun onStartMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.StartTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastStartUpdate)
                {
                    _controllerState.update { state -> state.copy(start = value) }
                }
                _lastStartUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onStartMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastSelectUpdate = Instant.ofEpochSecond(0)

    suspend fun onSelectMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.SelectTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastSelectUpdate)
                {
                    _controllerState.update { state -> state.copy(select = value) }
                }
                _lastSelectUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onSelectMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastHomeUpdate = Instant.ofEpochSecond(0)

    suspend fun onHomeMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.HomeTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastHomeUpdate)
                {
                    _controllerState.update { state -> state.copy(home = value) }
                }
                _lastHomeUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onHomeMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastBigHomeUpdate = Instant.ofEpochSecond(0)

    suspend fun onBigHomeMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.BigHomeTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastBigHomeUpdate)
                {
                    _controllerState.update { state -> state.copy(bigHome = value) }
                }
                _lastBigHomeUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onBigHomeMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastXUpdate = Instant.ofEpochSecond(0)

    suspend fun onXMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.XTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastXUpdate)
                {
                    _controllerState.update { state -> state.copy(x = value) }
                }
                _lastXUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onXMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastYUpdate = Instant.ofEpochSecond(0)

    suspend fun onYMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.YTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastYUpdate)
                {
                    _controllerState.update { state -> state.copy(y = value) }
                }
                _lastYUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onYMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastAUpdate = Instant.ofEpochSecond(0)

    suspend fun onAMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.ATopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastAUpdate)
                {
                    _controllerState.update { state -> state.copy(a = value) }
                }
                _lastAUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onAMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastBUpdate = Instant.ofEpochSecond(0)

    suspend fun onBMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.BTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastBUpdate)
                {
                    _controllerState.update { state -> state.copy(b = value) }
                }
                _lastBUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onBMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastUpUpdate = Instant.ofEpochSecond(0)

    suspend fun onUpMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.UpTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastUpUpdate)
                {
                    _controllerState.update { state -> state.copy(up = value) }
                }
                _lastUpUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onUpMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastRightUpdate = Instant.ofEpochSecond(0)

    suspend fun onRightMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.RightTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastRightUpdate)
                {
                    _controllerState.update { state -> state.copy(right = value) }
                }
                _lastRightUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onRightMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastDownUpdate = Instant.ofEpochSecond(0)

    suspend fun onDownMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.DownTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastDownUpdate)
                {
                    _controllerState.update { state -> state.copy(down = value) }
                }
                _lastDownUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onDownMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastLeftUpdate = Instant.ofEpochSecond(0)

    suspend fun onLeftMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.LeftTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastLeftUpdate)
                {
                    _controllerState.update { state -> state.copy(left = value) }
                }
                _lastLeftUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onLeftMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastLeftStickXUpdate = Instant.ofEpochSecond(0)

    suspend fun onLeftStickXMessage(timestamp: Instant, value: Any?)
    {
        if (value is Float)
        {
            _messages.emit("$timestamp: ${TopicConstants.LeftStickXTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastLeftStickXUpdate)
                {
                    _controllerState.update { state -> state.copy(leftStickX = value) }
                }
                _lastLeftStickXUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onLeftStickXMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastLeftStickYUpdate = Instant.ofEpochSecond(0)

    suspend fun onLeftStickYMessage(timestamp: Instant, value: Any?)
    {
        if (value is Float)
        {
            _messages.emit("$timestamp: ${TopicConstants.LeftStickYTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastLeftStickYUpdate)
                {
                    _controllerState.update { state -> state.copy(leftStickY = value) }
                }
                _lastLeftStickYUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onLeftStickYMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastLeftStickInUpdate = Instant.ofEpochSecond(0)

    suspend fun onLeftStickInMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.LeftStickInTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastLeftStickInUpdate)
                {
                    _controllerState.update { state -> state.copy(leftStickIn = value) }
                }
                _lastLeftStickInUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onLeftStickInMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastRightStickXUpdate = Instant.ofEpochSecond(0)

    suspend fun onRightStickXMessage(timestamp: Instant, value: Any?)
    {
        if (value is Float)
        {
            _messages.emit("$timestamp: ${TopicConstants.RightStickXTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastRightStickXUpdate)
                {
                    _controllerState.update { state -> state.copy(rightStickX = value) }
                }
                _lastRightStickXUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onRightStickXMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastRightStickYUpdate = Instant.ofEpochSecond(0)

    suspend fun onRightStickYMessage(timestamp: Instant, value: Any?)
    {
        if (value is Float)
        {
            _messages.emit("$timestamp: ${TopicConstants.RightStickYTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastRightStickYUpdate)
                {
                    _controllerState.update { state -> state.copy(rightStickY = value) }
                }
                _lastRightStickYUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onRightStickYMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastRightStickInUpdate = Instant.ofEpochSecond(0)

    suspend fun onRightStickInMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.RightStickInTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastRightStickInUpdate)
                {
                    _controllerState.update { state -> state.copy(rightStickIn = value) }
                }
                _lastRightStickInUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onRightStickInMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastLeftBumperUpdate = Instant.ofEpochSecond(0)

    suspend fun onLeftBumperMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.LeftBumperTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastLeftBumperUpdate)
                {
                    _controllerState.update { state -> state.copy(leftBumper = value) }
                }
                _lastLeftBumperUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onLeftBumperMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastLeftTriggerUpdate = Instant.ofEpochSecond(0)

    suspend fun onLeftTriggerMessage(timestamp: Instant, value: Any?)
    {
        if (value is Float)
        {
            _messages.emit("$timestamp: ${TopicConstants.LeftTriggerTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastLeftTriggerUpdate)
                {
                    _controllerState.update { state -> state.copy(leftTrigger = value) }
                }
                _lastLeftTriggerUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onLeftTriggerMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastRightBumperUpdate = Instant.ofEpochSecond(0)

    suspend fun onRightBumperMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean)
        {
            _messages.emit("$timestamp: ${TopicConstants.RightBumperTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastRightBumperUpdate)
                {
                    _controllerState.update { state -> state.copy(rightBumper = value) }
                }
                _lastRightBumperUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onRightBumperMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    private var _lastRightTriggerUpdate = Instant.ofEpochSecond(0)

    suspend fun onRightTriggerMessage(timestamp: Instant, value: Any?)
    {
        if (value is Float)
        {
            _messages.emit("$timestamp: ${TopicConstants.RightTriggerTopic} $value")

            synchronized(_stateLock)
            {
                if (timestamp > _lastRightTriggerUpdate)
                {
                    _controllerState.update { state -> state.copy(rightTrigger = value) }
                }
                _lastRightTriggerUpdate = timestamp
            }
        }
        else
        {
            throw IllegalArgumentException("${this::onRightTriggerMessage.javaClass.simpleName}: $value was not the expected type")
        }
    }

    suspend fun disconnect() {
        _client.disconnect().await()
    }

    override fun close()
    {
        runBlocking {
            disconnect()
        }
    }
}