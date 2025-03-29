package com.danjbower.pipelinecontrollerviewer.services

import android.util.Log
import com.danjbower.pipelinecontrollerviewer.data.IpPortPair
import com.danjbower.pipelinecontrollerviewer.data.ServerDataConverter
import com.danjbower.pipelinecontrollerviewer.data.TopicConstants
import com.hivemq.client.mqtt.MqttClient
import com.hivemq.client.mqtt.datatypes.MqttQos
import com.hivemq.client.mqtt.mqtt5.Mqtt5AsyncClient
import kotlinx.coroutines.future.await
import kotlinx.coroutines.runBlocking
import java.io.Closeable
import java.time.Instant
import java.util.UUID

class ControllerClient(ipPort: IpPortPair) : Closeable
{
    companion object {
        val TAG: String = ControllerClient::class.java.simpleName
    }

    private val _client: Mqtt5AsyncClient = MqttClient.builder()
        .useMqttVersion5()
        .serverHost(ipPort.ip)
        .serverPort(ipPort.port)
        .identifier(UUID.randomUUID().toString())
        .buildAsync()

    private val _callbackLookup : Map<Int, (Instant, Any?)-> Unit> = mapOf(
        TopicConstants.DebugLightTopicAlias to ::onDebugLightMessage
    )

    suspend fun connect() {
        _client.connectWith()
            .restrictions()
                .topicAliasMaximum(255)
                .applyRestrictions()
            .send()
            .await()

        TopicConstants.AliasedTopics.forEach { (alias, topic) ->
            _client.subscribeWith()
                .topicFilter(topic)
                .qos(MqttQos.AT_LEAST_ONCE)
                .callback { p ->
                    var (timestamp, data) = ServerDataConverter.extractData(p.payloadAsBytes)
                    _callbackLookup[alias]?.let { it(timestamp, data) }
                }
                .send()
                .await()
        }
    }

    fun onDebugLightMessage(timestamp: Instant, value: Any?)
    {
        if (value is Boolean) {
            Log.i(TAG, "$timestamp: $value")
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