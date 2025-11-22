package com.danjbower.pipelinecontrollerviewer.viewmodels.interfaces

import com.danjbower.pipelinecontrollerviewer.data.ApplicationState
import com.danjbower.pipelinecontrollerviewer.data.ControllerState
import kotlinx.coroutines.Deferred
import kotlinx.coroutines.flow.StateFlow

interface IUdpMessageViewModel
{
    val messages: StateFlow<List<String>>
    val applicationState: StateFlow<ApplicationState>
    val canClickConnect: StateFlow<Boolean>
    val canClickDisconnect: StateFlow<Boolean>
    val debugLight : StateFlow<Boolean>
    val controllerState : StateFlow<ControllerState>

    fun connect()
    fun disconnect(): Deferred<Unit>
}
