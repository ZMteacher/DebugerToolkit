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
using System.Diagnostics;
using UnityEngine;
using ZM.DebugerKit;

public partial class Debuger
{
    public static LogConfig cfg = new LogConfig();
    private static UnityLogHelper sLogHelper;
    private static FrameRateDisplay _sFrameRateDisplay;
    
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
            if (sLogHelper == null)
            {
                GameObject logObj = new GameObject("LogHelper");
                GameObject.DontDestroyOnLoad(logObj);
                sLogHelper = logObj.AddComponent<UnityLogHelper>();
                sLogHelper.InitLogFileModule(cfg.logFileSavePath,cfg.logFileName);
            }
        }
        if (cfg.showFPS)
        {
            if (_sFrameRateDisplay == null)
            {
                GameObject fpsObj = new GameObject("FrameRateDisplay");
                GameObject.DontDestroyOnLoad(fpsObj);
                _sFrameRateDisplay = fpsObj.AddComponent<FrameRateDisplay>();
            }
        }
    }

    internal static void NotifyLogHelperDestroyed(UnityLogHelper helper)
    {
        if (sLogHelper == helper)
        {
            sLogHelper = null;
        }
        DeveloperLogRegistry.ClearDeveloper();
    }

}
