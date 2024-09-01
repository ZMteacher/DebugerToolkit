/*------------------------------------------------------------------
*
* Title: 毕业级日志系统 
*
* Description: 支持本地文件写入、自定义颜色日志、FPS实时显示、手机日志运行时查看、日志代码编译剔除、ProtoBuff转Json、日志重定向
* 
* Author: https://www.taikr.com/user/63798c7981862239d5b3da44d820a7171f0ce14d 铸梦
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
                //匹配所有Log行
                Match matches = Regex.Match(stack_trace, @"\(at(.+)\)", RegexOptions.IgnoreCase);
                string pathline = "";
                if (matches.Success)
                {
                    int index = 0;
                    //找到非Debuger脚本进行跳转
                    while (matches.Groups[1].Value.Contains("Debuger.cs")&& index < 10)
                    {
                        matches = matches.NextMatch();
                        index++;
                    }
 
                    //跳转逻辑
                    if (matches.Success)
                    {
                        pathline = matches.Groups[1].Value;
                        pathline = pathline.Replace(" ", "");

                        //找到代码及行数
                        int split_index = pathline.LastIndexOf(":");
                        string path = pathline.Substring(0, split_index);
                        line = Convert.ToInt32(pathline.Substring(split_index + 1));
                        string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullpath += path;
                        string strPath = fullpath.Replace('/', '\\');
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(strPath, line);
                    }
                    else
                    {
                        Debug.LogError("DebugCodeLocation OnOpenAsset, Error StackTrace");
                    }

                    matches = matches.NextMatch();
                }

                return true;
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
