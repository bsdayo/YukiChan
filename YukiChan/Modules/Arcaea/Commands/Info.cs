using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Core;
using YukiChan.Modules.Arcaea.Models;
using YukiChan.Utils;

// ReSharper disable CheckNamespace
namespace YukiChan.Modules.Arcaea;

public partial class ArcaeaModule
{
    [Command("Info",
        Command = "info",
        Shortcut = "查定数",
        Description = "查询曲目信息",
        Usage = "a info <曲目名称>",
        Example = "a info pragmatism")]
    public static async Task<MessageBuilder> Info(Bot bot, MessageStruct message, string body)
    {
        var allSubFlags = new[] { "details", "detail" };
        var (args, subFlags) = CommonUtils.ParseCommandBody(body, allSubFlags);

        if (args.Length == 0)
            return message.Reply("请输入要查询的曲目哦~");

        try
        {
            if (ArcaeaSongDatabase.Exists())
                Logger.Debug("arcsong.db Exists.");

            var songname = string.Join(' ', args);

            var song = ArcaeaSongDatabase.Exists()
                ? ArcaeaSongDatabase.FuzzySearchSong(songname)
                : ArcaeaSong.FromAua(await AuaClient.Song.Info(songname));

            if (song is null)
                return message.Reply("没有找到指定的曲目哦~");

            // details subflag
            if (subFlags.Contains("detail") || subFlags.Contains("details"))
                return await ConstructInfoDetails(bot, song);

            var songCover = await AuaClient.GetSongCover(song.SongId);

            var mb = new MessageBuilder().Image(songCover);

            if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
            {
                var songCoverByd = await AuaClient.GetSongCover(
                    song.SongId, song.Difficulties[3].JacketOverride, ArcaeaDifficulty.Beyond);
                mb.Image(songCoverByd);
            }

            mb
                .Text(song.Difficulties[2].NameEn + "\n")
                .Text($"({song.SetFriendly})");

            for (var i = 0; i < song.Difficulties.Length; i++)
            {
                var rating = song.Difficulties[i].Rating;
                mb.Text($"\n{(ArcaeaDifficulty)i} {rating.GetDifficulty()} [{((double)rating / 10).ToString("0.0")}]");
            }

            return mb;
        }
        catch (AuaException e)
        {
            Logger.Error(e);
            return message.Reply($"API 发生了错误呢... ({e.Status}) {e.Message}");
        }
        catch (YukiException e)
        {
            Logger.Error(e);
            return message.Reply(e.Message);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return message.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }

    private static async Task<MessageBuilder> ConstructInfoDetails(Bot bot, ArcaeaSong song)
    {
        var multiMsg = new MultiMsgChain();

        for (var i = 0; i < song.Difficulties.Length; i++)
        {
            var chart = song.Difficulties[i];

            var songCover = await AuaClient.GetSongCover(
                song.SongId, chart.JacketOverride, (ArcaeaDifficulty)i);

            multiMsg.AddMessage(new MessageStruct(bot.Uin, bot.Name,
                new MessageBuilder()
                    .Image(songCover)
                    .Text($"{chart.NameEn}\n")
                    .Text(
                        $"{(ArcaeaDifficulty)i} {chart.Rating.GetDifficulty()} [{((double)chart.Rating / 10).ToString("0.0")}]\n\n")
                    //
                    .Text($"BPM: {chart.Bpm}\n")
                    .Text($"物量: {chart.Note}\n")
                    .Text($"时长: {chart.Time / 60}分{chart.Time % 60}秒\n\n")
                    //
                    .Text($"曲师: {chart.Artist}\n")
                    .Text($"谱师: {chart.ChartDesigner}\n")
                    .Text($"曲绘设计: {chart.JacketDesigner}\n\n")
                    //
                    .Text($"需要下载: {(chart.RemoteDownload ? "是" : "否")}\n")
                    .Text($"世界模式解锁: {(chart.WorldUnlock ? "是" : "否")}\n\n")
                    //
                    .Text($"发布版本: {chart.Version}\n")
                    .Text(
                        $"发布时间: {DateTimeOffset.FromUnixTimeSeconds(chart.Date).LocalDateTime:yyyy.MM.dd HH:mm:ss}")
                    .Build()));
        }

        var msgb = new MessageBuilder(multiMsg);

        return msgb;
    }
}