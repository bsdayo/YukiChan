using System.Text;
using Flandre.Core.Events.Logger;
using Flandre.Core.Utils;

namespace YukiChan.Utils;

public static class LoggerExtensions
{
    public static void SaveToFile(LoggerLoggingEvent e)
    {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var logFs = new FileStream($"{YukiDir.Logs}/yuki/{date}.log",
            FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        using var sw = new StreamWriter(logFs, Encoding.UTF8);
        sw.WriteLine(e.Message);
        sw.Flush();
    }

    public static void SaveCache(this Logger logger, string path)
    {
        logger.Info($"保存缓存: {path}");
    }
}