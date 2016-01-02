# Hello, World! Example for ReiPatcher
### Purpose
To show off the basic structure of a patcher and demonstrate how to make simple method call injections using Mono.Cecil.

**NOTE**: This is a mod for CM3D2. Attempting to apply this patch on other games may result in a crash in ReiPatcher (or the game).

### Requirements
This project requires the following assemblies:

* `ReiPatcher.exe`
* `Mono.Cecil.dll`

Put the required assemblies into `Libs` folder.

### Structure and function
This patcher was made following the PHP (Patcher-Hook-Plugin) mod architecture.
The mod contains two projects:

#### CM3D2.ExHelloWorld.Hook
Contains hook methods that are linked into the game using the patcher.

#### CM3D2.ExHelloWorld.Patcher
Contains the patcher which uses Mono.Cecil to patch the game.

For more exact functions refer to the commented source-code.