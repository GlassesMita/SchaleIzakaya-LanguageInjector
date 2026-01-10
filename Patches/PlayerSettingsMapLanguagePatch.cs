using HarmonyLib;
using System;
using System.Reflection;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch(typeof(GameData.Utils.PlayerSettings), "MapLanguage")]
    public class PlayerSettingsMapLanguagePatch
    {
        static void Postfix(ref object __result, string cultureName)
        {
            if (!Plugin.EnableCustomLanguage.Value)
            {
                return;
            }

            string customCode = Plugin.CustomLanguageCode.Value;
            if (cultureName?.StartsWith(customCode, StringComparison.OrdinalIgnoreCase) == true)
            {
                Plugin.Logger.LogInfo($"Mapping custom language code: {cultureName} -> 5");
                __result = 5;
            }
        }
    }
}
