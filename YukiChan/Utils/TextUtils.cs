namespace YukiChan.Utils;

public static class TextUtils
{
    /// <summary>
    ///     获取文本缩写 (eg. brain power => bp)
    /// </summary>
    /// <param name="source">源文本</param>
    /// <returns>文本缩写</returns>
    public static string GetAbbreviation(this string source)
    {
        var str = source[0].ToString();
        for (var i = 0; i < source.Length; i++)
            if (source[i] == ' ')
                str += source[i + 1];

        return str;
    }

    public static string RemoveString(this string source, string remove)
    {
        return source.Replace(remove, "");
    }
}