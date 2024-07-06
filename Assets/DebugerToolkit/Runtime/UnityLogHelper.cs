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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class LogData
{
    public string log;
    public string trace;
    public LogType type;
}

public class UnityLogHelper : MonoBehaviour
{
    /// <summary>
    /// �ļ�д����
    /// </summary>
    private StreamWriter mStreamWriter;
    /// <summary>
    /// ��־���ݶ���
    /// </summary>
    private readonly ConcurrentQueue<LogData> mConCurrentQueue = new ConcurrentQueue<LogData>();
    /// <summary>
    /// �����ź��¼�
    /// </summary>
    private readonly ManualResetEvent mManualRestEvent = new ManualResetEvent(false);
    private bool mThreadRuning = false;
    private string mNowTime { get { return DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"); } }
    public void InitLogFileModule(string savePath,string logfineName)
    {
        string logFilePath = Path.Combine(savePath,logfineName);
        Debug.Log("logFilePath:"+ logFilePath);
        mStreamWriter = new StreamWriter(logFilePath);
        Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
        mThreadRuning = true;
        Thread fileThread = new Thread(FileLogThread);
        fileThread.Start();
    }

    public void FileLogThread()
    {
        while (mThreadRuning)
        {
            mManualRestEvent.WaitOne();//���߳̽���ȴ�������������
            if (mStreamWriter==null)
            {
                break;
            }
            LogData data;
            while (mConCurrentQueue.Count>0&&mConCurrentQueue.TryDequeue(out data))
            {
                if (data.type==LogType.Log)
                {
                    mStreamWriter.Write("Log >>> ");
                    mStreamWriter.WriteLine(data.log);
                    mStreamWriter.WriteLine(data.trace);
                }
                else if(data.type == LogType.Warning)
                {
                    mStreamWriter.Write("Warning >>> ");
                    mStreamWriter.WriteLine(data.log);
                    mStreamWriter.WriteLine(data.trace);
                }
                else if (data.type == LogType.Error)
                {
                    mStreamWriter.Write("Error >>> ");
                    mStreamWriter.WriteLine(data.log);
                    mStreamWriter.Write('\n');
                    mStreamWriter.WriteLine(data.trace);
                }
                mStreamWriter.Write("\r\n");
            }
            //���浱ǰ�ļ����ݣ�ʹ����Ч
            mStreamWriter.Flush();
            mManualRestEvent.Reset();
            Thread.Sleep(1);
        }
    }
    public void OnApplicationQuit()
    {
        Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        mThreadRuning = false;
        mManualRestEvent.Reset();
        mStreamWriter.Close();
        mStreamWriter = null;
    }
    private void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
        mConCurrentQueue.Enqueue(new LogData { log= mNowTime +" "+ condition ,trace= stackTrace ,type=type});
        mManualRestEvent.Set();
        //mManualRestEvent.WaitOne();//���߳̽���ȴ�������������
        //mManualRestEvent.Set();//����һ���źţ���ʾ�߳�����Ҫ������
        //mManualRestEvent.Reset();//�����źţ���ʾû����ָ����Ҫ����
    }
}
