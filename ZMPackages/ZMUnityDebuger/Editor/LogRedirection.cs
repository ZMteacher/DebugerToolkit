/*------------------------------------------------------------------
*
* Title: ��ҵ����־ϵͳ 
*
* Description: ֧�ֱ����ļ�д�롢�Զ�����ɫ��־��FPSʵʱ��ʾ���ֻ���־����ʱ�鿴����־��������޳���ProtoBuffתJson����־�ض���
* 
* Author: https://www.taikr.com/user/63798c7981862239d5b3da44d820a7171f0ce14d ����
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
/// Unity Log�ض��򡣱༭���£����log��ת������λ��
/// </summary>
public class LogRedirection
{
#if UNITY_EDITOR
    /// <summary>
    /// ���ƥ��������
    /// </summary>
    private const int MaxRegexMatch = 20;
    // ����asset�򿪵�callback����
    [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    static bool OnOpenAsset(int instance, int line)
    {
        // �Զ��庯����������ȡstacktrace
        string stack_trace = GetStackTrace();

        // ͨ��stacktrace���ж��Ƿ����Զ���Log
        if (!string.IsNullOrEmpty(stack_trace))
        {
            if (stack_trace.Contains("Debuger:"))//����ġ�* ���ǴӶ�ջ��ɸѡ�Զ����Log
            {
                //ƥ������Log��
                Match matches = Regex.Match(stack_trace, @"\(at(.+)\)", RegexOptions.IgnoreCase);
                string pathline = "";
                if (matches.Success)
                {
                    int index = 0;
                    //�ҵ���Debuger�ű�������ת
                    while (matches.Groups[1].Value.Contains("Debuger.cs")&& index < 10)
                    {
                        matches = matches.NextMatch();
                        index++;
                    }
 
                    //��ת�߼�
                    if (matches.Success)
                    {
                        pathline = matches.Groups[1].Value;
                        pathline = pathline.Replace(" ", "");

                        //�ҵ����뼰����
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
        // �ҵ�UnityEditor.EditorWindow��assembly
        var assembly_unity_editor = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
        if (assembly_unity_editor == null) return null;

        // �ҵ���UnityEditor.ConsoleWindow
        var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
        if (type_console_window == null) return null;
        // �ҵ�UnityEditor.ConsoleWindow�еĳ�Աms_ConsoleWindow
        var field_console_window = type_console_window.GetField("ms_ConsoleWindow",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (field_console_window == null) return null;
        // ��ȡms_ConsoleWindow��ֵ
        var instance_console_window = field_console_window.GetValue(null);
        if (instance_console_window == null) return null;

        // ���console����ʱ���㴰�ڵĻ�����ȡstacktrace
        if ((object)UnityEditor.EditorWindow.focusedWindow == instance_console_window)
        {
            // ͨ��assembly��ȡ��ListViewState
            var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
            if (type_list_view_state == null) return null;

            // �ҵ���UnityEditor.ConsoleWindow�еĳ�Աm_ListView
            var field_list_view = type_console_window.GetField("m_ListView",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_list_view == null) return null;

            // ��ȡm_ListView��ֵ
            var value_list_view = field_list_view.GetValue(instance_console_window);
            if (value_list_view == null) return null;

            // �ҵ���UnityEditor.ConsoleWindow�еĳ�Աm_ActiveText
            var field_active_text = type_console_window.GetField("m_ActiveText",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field_active_text == null) return null;

            // ���m_ActiveText��ֵ������������Ҫ��stacktrace
            string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
            return value_active_text;
        }

        return null;
    }
#endif
}
