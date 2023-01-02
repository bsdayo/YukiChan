using ArcaeaUnlimitedAPI.Lib.Models;
using ArcaeaUnlimitedAPI.Lib.Responses;
using ArcaeaUnlimitedAPI.Lib.Utils;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Arcaea.Factories;
using YukiChan.Shared.Database.Models.Arcaea;
using YukiChan.Shared.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.recent")]
    [Option("nya", "-n <nya:bool>")]
    [Option("dark", "-d <dark:bool>")]
    [Option("user", "-u <user:string>")]
    [Shortcut("查最近")]
    [Alias("a")]
    public async Task<MessageContent> OnRecent(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetOption<string>("user");

        try
        {
            AuaUserInfoContent userInfo;

            if (string.IsNullOrEmpty(userArg)) // 未提供用户名/好友码
            {
                var dbUser = await _database.GetArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId);
                if (dbUser is null)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a best -u 名称或好友码 直接查询指定用户。");

                _logger.LogInformation(
                    "正在查询 {UserName}({UserId}) -> {ArcaeaName}({ArcaeaId}) 的最近成绩...",
                    ctx.Message.Sender.Name, ctx.Message.Sender.UserId,
                    dbUser.ArcaeaName, dbUser.ArcaeaId);

                userInfo = int.TryParse(dbUser.ArcaeaId, out var parsed)
                    ? await _service.AuaClient.User.Info(parsed, 1, AuaReplyWith.All)
                    : await _service.AuaClient.User.Info(dbUser.ArcaeaId, 1, AuaReplyWith.All);

                if (userInfo.RecentScore!.Length == 0)
                    return ctx.Reply("该用户最近没有游玩过曲目呢...");
            }
            else
            {
                _logger.LogInformation("正在查询 {ArcaeaName} 的最近成绩...", userArg);
                userInfo = await _service.AuaClient.User.Info(userArg, 1, AuaReplyWith.All);
            }

            var info = ArcaeaUserFactory.FromAua(userInfo.AccountInfo);
            var record = ArcaeaRecordFactory.FromAua(userInfo.RecentScore![0], userInfo.SongInfo![0]);

            _logger.LogInformation("正在为 {ArcaeaName}({ArcaeaId}) 生成最近成绩图片...", info.Name, info.Code);

            var pref = await _database.GetArcaeaUserPreferences(ctx.Bot.Platform, ctx.Message.Sender.UserId)
                       ?? new ArcaeaUserPreferences();
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");

            var image = await _service.ImageGenerator.SingleV1(info, record, _service.AuaClient, pref, _logger);

            return ctx.Reply()
                .Text($"{info.Name} ({info.Potential})\n")
                .Image(ImageSegment.FromData(image));
        }
        catch (AuaException e)
        {
            var errMsg = AuaErrorStatus.GetMessage(e.Status, e.Message);
            return ctx.Reply(errMsg);
        }
        catch (Exception e)
        {
            if (e is not AuaException)
                _logger.LogError(e);
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}