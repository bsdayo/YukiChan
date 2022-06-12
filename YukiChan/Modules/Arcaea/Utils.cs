namespace YukiChan.Modules.Arcaea;

public static class ArcaeaUtils
{
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