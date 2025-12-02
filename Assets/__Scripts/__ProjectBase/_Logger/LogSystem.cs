using UnityEngine;
using System.IO;
using System;

public class LogSystem : System<LogSystem>
{
    private static string logFolderPath;
    private static string logFilePath;
    private static StreamWriter logFileWriter;
    private static _E_LOG_TYPE currentLogLevel;
    private bool isHandlingLog = false;

    protected override void OnInit()
    {
        // 设置当前日志级别
#if UNITY_EDITOR
        currentLogLevel = _E_LOG_TYPE.Debug;
#else
        currentLogLevel = _E_LOG_TYPE.Info;
#endif
        // 设置日志文件路径
        SetLogFolderPath();

        // 管理日志文件
        ManageLogFiles();

        // 创建新的日志文件
        logFilePath = Path.Combine(logFolderPath, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");
        logFileWriter = new StreamWriter(logFilePath, true, System.Text.Encoding.UTF8);

        // 重定向Unity的日志
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceived += HandleLog;
    }

    protected override void OnShutdown()
    {
        // 确保退出时停止重定向日志
        Application.logMessageReceived -= HandleLog;

        // 关闭日志文件
        if (logFileWriter != null)
        {
            logFileWriter.Close();
            logFileWriter = null;
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logFileWriter == null)
        {
            return;
        }
        
        // 转换Unity的默认日志级别
        _E_LOG_TYPE level = _E_LOG_TYPE.Info;
        switch (type)
        {
            case LogType.Warning:
                level = _E_LOG_TYPE.Warning;
                break;
            case LogType.Error:
                level = _E_LOG_TYPE.Error;
                break;
            case LogType.Exception:
                level = _E_LOG_TYPE.Fatal;
                break;
        }

        WriteLogToFile(level, logString);
        if (type == LogType.Exception || type == LogType.Error)
        {
            WriteLogToFile(level, stackTrace);
        }
    }

    private void WriteLogToFile(_E_LOG_TYPE level, string message)
    {
        if (isHandlingLog)
        {
            return;
        }

        if (logFileWriter != null)
        {
            logFileWriter.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}");
            logFileWriter.Flush();
        }
    }

    private void SetLogFolderPath()
    {
#if UNITY_EDITOR
        // 在编辑器中，将日志文件目录设置为项目的根目录
        logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
#else
        // 在发布的游戏中，将日志文件目录设置为可执行文件所在的目录
        logFolderPath = Path.Combine(Application.dataPath, "Logs");
#endif
        logFolderPath = Path.Combine(logFolderPath, "ManagedLogs");
        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
        }
    }

    private void ManageLogFiles()
    {
        // 获取日志文件列表并按创建时间排序
        var logFiles = new DirectoryInfo(logFolderPath).GetFiles("*.log");
        Array.Sort(logFiles, (f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));

        // 删除旧的日志文件，只保留最近的5个
        int filesToDelete = logFiles.Length - 5;
        for (int i = 0; i < filesToDelete; i++)
        {
            logFiles[i].Delete();
        }
    }

    private void LogMessage(string message, _E_LOG_TYPE level)
    {
        if (level < currentLogLevel)
        {
            return;
        }

        isHandlingLog = true;

        // 根据日志级别使用Unity的日志方法
        switch (level)
        {
            case _E_LOG_TYPE.Debug:
                UnityEngine.Debug.Log(message);
                break;
            case _E_LOG_TYPE.Info:
                UnityEngine.Debug.Log(message);
                break;
            case _E_LOG_TYPE.Warning:
                UnityEngine.Debug.LogWarning(message);
                break;
            case _E_LOG_TYPE.Error:
                UnityEngine.Debug.LogError(message);
                break;
            case _E_LOG_TYPE.Fatal:
                UnityEngine.Debug.LogError(message);
                break;
        }

        isHandlingLog = false;

        WriteLogToFile(level, message);
    }

    public void Debug(int message)
    {
        Debug(message.ToString());
    }

    public void Debug(string message)
    {
        LogMessage(message, _E_LOG_TYPE.Debug);
    }

    public void Info(int message)
    {
        Info(message.ToString());
    }

    public void Info(string message)
    {
        LogMessage(message, _E_LOG_TYPE.Info);
    }

    public void Warn(int message)
    {
        Warn(message.ToString());
    }

    public void Warn(string message)
    {
        LogMessage(message, _E_LOG_TYPE.Warning);
    }

    public void Error(int message)
    {
        Error(message.ToString());
    }

    public void Error(string message)
    {
        LogMessage(message, _E_LOG_TYPE.Error);
    }

    public void Fatal(int message)
    {
        Fatal(message.ToString());
    }

    public void Fatal(string message)
    {
        LogMessage(message, _E_LOG_TYPE.Fatal);
    }
}