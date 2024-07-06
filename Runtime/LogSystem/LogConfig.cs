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
using UnityEngine;

public class LogConfig  
{
    /// <summary>
    /// 是否打开日志系统
    /// </summary>
    public bool openLog = true;
    /// <summary>
    /// 日志前缀
    /// </summary>
    public string logHeadFix = "###";
    /// <summary>
    /// 是否显示时间
    /// </summary>
    public bool openTime = true;
    /// <summary>
    /// 显示线程id
    /// </summary>
    public bool showThreadID = true;
    /// <summary>
    /// 日志文件储存开关
    /// </summary>
    public bool logSave = true;
    /// <summary>
    /// 是否显示FPS
    /// </summary>
    public bool showFPS = true;
    /// <summary>
    /// 显示颜色名称
    /// </summary>
    public bool showColorName = true;
    /// <summary>
    /// 文件储存路径
    /// </summary>
    public string logFileSavePath { get { return Application.persistentDataPath + "/"; } }
    /// <summary>
    /// 日志文件名称
    /// </summary>
    public string logFileName { get { return Application.productName + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm")+".log"; } }
}
