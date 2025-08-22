using UnityEngine;
using System.IO;
using System;

public class Log : MonoBehaviour
{
    private string logFilePath;

    void Start()
    {
        // 设置日志文件路径
        logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

        // 删除旧的日志文件（可选）
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        // 注册日志回调
        Application.logMessageReceived += HandleLog;

        Debug.Log("日志系统初始化完成，日志文件路径: " + logFilePath);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        try
        {
            // 格式化日志信息
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{type}] {logString}\n";

            // 如果是错误或异常，添加堆栈跟踪
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                logEntry += $"Stack Trace:\n{stackTrace}\n";
            }

            // 写入文件
            File.AppendAllText(logFilePath, logEntry);
        }
        catch (Exception e)
        {
            // 避免日志写入出错导致循环异常
            Debug.LogError("写入日志文件时出错: " + e.Message);
        }
    }

    void OnDestroy()
    {
        // 取消注册日志回调
        Application.logMessageReceived -= HandleLog;
    }
}