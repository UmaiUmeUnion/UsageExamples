/*
    ExSimpleIniConfig -- An example of how to use ExIni to handle plugins' preferenes.
*/
using System;
using ExIni;
using UnityInjector;
using UnityInjector.Attributes;

namespace ExSimpleIniConfig.Plugin
{
    /// <summary>
    /// This is a plugin.
    /// 
    /// It extends PluginBase, which means it also inherits MonoBehaviour. Thus normal Unity Scripting API methods are supported.
    /// </summary>
    [PluginName("Simple ExIni Usage Example Plugin"), PluginVersion("1.0")]
    public class ExSimpleIniConfigPlugin : PluginBase
    {
        private const string GREETING_DEFAULT = "Hello, world";
        private const int SWALLOW_AIRSPEED_VELOCITY_DEFAULT = 10;
        private const bool IS_UNLADEN_DEFAULT = true;

        private string greeting;
        private int swallowAirspeedVelocity;
        private bool isUnladen;

        /// <summary>
        /// You can use the constructor to initialize the plugin before it has been added into the game as a MonoBehaviour.
        /// 
        /// This is always run before Awake() method.
        /// 
        /// NOTE: Prefer using Awake() method to initialize/use in-game values/methods instead of this constructor.
        /// </summary>
        public ExSimpleIniConfigPlugin()
        {
            LoadConfig();
        }

        /// <summary>
        /// Loads the configuration for the plugin.
        /// </summary>
        private void LoadConfig()
        {
            bool needsSave = false;
            // Use Preferences property to load/save properties from/to ExSimpleIniConfigPlugin.ini (saved to UnityInjector\Config).
            // This line loads property Greeting from section General
            IniKey key = Preferences["General"]["Greeting"];

            // If the property was not created before, ExIni creates one automatically with value of "null".
            // If the property's value is "null" (does not exist) or if the value is empty (ExIni does not perform trimming), set the value to default.
            if (key.Value == null || (greeting = key.Value.Trim()).Length == 0)
            {
                greeting = GREETING_DEFAULT;
                key.Value = GREETING_DEFAULT;
                needsSave = true;
            }

            // Repeat the process, but for other properties.
            key = Preferences["Swallows"]["AirspeedVelocity"];

            if (key.Value == null || int.TryParse(key.Value, out swallowAirspeedVelocity))
            {
                swallowAirspeedVelocity = SWALLOW_AIRSPEED_VELOCITY_DEFAULT;
                key.Value = SWALLOW_AIRSPEED_VELOCITY_DEFAULT.ToString();
                needsSave = true;
            }

            key = Preferences["Swallows"]["Unladen"];

            if (key.Value == null || bool.TryParse(key.Value, out isUnladen))
            {
                isUnladen = IS_UNLADEN_DEFAULT;
                key.Value = IS_UNLADEN_DEFAULT.ToString();
                needsSave = true;
            }

            // Finally save the configuration file if it was edited (some value set to default).
            if(needsSave)
                SaveConfig();
        }

        /// <summary>
        /// Reminder: Awake is called after the constructor! On the other hand, Awake() is guaranteed to be called after all the game's important assets have been loaded.
        /// That is why Awake is a lot safer when the initialization code contains in-game methods.
        /// </summary>
        public void Awake()
        {
            Console.WriteLine($"{greeting}! The airspeed velocity of {(isUnladen ? "an unladen": "a")} swallow is {swallowAirspeedVelocity}.");
        }
    }
}
