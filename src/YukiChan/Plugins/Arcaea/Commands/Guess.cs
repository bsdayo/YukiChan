// ReSharper disable CheckNamespace

using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YukiChan.ImageGen.Utils;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Utils;

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    private class ArcaeaGuessSession
    {
        public long Timestamp { get; set; }
        public ArcaeaSongDbChart Chart { get; set; } = null!;
        public byte[] Cover { get; set; } = null!;
        public bool IsReady { get; set; }
    }

    private static readonly Dictionary<string, ArcaeaGuessSession> GuessSessions = new();

    [Command("a.guess [guessOrMode:text]")]
    [Shortcut("猜曲绘")]
    public async Task<MessageContent> OnGuess(MessageContext ctx, ParsedArgs args)
    {
        var guessOrMode = args.GetArgument<string>("guessOrMode");
        var sessionId = ctx.Message.SourceType == MessageSourceType.Channel
            ? $"{ctx.Platform}:{ctx.Message.ChannelId}"
            : $"{ctx.Platform}:private:{ctx.UserId}";

        try
        {
            if (GuessSessions.TryGetValue(sessionId, out var session))
            {
                if (string.IsNullOrWhiteSpace(guessOrMode))
                    return ctx.Reply("当前猜曲绘游戏正在进行，请等待结束后再开始新游戏~");

                if (!session.IsReady)
                    return ctx.Reply("题目正在初始化中，请稍等...");

                var guessSongId = await _service.SongDb.FuzzySearchId(guessOrMode);
                if (guessSongId is null)
                    return ctx.Reply("没有找到该曲目哦！");

                // 判断是否猜对
                if (guessSongId != session.Chart.SongId)
                    return ctx.Reply("猜错啦！");

                GuessSessions.Remove(sessionId);

                var set = await _service.SongDb.Packages
                    .AsNoTracking()
                    .FirstAsync(package => package.Set == session.Chart.Set);
                return ctx.Reply("猜对啦！")
                    .Image(session.Cover)
                    .Text($"{session.Chart.NameEn} - {session.Chart.Artist}\n")
                    .Text($"({set.Name})");
            }

            var mode = string.IsNullOrWhiteSpace(guessOrMode)
                ? ArcaeaGuessMode.Normal
                : ArcaeaUtils.GetGuessMode(guessOrMode);

            if (mode is null)
                return ctx.Reply("当前群内没有正在进行的猜曲绘游戏，可以发送“猜曲绘”来开启新游戏哦~");

            return await StartNewGuess(sessionId, ctx, mode.Value);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred in a.guess");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private async Task<MessageBuilder> StartNewGuess(string sessionId, MessageContext ctx, ArcaeaGuessMode mode)
    {
        var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

        // 占位
        GuessSessions.TryAdd(sessionId,
            new ArcaeaGuessSession { Timestamp = timestamp });

        var allCharts = await _service.SongDb.Charts
            .Where(chart => chart.RatingClass == (int)ArcaeaDifficulty.Future)
            .ToArrayAsync();
        var randomChart = allCharts[new Random().Next(allCharts.Length)];
        var cover = await ArcaeaImageUtils.GetSongCover(_yukiClient,
            randomChart.SongId, randomChart.JacketOverride,
            (ArcaeaDifficulty)randomChart.RatingClass);

        GuessSessions[sessionId].Chart = randomChart;
        GuessSessions[sessionId].Cover = cover;
        GuessSessions[sessionId].IsReady = true;

        _logger.LogDebug("{SessionId} 启动了新的猜曲绘会话：{ModeName} => {SongNameEn} ({SongId})",
            sessionId, mode.GetName(), randomChart.NameEn, randomChart.SongId);

        // 超时揭晓答案
#pragma warning disable CS4014
        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            if (!GuessSessions.TryGetValue(sessionId, out var session)) return;
            if (session.Timestamp != timestamp) return;
            GuessSessions.Remove(sessionId);
            var set = await _service.SongDb.Packages
                .AsNoTracking()
                .FirstAsync(package => package.Set == session.Chart.Set);
            ctx.Bot.SendMessage(ctx.Message,
                new MessageBuilder()
                    .Text("时间到！揭晓答案——")
                    .Image(session.Cover)
                    .Text($"{session.Chart.NameEn} - {session.Chart.Artist}\n")
                    .Text($"({set.Name})"));
        });
#pragma warning restore CS4014

        var image = _service.ImageGenerator.Guess(cover, mode);

        return new MessageBuilder()
            .Text($"本轮题目 [{mode.GetName()}模式]：")
            .Image(image)
            .Text("30秒后揭晓答案~");
    }
}