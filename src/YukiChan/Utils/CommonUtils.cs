using System.Text.RegularExpressions;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;

namespace YukiChan.Utils;

public static class CommonUtils
{
    public static MessageBuilder Reply(this MessageContext ctx)
    {
        if (ctx.Bot.Platform == "qqguild")
            return new MessageBuilder().Add(new AtSegment(ctx.Message.Sender.UserId));
        return new MessageBuilder().Add(new QuoteSegment(ctx.Message));
    }

    public static MessageBuilder Reply(this MessageContext ctx, string message)
    {
        return Reply(ctx).Text(message);
    }

    public static double Bytes2MiB(this long bytes, int roundDigits)
    {
        return Math.Round(bytes / 1048576.0, roundDigits);
    }

    public static Dictionary<string, string> GetMetaData(this string html, params string[] keys)
    {
        var metaDict = new Dictionary<string, string>();

        foreach (var i in keys)
        {
            var pattern = i + @"=""(.*?)""(.|\s)*?content=""(.*?)"".*?>";

            // Match results
            foreach (Match j in Regex.Matches(html, pattern, RegexOptions.Multiline))
                metaDict.TryAdd(j.Groups[1].Value, j.Groups[3].Value);
        }

        return metaDict;
    }

    public static string FormatTimestamp(this long timestamp, bool inMilliseconds = false)
    {
        return (inMilliseconds
                ? DateTimeOffset.FromUnixTimeMilliseconds(timestamp)
                : DateTimeOffset.FromUnixTimeSeconds(timestamp))
            .LocalDateTime
            .ToString("yyyy.MM.dd HH:mm:ss");
    }

    public static long GetTimestamp(this DateTime datetime, bool inMilliseconds = false)
    {
        var dto = new DateTimeOffset(datetime);
        return inMilliseconds
            ? dto.ToUnixTimeMilliseconds()
            : dto.ToUnixTimeSeconds();
    }
}

public class YukiException : Exception
{
    public YukiException(string message) : base(message)
    {
    }
}