# Minimal setup for allowing Android app to connect to local server

Guide assumes Windows 10 or 11 environment.

Pipeline Controller is a .NET 9 application that runs a MQTT server on the local network. When the server is running it outputs a UDP broadcast every second that contains the IP and port to connect to on the local network.

The Android app listens for the UDP broadcast and then connects to the MQTT server based on the information within the broadcast.

To be able to run the server and generate inputs to visualise within the app, the following steps must be taken:

* Install pre-requisites:
  * Install [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) - Any version of .NET 9 is okay
  * Install [dotnet-script 1.6.0](https://www.nuget.org/packages/dotnet-script/1.6.0)
* Make sure pre-requisites are available on the path environment variable:
  * `C:\Program Files\dotnet\` (or wherever it is installed)
  * `%USERPROFILE%\.dotnet\tools`
* Clone the Pipeline Controller Repository - <https://github.com/DanJBower/PipelineController>
* Generate the required .dll files by running `PipelineController\prototype\wpf_prototype\publish_dlls.bat`
  * Assumes working directory is `PipelineController\prototype\wpf_prototype`
* Open two command prompts in `PipelineController\prototype\wpf_prototype\scripts`:
  * In the first command prompt run `dotnet-script Server.csx` - This will start the controller server
  * In the second command prompt run `dotnet-script SampleAutomatedScript.csx` - This will automatically drive sample inputs to the server that can be visualised in the app
* The app auto starts the connection process when it is opened. It can also be manually started by clicked "Connect" in the app. Once the server is running and the app is searching for a connection, it will automatically connect to the server.
