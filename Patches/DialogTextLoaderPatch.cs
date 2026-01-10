using HarmonyLib;
using System;
using System.Reflection;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class DialogTextLoaderPatch
    {
        private static Type dialogManagerType;
        private static MethodInfo getDialogTextMethod;

        static DialogTextLoaderPatch()
        {
            // 尝试找到对话管理器类
            dialogManagerType = Type.GetType("GameData.Dialog.DialogManager, Assembly-CSharp");
            if (dialogManagerType == null)
            {
                dialogManagerType = Type.GetType("GameData.UI.DialogManager, Assembly-CSharp");
            }
            if (dialogManagerType == null)
            {
                dialogManagerType = Type.GetType("GameData.CoreLanguage.DialogManager, Assembly-CSharp");
            }

            if (dialogManagerType != null)
            {
                // 尝试找到获取对话文本的方法
                getDialogTextMethod = dialogManagerType.GetMethod("GetDialogText", BindingFlags.Public | BindingFlags.Static);
                if (getDialogTextMethod == null)
                {
                    getDialogTextMethod = dialogManagerType.GetMethod("GetText", BindingFlags.Public | BindingFlags.Static);
                }
                if (getDialogTextMethod == null)
                {
                    getDialogTextMethod = dialogManagerType.GetMethod("GetString", BindingFlags.Public | BindingFlags.Static);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static bool Prefix(ref string __result, string format, object[] args)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(format))
            {
                return true;
            }

            // 检查是否包含联动终端相关的文本
            if (format.Contains("欢迎访问") && format.Contains("联动终端"))
            {
                string customTranslation = Plugin.GetCustomTranslation("CollabModuleLang", "0");
                if (customTranslation != null)
                {
                    __result = customTranslation;
                    Plugin.Logger.LogDebug($"Replaced dialog text: {customTranslation}");
                    return false;
                }
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object) })]
        static void Postfix(ref string __result, string format, object arg0)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
            {
                return;
            }

            // 检查输出结果是否包含联动终端文本
            if (__result.Contains("欢迎访问") && __result.Contains("联动终端"))
            {
                string customTranslation = Plugin.GetCustomTranslation("CollabModuleLang", "0");
                if (customTranslation != null)
                {
                    __result = customTranslation;
                    Plugin.Logger.LogDebug($"Replaced dialog text: {customTranslation}");
                }
            }
        }
    }
}
