package com.danjbower.pipelinecontrollerviewer.data

enum class ApplicationState
{
    Disconnected,
    Disconnecting,
    Searching,
    ServerNotFound,
    ServerUnreachable,
    Connecting,
    Connected,
    Error,
    ;
}
