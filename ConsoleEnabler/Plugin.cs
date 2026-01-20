using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Reflection;

namespace ConsoleEnabler
{
    [BepInPlugin("com.consoleenabler", "Console Enabler", "1.0.0")]
    public class ConsoleEnablerPlugin : BasePlugin
    {
        internal static ManualLogSource Logger;
        internal static ConsoleEnablerPlugin Instance;

        public static ConfigEntry<bool> EnableDebugConsole;
        public static ConfigEntry<bool> ShowConsoleOnStartup;

        public override void Load()
        {
            Instance = this;
            Logger = base.Log;
            
            Logger.LogInfo($"Console Enabler Plugin is loading!");

            InitializeConfig();
            EnableDebugConsoleMethod();

            Harmony harmony = new Harmony("com.consoleenabler");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Console Enabler Plugin is loaded!");
        }

        private void InitializeConfig()
        {
            EnableDebugConsole = Config.Bind(
                "General",
                "EnableDebugConsole",
                true,
                "Enable EA Debug Console"
            );

            ShowConsoleOnStartup = Config.Bind(
                "General",
                "ShowConsoleOnStartup",
                false,
                "Show console on game startup"
            );
        }

        private void EnableDebugConsoleMethod()
        {
            if (!EnableDebugConsole.Value)
            {
                Logger.LogInfo("Debug console is disabled in config");
                return;
            }

            try
            {
                Type debugConsoleType = Type.GetType("PrototypingManagers.EADebugConsole, Assembly-CSharp");
                
                if (debugConsoleType == null)
                {
                    Logger.LogError("EADebugConsole type not found!");
                    return;
                }

                FieldInfo showConsoleTextField = debugConsoleType.GetField("showConsoleText", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo hideConsoleTextField = debugConsoleType.GetField("hideConsoleText", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo shouldOnGUIBuffConsoleShownField = debugConsoleType.GetField("shouldOnGUIBuffConsoleShown", BindingFlags.NonPublic | BindingFlags.Instance);

                if (showConsoleTextField == null || hideConsoleTextField == null || shouldOnGUIBuffConsoleShownField == null)
                {
                    Logger.LogError("Required fields not found in EADebugConsole!");
                    return;
                }

                FieldInfo newGameModeField = debugConsoleType.GetField("newGameMode", BindingFlags.NonPublic | BindingFlags.Instance);

                object debugConsoleInstance = GetEADebugConsoleInstance();
                
                if (debugConsoleInstance == null)
                {
                    Logger.LogWarning("EADebugConsole instance not found, console will be enabled on next game start");
                    return;
                }

                showConsoleTextField.SetValue(debugConsoleInstance, "1");
                hideConsoleTextField.SetValue(debugConsoleInstance, "0");
                shouldOnGUIBuffConsoleShownField.SetValue(debugConsoleInstance, true);
                
                if (newGameModeField != null)
                {
                    newGameModeField.SetValue(debugConsoleInstance, false);
                }

                Logger.LogInfo("Debug console enabled successfully!");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to enable debug console: {ex.Message}");
            }
        }

        private object GetEADebugConsoleInstance()
        {
            try
            {
                Type debugConsoleType = Type.GetType("PrototypingManagers.EADebugConsole, Assembly-CSharp");
                
                if (debugConsoleType == null)
                {
                    Logger.LogError("EADebugConsole type not found!");
                    return null;
                }

                FieldInfo instanceField = debugConsoleType.GetField("instance", BindingFlags.NonPublic | BindingFlags.Static);
                
                if (instanceField != null)
                {
                    return instanceField.GetValue(null);
                }

                Logger.LogWarning("instance field not found in EADebugConsole!");
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to get EADebugConsole instance: {ex.Message}");
                return null;
            }
        }
    }
}
