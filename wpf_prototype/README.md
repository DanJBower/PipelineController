# WPF Prototype

This folder in the repo contains a fully working prototype for the pipeline controller. It's written in C# for everything except for the Pi software. The Pi software is written for a Pi Pico W. The main components are:

* PiController - The Pi Pico controller software. Modify `**TODO INSERT FILE NAME**` to configure for your WiFi and which profile to build for (Xbox or PS5). Tested on PS5 and Xbox Series X. Note: Can probably support Xbox One/PS4 pretty easily with only small modifications as it relies on [GP2040-CE](https://gp2040-ce.info/) for the controller output, however, I didn't have one available for testing.
* A number of common projects - Includes projects for connecting to the MQTT server and server constants. (Platform independent apart from the WPF one)
* Server - The MQTT server. (Platform independent)
* Controller Passthrough Client - A WPF app for
* Server Message Monitor - A WPF app for visualising the current controller state being sent of the server.
* Scripts folder - A collection of scripts including a sample automated pipeline for sending controller inputs from a script. (Platform independent)

There's also a source generator project. This was to make it easier to write `DependencyProperty`s for WPF. It gets generated as a NuGet as ReSharper wouldn't detect it when it was referenced as a project reference even though it would successfully build.

Dependencies for building:

* NET 9
* Windows 10/11 if wanting to use the WPF apps.
* Windows SDK 10.0.26100 if wanting to use the WPF apps.
* Whatever dependencies defined for your OS by <https://datasheets.raspberrypi.com/pico/getting-started-with-pico.pdf>

Need to run publish_source_generators.bat or WPF projects won't build. This is separate from the main publish batch file. This clears the local NuGet cache and it must be run every time the source generator is changed. It should only be run when Visual Studio is closed other wise it will fail to clean properly.
