package com.danjbower.pipelinecontrollerviewer.viewmodels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.danjbower.pipelinecontrollerviewer.data.ApplicationState
import com.danjbower.pipelinecontrollerviewer.viewmodels.interfaces.IUdpMessageViewModel
import io.ktor.network.selector.ActorSelectorManager
import io.ktor.network.sockets.InetSocketAddress
import io.ktor.network.sockets.aSocket
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.cancelAndJoin
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import kotlin.coroutines.cancellation.CancellationException

class UdpMessageViewModel : ViewModel(), IUdpMessageViewModel
{
    companion object {
        private const val LISTEN_PORT = 8913

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
    }

    private val _messages = MutableStateFlow<List<String>>(emptyList())
    override val messages: StateFlow<List<String>> = _messages

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

    private var connectionJob: Job? = null

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

        connectionJob = viewModelScope.launch(Dispatchers.IO)
        {
            try
            {
                searchForServer()
            }
            catch (_: CancellationException)
            {
            }
            catch (_: Exception)
            {
                _applicationState.update { _ -> ApplicationState.Error }
            }
        }
    }

    private suspend fun searchForServer() : String = coroutineScope()
    {
        _applicationState.update { _ -> ApplicationState.Searching }
        _messages.update { currentList -> currentList + "Searching for server" }

        var socket = aSocket(ActorSelectorManager(Dispatchers.IO))
            .udp()
            .bind(InetSocketAddress("0.0.0.0", LISTEN_PORT))

        try
        {
            while (isActive) {  // Use 'isActive' so we can exit cleanly if the job is canceled
                val datagram = socket.receive()
                val messageText = datagram.packet.readText()

                _messages.update { currentList -> currentList + "UDP message received: $messageText" }

                val match = SERVER_REGEX.matchEntire(messageText)

                if (match != null)
                {
                    val ip = match.groupValues[1];
                    val port = match.groupValues[5];
                    _messages.update { currentList -> currentList + "Matched $ip:$port" }
                }
            }
        }
        catch (_: CancellationException)
        {
            _messages.update { currentList -> currentList + "Stopping search" }
        }
        finally
        {
            socket.close()
        }

        return@coroutineScope ""
    }

    override fun disconnect()
    {
        _applicationState.update { _ -> ApplicationState.Disconnecting }

        viewModelScope.launch()
        {
            connectionJob?.cancelAndJoin()
            connectionJob = null

            _messages.update { currentList -> currentList + "Disconnected" }
            _applicationState.update { _ -> ApplicationState.Disconnected }
        }
    }

    override fun onCleared() {
        super.onCleared()
        disconnect()
    }
}
