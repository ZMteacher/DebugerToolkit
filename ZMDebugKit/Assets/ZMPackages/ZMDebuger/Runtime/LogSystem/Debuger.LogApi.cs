using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;
using ZM.DebugerKit;

/// <summary>
/// 面向业务层的日志输出接口。
/// </summary>
public partial class Debuger
{
    /// <summary>
    /// 注册一个绑定开发者 TAG 和对应颜色的日志通道。
    /// 后续开发者在使用这个颜色打印日志时，会输出注册的Tag标签。
    /// 在多人协作时，区分是非输出的日志。
    /// </summary>
    public static void RegisterDeveloper(string developerTag, LogColor color)
    {
          DeveloperLogRegistry.Register(developerTag, color);
    }
    
    #region 普通日志

    [Conditional("OPEN_LOG")]
    public static void Log(object obj)
    {
        if (!cfg.openLog) return;
        UnityEngine.Debug.Log(GenerateLog(obj == null ? "null" : obj.ToString()));
    }

    [Conditional("OPEN_LOG")]
    public static void Log(string obj, params object[] args)
    {
        if (!cfg.openLog) return;
        UnityEngine.Debug.Log(GenerateLog(CombineMessage(obj, args)));
    }

    [Conditional("OPEN_LOG")]
    public static void LogWarning(object obj)
    {
        if (!cfg.openLog) return;
        UnityEngine.Debug.LogWarning(GenerateLog(obj == null ? "null" : obj.ToString()));
    }

    [Conditional("OPEN_LOG")]
    public static void LogWarning(string obj, params object[] args)
    {
        if (!cfg.openLog) return;
        UnityEngine.Debug.LogWarning(GenerateLog(CombineMessage(obj, args)));
    }

    [Conditional("OPEN_LOG")]
    public static void LogError(object obj)
    {
        if (!cfg.openLog) return;
        UnityEngine.Debug.LogError(GenerateLog(obj == null ? "null" : obj.ToString()));
    }

    [Conditional("OPEN_LOG")]
    public static void LogError(string obj, params object[] args)
    {
        if (!cfg.openLog) return;
        UnityEngine.Debug.LogError(GenerateLog(CombineMessage(obj, args)));
    }

    #endregion

    #region 颜色日志

    [Conditional("OPEN_LOG")]
    private static void ColorLog(LogColor color, object obj)
    {
        if (!cfg.openLog) return;
        string log = GenerateLog(obj == null ? "null" : obj.ToString(), color);
        UnityEngine.Debug.Log(GetUnityColor(log, color));
    }

    [Conditional("OPEN_LOG")]
    public static void LogGreen(object msg) => ColorLog(LogColor.Green, msg);

    [Conditional("OPEN_LOG")]
    public static void LogYellow(object msg) => ColorLog(LogColor.Yellow, msg);

    [Conditional("OPEN_LOG")]
    public static void LogOrange(object msg) => ColorLog(LogColor.Orange, msg);

    [Conditional("OPEN_LOG")]
    public static void LogRed(object msg) => ColorLog(LogColor.Red, msg);

    [Conditional("OPEN_LOG")]
    public static void LogBlue(object msg) => ColorLog(LogColor.Blue, msg);

    [Conditional("OPEN_LOG")]
    public static void LogMagenta(object msg) => ColorLog(LogColor.Magenta, msg);

    [Conditional("OPEN_LOG")]
    public static void LogCyan(object msg) => ColorLog(LogColor.Cyan, msg);

    #endregion

    #region 日志生成和写入
    
    private static string GenerateLog(string log, LogColor color = LogColor.None)
    {
        StringBuilder builder = new StringBuilder(cfg.logHeadFix, 100);
        if (color != LogColor.None)
        {
           string developerName = DeveloperLogRegistry.GetDeveloper(color);
           if (!string.IsNullOrEmpty(developerName))
           {
               builder.AppendFormat(" {0}", $"[{developerName}]");
           }
        }

        if (cfg.openTime) builder.AppendFormat(" {0}", DateTime.Now.ToString("HH:mm:ss-fff"));
        if (cfg.showThreadID) builder.AppendFormat(" ThreadID {0}:", Thread.CurrentThread.ManagedThreadId);
        if (cfg.showColorName) builder.AppendFormat(" {0}", color);
        builder.AppendFormat(" {0}", log);
        return builder.ToString();
    }

    internal static string GenerateDeveloperLog(string developerTag, string log, LogColor color)
    {
        StringBuilder builder = new StringBuilder(cfg.logHeadFix, 100);
        builder.AppendFormat(" [{0}]", developerTag);
        if (cfg.openTime) builder.AppendFormat(" {0}", DateTime.Now.ToString("HH:mm:ss-fff"));
        if (cfg.showThreadID) builder.AppendFormat(" ThreadID {0}:", Thread.CurrentThread.ManagedThreadId);
        if (cfg.showColorName) builder.AppendFormat(" {0}", color);
        builder.AppendFormat(" {0}", log);
        return GetUnityColor(builder.ToString(), color);
    }

    private static string GetUnityColor(string msg, LogColor color)
    {
        switch (color)
        {
            case LogColor.Blue: return $"<color=#0000FF>{msg}</color>";
            case LogColor.Cyan: return $"<color=#00FFFF>{msg}</color>";
            case LogColor.Darkblue: return $"<color=#8FBC8F>{msg}</color>";
            case LogColor.Green: return $"<color=#00FF00>{msg}</color>";
            case LogColor.Grey: return $"<color=#808080>{msg}</color>";
            case LogColor.Orange: return $"<color=#FFA500>{msg}</color>";
            case LogColor.Red: return $"<color=#FF0000>{msg}</color>";
            case LogColor.Yellow: return $"<color=#FFFF00>{msg}</color>";
            case LogColor.Magenta: return $"<color=#FF00FF>{msg}</color>";
            case LogColor.Purple: return $"<color=#800080>{msg}</color>";
            default: return msg;
        }
    }

    internal static string CombineMessage(string message, object[] args)
    {
        StringBuilder content = new StringBuilder(message ?? "null");
        if (args == null) return content.ToString();

        foreach (object item in args)
        {
            content.Append(item);
        }
        return content.ToString();
    }

    #endregion
}
