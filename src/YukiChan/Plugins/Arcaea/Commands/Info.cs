using ArcaeaUnlimitedAPI.Lib.Models;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.info <songname:text>")]
    [Option("nya", "-n <:bool>")]
    [Shortcut("查定数")]
    public async Task<MessageContent> OnInfo(MessageContext ctx, ParsedArgs args)
    {
        var nya = args.GetOption<bool>("nya");

        var song = await ArcaeaSongDatabase.FuzzySearchSong(args.GetArgument<string>("songname"));

        if (song is null)
            return ctx.Reply("没有找到该曲目哦~");

        var cover = await _service.AuaClient.GetSongCover(song.SongId, nya: nya, logger: _logger);

        var mb = new MessageBuilder().Image(ImageSegment.FromData(cover));

        if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
        {
            var bydCover = await _service.AuaClient.GetSongCover(song.SongId, true,
                ArcaeaDifficulty.Beyond, nya, _logger);
            mb.Image(ImageSegment.FromData(bydCover));
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
}