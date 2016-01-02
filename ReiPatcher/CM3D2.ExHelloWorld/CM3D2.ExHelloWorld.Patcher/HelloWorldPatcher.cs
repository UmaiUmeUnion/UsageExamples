using System;
using System.IO;
/*
    CM3D2.ExHelloWorld -- A Hello World program for ReiPatcher. Injects a simple Hello, World! method into CM3D2 assembly.
*/
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ReiPatcher;
using ReiPatcher.Patch;

namespace CM3D2.ExHelloWorld.Patcher
{
    /// <summary>
    /// The patcher for the HelloWorld mod.
    /// 
    /// The patcher will make Start() method (inside SceneLogo class in CM3D2) call PrintHelloWorld() method from the hook assembly.
    /// </summary>
    public class HelloWorldPatcher : PatchBase
    {
        /// <summary>
        /// The name of the patcher.
        /// 
        /// If not overwritten, defaults to the name of the assembly (CM3D2.ExHelloWorld.Patcher in this case).
        /// </summary>
        public override string Name => "CM3D2 Hello World Patcher";

        /// <summary>
        /// Version of the patcher.
        /// 
        /// If not overwritten, default to the version of the assembly.
        /// </summary>
        public override string Version => "1.0.0.0";

        /// <summary>
        /// Hook assembly (CM3D2.ExHelloWorld.Hook)
        /// </summary>
        private AssemblyDefinition hookAssembly;

        private const string PATCHED_TAG = "CM3D2_HELLO_WORLD_PATCHED";

        /// <summary>
        /// Checks if the provided assembly should be patched with this patcher.
        /// </summary>
        /// <param name="args">Information about the assembly.</param>
        /// <returns>True, if the assembly specified in args should be patched with this patcher. Otherwise, false.</returns>
        public override bool CanPatch(PatcherArguments args)
        {
            // Check that the name of the assembly is Assembly-CSharp and make sure the assembly wasn't already patched with this patcher.
            // The latter is done by looking at the assembly's attributes and finding the patched tag.
            return args.Assembly.Name.Name == "Assembly-CSharp" && GetPatchedAttributes(args.Assembly).All(att => att.Info != PATCHED_TAG);
        }

        /// <summary>
        /// Patches the specified assembly.
        /// </summary>
        /// <param name="args">Information about the assembly to patch.</param>
        public override void Patch(PatcherArguments args)
        {
            // Step 1: Get type definitions for SceneLogo and Hooks from appropriate assemblies.
            TypeDefinition sceneLogoType = args.Assembly.MainModule.GetType("SceneLogo");
            TypeDefinition hooksType = hookAssembly.MainModule.GetType("CM3D2.ExHelloWorld.Hook.Hooks");

            // Step 2: Get method definition for the target method (Start) and hook method (PrintHelloWorld).
            MethodDefinition startMethod = sceneLogoType.Methods.FirstOrDefault(m => m.Name == "Start");
            MethodDefinition printHelloWorldMethod = hooksType.Methods.FirstOrDefault(m => m.Name == "PrintHelloWorld");

            // Step 2.1: Import PrintHelloWorld method into the module where SceneLogo type is.
            // This is needed to ensure that the target assembly (Assembly-CSharp) will know where to find PrintHelloWorld method.
            MethodReference printHelloWorldReference = startMethod.Module.Import(printHelloWorldMethod);

            // Step 3: Perform patching.
            // Here we get the first instruction from Start method and an instance of ILProcessor to simplify IL manipulation.
            Instruction startInstruction = startMethod.Body.Instructions[0];
            ILProcessor il = startMethod.Body.GetILProcessor();

            // Insert a call to PrintHelloWorld. Since we don't need to pass any parameters, we don't need to add any more instructions.
            il.InsertBefore(startInstruction, il.Create(OpCodes.Call, printHelloWorldReference));

            // Step 4: Add pacher attribute.
            // Finally you must ALWAYS add the patcher tag to mark the asembly as patched. 
            // This tag is used in CanPatch(PatcherArguments args) method above to check if the assembly requires patching.
            SetPatchedAttribute(args.Assembly, PATCHED_TAG);
        }

        /// <summary>
        /// This method is called after the patcher was loaded but before actual patching commences.
        /// 
        /// In this method you would usually request and assembly to patch and load external assemblies.
        /// </summary>
        public override void PrePatch()
        {
            // Tell ReiPatcher that Assembly-CSharp.dll needs to be patched.
            RPConfig.RequestAssembly("Assembly-CSharp.dll");

            // Load the hook assembly.
            string path = Path.Combine(AssembliesDir, "CM3D2.ExHelloWorld.Hook.dll");
            if (!File.Exists(path))
                throw new FileNotFoundException("Missing hook dll!");

            using (Stream s = File.OpenRead(path))
            {
                hookAssembly = AssemblyDefinition.ReadAssembly(s);
            }

            if (hookAssembly == null)
                throw new NullReferenceException("Failed to read hook assembly!");
        }
    }
}
