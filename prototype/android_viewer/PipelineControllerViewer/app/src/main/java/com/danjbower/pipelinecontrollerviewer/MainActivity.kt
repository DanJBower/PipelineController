package com.danjbower.pipelinecontrollerviewer

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import com.danjbower.pipelinecontrollerviewer.viewmodels.UdpMessageViewModel
import com.danjbower.pipelinecontrollerviewer.views.UdpMessageView

class MainActivity : ComponentActivity()
{
    override fun onCreate(savedInstanceState: Bundle?)
    {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            val model = UdpMessageViewModel()
            UdpMessageView(model)
        }
    }
}
