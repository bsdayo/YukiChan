using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YukiChan.ImageGen.Utils;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.random [range:text]")]
    [Shortcut("随机曲目")]
    public async Task<MessageContent> OnRandom(MessageContext ctx, ParsedArgs args)
    {
        var range = args.GetArgument<string>("range")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        int start, end;
        var allCharts = await _service.SongDb.Charts.AsNoTracking().ToListAsync();

        try
        {
            switch (range.Length)
            {
                // 不提供参数，全曲 Future 难度随机
                case 0:
                    return await ConstructRandomReply(ctx, allCharts
                        .Where(chart => chart.RatingClass == (int)ArcaeaDifficulty.Future)
                        .ToArray());

                // 提供定数范围
                case 1:
                    start = ArcaeaSharedUtils.GetRatingRange(range[0]).Start;
                    end = ArcaeaSharedUtils.GetRatingRange(range[0]).End;

                    if (start == -1 || end == -1)
                        return ctx.Reply("输入了错误的定数呢...");
                    if (start is < 0 or > 120 || end is < 0 or > 120)
                        return ctx.Reply("定数超出范围啦！");

                    return await ConstructRandomReply(ctx, allCharts
                        .Where(chart => chart.Rating >= start && chart.Rating <= end)
                        .ToArray());

                // 提供最低和最高定数
                case 2:
                    start = ArcaeaSharedUtils.GetRatingRange(range[0]).Start;
                    end = ArcaeaSharedUtils.GetRatingRange(range[1]).End;

                    if (start == -1 || end == -1)
                        return ctx.Reply("输入了错误的定数呢...");
                    if (start > end)
                        return ctx.Reply("最低定数比最高定数大诶...");
                    if (start is < 0 or > 120 || end is < 0 or > 120)
                        return ctx.Reply("定数超出范围啦！");

                    return await ConstructRandomReply(ctx, allCharts
                        .Where(chart => chart.Rating >= start && chart.Rating <= end)
                        .ToArray());

                default:
                    return ctx.Reply("参数太多啦！");
            }
        }
        catch (YukiException e)
        {
            _logger.LogError(e, string.Empty);
            return ctx.Reply(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, string.Empty);
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private async Task<MessageBuilder> ConstructRandomReply(MessageContext ctx, ArcaeaSongDbChart[] allCharts)
    {
        if (allCharts.Length == 0)
            return ctx.Reply("该定数范围内没有曲目哦~");

        var chart = allCharts[new Random().Next(allCharts.Length)];

        var songCover = await ArcaeaImageUtils.GetSongCover(_yukiClient,
            chart.SongId, chart.JacketOverride, (ArcaeaDifficulty)chart.RatingClass);
        var package = await _service.SongDb.Packages
            .AsNoTracking()
            .FirstAsync(package => package.Set == chart.Set);

        return ctx.Reply()
            .Text("随机推荐曲目：\n")
            .Image(songCover)
            .Text($"{chart.NameEn}\n")
            .Text($"({package.Name})\n")
            .Text(
                $"{(ArcaeaDifficulty)chart.RatingClass} {chart.Rating.GetRatingText()} [{(double)chart.Rating / 10:0.0}]");
    }
}