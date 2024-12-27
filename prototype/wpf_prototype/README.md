# WPF Prototype

This folder in the repo contains a prototype for the pipeline controller. It's written in C# for everything. The microcontroller software is stored in mcu_client, this only contains the server and various clients for providing inputs. The main components are:

* A number of common projects - Includes projects for connecting to the MQTT server and server constants. (Platform independent apart from the WPF one)
* Server - The MQTT server. (Platform independent)
* Controller Passthrough Client - A WPF app for passing controller input (Keyboard, XInput, or PS5 controller) through the server.
* Server Message Monitor - A WPF app for visualising the current controller state being sent of the server.
* Scripts folder - A collection of scripts including passthrough scripts and sample automated pipeline for sending controller inputs from a script. (Platform independent)

There's also a source generator project. This was to make it easier to write `DependencyProperty`s for WPF. It gets generated as a NuGet as ReSharper wouldn't detect it when it was referenced as a project reference even though it would successfully build.

Dependencies for building:

* NET 9
* Windows 10/11 if wanting to use the WPF apps.
* Windows SDK 10.0.26100 if wanting to use the WPF apps.
* Need to `run publish_source_generators.bat` or WPF projects won't build.
* Need to `publish_dlls.bat` or the scripts won't build.

`publish.bat` will build all the common dlls required by the scripts and also create self contained executables for the different apps mentioned above.

There is a second version of the source generator publish, `publish_source_generators_clear_old.bat`. This clears the local NuGet cache and it must be run every time the source generator code is changed or the new NuGet won't be used. It should only be run when Visual Studio is closed other wise it will fail to clean properly.

Note: If any of the published executables look blurry on windows systems with multiple DPIs in use, right click the exe > Properties > Compatibility, Change high DPI settings. Tick the override high dpi scaling behavour and set it to either `System` or `System (Enhanced)`.

