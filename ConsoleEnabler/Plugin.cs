using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SchaleIzakaya.ConsoleEnabler
{
    [BepInPlugin("com.schale.consoleenabler", "SchaleIzakaya Console Enabler", "1.0.0")]
    public class Plugin : BasePlugin
    {
        internal static ManualLogSource Logger;
        internal static Plugin Instance;

        public static ConfigEntry<bool> EnableDebugConsole;
        public static ConfigEntry<bool> ShowConsoleOnStartup;

        private const uint JZ_OPCODE = 0x74;
        private const uint JNZ_OPCODE = 0x75;
        private const long PATCH_OFFSET = 0x42AF0F;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        public override void Load()
        {
            Instance = this;
            Logger = base.Log;
            
            Logger.LogInfo($"SchaleIzakaya Console Enabler Plugin is loading!");

            InitializeConfig();

            ApplyConsoleEnablePatch();

            Harmony harmony = new Harmony("com.schale.consoleenabler");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"SchaleIzakaya Console Enabler Plugin is loaded!");
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

        private void ApplyConsoleEnablePatch()
        {
            try
            {
                Logger.LogInfo("Applying console enable patch...");

                IntPtr gameAssemblyBase = GetGameAssemblyBaseAddress();
                if (gameAssemblyBase == IntPtr.Zero)
                {
                    Logger.LogWarning("Could not find GameAssembly.dll base address");
                    TryAlternativePatch();
                    return;
                }

                Logger.LogInfo($"GameAssembly.dll base: 0x{gameAssemblyBase.ToInt64():X16}");

                IntPtr patchAddress = IntPtr.Add(gameAssemblyBase, (int)PATCH_OFFSET);
                Logger.LogInfo($"Patch target address: 0x{patchAddress.ToInt64():X16}");

                if (PatchByte(patchAddress))
                {
                    Logger.LogInfo("Successfully patched console enable condition!");
                }
                else
                {
                    Logger.LogWarning("Failed to patch, trying alternative method...");
                    TryAlternativePatch();
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Patch failed: {ex.Message}");
                TryAlternativePatch();
            }
        }

        private IntPtr GetGameAssemblyBaseAddress()
        {
            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName != null && assembly.FullName.StartsWith("GameAssembly"))
                    {
                        Logger.LogInfo($"Found assembly: {assembly.FullName}");
                        try
                        {
                            var field = assembly.GetType("System.Reflection.AssemblyName")?
                                .GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (field != null)
                            {
                                Logger.LogInfo("Assembly name field found");
                            }
                        }
                        catch { }

                        return IntPtr.Zero;
                    }
                }

                IntPtr[] modules = new IntPtr[100];
                int count = 0;
                foreach (ProcessModule mod in System.Diagnostics.Process.GetCurrentProcess().Modules)
                {
                    if (count >= 100) break;
                    if (mod.ModuleName.Contains("GameAssembly") || mod.FileName.Contains("GameAssembly"))
                    {
                        Logger.LogInfo($"Found GameAssembly: {mod.FileName}");
                        return mod.BaseAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Error getting GameAssembly base: {ex.Message}");
            }
            return IntPtr.Zero;
        }

        private bool PatchByte(IntPtr address)
        {
            try
            {
                byte currentByte = Marshal.ReadByte(address);
                Logger.LogInfo($"Current byte at 0x{address.ToInt64():X16}: 0x{currentByte:X2}");

                if (currentByte == JNZ_OPCODE)
                {
                    Logger.LogInfo("Already patched (JNZ found)");
                    return true;
                }

                if (currentByte != JZ_OPCODE)
                {
                    Logger.LogWarning($"Unexpected byte 0x{currentByte:X2}, expected JZ (0x{JZ_OPCODE:X2}) or JNZ (0x{JNZ_OPCODE:X2})");
                    return false;
                }

                uint oldProtect;
                if (!VirtualProtect(address, (IntPtr)1, 0x40, out oldProtect))
                {
                    Logger.LogWarning("Failed to change memory protection");
                    return false;
                }

                Marshal.WriteByte(address, (byte)JNZ_OPCODE);

                VirtualProtect(address, (IntPtr)1, oldProtect, out _);

                byte verifyByte = Marshal.ReadByte(address);
                if (verifyByte == JNZ_OPCODE)
                {
                    Logger.LogInfo($"Successfully patched 0x{currentByte:X2} -> 0x{verifyByte:X2}");
                    return true;
                }

                Logger.LogWarning($"Verification failed: read 0x{verifyByte:X2}");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Patch error: {ex.Message}");
                return false;
            }
        }

        private void TryAlternativePatch()
        {
            Logger.LogInfo("Attempting alternative patch: creating GlobalDebugConsole after SceneManager.Awake...");
        }

        private static void ForceConsoleCreation()
        {
            if (!Plugin.EnableDebugConsole.Value)
                return;

            try
            {
                Plugin.Logger.LogInfo("Attempting to force GlobalDebugConsole creation...");

                Type globalDebugConsoleType = AccessTools.TypeByName("PrototypingManagers.GlobalDebugConsole");
                if (globalDebugConsoleType == null)
                {
                    Plugin.Logger.LogWarning("GlobalDebugConsole type not found");
                    return;
                }

                Plugin.Logger.LogInfo($"Found GlobalDebugConsole type: {globalDebugConsoleType.FullName}");

                MonoBehaviour[] existing = GameObject.FindObjectsOfType<MonoBehaviour>();
                foreach (var obj in existing)
                {
                    if (obj.GetType() == globalDebugConsoleType)
                    {
                        FieldInfo shownField = globalDebugConsoleType.GetField("shouldOnGUIBuffConsoleShown",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        if (shownField != null)
                        {
                            shownField.SetValue(obj, true);
                            Plugin.Logger.LogInfo("Enabled existing GlobalDebugConsole");
                        }
                        return;
                    }
                }

                Plugin.Logger.LogInfo("Creating new GlobalDebugConsole...");
                GameObject consoleGO = new GameObject("GlobalDebugConsole");
                UnityEngine.Object.DontDestroyOnLoad(consoleGO);

                MethodInfo addComponent = typeof(GameObject).GetMethod("AddComponent", new Type[] { });
                if (addComponent != null)
                {
                    object console = addComponent.Invoke(consoleGO, null);
                    if (console != null)
                    {
                        Plugin.Logger.LogInfo("GlobalDebugConsole created!");

                        FieldInfo shownField = globalDebugConsoleType.GetField("shouldOnGUIBuffConsoleShown",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        if (shownField != null)
                        {
                            shownField.SetValue(console, true);
                            Plugin.Logger.LogInfo("Set shouldOnGUIBuffConsoleShown = true");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"Failed: {ex.Message}");
            }
        }

        [HarmonyPatch(typeof(PrototypingManagers.GlobalDebugConsole), "OnGUI")]
        public class GlobalDebugConsoleOnGUIPatch
        {
            static bool Prefix(PrototypingManagers.GlobalDebugConsole __instance)
            {
                if (Plugin.EnableDebugConsole.Value)
                {
                    try
                    {
                        FieldInfo field = typeof(PrototypingManagers.GlobalDebugConsole).GetField("shouldOnGUIBuffConsoleShown", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            field.SetValue(__instance, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"OnGUI patch error: {ex.Message}");
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PrototypingManagers.GlobalDebugConsole), "Update")]
        public class GlobalDebugConsoleUpdatePatch
        {
            static void Postfix(PrototypingManagers.GlobalDebugConsole __instance)
            {
                if (Plugin.EnableDebugConsole.Value && Plugin.ShowConsoleOnStartup.Value)
                {
                    try
                    {
                        FieldInfo field = typeof(PrototypingManagers.GlobalDebugConsole).GetField("shouldOnGUIBuffConsoleShown", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            field.SetValue(__instance, true);
                        }
                    }
                    catch { }
                }
            }
        }

        [HarmonyPatch(typeof(PrototypingManagers.DebugConsoleBase), "Start")]
        public class DebugConsoleBaseStartPatch
        {
            static bool Prefix(PrototypingManagers.DebugConsoleBase __instance)
            {
                if (Plugin.EnableDebugConsole.Value)
                {
                    try
                    {
                        FieldInfo field = __instance.GetType().GetField("shouldOnGUIBuffConsoleShown", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            field.SetValue(__instance, true);
                            Plugin.Logger.LogInfo("Enabled console in Start");
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"Start patch error: {ex.Message}");
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(SplashScene.SceneManager), "Awake")]
        public class SceneManagerAwakePatch
        {
            static void Postfix()
            {
                Plugin.Logger.LogInfo("SplashScene.SceneManager.Awake completed");
                ForceConsoleCreation();
            }
        }
    }
}
