package com.danjbower.pipelinecontrollerviewer.views

import android.content.res.Configuration
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.statusBarsPadding
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
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.constraintlayout.compose.ConstraintLayout
import androidx.constraintlayout.compose.Dimension.Companion.fillToConstraints
import com.danjbower.pipelinecontrollerviewer.data.ApplicationState
import com.danjbower.pipelinecontrollerviewer.data.ControllerState
import com.danjbower.pipelinecontrollerviewer.ui.theme.PipelineControllerViewerTheme
import com.danjbower.pipelinecontrollerviewer.utilities.toTFString
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
        controllerState = ControllerState(
            start = true,
            select = false,
            home = false,
            bigHome = false,
            x = false,
            y = false,
            a = false,
            b = false,
            up = false,
            right = false,
            down = false,
            left = false,
            leftStickX = -0.311f,
            leftStickY = 0f,
            leftStickIn = false,
            rightStickX = 0f,
            rightStickY = 0f,
            rightStickIn = false,
            leftBumper = false,
            leftTrigger = 0f,
            rightBumper = false,
            rightTrigger = 0f,
        )
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
    val controllerState by viewModel.controllerState.collectAsState()

    PipelineControllerViewerTheme {
        Surface(
            modifier = Modifier
                .fillMaxSize(),
        )
        {
            Surface(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(start = 5.dp, end = 5.dp, bottom = 5.dp)
            )
            {
                ConstraintLayout(
                    modifier = Modifier
                        .fillMaxSize()
                        .statusBarsPadding()
                )
                {
                    val (
                        statusArea,
                        infoArea,
                        logArea,
                    ) = createRefs()

                    ConstraintLayout(
                        modifier = Modifier
                            .fillMaxWidth()
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

                                linkTo(
                                    top = connectButton.bottom,
                                    bottom = disconnectButton.top,
                                    bias = 0.5f
                                )
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

                    Column(
                        modifier = Modifier
                            // .background(Color.Blue)
                            .constrainAs(infoArea)
                            {
                                top.linkTo(statusArea.bottom)
                            }
                            .fillMaxWidth(),
                    )
                    {
                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "Debug Light: ${debugLight.toTFString()}")
                        }

                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "Start: ${controllerState.start.toTFString()}")
                            Text(text = "Select: ${controllerState.select.toTFString()}")
                        }

                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "Touch Pad: ${controllerState.bigHome.toTFString()}")
                            Text(text = "Home: ${controllerState.home.toTFString()}")
                        }

                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "A: ${controllerState.a.toTFString()}")
                            Text(text = "B: ${controllerState.b.toTFString()}")
                            Text(text = "X: ${controllerState.x.toTFString()}")
                            Text(text = "Y: ${controllerState.y.toTFString()}")
                        }

                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "↑: ${controllerState.up.toTFString()}")
                            Text(text = "→: ${controllerState.right.toTFString()}")
                            Text(text = "↓: ${controllerState.down.toTFString()}")
                            Text(text = "←: ${controllerState.left.toTFString()}")
                        }

                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "LT: ${"% 5.2f".format(controllerState.leftTrigger)}")
                            Text(text = "LB: ${controllerState.leftBumper.toTFString()}")
                            Text(text = "RT: ${"% 5.2f".format(controllerState.rightTrigger)}")
                            Text(text = "RB: ${controllerState.rightBumper.toTFString()}")
                        }

                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "LX: ${"% 5.2f".format(controllerState.leftStickX)}")
                            Text(text = "LY: ${"% 5.2f".format(controllerState.leftStickY)}")
                            Text(text = "L3: ${controllerState.leftStickIn.toTFString()}")
                        }

                        Row(
                            modifier = Modifier
                                // .background(Color.Blue)
                                .fillMaxWidth(),
                            horizontalArrangement = Arrangement.SpaceEvenly
                        )
                        {
                            Text(text = "RX: ${"% 5.2f".format(controllerState.rightStickX)}")
                            Text(text = "RY: ${"% 5.2f".format(controllerState.rightStickY)}")
                            Text(text = "R3: ${controllerState.rightStickIn.toTFString()}")
                        }
                    }

                    // Keep track of the LazyColumn’s scroll state
                    val listState = rememberLazyListState()
                    val coroutineScope = rememberCoroutineScope()

                    Box(
                        modifier = Modifier
                            // .background(Color.Red)
                            .constrainAs(logArea)
                            {
                                top.linkTo(infoArea.bottom)
                                bottom.linkTo(parent.bottom)
                                height = fillToConstraints
                            }
                            .fillMaxWidth(),
                    )
                    {
                        // Whenever `messages.size` changes, scroll to the bottom
                        LaunchedEffect(messages.size) {
                            // Scroll to the last index (if at least one item)
                            if (messages.isNotEmpty())
                            {
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
}
