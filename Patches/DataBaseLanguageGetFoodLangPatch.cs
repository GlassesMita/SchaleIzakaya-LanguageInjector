using HarmonyLib;
using System;
using System.Reflection;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch(typeof(GameData.CoreLanguage.Collections.DataBaseLanguage), "GetFoodLang")]
    public class DataBaseLanguageGetFoodLangPatch
    {
        static void Postfix(int id, ref object __result)
        {
            if (!Plugin.EnableCustomLanguage.Value)
            {
                return;
            }

            Type multiLanguageTextMeshCoreType = Type.GetType("GameData.MultiLanguageTextMesh.MultiLanguageTextMeshCore, Assembly-CSharp-firstpass");
            PropertyInfo currentLanguageTypeProperty = multiLanguageTextMeshCoreType?.GetProperty("CurrentLanguageType", BindingFlags.Public | BindingFlags.Static);
            
            object currentLanguage = currentLanguageTypeProperty?.GetValue(null);
            if (currentLanguage == null || (int)currentLanguage != 5)
            {
                return;
            }

            string customTranslation = Plugin.GetCustomTranslation("FoodsLang", id.ToString());
            if (customTranslation != null && __result != null)
            {
                Type resultType = __result.GetType();
                PropertyInfo nameProperty = resultType.GetProperty("Name");
                if (nameProperty != null)
                {
                    nameProperty.SetValue(__result, customTranslation, null);
                }
                Plugin.Logger.LogDebug($"Replaced food name for ID {id}: {customTranslation}");
            }
        }
    }
}
