using HarmonyLib;
using System;
using System.Reflection;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class LanguageTextLoaderPatch
    {
        private static Type dataBaseLanguageType;
        private static MethodInfo getTextMethod;

        static LanguageTextLoaderPatch()
        {
            dataBaseLanguageType = Type.GetType("GameData.CoreLanguage.Collections.DataBaseLanguage, Assembly-CSharp");
            
            if (dataBaseLanguageType != null)
            {
                getTextMethod = dataBaseLanguageType.GetMethod("GetText", BindingFlags.Public | BindingFlags.Static);
                if (getTextMethod == null)
                {
                    getTextMethod = dataBaseLanguageType.GetMethod("GetLang", BindingFlags.Public | BindingFlags.Static);
                }
                if (getTextMethod == null)
                {
                    getTextMethod = dataBaseLanguageType.GetMethod("GetString", BindingFlags.Public | BindingFlags.Static);
                }
            }
        }

        static bool Prefix(string key, ref string __result)
        {
            if (!Plugin.EnableCustomLanguage.Value)
            {
                return true;
            }

            // 检查是否有自定义翻译
            string customTranslation = Plugin.GetCustomTranslation("CommonPhrasesLang", key);
            if (customTranslation != null)
            {
                __result = customTranslation;
                Plugin.Logger.LogDebug($"Replaced text for key {key}: {customTranslation}");
                return false; // 跳过原始方法
            }

            return true; // 继续原始方法
        }
    }
}
