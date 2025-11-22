package com.danjbower.pipelinecontrollerviewer.viewmodels

import android.util.Log
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.danjbower.pipelinecontrollerviewer.data.ApplicationState
import com.danjbower.pipelinecontrollerviewer.data.ControllerState
import com.danjbower.pipelinecontrollerviewer.data.IpPortPair
import com.danjbower.pipelinecontrollerviewer.services.ControllerClient
import com.danjbower.pipelinecontrollerviewer.viewmodels.interfaces.IUdpMessageViewModel
import io.ktor.network.selector.ActorSelectorManager
import io.ktor.network.sockets.InetSocketAddress
import io.ktor.network.sockets.aSocket
import io.ktor.utils.io.readText
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.Job
import kotlinx.coroutines.cancelAndJoin
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.filterNotNull
import kotlinx.coroutines.flow.flatMapLatest
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import kotlin.coroutines.cancellation.CancellationException

@OptIn(ExperimentalCoroutinesApi::class)
class UdpMessageViewModel : ViewModel(), IUdpMessageViewModel
{
    companion object {
        private const val LISTEN_PORT = 8913
        private const val MAX_MESSAGE_LOG_SIZE = 1000

        private val CONNECT_STATES = listOf(
            ApplicationState.Disconnected,
            ApplicationState.ServerNotFound,
            ApplicationState.ServerUnreachable,
            ApplicationState.Error,
        )

        private val DISCONNECT_STATES = listOf(
            ApplicationState.Searching,
            ApplicationState.Connecting,
            ApplicationState.Connected,
        )

        private val SERVER_REGEX = """^PrototypeMqttServer_mqtt\._tcp@(((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}):(\d+)$""".toRegex()

        val TAG: String = UdpMessageViewModel::class.java.simpleName
    }

    private val _messagesBuffer = ArrayDeque<String>(MAX_MESSAGE_LOG_SIZE)
    private val _messages = MutableStateFlow<List<String>>(emptyList())
    override val messages: StateFlow<List<String>> = _messages

    private fun queueMessage(message: String)
    {
        synchronized(_messagesBuffer)
        {
            if (_messagesBuffer.size >= MAX_MESSAGE_LOG_SIZE) {
                _messagesBuffer.removeFirst()
            }
            _messagesBuffer.addLast(message)

            _messages.update { _ ->  _messagesBuffer.toList() }
        }
    }

    private val _applicationState = MutableStateFlow(ApplicationState.Disconnected)
    override val applicationState: StateFlow<ApplicationState> = _applicationState

    override val canClickConnect: StateFlow<Boolean> = applicationState
        .map { state -> state in CONNECT_STATES }
        .stateIn(
            scope = viewModelScope,
            started = SharingStarted.Eagerly,
            initialValue = false
        )

    override val canClickDisconnect: StateFlow<Boolean> = applicationState
        .map { state -> state in DISCONNECT_STATES }
        .stateIn(
            scope = viewModelScope,
            started = SharingStarted.Eagerly,
            initialValue = false
        )

    private var _connectionJob: Job? = null
    private var _messageListen: Job? = null

    private var _mqttClient = MutableStateFlow<ControllerClient?>(null)

    override val controllerState : StateFlow<ControllerState> = _mqttClient
        .filterNotNull()
        .flatMapLatest { it.controllerState }
        .stateIn(
            viewModelScope,
            SharingStarted.Lazily,
            ControllerState()
        )

    override val debugLight : StateFlow<Boolean> = _mqttClient
        .filterNotNull()
        .flatMapLatest { it.debugLight }
        .stateIn(
            viewModelScope,
            SharingStarted.Lazily,
            false
        )

    init
    {
        connect()
    }

    override fun connect()
    {
        if (!canClickConnect.value)
        {
            return
        }

        _connectionJob = viewModelScope.launch(Dispatchers.IO)
        {
            try
            {
                val ipPortPair = searchForServer()
                val server = connectToServer(ipPortPair)
                _mqttClient.update { _ -> server }
            }
            catch (_: CancellationException)
            {
                queueMessage("Stopping search")
            }
            catch (e: Exception)
            {
                queueMessage("Error: ${e.message}")
                Log.e(TAG, "Caught exception:", e)
                _applicationState.update { _ -> ApplicationState.Error }
            }
        }
    }

    private suspend fun searchForServer() : IpPortPair = coroutineScope()
    {
        _applicationState.update { _ -> ApplicationState.Searching }
        queueMessage("Searching for server")

        var socket = aSocket(ActorSelectorManager(Dispatchers.IO))
            .udp()
            .bind(InetSocketAddress("0.0.0.0", LISTEN_PORT))

        try
        {
            while (isActive) {  // Use 'isActive' so we can exit cleanly if the job is canceled
                val datagram = socket.receive()
                val messageText = datagram.packet.readText()

                queueMessage("UDP message received: $messageText")

                val match = SERVER_REGEX.matchEntire(messageText)

                if (match != null)
                {
                    val ip = match.groupValues[1];
                    val port = match.groupValues[5].toInt();
                    queueMessage("Matched $ip:$port")

                    return@coroutineScope IpPortPair(ip, port)
                }
            }
        }
        finally
        {
            socket.close()
        }

        throw IllegalStateException("Did not expect to get here")
    }

    private suspend fun connectToServer(ipPortPair: IpPortPair): ControllerClient = coroutineScope()
    {
        _applicationState.update { _ -> ApplicationState.Connecting }
        queueMessage("Connecting")

        val mqttClient = ControllerClient(ipPortPair, viewModelScope)

        _messageListen = viewModelScope.launch(Dispatchers.IO) {
            mqttClient.messages.collect { message -> queueMessage(message) }
        }

        mqttClient.connect()

        _applicationState.update { _ -> ApplicationState.Connected }
        queueMessage("Connected")
        return@coroutineScope mqttClient
    }

    override fun disconnect()
    {
        _applicationState.update { _ -> ApplicationState.Disconnecting }

        viewModelScope.launch()
        {
            _connectionJob?.cancelAndJoin()
            _connectionJob = null

            if (_mqttClient.value != null)
            {
                if (_mqttClient.value!!.isConnected())
                {
                    _mqttClient.value!!.disconnect()
                }
                else
                {
                    Log.i(TAG, "Already disconnected");
                }
            }

            _mqttClient.update { _ -> null }

            _messageListen?.cancelAndJoin()
            _messageListen = null

            queueMessage("Disconnected")
            _applicationState.update { _ -> ApplicationState.Disconnected }
        }
    }

    override fun onCleared() {
        super.onCleared()
        disconnect()
    }
}
