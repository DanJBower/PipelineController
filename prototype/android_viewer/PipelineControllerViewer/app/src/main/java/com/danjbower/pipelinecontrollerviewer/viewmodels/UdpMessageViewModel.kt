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
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.SharingStarted
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.map
import kotlinx.coroutines.flow.stateIn
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.net.DatagramPacket
import java.net.DatagramSocket
import java.net.InetAddress

class UdpMessageViewModel : ViewModel(), IUdpMessageViewModel
{
    companion object {
        private const val LISTEN_PORT = 8913
    }

    private val _messages = MutableStateFlow<List<String>>(emptyList())
    override val messages: StateFlow<List<String>> = _messages

    private val _applicationState = MutableStateFlow(ApplicationState.Disconnected)
    override val applicationState: StateFlow<ApplicationState> = _applicationState

    override val canClickConnect: StateFlow<Boolean> = applicationState
        .map { state ->
            state in listOf(
                ApplicationState.Disconnected,
                ApplicationState.ServerNotFound,
                ApplicationState.ServerUnreachable,
                ApplicationState.Error,
            )
        }
        .stateIn(
            scope = viewModelScope,
            started = SharingStarted.Eagerly,
            initialValue = false
        )

    override val canClickDisconnect: StateFlow<Boolean> = applicationState
        .map { state ->
            state in listOf(
                ApplicationState.Searching,
                ApplicationState.Connecting,
                ApplicationState.Connected,
            )
        }
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
            searchForServer()

        }
    }

    private suspend fun searchForServer() = withContext(Dispatchers.IO)
    {
        _applicationState.value = ApplicationState.Searching

        var socket = aSocket(ActorSelectorManager(Dispatchers.IO))
            .udp()
            .bind(InetSocketAddress("0.0.0.0", LISTEN_PORT))

        try
        {
            while (isActive) {  // Use 'isActive' so we can exit cleanly if the job is canceled
                val datagram = socket.receive()
                val messageText = datagram.packet.readText()

                _messages.update { currentList ->
                    currentList + messageText
                }
            }
        }
        finally
        {
            socket.close()
        }
    }

    override fun disconnect()
    {
        _applicationState.value = ApplicationState.Disconnecting

        // Cancel the job (this triggers the `finally` block which closes the socket)
        connectionJob?.cancel()
        connectionJob = null

        _applicationState.value = ApplicationState.Disconnected
    }

    override fun onCleared() {
        super.onCleared()
        disconnect()
    }
}
