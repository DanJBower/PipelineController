@ECHO OFF

@REM Elevate to admin https://stackoverflow.com/a/52517718/4601149
echo Checking elevated
set "params=%*"
cd /d "%~dp0" && ( if exist "%temp%\getadmin.vbs" del "%temp%\getadmin.vbs" ) && fsutil dirty query %systemdrive% 1>nul 2>nul || (  echo Set UAC = CreateObject^("Shell.Application"^) : UAC.ShellExecute "cmd.exe", "/c cd ""%~sdp0"" && ""%~s0"" %params%", "", "runas", 1 >> "%temp%\getadmin.vbs" && "%temp%\getadmin.vbs" && exit /B )

@REM Add rules
echo Adding MqttControllerServerBroadcastInbound
netsh advfirewall firewall add rule name=MqttControllerServerBroadcastInbound dir=in action=allow protocol=UDP localport=8913 remoteport=8914 profile=private,domain

pause
