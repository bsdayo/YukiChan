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
        var args = CommonUtils.ParseCommandBody(body);

        if (args.Length == 0)
            return CommonUtils.ReplyMessage(message)
                .Text("请输入要查询的曲目哦~");

        byte[]? songCoverByd = null;

        try
        {
            if (ArcaeaSongDatabase.Exists())
                BotLogger.Debug("arcsong.db Exists.");

            var song = ArcaeaSongDatabase.Exists()
                ? ArcaeaSongDatabase.FuzzySearchSong(args.Length > 1 ? string.Join(' ', args[..^1]) : args[0])
                : ArcaeaSong.FromAua(
                    await AuaClient.Song.Info(args.Length > 1 ? string.Join(' ', args[..^1]) : args[0]));

            if (song is null)
                throw new YukiException("没有找到指定的曲目哦~");

            var songCover = await CacheManager.GetBytes("Arcaea", "Song", $"{song.SongId}.jpg");
            if (songCover is null)
            {
                songCover = await AuaClient.Assets.Song(song.SongId, AuaSongQueryType.SongId);
                await CacheManager.SaveBytes(songCover, "Arcaea", "Song", $"{song.SongId}.jpg");
            }

            if (args.Length >= 2 && (args[^1] == "detail" || args[^1] == "details"))
            {
                var multiMsg = new MultiMsgChain();

                for (var i = 0; i < song.Difficulties.Length; i++)
                {
                    byte[]? songCoverOverride = null;
                    var chart = song.Difficulties[i];

                    if (chart.JacketOverride)
                    {
                        songCoverOverride = await CacheManager.GetBytes(
                            "Arcaea", "Song",
                            $"{song.SongId}-{((ArcaeaDifficulty)chart.RatingClass).ToString().ToLower()}.jpg");

                        if (songCoverOverride is null)
                        {
                            songCoverOverride = await AuaClient.Assets.Song(song.SongId, AuaSongQueryType.SongId,
                                (ArcaeaDifficulty)chart.RatingClass);

                            await CacheManager.SaveBytes(songCoverOverride,
                                "Arcaea", "Song",
                                $"{song.SongId}-{((ArcaeaDifficulty)chart.RatingClass).ToString().ToLower()}.jpg");
                        }
                    }


                    multiMsg.AddMessage(new MessageStruct(bot.Uin, bot.Name,
                        new MessageBuilder()
                            .Image(songCoverOverride ?? songCover)
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

                // if (File.Exists($"Assets/Arcaea/AudioPreview/{song.SongId}.ogg"))
                //     msgb.Record($"Assets/Arcaea/AudioPreview/{song.SongId}.ogg");
                // if (File.Exists($"Assets/Arcaea/AudioPreview/{song.SongId}-beyond.ogg"))
                //     msgb.Record($"Assets/Arcaea/AudioPreview/{song.SongId}-beyond.ogg");

                return msgb;
            }

            if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
                songCoverByd =
                    await AuaClient.Assets.Song(song.SongId, AuaSongQueryType.SongId, ArcaeaDifficulty.Beyond);

            var mb = new MessageBuilder().Image(songCover);

            if (songCoverByd is not null)
                mb.Image(songCoverByd);

            mb
                .Text(song.Difficulties[2].NameEn + "\n")
                .Text($"({song.SetFriendly})");

            for (var i = 0; i < song.Difficulties.Length; i++)
            {
                var rating = song.Difficulties[i].Rating;
                mb.Text($"\n{(ArcaeaDifficulty)i} {rating.GetDifficulty()} [{((double)rating / 10).ToString("0.0")}]");
            }

            // if (File.Exists($"Assets/Arcaea/AudioPreview/{song.SongId}.ogg"))
            //     mb.Record($"Assets/Arcaea/AudioPreview/{song.SongId}.ogg");

            return mb;
        }
        catch (AuaException e)
        {
            BotLogger.Error(e);
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text($"API 发生了错误呢... ({e.Status}) {e.Message}");
        }
        catch (YukiException e)
        {
            BotLogger.Error(e);
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text(e.Message);
        }
        catch (Exception e)
        {
            BotLogger.Error(e);
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text($"发生了奇怪的错误！({e.Message})");
        }
    }
}