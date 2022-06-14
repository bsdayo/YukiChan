using System.Text.RegularExpressions;
using Konata.Core.Message;
using Konata.Core.Message.Model;

namespace YukiChan.Utils;

public static class CommonUtils
{
    public static string[] ParseCommandBody(string body)
    {
        return body
            .Split(" ")
            .Where(elem => elem != "")
            .ToArray();
    }

    public static MessageBuilder ReplyMessage(MessageStruct message)
    {
        return new MessageBuilder()
            .Add(ReplyChain.Create(message));
    }

    public static double Bytes2MiB(this long bytes, int round)
    {
        return Math.Round(bytes / 1048576.0, round);
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

public class YukiException : Exception
{
    public YukiException(string message) : base(message)
    {
    }
}