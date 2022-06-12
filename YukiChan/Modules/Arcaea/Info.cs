using System;
using System.Threading.Tasks;
using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Responses;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Konata.Core;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using YukiChan.Attributes;

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
        AuaSongInfoContent songInfo;
        byte[] songCover;

        try
        {
            songInfo = await _auaClient.Song.Info(body);
            songCover = await _auaClient.Assets.Song(body);
        }
        catch (AuaException auae)
        {
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text($"API 发生了错误呢... ({auae.Status}) {auae.Message}");
        }
        catch (Exception e)
        {
            return new MessageBuilder()
                .Add(ReplyChain.Create(message))
                .Text($"发生了奇怪的错误！({e.Message})");
        }

        var mb = new MessageBuilder()
            .Image(songCover)
            .Text(songInfo.Difficulties[2].NameEn + "\n")
            .Text($"({songInfo.Difficulties[2].SetFriendly})");

        for (var i = 0; i < songInfo.Difficulties.Length; i++)
        {
            var rating = songInfo.Difficulties[i].Rating;
            mb.Text($"\n{(ArcaeaDifficulty)i} {rating.GetDifficulty()} [{(double)rating / 10}]");
        }

        return mb;
    }
}