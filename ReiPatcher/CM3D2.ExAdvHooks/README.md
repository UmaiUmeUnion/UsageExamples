# Advanced Hooks Example for ReiPatcher
### Purpose
To show how to use Mono.Cecil to handle hook methods with parameters.

**NOTE**: This is a mod for CM3D2. Attempting to apply this patch on other games may result in a crash in ReiPatcher (or the game).

### Requirements
This project requires the following assemblies:

* `ReiPatcher.exe`
* `Mono.Cecil.dll`
* `Assembly-CSharp.dll` (from CM3D2)
* `UnityEngine.dll`

Put the required assemblies into `Libs` folder.

### Structure and function
This patcher was made following the PHP (Patcher-Hook-Plugin) mod architecture.
The mod contains two projects:

#### CM3D2.ExHelloWorld.Hook
Contains hook methods that are linked into the game using the patcher.

The hook assembly contains two hooks to demonstrate the different ways to call hook methods.

The `CrossFadeHook` is to show how to pass method parameters to the hook, while `SetCurExciteMax` hook is to demonstrate
how to pass type fields to the hook. All other possible hook methods can be constructed with a combination of both hooks.

#### CM3D2.ExHelloWorld.Patcher
Contains the patcher which uses Mono.Cecil to patch the game.

In this example, pay attention to how Mono.Cecil is used to inject methods and pass parameters.
It is highly advised you consult a basic tutorial on IL (for instance, [this one](http://www.codeproject.com/Articles/3778/Introduction-to-IL-Assembly-Language)).
In addition, refer to the documentation of [OpCodes class on MSDN](https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes_fields(v=vs.110).aspx) for a list
of available OpCodes and their function.

For more exact functions refer to the commented source-code.