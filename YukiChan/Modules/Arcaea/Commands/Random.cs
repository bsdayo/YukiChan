using ArcaeaUnlimitedAPI.Lib.Models;
using Konata.Core;
using Konata.Core.Message;
using YukiChan.Core;
using YukiChan.Modules.Arcaea.Models;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("Random",
        Command = "random",
        Shortcut = "随机曲目",
        Description = "随即推荐曲目",
        Usage = "a random [最低定数] [最高定数]",
        Example = "a random 9.2 10+")]
    public static async Task<MessageBuilder> Random(Bot bot, MessageStruct message, string body)
    {
        var args = CommonUtils.ParseCommandBody(body);
        int start, end;
        var allCharts = ArcaeaSongDatabase.GetAllCharts();

        try
        {
            switch (args.Length)
            {
                // 不提供参数，全曲 Future 难度随机
                default:
                    return await ConstructRandomReply(message, allCharts
                        .Where(chart => chart.RatingClass == (int)ArcaeaDifficulty.Future)
                        .ToArray());

                // 提供定数范围
                case 1:
                    start = ArcaeaUtils.GetRatingRange(args[0]).Start;
                    end = ArcaeaUtils.GetRatingRange(args[0]).End;

                    if (start == -1 || end == -1)
                        return message.Reply("输入了错误的定数呢...");
                    if (start is < 0 or > 120 || end is < 0 or > 120)
                        return message.Reply("定数超出范围啦！");

                    return await ConstructRandomReply(message, allCharts
                        .Where(chart => chart.Rating >= start && chart.Rating <= end)
                        .ToArray());

                // 提供最低和最高定数
                case 2:
                    start = ArcaeaUtils.GetRatingRange(args[0]).Start;
                    end = ArcaeaUtils.GetRatingRange(args[1]).End;

                    if (start == -1 || end == -1)
                        return message.Reply("输入了错误的定数呢...");
                    if (start > end)
                        return message.Reply("最低定数比最高定数大诶...");
                    if (start is < 0 or > 120 || end is < 0 or > 120)
                        return message.Reply("定数超出范围啦！");

                    return await ConstructRandomReply(message, allCharts
                        .Where(chart => chart.Rating >= start && chart.Rating <= end)
                        .ToArray());
            }
        }
        catch (YukiException e)
        {
            BotLogger.Error(e);
            return message.Reply(e.Message);
        }
        catch (Exception e)
        {
            BotLogger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private static async Task<MessageBuilder> ConstructRandomReply(MessageStruct message, ArcaeaSongDbChart[] allCharts)
    {
        if (allCharts.Length == 0)
            return message.Reply("该定数范围内没有曲目哦~");

        var chart = allCharts[new Random().Next(allCharts.Length)];

        byte[]? songCover;

        if (chart.JacketOverride)
        {
            songCover = await CacheManager.GetBytes(
                "Arcaea", "Song",
                $"{chart.SongId}-{((ArcaeaDifficulty)chart.RatingClass).ToString().ToLower()}.jpg");

            if (songCover is null)
            {
                songCover = await AuaClient.Assets.Song(chart.SongId, AuaSongQueryType.SongId,
                    (ArcaeaDifficulty)chart.RatingClass);

                await CacheManager.SaveBytes(songCover,
                    "Arcaea", "Song",
                    $"{chart.SongId}-{((ArcaeaDifficulty)chart.RatingClass).ToString().ToLower()}.jpg");
            }
        }
        else
        {
            songCover = await CacheManager.GetBytes("Arcaea", "Song", $"{chart.SongId}.jpg");
            if (songCover is null)
            {
                songCover = await AuaClient.Assets.Song(chart.SongId, AuaSongQueryType.SongId);
                await CacheManager.SaveBytes(songCover, "Arcaea", "Song", $"{chart.SongId}.jpg");
            }
        }


        return message.Reply()
            .Text("随机推荐曲目：\n")
            .Image(songCover)
            .Text($"{chart.NameEn}\n")
            .Text($"({ArcaeaSongDatabase.GetPackageBySet(chart.Set)!.Name})\n")
            .Text(
                $"{(ArcaeaDifficulty)chart.RatingClass} {chart.Rating.GetDifficulty()} [{((double)chart.Rating / 10).ToString("0.0")}]");
    }
}