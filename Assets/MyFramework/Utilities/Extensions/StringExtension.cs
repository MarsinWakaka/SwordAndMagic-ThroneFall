using UnityEngine;

namespace MyFramework.Utilities.Extensions
{
    public static class StringExtension
    {
        // 带颜色的日志输出，方法名为什么比较好？
        public static void LogWithColor(this string message, Color color)
        {
            var colorHex = ColorUtility.ToHtmlStringRGBA(color);
            var formattedMessage = $"<color=#{colorHex}>{message}</color>";
            Debug.Log(formattedMessage);
        }
    }
}