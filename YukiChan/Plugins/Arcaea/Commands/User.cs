using ArcaeaUnlimitedAPI.Lib.Models;
using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using YukiChan.Plugins.Arcaea.Images;
using YukiChan.Plugins.Arcaea.Models.Database;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.user [user:string]")]
    [Shortcut("查用户")]
    public async Task<MessageContent> OnUser(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetArgument<string>("user");
        string? userId = null;

        try
        {
            if (string.IsNullOrWhiteSpace(userArg))
            {
                var user = await Global.YukiDb.GetArcaeaUser(ctx.Bot.Platform, ctx.Message.Sender.UserId);
                if (user is null)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a user 名称或好友码 直接查询指定用户。");
                userId = user.ArcaeaId;
            }

            var userInfo = await _auaClient.User.Info(userId ?? userArg, 1, AuaReplyWith.All);
            var pref = await Global.YukiDb.GetArcaeaUserPreferences(ctx.Bot.Platform, ctx.Message.Sender.UserId)
                       ?? new ArcaeaUserPreferences();
            var image = await ArcaeaImageGenerator.User(userInfo, pref, _auaClient, Logger);
            return ctx.Reply().Image(image);
        }
        catch (Exception e)
        {
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}