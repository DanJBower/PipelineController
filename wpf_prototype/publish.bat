dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRunShowWarnings=true "common\ServerInfo\ServerInfo.csproj" -o release -c release
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRunShowWarnings=true "common\CommonClient\CommonClient.csproj" -o release -c release
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRunShowWarnings=true "Server\Server\Server.csproj" -o release -c release
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRunShowWarnings=true "ControllerPassthroughClient\ControllerPassthroughClient\ControllerPassthroughClient.csproj" -o release -c release
dotnet publish /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRunShowWarnings=true "ServerMessageMonitor\ServerMessageMonitor\ServerMessageMonitor.csproj" -o release -c release
