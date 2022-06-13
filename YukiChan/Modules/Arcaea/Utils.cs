namespace YukiChan.Modules.Arcaea;

public static class ArcaeaUtils
{
    /// <summary>
    ///     转换 rating (eg. 98) 为难度 (eg. 9+)
    /// </summary>
    /// <param name="rating">曲目 rating (定数*10)</param>
    /// <returns>难度文本</returns>
    public static string GetDifficulty(this int rating)
    {
        var i = rating;

        while (i > 9)
            i = rating % 10;

        if (rating > 90 && i >= 7)
            return rating / 10 + "+";

        return (rating / 10).ToString();
    }
}