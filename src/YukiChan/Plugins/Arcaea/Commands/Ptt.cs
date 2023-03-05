using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.ptt")]
    public async Task<MessageContent> OnPtt(MessageContext ctx, string[] args)
    {
        var difficulty = ArcaeaDifficulty.Future;
        string songname;
        var score = 0;

        switch (args.Length)
        {
            case 0:
                return ctx.Reply("请输入需要计算的曲名哦~");

            case 1:
                return ctx.Reply("请输入需要计算的得分哦~");

            case 2:
                songname = args[0];
                if (!int.TryParse(args[1], out score))
                    return ctx.Reply("得分格式有误，请检查输入。");
                break;

            default:
                var scoreIndex = Array.FindIndex(args, a => int.TryParse(a, out score));
                if (scoreIndex < 0) return ctx.Reply("得分格式有误，请检查输入。");
                songname = string.Join(' ', args[..scoreIndex]);
                if (args.Length <= scoreIndex + 1) break;
                var diff = ArcaeaSharedUtils.GetArcaeaDifficulty(args[scoreIndex + 1]);
                if (diff is null)
                    return ctx.Reply("难度输入有误，请检查输入。");
                difficulty = diff.Value;
                break;
        }

        var song = await _service.SongDb.FuzzySearchSong(songname);
        if (song is null)
            return ctx.Reply("没有找到该曲目呢...");

        var rating = song.Difficulties[(int)difficulty].Rating;

        var ptt = ArcaeaSharedUtils.CalculatePotential(rating, score);

        return ctx.Reply()
            .Text($"在曲目 {song.Difficulties[(int)difficulty].NameEn} [{difficulty}] 中，")
            .Text($"得分 {score} 的单曲潜力值为 {ptt:N4}。");
    }
}