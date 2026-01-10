using HarmonyLib;
using System;
using System.Reflection;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch(typeof(GameData.CoreLanguage.Collections.DataBaseLanguage), "GetIngredientLang")]
    public class DataBaseLanguageGetIngredientLangPatch
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

            string customTranslation = Plugin.GetCustomTranslation("IngredientsLang", id.ToString());
            if (customTranslation != null && __result != null)
            {
                Type resultType = __result.GetType();
                PropertyInfo nameProperty = resultType.GetProperty("Name");
                if (nameProperty != null)
                {
                    nameProperty.SetValue(__result, customTranslation, null);
                }
                Plugin.Logger.LogDebug($"Replaced ingredient name for ID {id}: {customTranslation}");
            }
        }
    }
}
