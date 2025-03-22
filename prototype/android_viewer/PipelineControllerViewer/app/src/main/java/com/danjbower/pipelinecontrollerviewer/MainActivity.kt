package com.danjbower.pipelinecontrollerviewer

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import com.danjbower.pipelinecontrollerviewer.ui.theme.PipelineControllerViewerTheme
import com.danjbower.pipelinecontrollerviewer.viewmodels.UdpMessageViewModel
import com.danjbower.pipelinecontrollerviewer.views.UdpMessageView

class MainActivity : ComponentActivity()
{
    override fun onCreate(savedInstanceState: Bundle?)
    {
        val udpMessageViewModel = UdpMessageViewModel()

        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            PipelineControllerViewerTheme {
                Surface(modifier = Modifier
                    .fillMaxSize(),
                )
                {
                    Box(modifier = Modifier
                        .fillMaxSize(),
                        contentAlignment = Alignment.Center,
                    )
                    {
                        UdpMessageView(viewModel = udpMessageViewModel)
                    }
                }
            }
        }
    }
}

@Composable
fun Greeting(name: String, modifier: Modifier = Modifier)
{
    Text(
        text = "Hello $name!",
        modifier = modifier
    )
}

@Preview(showBackground = true)
@Composable
fun GreetingPreview()
{
    PipelineControllerViewerTheme {
        Greeting("Android")
    }
}