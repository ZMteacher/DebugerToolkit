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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;

public class Debuger  
{
    public static LogConfig cfg;
    
    [Conditional("OPEN_LOG")]
    public static void InitLog(LogConfig _cfg = null)
    {
        if (_cfg==null)
        {
            cfg = new LogConfig();
        }
        else
        {
            cfg = _cfg;
        }
        if (cfg.logSave)
        {
            GameObject logObj = new GameObject("LogHelper");
            GameObject.DontDestroyOnLoad(logObj);
            UnityLogHelper unityLogHelper= logObj.AddComponent<UnityLogHelper>();
            unityLogHelper.InitLogFileModule(cfg.logFileSavePath,cfg.logFileName);
        }
        if (cfg.showFPS)
        {
            GameObject fpsObj = new GameObject("FPS");
            GameObject.DontDestroyOnLoad(fpsObj);
            fpsObj.AddComponent<FPS>();
        }
    }

    #region 普通日志
    [Conditional("OPEN_LOG")]
    public static void Log(object obj)
    {
        if (!cfg.openLog)
        {
            return;
        }
        string log= GenerateLog(obj.ToString());
        UnityEngine.Debug.Log(log);
    }
    [Conditional("OPEN_LOG")]
    public static void Log(string obj,params object[] args)
    {
        if (!cfg.openLog)
        {
            return;
        }
        string conent = string.Empty;
        if (args!=null)
        {
            foreach (var item in args)
            {
                conent += item;
            }
        }
        string log = GenerateLog(obj+conent);
        UnityEngine.Debug.Log(log);
    }
    [Conditional("OPEN_LOG")]
    public static void LogWarning(object obj)
    {
        if (!cfg.openLog)
        {
            return;
        }
        string log = GenerateLog(obj.ToString());
        UnityEngine.Debug.LogWarning(log);
    }
    [Conditional("OPEN_LOG")]
    public static void LogWarning(string obj, params object[] args)
    {
        if (!cfg.openLog)
        {
            return;
        }
        string conent = string.Empty;
        if (args != null)
        {
            foreach (var item in args)
            {
                conent += item;
            }
        }
        string log = GenerateLog(obj + conent);
        UnityEngine.Debug.LogWarning(log);
    }
    [Conditional("OPEN_LOG")]
    public static void LogError(object obj)
    {
        if (!cfg.openLog)
        {
            return;
        }
        string log = GenerateLog(obj.ToString());
        UnityEngine.Debug.LogError(log);
    }
    [Conditional("OPEN_LOG")]
    public static void LogError(string obj, params object[] args)
    {
        if (!cfg.openLog)
        {
            return;
        }
        string conent = string.Empty;
        if (args != null)
        {
            foreach (var item in args)
            {
                conent += item;
            }
        }
        string log = GenerateLog(obj + conent);
        UnityEngine.Debug.LogError(log);
    }

    #endregion

    #region 颜色日志打印
    [Conditional("OPEN_LOG")]
    public static void ColorLog(LogColor color,object obj)
    {
        if (!cfg.openLog)
        {
            return;
        }
        string log = GenerateLog(obj.ToString(),color);
        log = GetUnityColor(log, color);
        UnityEngine.Debug.Log(log);
    }
    [Conditional("OPEN_LOG")]
    public static void LogGreen(object msg)
    {
        ColorLog(LogColor.Green,msg);
    }
    [Conditional("OPEN_LOG")]
    public static void LogYellow(object msg)
    {
        ColorLog(LogColor.Yellow, msg);
    }
    [Conditional("OPEN_LOG")]
    public static void LogOrange(object msg)
    {
        ColorLog(LogColor.Orange, msg);
    }
    [Conditional("OPEN_LOG")]
    public static void LogRed(object msg)
    {
        ColorLog(LogColor.Red, msg);
    }
    [Conditional("OPEN_LOG")]
    public static void LogBlue(object msg)
    {
        ColorLog(LogColor.Blue, msg);
    }
    [Conditional("OPEN_LOG")]
    public static void LogMagenta(object msg)
    {
        ColorLog(LogColor.Magenta, msg);
    }
    [Conditional("OPEN_LOG")]
    public static void LogCyan(object msg)
    {
        ColorLog(LogColor.Cyan, msg);
    }
#endregion

    public static string GenerateLog(string log,LogColor color=LogColor.None)
    {
        StringBuilder stringBuilder = new StringBuilder(cfg.logHeadFix,100);
        if (cfg.openTime)
        {
            stringBuilder.AppendFormat(" {0}",DateTime.Now.ToString("hh:mm:ss-fff"));
        }
        if (cfg.showThreadID)
        {
            stringBuilder.AppendFormat(" ThreadID {0}:", Thread.CurrentThread.ManagedThreadId);
        }
        if (cfg.showColorName)
        {
            stringBuilder.AppendFormat(" {0}", color.ToString());
        }
        stringBuilder.AppendFormat(" {0}", log);
        return stringBuilder.ToString();
    }

    public static string GetUnityColor(string msg,LogColor color)
    {
        if (color== LogColor.None)
        {
            return msg;
        }
        switch (color)
        {
            case LogColor.Blue:
                msg = $"<color=#0000FF>{msg}</color>";
                break;
            case LogColor.Cyan:
                msg = $"<color=#00FFFF>{msg}</color>";
                break;
            case LogColor.Darkblue:
                msg = $"<color=#8FBC8F>{msg}</color>";
                break;
            case LogColor.Green:
                msg = $"<color=#00FF00>{msg}</color>";
                break;
            case LogColor.Orange:
                msg = $"<color=#FFA500>{msg}</color>";
                break;
            case LogColor.Red:
                msg = $"<color=#FF0000>{msg}</color>";
                break;
            case LogColor.Yellow:
                msg = $"<color=#FFFF00>{msg}</color>";
                break;
            case LogColor.Magenta:
                msg = $"<color=#FF00FF>{msg}</color>";
                break;
        }
        return msg;
    }
}
