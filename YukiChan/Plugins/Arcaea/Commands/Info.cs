using ArcaeaUnlimitedAPI.Lib.Models;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("info <songname: string>")]
    [Option("nya", "-n <:bool>")]
    public async Task<MessageContent> OnInfo(MessageContext ctx, ParsedArgs args)
    {
        var nya = args.GetOption<bool>("nya");

        var song = await ArcaeaSongDatabase.FuzzySearchSong(args.GetArgument<string>("songname"));

        if (song is null)
            return "没有找到该曲目哦~";

        var cover = await _auaClient.GetSongCover(song.SongId, nya: nya, logger: Logger);

        var mb = new MessageBuilder().Image(ImageSegment.FromData(cover));

        if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
        {
            var bydCover = await _auaClient.GetSongCover(song.SongId, true,
                ArcaeaDifficulty.Beyond, nya, Logger);
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