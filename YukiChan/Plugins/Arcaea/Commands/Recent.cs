using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Responses;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models;
using YukiChan.Plugins.Arcaea.Models.Database;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("recent")]
    [Option("nya", "-n <nya:bool>")]
    [Option("dark", "-d <dark:bool>")]
    [Option("user", "-u <user:string>")]
    public async Task<MessageContent> OnRecent(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetOption<string>("user");

        try
        {
            AuaUserInfoContent userInfo;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var dbUser = await Global.YukiDb.GetArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId);
                if (dbUser is null)
                    return new MessageBuilder()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a best -u 名称或好友码 直接查询指定用户。");

                Logger.Info(
                    $"正在查询 {ctx.Message.Sender.Name}({ctx.Message.Sender.UserId}) -> {dbUser.ArcaeaName}({dbUser.ArcaeaId}) 的最近成绩...");

                userInfo = int.TryParse(dbUser.ArcaeaId, out var parsed)
                    ? await _auaClient.User.Info(parsed, 1, AuaReplyWith.All)
                    : await _auaClient.User.Info(dbUser.ArcaeaId, 1, AuaReplyWith.All);

                if (userInfo.RecentScore!.Length == 0)
                    return "该用户最近没有游玩过曲目呢...";
            }
            else
            {
                Logger.Info($"正在查询 {userArg} 的最近成绩...");
                userInfo = await _auaClient.User.Info(userArg, 1, AuaReplyWith.All);
            }

            var info = ArcaeaUser.FromAua(userInfo.AccountInfo);
            var record = ArcaeaRecord.FromAua(userInfo.RecentScore![0], userInfo.SongInfo![0]);

            Logger.Info($"正在为 {info.Name}({info.Id}) 生成最近成绩图片...");

            var pref = await Global.YukiDb.GetArcaeaUserPreferences(ctx.Bot.Platform, ctx.Message.Sender.UserId)
                       ?? new ArcaeaUserPreferences();
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");

            var image = await ArcaeaImageGenerator.Single(info, record, _auaClient, pref, Logger);

            return new MessageBuilder()
                .Text($"{info.Name} ({info.Potential})\n")
                .Image(ImageSegment.FromData(image));
        }
        catch (Exception e)
        {
            if (e is not AuaException)
                Logger.Error(e);
            return $"发生了奇怪的错误！({e.Message})";
        }
    }
}