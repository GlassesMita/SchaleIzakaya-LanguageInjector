using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;
using UnityEngine;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class FinalTextInterceptorPatch
    {
        // 核心文本映射表
        private static Dictionary<string, string> coreTextMappings = new Dictionary<string, string>
        {
            { "祝您玩的开心！", "祝君游之畅！" },
            { "欢迎访问《东方夜雀食堂》联动终端！在这里您可以管理与其他联动作品的联动活动！", "欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！" },
            { "已经开启了！有需要的话，也可以在这里随时关闭！祝您玩的开心！", "已启矣！若有所需，亦可于此随时闭之！祝君游之畅！" },
            { "明白了，让这个联动活动暂时关闭对吗？", "明矣，欲暂闭此联运活动乎？" },
            { "该联动活动已经关闭！有需要的话，也可以在这里随时开启！祝您玩的开心！", "此联运活动已闭！若有所需，亦可于此随时启之！祝君游之畅！" },
            { "已经要结束了吗？我会一直在这里，需要服务时请随时来找我！", "将终矣乎？吾将常驻于此，若有所需，请随时来寻吾！" }
        };

        // 文本处理函数
        private static string ProcessFinalText(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            // 首先尝试精确匹配
            if (coreTextMappings.ContainsKey(inputText))
            {
                return coreTextMappings[inputText];
            }

            // 然后尝试包含匹配
            string result = inputText;
            foreach (var mapping in coreTextMappings)
            {
                if (result.Contains(mapping.Key))
                {
                    result = result.Replace(mapping.Key, mapping.Value);
                }
            }

            return result;
        }

        // 方法1: 拦截字符串格式化 - 最可能的路径
        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static void StringFormatArrayPostfix(ref string __result, string format, object[] args)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            string original = __result;
            __result = ProcessFinalText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogInfo($"[StringFormatArray] Replaced: '{original}' -> '{__result}'");
            }
        }

        // 方法2: 拦截字符串连接
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
            __result = ProcessFinalText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogInfo($"[StringConcat] Replaced: '{original}' -> '{__result}'");
            }
        }

        // 方法3: 拦截字符串数组连接
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
            __result = ProcessFinalText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogInfo($"[StringConcatArray] Replaced: '{original}' -> '{__result}'");
            }
        }

        // 方法4: 拦截对象转字符串
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnityEngine.Object), "ToString")]
        static void ObjectToStringPostfix(ref string __result)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            // 特别检查 "祝您玩的开心！"
            if (__result == "祝您玩的开心！")
            {
                __result = "祝君游之畅！";
                Plugin.Logger.LogInfo($"[ObjectToString] Special replacement: '祝您玩的开心！' -> '祝君游之畅！'");
                return;
            }

            string original = __result;
            __result = ProcessFinalText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogInfo($"[ObjectToString] Replaced: '{original}' -> '{__result}'");
            }
        }

        // 方法5: 拦截字符串替换操作
        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Replace", new Type[] { typeof(string), typeof(string) })]
        static void StringReplacePostfix(ref string __result, string oldValue, string newValue)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            // 特别检查 "祝您玩的开心！"
            if (__result == "祝您玩的开心！")
            {
                __result = "祝君游之畅！";
                Plugin.Logger.LogInfo($"[StringReplace] Special replacement: '祝您玩的开心！' -> '祝君游之畅！'");
                return;
            }

            string original = __result;
            __result = ProcessFinalText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogInfo($"[StringReplace] Replaced: '{original}' -> '{__result}'");
            }
        }

        // 方法6: 拦截调试日志（用于监控）
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnityEngine.Debug), "Log", new Type[] { typeof(object) })]
        static void DebugLogPostfix(object message)
        {
            if (!Plugin.EnableCustomLanguage.Value || message == null)
                return;

            string messageText = message.ToString();
            
            // 检查是否包含我们的目标文本
            if (messageText.Contains("祝您玩的开心！"))
            {
                Plugin.Logger.LogInfo($"[DebugLog] Detected target text in log: {messageText}");
                
                // 尝试替换
                string processedText = ProcessFinalText(messageText);
                if (processedText != messageText)
                {
                    Plugin.Logger.LogInfo($"[DebugLog] Would replace: '{messageText}' -> '{processedText}'");
                }
            }
        }
    }
}
