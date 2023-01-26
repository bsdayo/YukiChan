using System.Text;
using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.Shared.Utils;

public static class ArcaeaSharedUtils
{
    /// <summary>
    /// 转换 rating (eg. 98) 为难度 (eg. 9+)
    /// </summary>
    /// <param name="rating">曲目 rating (定数*10)</param>
    /// <returns>难度文本</returns>
    public static string GetRatingText(this int rating)
    {
        var i = rating;

        while (i > 9)
            i = rating % 10;

        if (rating > 90 && i >= 7)
            return rating / 10 + "+";

        return (rating / 10).ToString();
    }

    public static string GetDisplayPotential(double potential)
    {
        return potential >= 0 ? potential.ToString("F2") : "?";
    }

    /// <summary>
    /// 转换难度文本 (eg. 9+) 为难度区间 (eg. 97 ~ 99)
    /// </summary>
    /// <param name="difficulty">难度文本</param>
    /// <returns>难度区间</returns>
    public static (int Start, int End) GetRatingRange(string difficulty)
    {
        return difficulty switch
        {
            "1" => (10, 19),
            "2" => (20, 29),
            "3" => (30, 39),
            "4" => (40, 49),
            "5" => (50, 59),
            "6" => (60, 69),
            "7" => (70, 79),
            "8" => (80, 89),
            "9" => (90, 96),
            "9+" => (97, 99),
            "10" => (100, 106),
            "10+" => (107, 109),
            "11" => (110, 116),
            "11+" => (117, 119),
            "12" => (120, 126),
            _ => double.TryParse(difficulty, out var rating)
                ? ((int)(rating * 10), (int)(rating * 10))
                : (-1, -1)
        };
    }

    public static string FormatScore(this int score)
    {
        var span = score.ToString("00000000").AsSpan();
        var sb = new StringBuilder();
        sb.Append(span[..2]).Append('\'').Append(span[2..5]).Append('\'').Append(span[5..]);
        return sb.ToString();
    }

    public static ArcaeaDifficulty? GetRatingClass(string difficultyText)
    {
        return difficultyText.ToLower() switch
        {
            "0" or "pst" or "past" => ArcaeaDifficulty.Past,
            "1" or "prs" or "present" => ArcaeaDifficulty.Present,
            "2" or "ftr" or "future" => ArcaeaDifficulty.Future,
            "3" or "byd" or "byn" or "beyond" => ArcaeaDifficulty.Beyond,
            _ => null
        };
    }

    public static ArcaeaGrade GetGrade(int score)
    {
        return score switch
        {
            >= 9900000 => ArcaeaGrade.EXP,
            >= 9800000 and < 9900000 => ArcaeaGrade.EX,
            >= 9500000 and < 9800000 => ArcaeaGrade.AA,
            >= 9200000 and < 9500000 => ArcaeaGrade.A,
            >= 8900000 and < 9200000 => ArcaeaGrade.B,
            >= 8600000 and < 8900000 => ArcaeaGrade.C,
            _ => ArcaeaGrade.D
        };
    }

    public static double CalculatePotential(double rating, int score)
    {
        var ptt = score switch
        {
            >= 10000000 => rating + 2,
            >= 9800000 => rating + 1 + ((double)score - 9800000) / 200000,
            _ => rating + ((double)score - 9500000) / 300000
        };

        return Math.Max(0, ptt);
    }
}