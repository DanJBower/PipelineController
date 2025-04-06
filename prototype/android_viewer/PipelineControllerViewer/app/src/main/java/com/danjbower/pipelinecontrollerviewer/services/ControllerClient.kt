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