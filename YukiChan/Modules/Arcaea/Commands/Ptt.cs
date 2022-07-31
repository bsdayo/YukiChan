using ArcaeaUnlimitedAPI.Lib.Models;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("Ptt",
        Command = "ptt",
        Description = "根据分数计算单曲潜力值",
        Usage = "a ptt <曲名> [难度] <得分>",
        Example = "a ptt testify byd 9900000")]
    public static MessageBuilder PttCalc(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);

        var difficulty = ArcaeaDifficulty.Future;
        var songname = "";
        var scoreStr = "";

        switch (args.Length)
        {
            case 0:
                return message.Reply("请输入需要计算的曲名哦~");

            case 1:
                return message.Reply("请输入需要计算的得分哦~");

            case 2:
                songname = args[0];
                scoreStr = args[1];
                break;

            case 3:
                songname = args[0];
                var diff = ArcaeaUtils.GetRatingClass(args[1]);
                if (diff is null)
                    return message.Reply("难度输入有误，请检查输入。");
                difficulty = diff.Value;
                scoreStr = args[2];
                break;
        }

        if (!double.TryParse(scoreStr, out var score))
            return message.Reply("得分格式有误，请检查输入。");

        var song = ArcaeaSongDatabase.FuzzySearchSong(songname);
        if (song is null)
            return message.Reply("没有找到该曲目呢...");

        var rating = (double)song.Difficulties[(int)difficulty].Rating / 10;

        var ptt = score switch
        {
            >= 10000000 => rating + 2,
            >= 9800000 => rating + 1 + (score - 9800000) / 200000,
            _ => rating + (score - 9500000) / 300000
        };

        ptt = Math.Max(0, ptt);

        return message.Reply()
            .Text($"在曲目 {song.Difficulties[(int)difficulty].NameEn} [{difficulty}] 中，")
            .Text($"得分 {(int)score} 的单曲潜力值为 {ptt:F4}。");
    }
}