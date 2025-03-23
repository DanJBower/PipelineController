package com.danjbower.pipelinecontrollerviewer.views

import android.content.res.Configuration
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.lazy.rememberLazyListState
import androidx.compose.material3.Button
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.constraintlayout.compose.ConstraintLayout
import com.danjbower.pipelinecontrollerviewer.data.ApplicationState
import com.danjbower.pipelinecontrollerviewer.ui.theme.PipelineControllerViewerTheme
import com.danjbower.pipelinecontrollerviewer.viewmodels.interfaces.IUdpMessageViewModel
import com.danjbower.pipelinecontrollerviewer.viewmodels.mocks.MockUdpMessageViewModel
import kotlinx.coroutines.launch

@Preview(
    name = "Dark Mode",
    uiMode = Configuration.UI_MODE_NIGHT_YES or Configuration.UI_MODE_TYPE_NORMAL,
)
@Composable
fun ServerConnectionViewPreview()
{
    val model = MockUdpMessageViewModel(
        applicationState = ApplicationState.Disconnected,
        canClickConnect = true,
        canClickDisconnect = false,
        messages = listOf("Hello")
    )
    UdpMessageView(model)
}

@Composable
fun UdpMessageView(viewModel: IUdpMessageViewModel)
{
    val appState = viewModel.applicationState.collectAsState()
    val messages by viewModel.messages.collectAsState()
    val canClickConnect by viewModel.canClickConnect.collectAsState()
    val canClickDisconnect by viewModel.canClickDisconnect.collectAsState()

    PipelineControllerViewerTheme {
        Surface(modifier = Modifier
            .fillMaxSize(),
        )
        {
            Column(modifier = Modifier
                .padding(start = 5.dp, top = 30.dp, end = 5.dp, bottom = 5.dp)
            ) {
                ConstraintLayout(modifier = Modifier.fillMaxWidth())
                {
                    val (
                        stateText,
                        connectButton,
                        disconnectButton,
                    ) = createRefs()

                    Text(
                        text = "State: ${appState.value}",
                        modifier = Modifier.constrainAs(stateText)
                        {
                            start.linkTo(parent.start)

                            linkTo(top = connectButton.bottom,
                                bottom = disconnectButton.top,
                                bias = 0.5f)
                        },
                    )

                    Button(
                        modifier = Modifier.constrainAs(connectButton)
                        {
                            end.linkTo(parent.end)
                        },
                        onClick = { viewModel.connect() },
                        enabled = canClickConnect
                    ) {
                        Text("Connect")
                    }

                    Button(
                        modifier = Modifier.constrainAs(disconnectButton)
                        {
                            end.linkTo(parent.end)
                            top.linkTo(connectButton.bottom, margin = 2.dp)
                        },
                        onClick = { viewModel.disconnect() },
                        enabled = canClickDisconnect
                    ) {
                        Text("Disconnect")
                    }
                }

                // Keep track of the LazyColumn’s scroll state
                val listState = rememberLazyListState()
                val coroutineScope = rememberCoroutineScope()

                Box(modifier = Modifier
                    // .background(Color.Red)
                    .fillMaxSize(),
                    contentAlignment = Alignment.Center,
                )
                {
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
                            Text(text = msg)
                        }
                    }
                }
            }
        }
    }
}
