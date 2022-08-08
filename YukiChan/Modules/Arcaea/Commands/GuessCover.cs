﻿using ArcaeaUnlimitedAPI.Lib.Models;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Modules.Arcaea.Images;
using YukiChan.Modules.Arcaea.Models;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public enum ArcaeaGuessMode
{
    Easy, // 简单
    Normal, // 正常
    Hard, // 困难
    Flash, // 闪照
    GrayScale, // 灰度
    Invert // 反色
}

public partial class ArcaeaModule
{
    private class ArcaeaGuessSession
    {
        public long Timestamp { get; set; }
        public ArcaeaGuessMode Mode { get; set; }
        public ArcaeaSongDbChart Chart { get; set; } = null!;
        public byte[] Cover { get; set; } = null!;
        public bool Inited { get; set; }
    }

    private static readonly Dictionary<uint, ArcaeaGuessSession> GuessSessions = new();

    [Command("GuessCover",
        Command = "guesscover",
        Shortcut = "猜曲绘",
        Description = "猜曲绘小游戏",
        Usage = "a guesscover [模式|rank]",
        Example = "猜曲绘 easy")]
    public static async Task<MessageBuilder> Guess(Bot bot, MessageStruct message, string body)
    {
        if (!Global.YukiConfig.Arcaea.EnableGuess)
            return message.Reply("本实例没有开启猜曲绘功能哦~");

        var args = CommonUtils.ParseCommandBody(body);

        try
        {
            if (args.Length == 0)
            {
                if (GuessSessions.ContainsKey(message.Receiver.Uin))
                    return message.Reply("当前猜曲绘游戏正在进行，请等待结束后再开始新游戏~");

                return await GetNewGuess(bot, message, ArcaeaGuessMode.Normal);
            }

            switch (args[0])
            {
                case "rank":
                case "排名":
                    ArcaeaGuessMode? mode = null;
                    if (args.Length >= 2)
                        mode = ArcaeaUtils.GetGuessMode(args[1]);
                    return GetGuessRank(message, mode ?? ArcaeaGuessMode.Normal, DateTime.Now);

                case "info":
                case "信息":
                    return GetGuessInfo(message, args.Length >= 2 && args[1] == "all");
            }

            if (GuessSessions.ContainsKey(message.Receiver.Uin))
            {
                var guessSongId = ArcaeaSongDatabase.FuzzySearchId(string.Join(" ", args));
                if (guessSongId is null)
                    return message.Reply("没有找到该曲目哦！");

                var session = GuessSessions[message.Receiver.Uin];

                if (!session.Inited)
                    return message.Reply("题目正在初始化中，请稍等...");

                // 判断是否猜对
                if (guessSongId == session.Chart.SongId)
                {
                    Global.YukiDb.AddArcaeaGuessCount(
                        message.Receiver.Uin, message.Receiver.Name,
                        message.Sender.Uin, message.Sender.Name,
                        session.Mode, true);
                    GuessSessions.Remove(message.Receiver.Uin);

                    return message.Reply("猜对啦！")
                        .Image(session.Cover)
                        .Text($"{session.Chart.NameEn} - {session.Chart.Artist}\n")
                        .Text($"({ArcaeaSongDatabase.GetPackageBySet(session.Chart.Set)!.Name})");
                }

                Global.YukiDb.AddArcaeaGuessCount(
                    message.Receiver.Uin, message.Receiver.Name,
                    message.Sender.Uin, message.Sender.Name,
                    session.Mode, false);
                return message.Reply("猜错啦！");
            }

            {
                var mode = ArcaeaUtils.GetGuessMode(args[0]);

                if (mode is null)
                    return message.Reply("当前群内没有正在进行的猜曲绘游戏，可以发送 #a guess 来开启新游戏哦~");

                return await GetNewGuess(bot, message, mode.Value);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private static async Task<MessageBuilder> GetNewGuess(Bot bot, MessageStruct message, ArcaeaGuessMode mode)
    {
        var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

        // 占位
        GuessSessions.TryAdd(message.Receiver.Uin,
            new ArcaeaGuessSession { Timestamp = timestamp, Mode = mode });

        var allCharts = ArcaeaSongDatabase.GetAllCharts()
            .Where(chart => chart.RatingClass == (int)ArcaeaDifficulty.Future)
            .ToArray();
        var randomChart = allCharts[new Random().Next(allCharts.Length)];
        var cover = await AuaClient.GetSongCover(randomChart.SongId, randomChart.JacketOverride,
            (ArcaeaDifficulty)randomChart.RatingClass);

        GuessSessions[message.Receiver.Uin].Chart = randomChart;
        GuessSessions[message.Receiver.Uin].Cover = cover;
        GuessSessions[message.Receiver.Uin].Inited = true;
        Logger.Debug($"新的猜曲绘会话: {mode} 模式   {randomChart.SongId} -> {randomChart.NameEn}");

        // 超时揭晓答案
#pragma warning disable CS4014
        Task.Run(() =>
        {
            Task.Delay(30000).Wait();
            if (GuessSessions.ContainsKey(message.Receiver.Uin))
            {
                var session = GuessSessions[message.Receiver.Uin];
                if (session.Timestamp != timestamp) return;
                bot.Send(message,
                    new MessageBuilder()
                        .Text("时间到！揭晓答案——")
                        .Image(session.Cover)
                        .Text($"{session.Chart.NameEn} - {session.Chart.Artist}\n")
                        .Text($"({ArcaeaSongDatabase.GetPackageBySet(session.Chart.Set)!.Name})"));
                GuessSessions.Remove(message.Receiver.Uin);
            }
        });
#pragma warning restore CS4014

        var image = ArcaeaGuessImageGenerator.Generate(cover, mode);

        if (mode == ArcaeaGuessMode.Flash)
        {
            await bot.Send(message, new MessageBuilder()
                .Text($"本轮题目 ({mode.GetName()}模式)！\n")
                .Text("抓紧时间哦~30秒后揭晓答案！"));
            return new MessageBuilder()
                .Add(FlashImageChain.CreateFromImageChain(ImageChain.Create(image)));
        }

        return new MessageBuilder()
            .Text($"本轮题目 ({mode.GetName()}模式)：")
            .Image(image)
            .Text("30秒后揭晓答案~");
    }

    private static MessageBuilder GetGuessRank(MessageStruct message, ArcaeaGuessMode mode, DateTime date)
    {
        var users = Global.YukiDb
            .GetArcaeaGuessUsersOfDate(message.Receiver.Uin, date)
            .FindAll(u => !double.IsNaN(mode switch
            {
                ArcaeaGuessMode.Easy => u.EasyCorrectRate,
                ArcaeaGuessMode.Normal => u.NormalCorrectRate,
                ArcaeaGuessMode.Hard => u.HardCorrectRate,
                ArcaeaGuessMode.Flash => u.FlashCorrectRate,
                ArcaeaGuessMode.GrayScale => u.GrayScaleCorrectRate,
                ArcaeaGuessMode.Invert => u.InvertCorrectRate,
                _ => double.NaN
            }));

        users.Sort((userA, userB) =>
        {
            return mode switch
            {
                ArcaeaGuessMode.Easy => (int)(userB.EasyCorrectRate * 10000 - userA.EasyCorrectRate * 10000),
                ArcaeaGuessMode.Normal => (int)(userB.NormalCorrectRate * 10000 - userA.NormalCorrectRate * 10000),
                ArcaeaGuessMode.Hard => (int)(userB.HardCorrectRate * 10000 - userA.HardCorrectRate * 10000),
                ArcaeaGuessMode.Flash => (int)(userB.FlashCorrectRate * 10000 - userA.FlashCorrectRate * 10000),
                ArcaeaGuessMode.GrayScale => (int)(userB.GrayScaleCorrectRate * 10000 -
                                                   userA.GrayScaleCorrectRate * 10000),
                ArcaeaGuessMode.Invert => (int)(userB.InvertCorrectRate * 10000 - userA.InvertCorrectRate * 10000),
                _ => 0
            };
        });

        var mb = message.Reply($"今日猜曲绘排名 ({mode.GetName()}模式)");
        for (int i = 0, j = 0; i < users.Count; i++)
        {
            if (j >= 5) break;
            var user = users[i];

            int correctCount = 0, wrongCount = 0;
            double rate = 0;

            switch (mode)
            {
                case ArcaeaGuessMode.Easy:
                    correctCount = user.EasyCorrectCount;
                    wrongCount = user.EasyWrongCount;
                    rate = user.EasyCorrectRate;
                    break;
                case ArcaeaGuessMode.Normal:
                    correctCount = user.NormalCorrectCount;
                    wrongCount = user.NormalWrongCount;
                    rate = user.NormalCorrectRate;
                    break;
                case ArcaeaGuessMode.Hard:
                    correctCount = user.HardCorrectCount;
                    wrongCount = user.HardWrongCount;
                    rate = user.HardCorrectRate;
                    break;
                case ArcaeaGuessMode.Flash:
                    correctCount = user.FlashCorrectCount;
                    wrongCount = user.FlashWrongCount;
                    rate = user.FlashCorrectRate;
                    break;
                case ArcaeaGuessMode.GrayScale:
                    correctCount = user.GrayScaleCorrectCount;
                    wrongCount = user.GrayScaleWrongCount;
                    rate = user.GrayScaleCorrectRate;
                    break;
                case ArcaeaGuessMode.Invert:
                    correctCount = user.InvertCorrectCount;
                    wrongCount = user.InvertWrongCount;
                    rate = user.InvertCorrectRate;
                    break;
            }

            mb.Text($"\n{j + 1}. {user.UserName}   {correctCount}√ {wrongCount}×  {rate:P2}");
            j++;
        }

        return mb;
    }

    private static MessageBuilder GetGuessInfo(MessageStruct message, bool all = false)
    {
        var user = new List<ArcaeaGuessUser>();
        if (all)
        {
            user.AddRange(Global.YukiDb.GetArcaeaGuessUserOfAllTime(message.Sender.Uin));
        }
        else
        {
            var u = Global.YukiDb.GetArcaeaGuessUserOfDate(message.Sender.Uin, DateTime.Today);
            if (u is not null) user.Add(u);
        }


        if (!user.Any())
            return message.Reply("您还没有过用户数据哦，请猜一次曲绘再试吧~");

        var easyRate = user.Sum(u => u.EasyCorrectRate) / user.Count;
        var normalRate = user.Sum(u => u.NormalCorrectRate) / user.Count;
        var hardRate = user.Sum(u => u.HardCorrectRate) / user.Count;
        var flashRate = user.Sum(u => u.FlashCorrectRate) / user.Count;
        var grayScaleRate = user.Sum(u => u.GrayScaleCorrectRate) / user.Count;
        var invertRate = user.Sum(u => u.InvertCorrectRate) / user.Count;

        return message.Reply($"您的{(all ? "全局" : "今日")}猜曲绘信息\n")
            .Text(
                $"简单 / {user.Sum(u => u.EasyCorrectCount)}√  {user.Sum(u => u.EasyWrongCount)}×  {(double.IsNaN(easyRate) ? 0d : easyRate):P2}\n")
            .Text(
                $"正常 / {user.Sum(u => u.NormalCorrectCount)}√  {user.Sum(u => u.NormalWrongCount)}×  {(double.IsNaN(normalRate) ? 0d : normalRate):P2}\n")
            .Text(
                $"困难 / {user.Sum(u => u.HardCorrectCount)}√  {user.Sum(u => u.HardWrongCount)}×  {(double.IsNaN(hardRate) ? 0d : hardRate):P2}\n")
            .Text(
                $"闪照 / {user.Sum(u => u.FlashCorrectCount)}√  {user.Sum(u => u.FlashWrongCount)}×  {(double.IsNaN(flashRate) ? 0d : flashRate):P2}\n")
            .Text(
                $"灰度 / {user.Sum(u => u.GrayScaleCorrectCount)}√  {user.Sum(u => u.GrayScaleWrongCount)}×  {(double.IsNaN(grayScaleRate) ? 0d : grayScaleRate):P2}\n")
            .Text(
                $"反色 / {user.Sum(u => u.InvertCorrectCount)}√  {user.Sum(u => u.InvertWrongCount)}×  {(double.IsNaN(invertRate) ? 0d : invertRate):P2}");
    }
}