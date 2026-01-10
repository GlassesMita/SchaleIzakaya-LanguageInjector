using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class TextReplacementPatch
    {
        private static HashSet<string> targetTexts = new HashSet<string>
        {
            "欢迎访问《东方夜雀食堂》联动终端！在这里您可以管理与其他联动作品的联动活动！",
            "已经开启了！有需要的话，也可以在这里随时关闭！祝您玩的开心！",
            "明白了，让这个联动活动暂时关闭对吗？",
            "该联动活动已经关闭！有需要的话，也可以在这里随时开启！祝您玩的开心！",
            "已经要结束了吗？我会一直在这里，需要服务时请随时来找我！",
            "祝您玩的开心！"
        };

        private static Dictionary<string, string> textMappings = new Dictionary<string, string>
        {
            { "欢迎访问《东方夜雀食堂》联动终端！在这里您可以管理与其他联动作品的联动活动！", "欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！" },
            { "已经开启了！有需要的话，也可以在这里随时关闭！祝您玩的开心！", "已启矣！若有所需，亦可于此随时闭之！祝君游之畅！" },
            { "明白了，让这个联动活动暂时关闭对吗？", "明矣，欲暂闭此联运活动乎？" },
            { "该联动活动已经关闭！有需要的话，也可以在这里随时开启！祝您玩的开心！", "此联运活动已闭！若有所需，亦可于此随时启之！祝君游之畅！" },
            { "已经要结束了吗？我会一直在这里，需要服务时请随时来找我！", "将终矣乎？吾将常驻于此，若有所需，请随时来寻吾！" },
            { "祝您玩的开心！", "祝君游之畅！" }
        };

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static void Postfix(ref string __result, string format, object[] args)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
            {
                return;
            }

            // 检查是否包含目标文本
            foreach (var targetText in targetTexts)
            {
                if (__result.Contains(targetText))
                {
                    if (textMappings.TryGetValue(targetText, out string replacement))
                    {
                        __result = __result.Replace(targetText, replacement);
                        Plugin.Logger.LogDebug($"Replaced text: {targetText} -> {replacement}");
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object) })]
        static void Postfix(ref string __result, string format, object arg0)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
            {
                return;
            }

            // 检查是否包含目标文本
            foreach (var targetText in targetTexts)
            {
                if (__result.Contains(targetText))
                {
                    if (textMappings.TryGetValue(targetText, out string replacement))
                    {
                        __result = __result.Replace(targetText, replacement);
                        Plugin.Logger.LogDebug($"Replaced text: {targetText} -> {replacement}");
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object), typeof(object) })]
        static void Postfix(ref string __result, string format, object arg0, object arg1)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
            {
                return;
            }

            // 检查是否包含目标文本
            foreach (var targetText in targetTexts)
            {
                if (__result.Contains(targetText))
                {
                    if (textMappings.TryGetValue(targetText, out string replacement))
                    {
                        __result = __result.Replace(targetText, replacement);
                        Plugin.Logger.LogDebug($"Replaced text: {targetText} -> {replacement}");
                        break;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) })]
        static void Postfix(ref string __result, string format, object arg0, object arg1, object arg2)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
            {
                return;
            }

            // 检查是否包含目标文本
            foreach (var targetText in targetTexts)
            {
                if (__result.Contains(targetText))
                {
                    if (textMappings.TryGetValue(targetText, out string replacement))
                    {
                        __result = __result.Replace(targetText, replacement);
                        Plugin.Logger.LogDebug($"Replaced text: {targetText} -> {replacement}");
                        break;
                    }
                }
            }
        }
    }
}
