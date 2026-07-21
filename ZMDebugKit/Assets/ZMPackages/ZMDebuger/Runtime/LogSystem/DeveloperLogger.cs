using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace ZM.DebugerKit
{
 

    /// <summary>
    /// 开发者日志通道注册表。相同 TAG 只能绑定同一种颜色。
    /// </summary>
    internal static class DeveloperLogRegistry
    {
        private const int MaxTagLength = 32;
        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<LogColor,string> Developers = new Dictionary<LogColor,string>();

        internal static void Register(string tag, LogColor color)
        {
            string normalizedTag = NormalizeTag(tag);
            if (color == LogColor.None)
                throw new ArgumentException("开发者日志通道必须选择一种颜色。", nameof(color));

            lock (SyncRoot)
            {
                
                if (Developers.TryGetValue(color, out var  developerName ) && !string.IsNullOrEmpty(developerName))
                {
                    if (developerName!= tag) throw new InvalidOperationException($"开发者 TAG [{normalizedTag}] 已绑定 {color}，不能重复绑定为 {color}。");
                    return ;
                }

                Developers.Add(color, tag);
            }
        }

        public static string GetDeveloper(LogColor color)
        {
            Developers.TryGetValue(color, out var developerName);
            return developerName;
        }

        private static string NormalizeTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("开发者 TAG 不能为空。", nameof(tag));

            string normalized = tag.Trim();
            while (normalized.Length >= 2 && normalized[0] == '[' && normalized[normalized.Length - 1] == ']')
                normalized = normalized.Substring(1, normalized.Length - 2).Trim();

            StringBuilder builder = new StringBuilder(normalized.Length);
            foreach (char character in normalized)
            {
                if (!char.IsControl(character) && character != '<' && character != '>' && character != '[' && character != ']')
                    builder.Append(character);
            }

            normalized = builder.ToString().Trim();
            if (normalized.Length == 0)
                throw new ArgumentException("开发者 TAG 不能只包含括号、控制字符或富文本符号。", nameof(tag));
            if (normalized.Length > MaxTagLength)
                throw new ArgumentException($"开发者 TAG 不能超过 {MaxTagLength} 个字符。", nameof(tag));

            return normalized;
        }
        
        public static void ClearDeveloper()
        {
            Developers.Clear();
        }
    }
}
