using HarmonyLib;
using System;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class SimpleTextReplacementPatch
    {
        private static readonly Dictionary<string, string> textMappings = new Dictionary<string, string>
        {
            { "祝您玩的开心！", "祝君游之畅！" },
            { "欢迎访问《东方夜雀食堂》联动终端！在这里您可以管理与其他联动作品的联动活动！", "欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！" },
            { "已经开启了！有需要的话，也可以在这里随时关闭！祝您玩的开心！", "已启矣！若有所需，亦可于此随时闭之！祝君游之畅！" },
            { "明白了，让这个联动活动暂时关闭对吗？", "明矣，欲暂闭此联运活动乎？" },
            { "该联动活动已经关闭！有需要的话，也可以在这里随时开启！祝您玩的开心！", "此联运活动已闭！若有所需，亦可于此随时启之！祝君游之畅！" },
            { "已经要结束了吗？我会一直在这里，需要服务时请随时来找我！", "将终矣乎？吾将常驻于此，若有所需，请随时来寻吾！" }
        };

        private static readonly HashSet<string> processedTexts = new HashSet<string>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static void StringFormatPostfix1(ref string __result, string format, object[] args)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result) || processedTexts.Contains(__result))
                return;

            string original = __result;
            __result = ReplaceText(__result);
            
            if (original != __result)
            {
                processedTexts.Add(__result);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object) })]
        static void StringFormatPostfix2(ref string __result, string format, object arg0)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result) || processedTexts.Contains(__result))
                return;

            string original = __result;
            __result = ReplaceText(__result);
            
            if (original != __result)
            {
                processedTexts.Add(__result);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object), typeof(object) })]
        static void StringFormatPostfix3(ref string __result, string format, object arg0, object arg1)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result) || processedTexts.Contains(__result))
                return;

            string original = __result;
            __result = ReplaceText(__result);
            
            if (original != __result)
            {
                processedTexts.Add(__result);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Concat", new Type[] { typeof(string), typeof(string) })]
        static void StringConcatPostfix(ref string __result, string str0, string str1)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result) || processedTexts.Contains(__result))
                return;

            string original = __result;
            __result = ReplaceText(__result);
            
            if (original != __result)
            {
                processedTexts.Add(__result);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Concat", new Type[] { typeof(string[]) })]
        static void StringConcatArrayPostfix(ref string __result, string[] values)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result) || processedTexts.Contains(__result))
                return;

            string original = __result;
            __result = ReplaceText(__result);
            
            if (original != __result)
            {
                processedTexts.Add(__result);
            }
        }

        private static string ReplaceText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            foreach (var kvp in textMappings)
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
