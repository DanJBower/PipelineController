package com.danjbower.pipelinecontrollerviewer.views

import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.rememberLazyListState
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.rememberCoroutineScope
import com.danjbower.pipelinecontrollerviewer.viewmodels.UdpMessageViewModel
import kotlinx.coroutines.launch
import androidx.compose.runtime.getValue
import androidx.compose.foundation.lazy.items

@Composable
fun UdpMessageView(viewModel: UdpMessageViewModel)
{
    val messages by viewModel.messages.collectAsState()

    // Keep track of the LazyColumn’s scroll state
    val listState = rememberLazyListState()
    val coroutineScope = rememberCoroutineScope()

    // Whenever `messages.size` changes, scroll to the bottom
    LaunchedEffect(messages.size) {
        // Scroll to the last index (if at least one item)
        if (messages.isNotEmpty()) {
            coroutineScope.launch {
                listState.animateScrollToItem(messages.size - 1)
            }
        }
    }

    // Show messages in a lazy list
    LazyColumn(state = listState) {
        items(messages) { msg ->
            Text(text = "${msg.senderIp}: ${msg.text}")
        }
    }
}
