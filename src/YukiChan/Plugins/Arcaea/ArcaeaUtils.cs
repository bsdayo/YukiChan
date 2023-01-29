using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins.Arcaea;

public static class ArcaeaUtils
{
    public static (string, ArcaeaDifficulty) ParseMixedSongNameAndDifficulty(string textArg)
    {
        var arr = textArg.Split(' ',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var difficulty = ArcaeaSharedUtils.GetArcaeaDifficulty(arr[^1]);

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