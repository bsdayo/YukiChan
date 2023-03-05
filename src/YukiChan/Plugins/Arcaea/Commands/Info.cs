using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Attributes;
using YukiChan.ImageGen.Utils;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Shared.Utils;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.info")]
    [StringShortcut("查定数", AllowArguments = true)]
    public async Task<MessageContent> OnInfo(MessageContext ctx,
        string[] songname,
        [Option(ShortName = 'n')] bool nya)
    {
        var song = await _service.SongDb.FuzzySearchSong(string.Join(' ', songname));

        // TODO: query from server

        if (song is null)
            return ctx.Reply("没有找到该曲目哦~");

        var cover = await ArcaeaImageUtils.GetSongCover(_yukiClient,
            song.SongId, nya: nya, logger: _logger);

        var mb = new MessageBuilder().Image(ImageSegment.FromData(cover));

        if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
        {
            var bydCover = await ArcaeaImageUtils.GetSongCover(_yukiClient,
                song.SongId, true, ArcaeaDifficulty.Beyond, nya, _logger);
            mb.Image(ImageSegment.FromData(bydCover));
        }

        mb
            .Text(song.Difficulties[2].NameEn + "\n")
            .Text($"({song.SetFriendly})");

        for (var i = 0; i < song.Difficulties.Length; i++)
        {
            var rating = song.Difficulties[i].Rating;
            mb.Text($"\n{(ArcaeaDifficulty)i} {rating.ToDisplayRating()} [{rating:N1}]");
        }

        return mb;
    }
}