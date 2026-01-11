using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector
{
    [BepInPlugin("com.schale.languageinjector", "SchaleIzakaya Language Injector", "1.0.0")]
    public class Plugin : BasePlugin
    {
        internal static ManualLogSource Logger;
        internal static Plugin Instance;

        public static ConfigEntry<bool> EnableCustomLanguage;
        public static ConfigEntry<string> CustomLanguageCode;
        public static ConfigEntry<string> TranslationFilePath;

        private static Dictionary<string, string> customTranslations = new Dictionary<string, string>();
        private static bool isLoaded = false;

        public override void Load()
        {
            Instance = this;
            Logger = base.Log;
            
            Logger.LogInfo($"Plugin SchaleIzakaya Language Injector is loading!");

            InitializeConfig();
            LoadCustomTranslations();

            Harmony harmony = new Harmony("com.schale.languageinjector");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin SchaleIzakaya Language Injector is loaded!");
        }

        private void InitializeConfig()
        {
            EnableCustomLanguage = Config.Bind(
                "General",
                "EnableCustomLanguage",
                true,
                "Enable custom language injection"
            );

            CustomLanguageCode = Config.Bind(
                "General",
                "CustomLanguageCode",
                "zh-CN",
                "Custom language code (e.g., zh-CN, zh-TW)"
            );

            TranslationFilePath = Config.Bind(
                "General",
                "TranslationFilePath",
                "./BepInEx/plugins/SchaleIzakaya.LanguageInjector/zh_CN",
                "Path to custom translation folder"
            );
        }

        private void LoadCustomTranslations()
        {
            if (!EnableCustomLanguage.Value)
            {
                Logger.LogInfo("Custom language is disabled in config");
                return;
            }

            string path = TranslationFilePath.Value;
            if (!Directory.Exists(path))
            {
                Logger.LogWarning($"Translation folder not found at: {path}");
                return;
            }

            try
            {
                customTranslations.Clear();
                string[] txtFiles = Directory.GetFiles(path, "*.txt");
                
                foreach (string txtFile in txtFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(txtFile);
                    string[] lines = File.ReadAllLines(txtFile, System.Text.Encoding.UTF8);
                    
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        string[] parts = line.Split(new[] { '\t' }, 2);
                        if (parts.Length >= 2)
                        {
                            string key = fileName + "_" + parts[0];
                            string value = parts[1];
                            customTranslations[key] = value;
                        }
                    }
                }

                Logger.LogInfo($"Loaded {customTranslations.Count} custom translations from {txtFiles.Length} files");
                isLoaded = true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load translations: {ex.Message}");
            }
        }

        public static string GetCustomTranslation(string fileName, string id)
        {
            if (!isLoaded || !EnableCustomLanguage.Value)
            {
                return null;
            }

            string key = fileName + "_" + id;
            if (customTranslations.TryGetValue(key, out string translation))
            {
                return translation;
            }

            return null;
        }

        public static string ReplaceText(string text)
        {
            if (string.IsNullOrEmpty(text) || !EnableCustomLanguage.Value)
            {
                return text;
            }

            string original = text;
            
            foreach (var kvp in customTranslations)
            {
                if (text.Contains(kvp.Key))
                {
                    text = text.Replace(kvp.Key, kvp.Value);
                }
            }

            if (original != text)
            {
                Logger.LogInfo($"Replaced: '{original}' -> '{text}'");
            }

            return text;
        }
    }
}
