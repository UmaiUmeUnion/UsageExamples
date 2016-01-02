# ReiPatcher usage examples

Most of the examples are written following the PHP (Patcher-Hook-Plugin) architecture and naming convention.

List of available examples:

| Example name | Description |
| ------------ | ----------- |
| `CM3D2.ExHelloWorld` | "Hello, world!" tutorial for ReiPatcher. Injects one parameterless hook into one method. Created for CM3D2. |
| `ExMultiGame` | Extended "Hello, world!" tutorial to show how to make a game-independent patcher. Demonstrates the usage of `RPConfig` class. Injects one parameterless hook into one method. |
| `CM3D2.ExAdvHooks` |  Shows how to use Mono.Cecil to inject hook methods with parameters. Injects two hooks (both with different parameters) into two different methods. Created for CM3D2.|

Each project folder contains the patcher projects, external projects and a README with a basic description and a list of required assemblies.

Every source-code file in the example is documented and commented.