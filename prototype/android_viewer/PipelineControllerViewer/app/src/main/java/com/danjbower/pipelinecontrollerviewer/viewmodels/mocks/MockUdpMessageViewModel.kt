package com.danjbower.pipelinecontrollerviewer.viewmodels.mocks

import androidx.lifecycle.ViewModel
import com.danjbower.pipelinecontrollerviewer.data.ApplicationState
import com.danjbower.pipelinecontrollerviewer.data.ControllerState
import com.danjbower.pipelinecontrollerviewer.viewmodels.interfaces.IUdpMessageViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow

class MockUdpMessageViewModel(
    messages : List<String>,
    applicationState : ApplicationState,
    canClickConnect : Boolean,
    canClickDisconnect : Boolean,
    debugLight : Boolean,
    controllerState : ControllerState,
) : ViewModel(), IUdpMessageViewModel
{
    override val messages: StateFlow<List<String>> = MutableStateFlow<List<String>>(messages)
    override val applicationState: StateFlow<ApplicationState> = MutableStateFlow(applicationState)
    override val canClickConnect: StateFlow<Boolean> = MutableStateFlow(canClickConnect)
    override val canClickDisconnect: StateFlow<Boolean> = MutableStateFlow(canClickDisconnect)
    override val debugLight: StateFlow<Boolean> = MutableStateFlow(debugLight)
    override val controllerState: StateFlow<ControllerState> = MutableStateFlow(controllerState)

    override fun connect() { }
    override fun disconnect() { }
}
