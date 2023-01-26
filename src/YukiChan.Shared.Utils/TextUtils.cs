namespace YukiChan.Shared.Utils;

public static class TextUtils
{
    /// <summary>
    /// 获取文本缩写 (eg. brain power => bp)
    /// </summary>
    /// <param name="source">源文本</param>
    /// <returns>文本缩写</returns>
    public static string GetAbbreviation(this string source)
    {
        var str = "";
        var p = new StringParser(source);
        while (!p.SkipSpaces().IsEnd())
        {
            str += p.Current;
            p.Read(' ');
        }

        return str;
    }

    /// <summary>
    /// 从文本中删除指定的字符串
    /// </summary>
    /// <param name="source">源文本</param>
    /// <param name="removal">要删除的字符串</param>
    /// <returns>删除字符串后的文本</returns>
    public static string RemoveString(this string source, string removal)
    {
        return source.Replace(removal, string.Empty);
    }
}