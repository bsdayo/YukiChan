using System.Text.RegularExpressions;

namespace YukiChan.Utils;

public static class CommonUtils
{
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