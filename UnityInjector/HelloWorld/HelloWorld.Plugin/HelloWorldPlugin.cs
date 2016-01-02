/*
    HelloWorldPlugin -- A plugin to show off basic UnityInjector plugin structure and commands to interact with the Debugger console.
*/
using System;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;
using UnityInjector.ConsoleUtil;

namespace HelloWorld.Plugin
{
    /// <summary>
    /// This is a plugin.
    /// 
    /// It extends PluginBase, which means it also inherits MonoBehaviour. Thus normal Unity Scripting API methods are supported.
    /// </summary>
    [PluginName("Hello World Plugin"), PluginVersion("1.0")]
    public class HelloWorldPlugin : PluginBase
    {
        public void Awake()
        {
            // You can use Unity Scripting API to output log messages into the Debugger console.
            Debug.Log("Hello, world!");
        }

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.A))
                return;
            // If you are making a multigame plugin, use SafeConsole to change console's color.
            ConsoleColor prev = SafeConsole.ForegroundColor;
            SafeConsole.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hello, green world!");
            SafeConsole.ForegroundColor = prev;
        }

        public void OnDestroy()
        {
            // You can also use normal STDIO methods to output messages to the Debugger console
            Console.WriteLine("Goodbye, world!");
        }
    }
}
