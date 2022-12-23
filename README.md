# Pipeline controller

Pipeline controller provides a way to send Xbox Series X/S and PS5 controller signals to Xbox Series X/S, Windows PCs, and PS5s

It allows automated plans to be created and executed on the target machine as if a real controller were playing the game

Created initially as a way to perform more complex grinding tasks within games

Disclaimer: You should **not** be using this to cheat in games and it is best **not** to use this in multiplayer if you don't want to get banned

This will for now be privately developed. If the code is made public, it may or may not be made open source. I'd like to find a license that basically says you can use this for whatever open-source(would it need a share-alike license like GPL?), research, education, internal use, or private personal use you'd like. However, you may not use anything substantial from this project for making any money, patents, etc...

## Roadmap

* [Version 0.1 - Basic PS5 Support](https://github.com/DanJBower/PipelineController/milestone/1)
* [Version 1.0 - Public Release](https://github.com/DanJBower/PipelineController/milestone/2)

## How to Install

**TODO**

## How to Use

**TODO**

### Client

Press buttons :)

### Server

`portable-controller --target xbox series x/ps5 --max-readonly-connections [∞] --password [NONE] --readonly-password [Same as password] --help`

## Development Dependencies

* Visual Studio?
  * With what packages
* .NET 7.0 - **TODO** Clarify exactly what is needed
* MAUI - **TODO** Clarify exactly what is needed
* Android - **TODO** Clarify exactly what is needed
* [dotnet script](https://github.com/dotnet-script/dotnet-script) - Run the `Restore System Dependencies` VS Code task
* [Stryker.NET](https://stryker-mutator.io/) - Run the `Restore System Dependencies` VS Code task
* [Node.js](https://nodejs.org/en/) - `npm` and `node` commands need to be accessible on the path

## VS Code Tasks

* `Publish All` - Runs all the other publish tasks
* `Publish Android App` - Creates a distributable `.apk` file that can be installed on any Android X+ mobile
* `Publish Windows App` - Creates a distributable `(windows app installer?)` file that can be installed on any Windows 10+ device
* `Publish Windows App Server` - Creates a distributable `(windows app installer?)` that installs the server CLI and adds to path (somehow)
* `Publish Windows Standalone Server` - Creates a distributable `.exe` version of the server that can be run on Windows 10+ without requiring any installation
* `Publish Raspberry Pi 4 Ubuntu Arm 64 Server` - Creates a distributable `(linux thing)` that can be installed on Ubuntu Arm64 Version I have - **TODO** Clarify
* ~~`Publish Windows Standalone` - A runnable client exe that can be run without installing~~ - This is not currently possible, need to wait for this [GitHub issue](https://github.com/dotnet/maui/issues/10564) to be completed - Sadly this has been pushed to .NET 8
* `Run Mutation Tests` -
* `Run Tests` -
* `Generate API Documentation` -
* `Restore System Dependencies` - Restores .NET tool dependencies
* `Clean` - Removes all temporary files

## Github Actions

### How to Release a New Version

* Add git tag in format XXX
* Distinguish between pre-release and proper release

### Pull Requests

* A CI pipeline gets run for every pull request and commit to main that does: **TODO**
