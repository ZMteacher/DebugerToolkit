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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogConfig  
{
    /// <summary>
    /// �Ƿ����־ϵͳ
    /// </summary>
    public bool openLog = true;
    /// <summary>
    /// ��־ǰ׺
    /// </summary>
    public string logHeadFix = "###";
    /// <summary>
    /// �Ƿ���ʾʱ��
    /// </summary>
    public bool openTime = true;
    /// <summary>
    /// ��ʾ�߳�id
    /// </summary>
    public bool showThreadID = true;
    /// <summary>
    /// ��־�ļ����濪��
    /// </summary>
    public bool logSave = true;
    /// <summary>
    /// �Ƿ���ʾFPS
    /// </summary>
    public bool showFPS = true;
    /// <summary>
    /// ��ʾ��ɫ����
    /// </summary>
    public bool showColorName = true;
    /// <summary>
    /// �ļ�����·��
    /// </summary>
    public string logFileSavePath { get { return Application.persistentDataPath + "/"; } }
    /// <summary>
    /// ��־�ļ�����
    /// </summary>
    public string logFileName { get { return Application.productName + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm")+".log"; } }
}
