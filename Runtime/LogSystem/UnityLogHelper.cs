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
    /// 文件写入流
    /// </summary>
    private StreamWriter mStreamWriter;
    /// <summary>
    /// 日志数据队列
    /// </summary>
    private readonly ConcurrentQueue<LogData> mConCurrentQueue = new ConcurrentQueue<LogData>();
    /// <summary>
    /// 工作信号事件
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
            mManualRestEvent.WaitOne();//让线程进入等待，并进行阻塞
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
            //保存当前文件内容，使其生效
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
        //mManualRestEvent.WaitOne();//让线程进入等待，并进行阻塞
        //mManualRestEvent.Set();//设置一个信号，表示线程是需要工作的
        //mManualRestEvent.Reset();//重置信号，表示没有人指定需要工作
    }
}
