using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
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
    private StreamWriter mStreamWriter;
    private readonly ConcurrentQueue<LogData> mQueue = new ConcurrentQueue<LogData>();
    private readonly AutoResetEvent mLogAvailableEvent = new AutoResetEvent(false);
    private readonly object mCallbackLock = new object();
    private Thread mFileThread;
    private volatile bool mStopRequested;
    private volatile bool mAcceptLogs;
    private int mShutdownStarted;

    private string NowTime
    {
        get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"); }
    }

    public void InitLogFileModule(string savePath, string logFileName)
    {
        Directory.CreateDirectory(savePath);
        string logFilePath = Path.Combine(savePath, logFileName);
        Debug.Log("logFilePath:" + logFilePath);

        mStreamWriter = new StreamWriter(logFilePath, true, new UTF8Encoding(false));
        mAcceptLogs = true;
        Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;

        mFileThread = new Thread(FileLogThread)
        {
            IsBackground = true,
            Name = "ZMLogFileWriter"
        };
        mFileThread.Start();
    }

    private void FileLogThread()
    {
        try
        {
            while (true)
            {
                mLogAvailableEvent.WaitOne();
                DrainQueue();

                if (mStopRequested && mQueue.IsEmpty)
                {
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            lock (mCallbackLock)
            {
                mAcceptLogs = false;
            }
            Debug.LogException(exception);
        }
        finally
        {
            if (mStreamWriter != null)
            {
                mStreamWriter.Flush();
                mStreamWriter.Dispose();
                mStreamWriter = null;
            }
        }
    }

    private void DrainQueue()
    {
        LogData data;
        while (mQueue.TryDequeue(out data))
        {
            mStreamWriter.Write(GetLogTypeName(data.type));
            mStreamWriter.Write(" >>> ");
            mStreamWriter.WriteLine(data.log);
            if (!string.IsNullOrEmpty(data.trace))
            {
                mStreamWriter.WriteLine(data.trace);
            }
            mStreamWriter.WriteLine();
        }
        mStreamWriter.Flush();
    }

    private static string GetLogTypeName(LogType type)
    {
        switch (type)
        {
            case LogType.Warning: return "Warning";
            case LogType.Error: return "Error";
            case LogType.Assert: return "Assert";
            case LogType.Exception: return "Exception";
            default: return "Log";
        }
    }

    private void OnApplicationQuit()
    {
        Shutdown();
    }

    private void OnDestroy()
    {
        Shutdown();
    }

    public void Shutdown()
    {
        if (Interlocked.Exchange(ref mShutdownStarted, 1) != 0)
        {
            return;
        }

        lock (mCallbackLock)
        {
            mAcceptLogs = false;
        }
        Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        mStopRequested = true;
        mLogAvailableEvent.Set();

        if (mFileThread != null && mFileThread.IsAlive && Thread.CurrentThread != mFileThread)
        {
            mFileThread.Join();
        }

        Debuger.NotifyLogHelperDestroyed(this);
    }

    private void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
        lock (mCallbackLock)
        {
            if (!mAcceptLogs)
            {
                return;
            }

            mQueue.Enqueue(new LogData
            {
                log = NowTime + " " + condition,
                trace = stackTrace,
                type = type
            });
            mLogAvailableEvent.Set();
        }
    }
}
