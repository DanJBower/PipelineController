package com.danjbower.pipelinecontrollerviewer.viewmodels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.danjbower.pipelinecontrollerviewer.data.UdpMessage
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.update
import kotlinx.coroutines.launch
import java.net.DatagramPacket
import java.net.DatagramSocket
import java.net.InetAddress

class UdpMessageViewModel : ViewModel()
{
    companion object {
        private const val LISTEN_PORT = 8913
    }

    private val _messages = MutableStateFlow<List<UdpMessage>>(emptyList())
    val messages: StateFlow<List<UdpMessage>> = _messages

    init {
        viewModelScope.launch(Dispatchers.IO) {
            listenForUdpBroadcasts()
        }
    }

    private fun listenForUdpBroadcasts() {
        // Bind to all network interfaces (0.0.0.0) on the specified port
        val socket = DatagramSocket(LISTEN_PORT, InetAddress.getByName("0.0.0.0"))
        socket.broadcast = true

        val buffer = ByteArray(1024)

        while (true) {
            val packet = DatagramPacket(buffer, buffer.size)
            // Blocking call that waits for incoming packet
            socket.receive(packet)

            val messageText = String(packet.data, 0, packet.length)
            val senderIp = packet.address.hostAddress

            // Update the list of messages in a thread-safe way
            _messages.update { currentList ->
                currentList + UdpMessage(senderIp, messageText)
            }
        }
        // If needed, handle `socket.close()` somewhere (e.g. onCleared)
        // but in many scenarios you’ll keep listening indefinitely
    }
}