/*------------------------------------------------------------------
*
* Title: 毕业级日志系统 
*
* Description: 支持本地文件写入、自定义颜色日志、FPS实时显示、手机日志运行时查看、日志代码编译剔除、ProtoBuff转Json、日志重定向
* 
* Author: https://www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b 铸梦
*
* Date: 2023.8.13
*
* Modify: 
-------------------------------------------------------------------*/
using UnityEngine;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace ZM.DebugerKit
{
/// <summary>
/// Unity Log重定向。编辑器下，点击log跳转到代码位置
/// </summary>
public class LogRedirection
{
#if UNITY_EDITOR
    /// <summary>
    /// 最大匹配检索深度
    /// </summary>
    private const int MaxRegexMatch = 20;
    // 处理asset打开的callback函数
    [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    static bool OnOpenAsset(int instance, int line)
    {
        // 自定义函数，用来获取stacktrace
        string stack_trace = GetStackTrace();

        // 通过stacktrace来判断是否是自定义Log
        if (!string.IsNullOrEmpty(stack_trace))
        {
            if (stack_trace.Contains("Debuger:"))//这里的“* ”是从堆栈中筛选自定义的Log
            {
                MatchCollection matches = Regex.Matches(stack_trace, @"\(at\s+(.+):(\d+)\)", RegexOptions.IgnoreCase);
                for (int i = 0; i < matches.Count && i < MaxRegexMatch; i++)
                {
                    Match match = matches[i];
                    string path = match.Groups[1].Value.Trim();
                    if (path.EndsWith("Debuger.cs", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    int targetLine;
                    if (!int.TryParse(match.Groups[2].Value, out targetLine))
                    {
                        continue;
                    }

                    string projectRoot = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                    string fullPath = System.IO.Path.Combine(projectRoot, path).Replace('/', '\\');
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, targetLine);
                    return true;
                }
            }
        }

        return false;
    }

    static string GetStackTrace()
    {
        // 找到UnityEditor.EditorWindow的assembly
        var assembly_unity_editor = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
        if (assembly_unity_editor == null) return null;

        // 找到类UnityEditor.ConsoleWindow
        var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
        if (type_console_window == null) return null;
        // 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
        var field_console_window = type_console_window.GetField("ms_ConsoleWindow",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (field_console_window == null) return null;
        // 获取ms_ConsoleWindow的值
        var instance_console_window = field_console_window.GetValue(null);
        if (instance_console_window == null) return null;

        // 如果console窗口时焦点窗口的话，获取stacktrace
        if ((object)UnityEditor.EditorWindow.focusedWindow == instance_console_window)
        {
            // 通过assembly获取类ListViewState
            var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
            if (type_list_view_state == null) return null;

            // 找到类UnityEditor.ConsoleWindow中的成员m_ListView
            var field_list_view = type_console_window.GetField("m_ListView",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_list_view == null) return null;

            // 获取m_ListView的值
            var value_list_view = field_list_view.GetValue(instance_console_window);
            if (value_list_view == null) return null;

            // 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
            var field_active_text = type_console_window.GetField("m_ActiveText",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_active_text == null) return null;

            // 获得m_ActiveText的值，就是我们需要的stacktrace
            string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
            return value_active_text;
        }

        return null;
    }
#endif
}
}
