/*
    CM3D2.ExHelloWorld -- A Hello World program for ReiPatcher. Injects a simple Hello, World! method into CM3D2 assembly.
*/
using System;

namespace CM3D2.ExHelloWorld.Hook
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
