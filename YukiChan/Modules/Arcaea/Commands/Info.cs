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
        Description = "查询曲目信息",
        Usage = "a info <曲目名称>",
        Example = "a info pragmatism")]
    public static async Task<MessageBuilder> Info(Bot bot, MessageStruct message, string body)
    {
        ArcaeaSong? song;
        byte[] songCover;
        byte[]? songCoverByd = null;

        try
        {
            if (ArcaeaSongDatabase.Exists())
                BotLogger.Debug("arcsong.db Exists.");

            song = ArcaeaSongDatabase.Exists()
                ? ArcaeaSongDatabase.FuzzySearchSong(body)
                : ArcaeaSong.FromAua(await AuaClient.Song.Info(body));

            if (song is null)
                throw new YukiException("");

            songCover = await AuaClient.Assets.Song(song.SongId, AuaSongQueryType.SongId);
            if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
                songCoverByd =
                    await AuaClient.Assets.Song(song.SongId, AuaSongQueryType.SongId, ArcaeaDifficulty.Beyond);
        }
        catch (AuaException e)
        {
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text($"API 发生了错误呢... ({e.Status}) {e.Message}");
        }
        catch (YukiException e)
        {
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text(e.Message);
        }
        catch (Exception e)
        {
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text($"发生了奇怪的错误！({e.Message})");
        }

        var mb = new MessageBuilder().Image(songCover);

        if (songCoverByd is not null)
            mb.Image(songCoverByd);

        mb
            .Text(song.Difficulties[2].NameEn + "\n")
            .Text($"({song.SetFriendly})");

        for (var i = 0; i < song.Difficulties.Length; i++)
        {
            var rating = song.Difficulties[i].Rating;
            mb.Text($"\n{(ArcaeaDifficulty)i} {rating.GetDifficulty()} [{(double)rating / 10}]");
        }

        return mb;
    }
}