using ArcaeaUnlimitedAPI.Lib.Models;
using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Shared.Arcaea;
using YukiChan.Shared.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.ptt <args:text>")]
    public static async Task<MessageContent> OnPtt(MessageContext ctx, ParsedArgs args)
    {
        var argsArr = args.GetArgument<string>("args").Split(' ',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var difficulty = ArcaeaDifficulty.Future;
        string songname;
        var score = 0;

        switch (argsArr.Length)
        {
            case 0:
                return ctx.Reply("请输入需要计算的曲名哦~");

            case 1:
                return ctx.Reply("请输入需要计算的得分哦~");

            case 2:
                songname = argsArr[0];
                if (!int.TryParse(argsArr[1], out score))
                    return ctx.Reply("得分格式有误，请检查输入。");
                break;

            default:
                var scoreIndex = Array.FindIndex(argsArr, a => int.TryParse(a, out score));
                if (scoreIndex < 0) return ctx.Reply("得分格式有误，请检查输入。");
                songname = string.Join(' ', argsArr[..scoreIndex]);
                if (argsArr.Length <= scoreIndex + 1) break;
                var diff = ArcaeaUtils.GetRatingClass(argsArr[scoreIndex + 1]);
                if (diff is null)
                    return ctx.Reply("难度输入有误，请检查输入。");
                difficulty = diff.Value;
                break;
        }

        var song = await ArcaeaSongDatabase.Default.FuzzySearchSong(songname);
        if (song is null)
            return ctx.Reply("没有找到该曲目呢...");

        var rating = (double)song.Difficulties[(int)difficulty].Rating / 10;

        var ptt = ArcaeaUtils.CalculatePotential(rating, score);

        return ctx.Reply()
            .Text($"在曲目 {song.Difficulties[(int)difficulty].NameEn} [{difficulty}] 中，")
            .Text($"得分 {score} 的单曲潜力值为 {ptt:N4}。");
    }
}