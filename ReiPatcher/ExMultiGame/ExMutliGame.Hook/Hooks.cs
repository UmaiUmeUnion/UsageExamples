/*
    ExMutliGame -- An example of creating patchers for multiple different games. Uses ReiPatcher game configuration to get the target method to patch.
*/
using System;

namespace ExMultiGame.Hook
{
    /// <summary>
    /// Class for our hooks.
    /// </summary>
    public static class Hooks
    {
        /// <summary>
        /// A hook method that will print 'Hello, world' into active console.
        /// </summary>
        public static void PrintHelloWorld()
        {
            Console.WriteLine("Hello, world!");
        }
    }
}
