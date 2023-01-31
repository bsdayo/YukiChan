using System.Text;
using Microsoft.Extensions.Logging;
using YukiChan.Client.Console;
using YukiChan.Core;
using YukiChan.Shared.Models.Arcaea;

namespace YukiChan.ImageGen.Utils;

public static class ArcaeaImageUtils
{
    public static Task<byte[]> GetDefaultCover() => File.ReadAllBytesAsync(
        $"{YukiDir.ArcaeaAssets}/images/song-cover-placeholder.png");

    /// <summary>
    /// 获取曲绘，首选从缓存获取，若缓存不存在则向 AUA 请求
    /// </summary>
    /// <param name="client">AuaClient</param>
    /// <param name="songId">曲目 ID</param>
    /// <param name="jacketOverride">谱面 JacketOverride</param>
    /// <param name="difficulty">谱面难度</param>
    /// <param name="nya">使用 arcanya 曲绘</param>
    /// <param name="logger">日志记录</param>
    public static async Task<byte[]> GetSongCover(YukiConsoleClient? client, string songId, bool jacketOverride = false,
        ArcaeaDifficulty difficulty = ArcaeaDifficulty.Future, bool nya = false,
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
                var path = $"{YukiDir.ArcaeaCache}/song/{songId}-{(int)difficulty}.jpg";
                if (File.Exists(path))
                    return await File.ReadAllBytesAsync(path);

                if (client is null) return await GetDefaultCover();

                songCover = await client.Assets.GetArcaeaSongCover(songId, difficulty);
                await File.WriteAllBytesAsync(path, songCover);
            }
            else
            {
                var path = $"{YukiDir.ArcaeaCache}/song/{songId}.jpg";
                if (File.Exists(path))
                    return await File.ReadAllBytesAsync(path);

                if (client is null) return await GetDefaultCover();

                songCover = await client.Assets.GetArcaeaSongCover(songId, ArcaeaDifficulty.Future);
                await File.WriteAllBytesAsync(path, songCover);
            }
        }
        catch
        {
            return await GetDefaultCover();
        }

        return songCover;
    }

    public static async Task<byte[]> GetCharImage(YukiConsoleClient? client, int charId,
        bool awakened = false, ILogger? logger = null)
    {
        byte[] charImage;

        try
        {
            var path = $"{YukiDir.ArcaeaCache}/char/{charId}{(awakened ? "-awakened.jpg" : ".jpg")}";
            if (File.Exists(path))
                return await File.ReadAllBytesAsync(path);

            if (client is null) return await GetDefaultCover();

            charImage = await client.Assets.GetArcaeaCharImage(charId, awakened);
            await File.WriteAllBytesAsync(path, charImage);
        }
        catch
        {
            return await GetDefaultCover();
        }

        return charImage;
    }

    public static string GetClearTypeImagePath(ArcaeaClearType clearType)
    {
        return $"{YukiDir.ArcaeaAssets}/images/" + clearType switch
        {
            ArcaeaClearType.NormalClear => "clear-tc.png",
            ArcaeaClearType.EasyClear => "clear-tc.png",
            ArcaeaClearType.HardClear => "clear-tc.png",
            ArcaeaClearType.TrackLost => "clear-tl.png",
            ArcaeaClearType.FullRecall => "clear-fr.png",
            ArcaeaClearType.PureMemory => "clear-pm.png",
            _ => "clear-tc.png"
        };
    }

    public static string GetMiniClearTypeImagePath(ArcaeaClearType clearType)
    {
        return $"{YukiDir.ArcaeaAssets}/images/" + clearType switch
        {
            ArcaeaClearType.NormalClear => "clear-mini-nc.png",
            ArcaeaClearType.EasyClear => "clear-mini-ec.png",
            ArcaeaClearType.HardClear => "clear-mini-hc.png",
            ArcaeaClearType.TrackLost => "clear-mini-tl.png",
            ArcaeaClearType.FullRecall => "clear-mini-fr.png",
            ArcaeaClearType.PureMemory => "clear-mini-pm.png",
            _ => "clear-mini-nc.png"
        };
    }

    public static string GetMiniGradeImagePath(ArcaeaGrade grade)
    {
        return $"{YukiDir.ArcaeaAssets}/images/" + grade switch
        {
            ArcaeaGrade.EXP => "grade-mini-exp.png",
            ArcaeaGrade.EX => "grade-mini-ex.png",
            ArcaeaGrade.AA => "grade-mini-aa.png",
            ArcaeaGrade.A => "grade-mini-a.png",
            ArcaeaGrade.B => "grade-mini-b.png",
            ArcaeaGrade.C => "grade-mini-c.png",
            ArcaeaGrade.D => "grade-mini-d.png",
            _ => "grade-mini-d.png"
        };
    }

    public static string GetSpacedUserCode(string usercode)
    {
        var span = usercode.AsSpan();
        var sb = new StringBuilder();
        return sb
            .Append(span[..3])
            .Append(' ')
            .Append(span[3..6])
            .Append(' ')
            .Append(span[6..])
            .ToString();
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
}