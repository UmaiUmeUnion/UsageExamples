/*
    CM3D2.ExAdvHooks -- An example to show off how to use Mono.Cecil to inject methods with parameters and memeber fields.
*/
using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ReiPatcher;
using ReiPatcher.Patch;

namespace CM3D2.ExAdvHooks.Patcher
{
    /// <summary>
    /// The patcher for the ExAdvHooks mod.
    /// 
    /// The patcher will take method CrossFade (from class Maid) and SetCurExcite (from class MaidParam) and inject methods CrossFadeHook and SetCurExciteMax.
    /// 
    /// The target assembly is Assembly-CSharp.dll.
    /// </summary>
    public class ExAdvHooksPatcher : PatchBase
    {
        /// <summary>
        /// The name of the patcher.
        /// 
        /// If not overwritten, defaults to the name of the assembly (CM3D2.ExAdvHooks.Patcher in this case).
        /// </summary>
        public override string Name => "Advanced Hooks Patcher Example";

        /// <summary>
        /// Hook assembly (CM3D2.ExAdvHooks.Hook)
        /// </summary>
        private AssemblyDefinition hookAssembly;

        private const string PATCHED_TAG = "EX_ADV_HOOKS_PATCHED";

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
            // Check that the name of the assembly is Assembly-CSharp and make sure the assembly wasn't already patched with this patcher.
            // The latter is done by looking at the assembly's attributes and finding the patched tag.
            return args.Assembly.Name.Name == "Assembly-CSharp" && GetPatchedAttributes(args.Assembly).All(att => att.Info != PATCHED_TAG);
        }

        public override void Patch(PatcherArguments args)
        {
            // Step 1: Get type definitions for SceneLogo and Hooks from appropriate assemblies.
            TypeDefinition maidType = args.Assembly.MainModule.GetType("Maid");
            TypeDefinition maidParamType = args.Assembly.MainModule.GetType("MaidParam");
            TypeDefinition hooksType = hookAssembly.MainModule.GetType("CM3D2.ExHelloWorld.Hook.Hooks");

            // Step 2: Get method definition for the target method (Start) and hook method (PrintHelloWorld).
            MethodDefinition crossFadeMethod = maidType.Methods.FirstOrDefault(m => m.Name == "CrossFade");
            MethodDefinition setCurExciteMethod = maidParamType.Methods.FirstOrDefault(m => m.Name == "SetCurExcite");

            MethodDefinition crossFadeHookMethod = hooksType.Methods.FirstOrDefault(m => m.Name == "CrossFadeHook");
            MethodDefinition setCurExciteMaxMethod = hooksType.Methods.FirstOrDefault(m => m.Name == "SetCurExciteMax");

            // Step 2.1: Import hook methods into the module where target types are.
            // This is needed to ensure that the target assembly (Assembly-CSharp) will know where to find the hook methods.
            MethodReference crossFadeHookReference = crossFadeMethod.Module.Import(crossFadeHookMethod);
            MethodReference setCurExciteMaxReference = setCurExciteMethod.Module.Import(setCurExciteMaxMethod);

            // Step 3.1: Perform patching (CrossFade).
            // Here we get the first instruction from CrossFade method and an instance of ILProcessor to simplify IL manipulation.
            Instruction startInstruction = crossFadeMethod.Body.Instructions[0];
            ILProcessor il = crossFadeMethod.Body.GetILProcessor();

            // Firstly, push method's parameters onto the evaluation stack.
            // Since CrossFade is not static ldarg.0 will push the value of "this" onto the evaluation stack.
            // ldarg.1-6 will load method's other parameters.
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_1));
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_2));
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_3));
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_S, 4));
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_S, 5));
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_S, 6));
            // Finally, we insert the call to CrossFadeHook method
            il.InsertBefore(startInstruction, il.Create(OpCodes.Call, crossFadeHookReference));

            // Step 3.2: Perform patching (SetCurExcite).
            // Here we get the last instruction (ret) from SetCurExcite method and an instance of ILProcessor to simplify IL manipulation.
            startInstruction = setCurExciteMethod.Body.Instructions[setCurExciteMethod.Body.Instructions.Count - 1];
            il = setCurExciteMethod.Body.GetILProcessor();

            // Get reference to the status_ field defined in MaidParam. Since the field is defined in the same assembly as SetCurExcite, there is no need to explicitly import the field.
            FieldReference statusField = maidParamType.Fields.FirstOrDefault(f => f.Name == "status_");

            // Firstly we get the value of status_. Since it is an instance field, we first use ldarg.0 to get "this" and then ldfld to get the value of status_ for "this".
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(startInstruction, il.Create(OpCodes.Ldfld, statusField));
            // Finally we call SetCurExciteMax itself.
            il.InsertBefore(startInstruction, il.Create(OpCodes.Call, setCurExciteMaxReference));

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
            string path = Path.Combine(AssembliesDir, "CM3D2.ExAdvHooks.Hook.dll");
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
