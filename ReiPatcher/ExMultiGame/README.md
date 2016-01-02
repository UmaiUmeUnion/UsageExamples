# Multiple Game Patcher Example for ReiPatcher
### Purpose
To demonstrate how to use `RPConfig` class to create game-independent patchers.

### Requirements
This project requires the following assemblies:

* `ReiPatcher.exe`
* `Mono.Cecil.dll`

Put the required assemblies into `Libs` folder.

### Structure and function
This patcher was made following the PHP (Patcher-Hook-Plugin) mod architecture.

The mod has the same function as CM3D2.ExHelloWorld, except that this patcher does not depend on a specific game assembly.
The mod contains two projects:

#### CM3D2.ExHelloWorld.Hook
Contains hook methods that are linked into the game using the patcher.

#### CM3D2.ExHelloWorld.Patcher
Contains the patcher which uses Mono.Cecil to patch the game.

The main difference from CM3D2.ExHelloWorld is that this patcher uses `RPConfig` class to get information about the
target assembly, target type and target method. The `RPConfig` class allows to use ReiPatcher's game configuration
(the configuration file passed with `-c` flag to ReiPatcher.exe) to get the targets. Since the configuration varies
from game to game, the values may vary as well. 

A good example of a mod that uses this technique is UnityInjector.

For more exact functions refer to the commented source-code.