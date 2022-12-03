using ArcaeaUnlimitedAPI.Lib;
using ArcaeaUnlimitedAPI.Lib.Models;
using Microsoft.Extensions.Logging;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Utils;

namespace YukiChan.Plugins.Arcaea;

public static class ArcaeaUtils
{
    /// <summary>
    /// 转换 rating (eg. 98) 为难度 (eg. 9+)
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

    /// <summary>
    /// 获取曲绘，首选从缓存获取，若缓存不存在则向 AUA 请求
    /// </summary>
    /// <param name="client">AuaClient</param>
    /// <param name="songId">曲目 ID</param>
    /// <param name="jacketOverride">谱面 JacketOverride</param>
    /// <param name="difficulty">谱面难度</param>
    /// <param name="nya">使用 arcanya 曲绘</param>
    /// <param name="logger">日志记录</param>
    public static async Task<byte[]> GetSongCover(this AuaClient client, string songId,
        bool jacketOverride = false, ArcaeaDifficulty difficulty = ArcaeaDifficulty.Future, bool nya = false,
        ILogger? logger = null)
    {
        byte[] songCover;

        try
        {
            if (nya)
            {
                var path = jacketOverride
                    ? $"{YukiDir.ArcaeaAssets}/arcanya/{songId}-{difficulty.ToString().ToLower()}.png"
                    : $"{YukiDir.ArcaeaAssets}/arcanya/{songId}.png";

                if (File.Exists(path))
                    return await File.ReadAllBytesAsync(path);
            }

            if (jacketOverride)
            {
                var path = $"{YukiDir.ArcaeaCache}/song/{songId}-{difficulty.ToString().ToLower()}.jpg";
                if (File.Exists(path))
                    return await File.ReadAllBytesAsync(path);

                songCover = await client.Assets.Song(songId, AuaSongQueryType.SongId, difficulty);
                await File.WriteAllBytesAsync(path, songCover);
                logger?.SaveCache(path);
            }
            else
            {
                var path = $"{YukiDir.ArcaeaCache}/song/{songId}.jpg";
                if (File.Exists(path))
                    return await File.ReadAllBytesAsync(path);

                songCover = await client.Assets.Song(songId, AuaSongQueryType.SongId);
                await File.WriteAllBytesAsync(path, songCover);
                logger?.SaveCache(path);
            }
        }
        catch
        {
            songCover = await File.ReadAllBytesAsync(
                $"{YukiDir.ArcaeaAssets}/images/song-cover-placeholder.png");
        }

        return songCover;
    }

    public static async Task<byte[]> GetCharImage(this AuaClient client, int charId,
        bool awakened = false, ILogger? logger = null)
    {
        byte[] charImage;

        try
        {
            var path = $"{YukiDir.ArcaeaCache}/char/{charId}{(awakened ? "-awakened" : "")}.jpg";
            if (File.Exists(path))
                return await File.ReadAllBytesAsync(path);

            charImage = await client.Assets.Char(charId, awakened);
            await File.WriteAllBytesAsync(path, charImage);
            logger?.SaveCache(path);
        }
        catch
        {
            charImage = await File.ReadAllBytesAsync(
                $"{YukiDir.ArcaeaAssets}/images/song-cover-placeholder.png");
        }

        return charImage;
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
            "0" or "pst" or "past" => ArcaeaDifficulty.Past,
            "1" or "prs" or "present" => ArcaeaDifficulty.Present,
            "2" or "ftr" or "future" => ArcaeaDifficulty.Future,
            "3" or "byd" or "byn" or "beyond" => ArcaeaDifficulty.Beyond,
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
            .Replace('ω', 'w')
            .Replace('ο', 'o')
            .Replace('κ', 'k');
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

    public static (string, ArcaeaDifficulty) ParseMixedSongNameAndDifficulty(string textArg)
    {
        var arr = textArg.Split(' ',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var difficulty = GetRatingClass(arr[^1]);

        var songname = difficulty is null
            ? textArg
            : string.Join(' ', arr[..^1]);
        return (songname, difficulty ?? ArcaeaDifficulty.Future);
    }

    public static ArcaeaGuessMode? GetGuessMode(string text)
    {
        return text.ToLower() switch
        {
            "e" => ArcaeaGuessMode.Easy,
            "easy" => ArcaeaGuessMode.Easy,
            "简单" => ArcaeaGuessMode.Easy,

            "n" => ArcaeaGuessMode.Normal,
            "normal" => ArcaeaGuessMode.Normal,
            "正常" => ArcaeaGuessMode.Normal,

            "h" => ArcaeaGuessMode.Hard,
            "hard" => ArcaeaGuessMode.Hard,
            "困难" => ArcaeaGuessMode.Hard,

            "f" => ArcaeaGuessMode.Flash,
            "flash" => ArcaeaGuessMode.Flash,
            "闪照" => ArcaeaGuessMode.Flash,

            "g" => ArcaeaGuessMode.GrayScale,
            "gray" => ArcaeaGuessMode.GrayScale,
            "grayscale" => ArcaeaGuessMode.GrayScale,
            "灰度" => ArcaeaGuessMode.GrayScale,

            "i" => ArcaeaGuessMode.Invert,
            "invert" => ArcaeaGuessMode.Invert,
            "反色" => ArcaeaGuessMode.Invert,

            _ => null
        };
    }

    public static string GetName(this ArcaeaGuessMode mode)
    {
        return mode switch
        {
            ArcaeaGuessMode.Easy => "简单",
            ArcaeaGuessMode.Normal => "正常",
            ArcaeaGuessMode.Hard => "困难",
            ArcaeaGuessMode.Flash => "闪照",
            ArcaeaGuessMode.GrayScale => "灰度",
            ArcaeaGuessMode.Invert => "反色",
            _ => "未知模式"
        };
    }
}