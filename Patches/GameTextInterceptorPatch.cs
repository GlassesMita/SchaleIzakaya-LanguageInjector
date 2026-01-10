using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;
using UnityEngine;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    /// <summary>
    /// 基于游戏真实架构的文本替换系统
    /// 针对 GameData.MultiLanguageTextMesh 和 DialogPackage 系统进行补丁
    /// </summary>
    [HarmonyPatch]
    public class GameTextInterceptorPatch
    {
        // 核心文本映射表 - 基于实际游戏内容
        private static Dictionary<string, string> gameTextMappings = new Dictionary<string, string>
        {
            { "祝您玩的开心！", "祝君游之畅！" },
            { "欢迎访问《东方夜雀食堂》联动终端！在这里您可以管理与其他联动作品的联动活动！", "欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！" },
            { "已经开启了！有需要的话，也可以在这里随时关闭！祝您玩的开心！", "已启矣！若有所需，亦可于此随时闭之！祝君游之畅！" },
            { "明白了，让这个联动活动暂时关闭对吗？", "明矣，欲暂闭此联运活动乎？" },
            { "该联动活动已经关闭！有需要的话，也可以在这里随时开启！祝您玩的开心！", "此联运活动已闭！若有所需，亦可于此随时启之！祝君游之畅！" },
            { "已经要结束了吗？我会一直在这里，需要服务时请随时来找我！", "将终矣乎？吾将常驻于此，若有所需，请随时来寻吾！" }
        };

        /// <summary>
        /// 文本处理函数
        /// </summary>
        private static string ProcessGameText(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            // 首先尝试精确匹配
            if (gameTextMappings.ContainsKey(inputText))
            {
                return gameTextMappings[inputText];
            }

            // 然后尝试包含匹配
            string result = inputText;
            foreach (var mapping in gameTextMappings)
            {
                if (result.Contains(mapping.Key))
                {
                    result = result.Replace(mapping.Key, mapping.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// 拦截 DialogPackage 的文本获取
        /// 这是游戏对话系统的核心
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("GameData.Profile.DialogPackage:get_dialogContext")]
        static void DialogPackageGetDialogContextPostfix(ref object __result)
        {
            if (!Plugin.EnableCustomLanguage.Value || __result == null)
                return;

            try
            {
                // 获取文本内容
                var resultType = __result.GetType();
                var textProperty = resultType.GetProperty("text");
                
                if (textProperty != null)
                {
                    string originalText = textProperty.GetValue(__result) as string;
                    string processedText = ProcessGameText(originalText);
                    
                    if (originalText != processedText)
                    {
                        textProperty.SetValue(__result, processedText);
                        Plugin.Logger.LogInfo($"[DialogPackage] Replaced: '{originalText}' -> '{processedText}'");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogDebug($"[DialogPackage] Error processing dialog context: {ex.Message}");
            }
        }

        /// <summary>
        /// 拦截 MultiLanguageTextMeshCore 的语言设置
        /// 当语言切换时触发文本更新
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("GameData.MultiLanguageTextMeshCore:SetLanguageType")]
        static void MultiLanguageTextMeshSetLanguagePostfix(object type)
        {
            if (!Plugin.EnableCustomLanguage.Value)
                return;

            Plugin.Logger.LogInfo($"[MultiLanguageTextMeshCore] Language type changed to: {type}");
        }

        /// <summary>
        /// 拦截 MultiLanguageTextMesh 的文本分配
        /// 这是游戏的核心多语言文本系统
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("GameData.MultiLanguageTextMesh:OnAssignConfig")]
        static void MultiLanguageTextMeshAssignPostfix()
        {
            if (!Plugin.EnableCustomLanguage.Value)
                return;

            Plugin.Logger.LogDebug("[MultiLanguageTextMesh] Configuration assigned");
        }

        /// <summary>
        /// 拦截 CollabBehaviourComponent 的交互
        /// 这是联动终端的主要交互点
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("DayScene.Interactables.Collections.BehaviourComponents.CollabBehaviourComponent:OnInteract")]
        static void CollabBehaviourInteractPostfix()
        {
            if (!Plugin.EnableCustomLanguage.Value)
                return;

            Plugin.Logger.LogInfo("[CollabBehaviour] Interaction detected, monitoring dialog text...");
        }

        /// <summary>
        /// 拦截字符串格式化 - 备用方法
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static void StringFormatPostfix(ref string __result, string format, object[] args)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(__result))
                return;

            string original = __result;
            __result = ProcessGameText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogDebug($"[StringFormat] Replaced: '{original}' -> '{__result}'");
            }
        }

        /// <summary>
        /// 拦截字符串连接 - 备用方法
        /// </summary>
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
            __result = ProcessGameText(__result);
            
            if (original != __result)
            {
                Plugin.Logger.LogInfo($"[StringConcat] Replaced: '{original}' -> '{__result}'");
            }
        }

        /// <summary>
        /// 拦截调试日志（用于监控）
        /// </summary>
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
                string processedText = ProcessGameText(messageText);
                if (processedText != messageText)
                {
                    Plugin.Logger.LogInfo($"[DebugLog] Would replace: '{messageText}' -> '{processedText}'");
                }
            }
        }
    }
}
