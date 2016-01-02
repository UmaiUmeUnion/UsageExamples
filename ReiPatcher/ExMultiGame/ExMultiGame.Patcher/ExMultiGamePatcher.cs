/*
    ExMutliGame -- An example of creating patchers for multiple different games. Uses ReiPatcher game configuration to get the target method to patch.
*/
using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ReiPatcher;
using ReiPatcher.Patch;

namespace ExMultiGame.Patcher
{
    /// <summary>
    /// The patcher for the ExMultiGame mod.
    /// 
    /// The patcher will make some method in some assembly call PrintHelloWorld() method from the hook assembly.
    /// 
    /// The target method and the target assembly are read from ReiPatcher game configuration (the configuration file that is passed by "-c" flag to ReiPatcher.exe).
    /// </summary>
    public class ExMultiGamePatcher : PatchBase
    {
        /// <summary>
        /// The name of the patcher.
        /// 
        /// If not overwritten, defaults to the name of the assembly (ExMultiGame.Patcher in this case).
        /// </summary>
        public override string Name => "Multiple Game Patcher Example";

        /// <summary>
        /// Hook assembly (ExMultiGame.Hook)
        /// </summary>
        private AssemblyDefinition hookAssembly;

        private string assembly, targetType, targetMethod;

        private const string PATCHED_TAG = "EX_MULTI_GAME_PATCHED";

        /// <summary>
        /// Version of the patcher.
        /// 
        /// If not overwritten, default to the version of the assembly.
        /// </summary>
        public override string Version => "1.0.0.0";

        /// <summary>
        /// Checks if the provided assembly should be patched with this patcher.
        /// </summary>
        /// <param name="args">Information about the assembly.</param>
        /// <returns>True, if the assembly specified in args should be patched with this patcher. Otherwise, false.</returns>
        public override bool CanPatch(PatcherArguments args)
        {
            // Check that the name of the assembly is the same as specified in the configuration and make sure the assembly wasn't already patched with this patcher.
            // The latter is done by looking at the assembly's attributes and finding the patched tag.
            return args.Assembly.Name.Name == assembly && GetPatchedAttributes(args.Assembly).All(att => att.Info != PATCHED_TAG);
        }

        /// <summary>
        /// Patches the specified assembly.
        /// </summary>
        /// <param name="args">Information about the assembly to patch.</param>
        public override void Patch(PatcherArguments args)
        {
            // Step 1: Get type definitions for the target type (specified in the configs) and Hooks from appropriate assemblies.
            TypeDefinition targetTypeDefinition = args.Assembly.MainModule.GetType(targetType);
            TypeDefinition hooksType = hookAssembly.MainModule.GetType("ExMultiGame.Hook.Hooks");

            // Step 2: Get method definition for the target method (specified in the configs) and hook method (PrintHelloWorld).
            MethodDefinition targetMethodDefinition = targetTypeDefinition.Methods.FirstOrDefault(m => m.Name == targetMethod);
            MethodDefinition printHelloWorldMethod = hooksType.Methods.FirstOrDefault(m => m.Name == "PrintHelloWorld");

            // Step 2.1: Import PrintHelloWorld method into the module where the target type is.
            // This is needed to ensure that the target assembly will know where to find PrintHelloWorld method.
            MethodReference printHelloWorldReference = targetMethodDefinition.Module.Import(printHelloWorldMethod);

            // Step 3: Perform patching.
            // Here we get the first instruction from the target method and an instance of ILProcessor to simplify IL manipulation.
            Instruction startInstruction = targetMethodDefinition.Body.Instructions[0];
            ILProcessor il = targetMethodDefinition.Body.GetILProcessor();

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
            // You can read and create properties in ReiPatcher patcher configuration by using methods in RPConfig class.
            assembly = RPConfig.GetConfig("ExMultiGame", "Assembly");
            targetType = RPConfig.GetConfig("ExMultiGame", "Type");
            targetMethod = RPConfig.GetConfig("ExMultiGame", "Method");

            bool isInvalidValue = IsNullOrEmpty(assembly) || IsNullOrEmpty(targetType) || IsNullOrEmpty(targetMethod);

            if (isInvalidValue)
            {
                // Calling RPConfig.GetConfig(string sec, string key) will create the property if it did not exist before.
                // Calling RPConfig.Save() will save the configuration file and thus save the created ExRPConfig section (if it did not exist before).
                RPConfig.Save();

                // Throwing an exception will stop ReiPatcher and prevent any assemblies to be patched further.
                // ALWAYS consider whether to throw an exception and stop ReiPatcher or to use CanPatch(PatcherArguments args) to make ReiPatcher only skip the faulty patcher.
                // The rule of thumb is: if other mods/patchers stronlgy rely on your mod/patch, it is better to throw an exception.
                // Moreover, end-users are more likely to notice an exception being thrown instead of just printing the error and skipping the patcher.
                throw new Exception($"Some property/properties don't have a value (or have an invalid one) in {Path.GetFileName(RPConfig.ConfigFilePath)} in ExRPConfig section! Edit the configuration file and fill in the values before using this patcher.");
            }

            // Tell ReiPatcher that Assembly-CSharp.dll needs to be patched.
            RPConfig.RequestAssembly($"{assembly}.dll");

            // Load the hook assembly.
            string path = Path.Combine(AssembliesDir, "ExMultiGame.Hook.dll");
            if (!File.Exists(path))
                throw new FileNotFoundException("Missing hook dll!");

            using (Stream s = File.OpenRead(path))
            {
                hookAssembly = AssemblyDefinition.ReadAssembly(s);
            }

            if (hookAssembly == null)
                throw new NullReferenceException("Failed to read hook assembly!");
        }

        private static bool IsNullOrEmpty(string s)
        {
            return s == null || s.Trim().Length == 0;
        }
    }
}
