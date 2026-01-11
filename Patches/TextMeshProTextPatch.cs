using HarmonyLib;
using System;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class TextMeshProTextPatch
    {
        private static HashSet<string> processedTexts = new HashSet<string>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TMPro.TextMeshProUGUI), "text", MethodType.Setter)]
        static void TextMeshProSetterPostfix(ref string value)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(value) || processedTexts.Contains(value))
                return;

            string original = value;
            value = ReplaceText(value);
            
            if (original != value)
            {
                Plugin.Logger.LogInfo($"[TextMeshPro] Replaced: '{original}' -> '{value}'");
                processedTexts.Add(value);
            }
        }

        private static string ReplaceText(string text)
        {
            foreach (var kvp in Plugin.customTranslations)
            {
                if (text.Contains(kvp.Key))
                {
                    text = text.Replace(kvp.Key, kvp.Value);
                }
            }

            return text;
        }
    }
}
