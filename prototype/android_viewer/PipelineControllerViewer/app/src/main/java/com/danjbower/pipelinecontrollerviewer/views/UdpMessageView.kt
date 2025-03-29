package com.danjbower.pipelinecontrollerviewer.views

import android.content.res.Configuration
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Box
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
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.constraintlayout.compose.ConstraintLayout
import androidx.constraintlayout.compose.Dimension.Companion.fillToConstraints
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
        messages = (1..50).map { "Hello %02d".format(it) },
        debugLight = true,
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
    val debugLight by viewModel.debugLight.collectAsState()

    PipelineControllerViewerTheme {
        Surface(modifier = Modifier
            .fillMaxSize(),
        )
        {
            ConstraintLayout(modifier = Modifier.fillMaxSize()
                .padding(start = 5.dp, top = 30.dp, end = 5.dp, bottom = 5.dp))
            {
                val (
                    statusArea,
                    debugLightArea,
                    logArea,
                ) = createRefs()

                ConstraintLayout(
                    modifier = Modifier.fillMaxWidth()
                        .constrainAs(statusArea)
                        {
                            top.linkTo(parent.top)
                        }
                )
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

                Row(modifier = Modifier
                    // .background(Color.Blue)
                    .constrainAs(debugLightArea)
                    {
                        top.linkTo(statusArea.bottom)
                    }
                    .fillMaxWidth(),
                )
                {
                    Text(text = "Debug Light: $debugLight")
                }

                // Keep track of the LazyColumn’s scroll state
                val listState = rememberLazyListState()
                val coroutineScope = rememberCoroutineScope()

                Box(modifier = Modifier
                    // .background(Color.Red)
                    .constrainAs(logArea)
                    {
                        top.linkTo(debugLightArea.bottom)
                        bottom.linkTo(parent.bottom)
                        height = fillToConstraints
                    }
                    .fillMaxWidth(),
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
