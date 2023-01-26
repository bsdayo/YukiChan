using System.Text.RegularExpressions;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using YukiChan.Shared.Data;

namespace YukiChan.Utils;

public static class CommonUtils
{
    public static MessageBuilder Reply(this MessageContext ctx)
    {
        return ctx.Bot.Platform == "qqguild"
            ? new MessageBuilder().Add(new AtSegment(ctx.Message.Sender.UserId))
            : new MessageBuilder().Add(new QuoteSegment(ctx.Message));
    }

    public static MessageBuilder Reply(this MessageContext ctx, string message)
    {
        return Reply(ctx).Text(message);
    }

    public static MessageBuilder ReplyServerError(this MessageContext ctx, YukiResponse resp)
    {
        return resp.Code.IsArcaeaAuaError()
            ? Reply(ctx, resp.Message ?? $"AuaErr{(int)resp.Code}: {resp.Message}")
            : Reply(ctx, $"发生了奇怪的错误！({(int)resp.Code}: {resp.Message})");
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
}

public sealed class YukiException : Exception
{
    public YukiException(string message) : base(message)
    {
    }
}