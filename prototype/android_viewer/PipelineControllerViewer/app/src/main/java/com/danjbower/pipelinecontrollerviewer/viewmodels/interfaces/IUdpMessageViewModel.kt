package com.danjbower.pipelinecontrollerviewer.viewmodels.interfaces

import com.danjbower.pipelinecontrollerviewer.data.ApplicationState
import kotlinx.coroutines.flow.StateFlow

interface IUdpMessageViewModel
{
    val messages: StateFlow<List<String>>
    val applicationState: StateFlow<ApplicationState>
    val canClickConnect: StateFlow<Boolean>
    val canClickDisconnect: StateFlow<Boolean>

    fun connect()
    fun disconnect()
}
