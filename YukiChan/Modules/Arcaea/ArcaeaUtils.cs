using ArcaeaUnlimitedAPI.Lib;
using ArcaeaUnlimitedAPI.Lib.Models;
using YukiChan.Utils;

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

    /// <summary>
    ///     转换难度文本 (eg. 9+) 为难度区间 (eg. 97 ~ 99)
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

    /// <summary>
    ///     获取曲绘，首选从缓存获取，若缓存不存在则向 AUA 请求
    /// </summary>
    /// <param name="client">AuaClient</param>
    /// <param name="songId">曲目 ID</param>
    /// <param name="jacketOverride">谱面 JacketOverride</param>
    /// <param name="difficulty">谱面难度</param>
    /// <returns>曲绘</returns>
    public static async Task<byte[]> GetSongCover(this AuaClient client,
        string songId, bool jacketOverride = false, ArcaeaDifficulty difficulty = ArcaeaDifficulty.Future)
    {
        byte[] songCover;

        try
        {
            if (jacketOverride)
            {
                var path = $"Cache/Arcaea/Song/{songId}-{difficulty.ToString().ToLower()}.jpg";
                try
                {
                    return await File.ReadAllBytesAsync(path);
                }
                catch
                {
                    songCover = await client.Assets.Song(songId, AuaSongQueryType.SongId, difficulty);

                    await File.WriteAllBytesAsync(path, songCover);
                    YukiLogger.SaveCache(path);
                }
            }
            else
            {
                var path = $"Cache/Arcaea/Song/{songId}.jpg";
                try
                {
                    return await File.ReadAllBytesAsync(path);
                }
                catch
                {
                    songCover = await client.Assets.Song(songId, AuaSongQueryType.SongId);
                    await File.WriteAllBytesAsync(path, songCover);
                    YukiLogger.SaveCache(path);
                }
            }
        }
        catch
        {
            songCover = await File.ReadAllBytesAsync("Assets/Arcaea/Images/SongCoverPlaceholder.jpg");
        }

        return songCover;
    }

    public static string FormatScore(this int score)
    {
        return score
            .ToString("N0")
            .PadLeft(10, '0')
            .Replace(',', '\'');
    }

    public static ArcaeaDifficulty? GetRatingClass(string difficultyText)
    {
        return difficultyText.ToLower() switch
        {
            "0" => ArcaeaDifficulty.Past,
            "pst" => ArcaeaDifficulty.Past,
            "past" => ArcaeaDifficulty.Past,

            "1" => ArcaeaDifficulty.Present,
            "prs" => ArcaeaDifficulty.Present,
            "present" => ArcaeaDifficulty.Present,

            "2" => ArcaeaDifficulty.Future,
            "ftr" => ArcaeaDifficulty.Future,
            "future" => ArcaeaDifficulty.Future,

            "3" => ArcaeaDifficulty.Beyond,
            "byd" => ArcaeaDifficulty.Beyond,
            "byn" => ArcaeaDifficulty.Beyond,
            "beyond" => ArcaeaDifficulty.Beyond,

            _ => null
        };
    }

    public static string ReplaceNotSupportedChar(string text)
    {
        return text
            .Replace('：', ':')
            .Replace('α', 'a')
            .Replace('β', 'b')
            .Replace('έ', 'e')
            .Replace('ό', 'o')
            .Replace('γ', 'g')
            .Replace('Ä', 'A')
            .Replace('ö', 'o')
            .Replace('δ', 'd')
            .Replace('ω', 'w');
    }
}