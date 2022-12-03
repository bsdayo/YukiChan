using Microsoft.Extensions.Logging;

namespace YukiChan.Utils;

public static class LoggerExtensions
{
    public static void SaveCache(this ILogger logger, string path)
    {
        logger.LogInformation("Cache saved: {Path}", path);
    }

    public static void LogError(this ILogger logger, Exception e)
    {
        logger.LogError(e, "Error occurred.");
    }
}