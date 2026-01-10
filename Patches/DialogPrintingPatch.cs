using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;
using UnityEngine;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class DialogPrintingPatch
    {
        // 对话框文本映射
        private static Dictionary<string, string> dialogTextMappings = new Dictionary<string, string>
        {
            { "0", "欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！" },
            { "1", "已启矣！若有所需，亦可于此随时闭之！祝君游之畅！" },
            { "2", "明矣，欲暂闭此联运活动乎？" },
            { "3", "此联运活动已闭！若有所需，亦可于此随时启之！祝君游之畅！" },
            { "4", "将终矣乎？吾将常驻于此，若有所需，请随时来寻吾！" },
            { "5", "祝君游之畅！" }
        };

        private static Type dialogPanelType;
        private static MethodInfo printDialogMethod;

        static DialogPrintingPatch()
        {
            // 尝试找到对话框打印方法
            dialogPanelType = Type.GetType("GameData.UI.DialogPanel, Assembly-CSharp");
            if (dialogPanelType == null)
            {
                dialogPanelType = Type.GetType("GameData.Dialog.DialogPanel, Assembly-CSharp");
            }
            if (dialogPanelType == null)
            {
                dialogPanelType = Type.GetType("DialogPanel, Assembly-CSharp");
            }

            if (dialogPanelType != null)
            {
                // 尝试找到打印对话框的方法
                printDialogMethod = dialogPanelType.GetMethod("PrintDialog", BindingFlags.Public | BindingFlags.Instance);
                if (printDialogMethod == null)
                {
                    printDialogMethod = dialogPanelType.GetMethod("Print", BindingFlags.Public | BindingFlags.Instance);
                }
                if (printDialogMethod == null)
                {
                    printDialogMethod = dialogPanelType.GetMethod("ShowText", BindingFlags.Public | BindingFlags.Instance);
                }
            }
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
            
            // 检查是否包含对话框打印相关的日志
            if (messageText.Contains("Printing Dialog:") || messageText.Contains("DialPann: Printing Dialog"))
            {
                string originalMessage = messageText;
                
                // 尝试替换所有可能的文本
                foreach (var mapping in dialogTextMappings)
                {
                    if (messageText.Contains(mapping.Key))
                    {
                        messageText = messageText.Replace(mapping.Key, mapping.Value);
                        Plugin.Logger.LogInfo($"[Dialog Log Interceptor] Replaced: {mapping.Key} -> {mapping.Value}");
                    }
                }
                
                if (originalMessage != messageText)
                {
                    message = messageText;
                }
            }
            
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnityEngine.Debug), "Log", new Type[] { typeof(object) })]
        static void Postfix1(ref object __result, object message)
        {
            // 这个补丁主要是为了记录日志，不需要实际操作
        }
    }
}
