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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.DebugerKit
{
    public class LogSystem : MonoBehaviour
    {
        void Awake()
        {
#if OPEN_LOG
            //菜单栏-ZMLog
            Debuger.InitLog(new LogConfig
            {
                openLog = true,
                openTime = true,
                showThreadID = true,
                showColorName = true,
                logSave = true,
                showFPS = true,
            });
            //注册一个绑定开发者 TAG 和对应颜色的日志通道。后续开发者在使用这个颜色打印日志时，会输出注册的Tag标签。在多人协作时，区分是非输出的日志。
            Debuger.RegisterDeveloper("铸梦", LogColor.Cyan);
            Debuger.RegisterDeveloper("老唐", LogColor.Green);
            //日志测试
            Debuger.Log("Log system initialized successfully.");
            Debuger.LogCyan("This is a cyan test log.");
            Debuger.LogGreen("This is a green test log.");
            Debuger.LogWarning("This is a warning test log.");
            Debuger.LogError("This is an error test log.");
#else
            Debug.Log("LogSystem is closed, please open it in the menu bar-ZMLog");
#endif
        }

         
    }
}
