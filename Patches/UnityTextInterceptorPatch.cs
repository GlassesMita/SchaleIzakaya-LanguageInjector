using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;
using UnityEngine;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class UnityTextInterceptorPatch
    {
        // 完整的文本映射表
        private static Dictionary<string, string> textMappings = new Dictionary<string, string>
        {
            // 联动模块文本
            { "祝您玩的开心！", "祝君游之畅！" },
            { "已经开启了！有需要的话，也可以在这里随时关闭！祝您玩的开心！", "已启矣！若有所需，亦可于此随时闭之！祝君游之畅！" },
            { "明白了，让这个联动活动暂时关闭对吗？", "明矣，欲暂闭此联运活动乎？" },
            { "该联动活动已经关闭！有需要的话，也可以在这里随时开启！祝您玩的开心！", "此联运活动已闭！若有所需，亦可于此随时启之！祝君游之畅！" },
            { "已经要结束了吗？我会一直在这里，需要服务时请随时来找我！", "将终矣乎？吾将常驻于此，若有所需，请随时来寻吾！" },
            { "欢迎访问《东方夜雀食堂》联动终端！在这里您可以管理与其他联动作品的联动活动！", "欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！" },
            
            // 地点描述文本
            { "坐落在兽道的小树屋。原本只是个大一些的鸟巢，经过米斯蒂娅长年累月的拾掇，渐渐才有了家的模样。", "坐落兽道之小树屋。初本唯大鸟巢，经米斯蒂娅长年拾掇，渐成家貌。" },
            { "人间之里的中心街道。", "人间之里中心街道。" },
            { "博丽神社的境内。", "博丽神社境内。" },
            { "红魔馆的走廊。", "红魔馆走廊。" },
            { "迷途竹林深处的永远亭。", "迷途竹林深处永远亭。" }
        };

        // 创建一个新的方法来处理文本替换，避免递归
        private static string ProcessText(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            // 首先尝试精确匹配
            if (textMappings.ContainsKey(inputText))
            {
                return textMappings[inputText];
            }

            // 然后尝试包含匹配
            string result = inputText;
            foreach (var mapping in textMappings)
            {
                if (result.Contains(mapping.Key))
                {
                    result = result.Replace(mapping.Key, mapping.Value);
                }
            }

            return result;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static void StringFormatPostfix1(ref string __result, string format, object[] args)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            string original = __result;
            __result = ProcessText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogDebug($"[StringFormat] Replaced: {original} -> {__result}");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object) })]
        static void StringFormatPostfix2(ref string __result, string format, object arg0)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            string original = __result;
            __result = ProcessText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogDebug($"[StringFormat] Replaced: {original} -> {__result}");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnityEngine.Debug), "Log", new Type[] { typeof(object) })]
        static bool DebugLogPrefix(ref object message)
        {
            if (!Plugin.EnableCustomLanguage.Value || message == null)
                return true;

            string messageText = message.ToString();
            
            // 检查是否是对话框相关的日志
            if (messageText.Contains("Printing Dialog:") || messageText.Contains("DialPann:"))
            {
                string originalMessage = messageText;
                string processedMessage = ProcessText(messageText);
                
                if (originalMessage != processedMessage)
                {
                    message = processedMessage;
                    Plugin.Logger.LogInfo($"[DebugLog] Replaced dialog text: {originalMessage} -> {processedMessage}");
                }
            }
            
            return true;
        }

        // 添加一个特殊的补丁来拦截 "祝您玩的开心！"
        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Concat", new Type[] { typeof(string), typeof(string) })]
        static void StringConcatPostfix(ref string __result, string str0, string str1)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            // 特别检查 "祝您玩的开心！"
            if (__result == "祝您玩的开心！")
            {
                __result = "祝君游之畅！";
                Plugin.Logger.LogInfo($"[StringConcat] Special replacement: '祝您玩的开心！' -> '祝君游之畅！'");
                return;
            }

            string original = __result;
            __result = ProcessText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogDebug($"[StringConcat] Replaced: {original} -> {__result}");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Concat", new Type[] { typeof(string[]) })]
        static void StringConcatArrayPostfix(ref string __result, string[] values)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            // 特别检查 "祝您玩的开心！"
            if (__result == "祝您玩的开心！")
            {
                __result = "祝君游之畅！";
                Plugin.Logger.LogInfo($"[StringConcatArray] Special replacement: '祝您玩的开心！' -> '祝君游之畅！'");
                return;
            }

            string original = __result;
            __result = ProcessText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogDebug($"[StringConcatArray] Replaced: {original} -> {__result}");
            }
        }
    }
}
