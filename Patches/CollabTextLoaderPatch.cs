using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using SchaleIzakaya.LanguageInjector.Models;

namespace SchaleIzakaya.LanguageInjector.Patches
{
    [HarmonyPatch]
    public class CollabTextLoaderPatch
    {
        // 联动模块文本映射
        private static Dictionary<string, string> collabTextMappings = new Dictionary<string, string>
        {
            { "0", "欢迎访问《东方夜雀食堂》联运终端！于此，君可管理他联运作品之联运活动！" },
            { "1", "已启矣！若有所需，亦可于此随时闭之！祝君游之畅！" },
            { "2", "明矣，欲暂闭此联运活动乎？" },
            { "3", "此联运活动已闭！若有所需，亦可于此随时启之！祝君游之畅！" },
            { "4", "将终矣乎？吾将常驻于此，若有所需，请随时来寻吾！" },
            { "5", "祝君游之畅！" }
        };

        private static Type dataBaseLanguageType;
        private static MethodInfo getCollabTextMethod;

        static CollabTextLoaderPatch()
        {
            dataBaseLanguageType = Type.GetType("GameData.CoreLanguage.Collections.DataBaseLanguage, Assembly-CSharp");
            
            if (dataBaseLanguageType != null)
            {
                // 尝试找到获取联动文本的方法
                getCollabTextMethod = dataBaseLanguageType.GetMethod("GetCollabText", BindingFlags.Public | BindingFlags.Static);
                if (getCollabTextMethod == null)
                {
                    getCollabTextMethod = dataBaseLanguageType.GetMethod("GetCollabModuleText", BindingFlags.Public | BindingFlags.Static);
                }
                if (getCollabTextMethod == null)
                {
                    getCollabTextMethod = dataBaseLanguageType.GetMethod("GetModuleText", BindingFlags.Public | BindingFlags.Static);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object[]) })]
        static void Postfix1(ref string __result, string format, object[] args)
        {
            ReplaceCollabText(ref __result);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object) })]
        static void Postfix2(ref string __result, string format, object arg0)
        {
            ReplaceCollabText(ref __result);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(string), "Format", new Type[] { typeof(string), typeof(object), typeof(object) })]
        static void Postfix3(ref string __result, string format, object arg0, object arg1)
        {
            ReplaceCollabText(ref __result);
        }

        private static void ReplaceCollabText(ref string text)
        {
            if (!Plugin.EnableCustomLanguage.Value || string.IsNullOrEmpty(text))
            {
                return;
            }

            // 检查是否包含联动终端相关的文本
            if (text.Contains("欢迎访问") && text.Contains("联动终端"))
            {
                text = collabTextMappings["0"];
                Plugin.Logger.LogDebug($"Replaced collab welcome text: {text}");
            }
            else if (text.Contains("已经开启了") && text.Contains("有需要的话"))
            {
                text = collabTextMappings["1"];
                Plugin.Logger.LogDebug($"Replaced collab active text: {text}");
            }
            else if (text.Contains("明白了") && text.Contains("暂时关闭"))
            {
                text = collabTextMappings["2"];
                Plugin.Logger.LogDebug($"Replaced collab confirm close text: {text}");
            }
            else if (text.Contains("该联动活动已经关闭") && text.Contains("有需要的话"))
            {
                text = collabTextMappings["3"];
                Plugin.Logger.LogDebug($"Replaced collab closed text: {text}");
            }
            else if (text.Contains("已经要结束了") && text.Contains("需要服务时"))
            {
                text = collabTextMappings["4"];
                Plugin.Logger.LogDebug($"Replaced collab ending text: {text}");
            }
            else if (text == "祝您玩的开心！")
            {
                text = collabTextMappings["5"];
                Plugin.Logger.LogDebug($"Replaced collab farewell text: {text}");
            }
        }
    }
}
