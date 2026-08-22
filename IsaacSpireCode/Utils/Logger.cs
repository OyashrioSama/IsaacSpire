using System;
using System.IO;
using System.Text;

namespace IsaacSpire.Utils;

public static class LogHelper
{
    private static readonly string LogPath = @"D:\IsaacSpire_Log.txt";
    private static readonly object _lock = new object();

    static LogHelper()
    {
        try
        {
            if (System.IO.File.Exists(LogPath))
                System.IO.File.Delete(LogPath);
        }
        catch { }
    }

    public static void Log(string message)
    {
        try
        {
            lock (_lock)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logLine = $"[{timestamp}] {message}";
                System.IO.File.AppendAllText(LogPath, logLine + Environment.NewLine);
                Godot.GD.Print(logLine);
            }
        }
        catch { }
    }

    public static void LogError(string message, Exception? ex = null)
    {
        var fullMessage = ex != null ? $"{message} | Exception: {ex.Message}\n{ex.StackTrace}" : message;
        Log($"【ERROR】{fullMessage}");
    }
}