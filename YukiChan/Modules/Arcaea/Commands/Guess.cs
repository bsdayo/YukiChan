using ArcaeaUnlimitedAPI.Lib.Models;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Modules.Arcaea.Images;
using YukiChan.Modules.Arcaea.Models;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public enum ArcaeaGuessMode
{
    Easy,
    Normal,
    Hard
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

            if (args[0] == "rank")
            {
                ArcaeaGuessMode? mode = null;
                if (args.Length >= 2)
                    mode = ArcaeaUtils.GetGuessMode(args[1]);
                return GetGuessRank(message, mode ?? ArcaeaGuessMode.Normal, DateTime.Now);
            }

            if (GuessSessions.ContainsKey(message.Receiver.Uin))
            {
                var guessSongId = ArcaeaSongDatabase.FuzzySearchId(string.Join(" ", args));
                var session = GuessSessions[message.Receiver.Uin];

                if (!session.Inited)
                    return message.Reply("题目正在初始化中，请稍等...");

                // 判断是否猜对
                if (guessSongId == session.Chart.SongId)
                {
                    Global.YukiDb.AddArcaeaGuessCount(
                        message.Receiver.Uin, message.Sender.Uin, message.Sender.Name, session.Mode, true);
                    GuessSessions.Remove(message.Receiver.Uin);

                    return message.Reply("猜对啦！")
                        .Image(session.Cover)
                        .Text($"{session.Chart.NameEn} - {session.Chart.Artist}\n")
                        .Text($"({ArcaeaSongDatabase.GetPackageBySet(session.Chart.Set)!.Name})");
                }

                Global.YukiDb.AddArcaeaGuessCount(
                    message.Receiver.Uin, message.Sender.Uin, message.Sender.Name, session.Mode, false);
                return message.Reply("猜错啦！");
            }
            else
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
                bot.SendReply(message,
                    new MessageBuilder()
                        .Text("时间到！揭晓答案——")
                        .Image(session.Cover)
                        .Text($"{session.Chart.NameEn} - {session.Chart.Artist}\n")
                        .Text($"({ArcaeaSongDatabase.GetPackageBySet(session.Chart.Set)!.Name})"));
                GuessSessions.Remove(message.Receiver.Uin);
            }
        });
#pragma warning restore CS4014

        var image = ArcaeaGuessImageGenerator.Normal(cover, mode);
        return new MessageBuilder()
            .Text($"本轮题目 ({mode} 模式)：")
            .Image(image)
            .Text("30秒后揭晓答案~");
    }

    private static MessageBuilder GetGuessRank(MessageStruct message, ArcaeaGuessMode mode, DateTime date)
    {
        var users = Global.YukiDb.GetArcaeaGuessUsersOfDate(date);
        users.Sort((userA, userB) =>
        {
            return mode switch
            {
                ArcaeaGuessMode.Easy => (int)userB.EasyCorrectRate - (int)userA.EasyCorrectRate,
                ArcaeaGuessMode.Normal => (int)userB.NormalCorrectRate - (int)userA.NormalCorrectRate,
                ArcaeaGuessMode.Hard => (int)userB.HardCorrectRate - (int)userA.HardCorrectRate,
                _ => 0
            };
        });

        var mb = message.Reply($"今日猜曲绘排名 ({mode} 模式)");
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
            }

            if (double.IsNaN(rate))
                continue;
            
            mb.Text($"\n{j + 1}. {user.UserName}   {correctCount}√ {wrongCount}×  {rate:P2}");
            j++;
        }

        return mb;
    }
}