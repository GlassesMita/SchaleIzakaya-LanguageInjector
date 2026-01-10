using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;
using UnityEngine;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class UniversalTextInterceptorPatch
    {
        // 完整的文本映射表
        private static Dictionary<string, string> universalTextMappings = new Dictionary<string, string>
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
            { "迷途竹林深处的永远亭。", "迷途竹林深处永远亭。" },
            
            // 其他常用文本
            { "需要", "所需" },
            { "开心", "畅" },
            { "管理", "掌管" },
            { "访问", "造访" },
            { "活动", "举" },
            { "关闭", "闭" },
            { "开启", "启" },
            { "结束", "终" },
            { "服务", "服" },
            { "随时", "随时" },
            { "需要", "须" }
        };

        // 缓存已处理的文本以避免重复处理
        private static HashSet<string> processedTexts = new HashSet<string>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static void Postfix1(ref string __result, string format, object[] args)
        {
            ReplaceTextUniversal(ref __result, "string.Format(object[])");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object) })]
        static void Postfix2(ref string __result, string format, object arg0)
        {
            ReplaceTextUniversal(ref __result, "string.Format(object)");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object), typeof(object) })]
        static void Postfix3(ref string __result, string format, object arg0, object arg1)
        {
            ReplaceTextUniversal(ref __result, "string.Format(object,object)");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) })]
        static void Postfix4(ref string __result, string format, object arg0, object arg1, object arg2)
        {
            ReplaceTextUniversal(ref __result, "string.Format(object,object,object)");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnityEngine.Debug), "Log", new Type[] { typeof(object) })]
        static bool Prefix1(ref object message)
        {
            if (!Plugin.EnableCustomLanguage.Value || message == null)
            {
                return true;
            }

            string messageText = message.ToString();
            
            // 检查是否是对话框相关的日志
            if (messageText.Contains("Printing Dialog:") || messageText.Contains("DialPann:"))
            {
                string originalMessage = messageText;
                
                // 尝试替换所有可能的文本
                foreach (var mapping in universalTextMappings)
                {
                    if (messageText.Contains(mapping.Key))
                    {
                        messageText = messageText.Replace(mapping.Key, mapping.Value);
                        Plugin.Logger.LogInfo($"[Log Interceptor] Replaced in debug log: {mapping.Key} -> {mapping.Value}");
                    }
                }
                
                if (originalMessage != messageText)
                {
                    message = messageText;
                }
            }
            
            return true;
        }

        private static void ReplaceTextUniversal(ref string text, string methodName)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(text) || processedTexts.Contains(text))
            {
                return;
            }

            string originalText = text;
            
            // 尝试精确匹配
            if (universalTextMappings.ContainsKey(text))
            {
                text = universalTextMappings[text];
                Plugin.Logger.LogDebug($"[{methodName}] Exact match: {originalText} -> {text}");
                processedTexts.Add(text);
                return;
            }
            
            // 尝试包含匹配
            foreach (var mapping in universalTextMappings)
            {
                if (text.Contains(mapping.Key))
                {
                    text = text.Replace(mapping.Key, mapping.Value);
                    Plugin.Logger.LogDebug($"[{methodName}] Partial match: {mapping.Key} -> {mapping.Value}");
                    processedTexts.Add(text);
                    break;
                }
            }
            
            // 如果没有替换，添加到已处理列表避免重复检查
            if (originalText == text)
            {
                processedTexts.Add(text);
            }
        }

        // 定期清理处理过的文本缓存以避免内存泄漏
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnityEngine.Application), "Quit")]
        static void Cleanup()
        {
            processedTexts.Clear();
        }
    }
}
