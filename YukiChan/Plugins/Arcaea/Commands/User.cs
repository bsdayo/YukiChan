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
    [Option("nya", "-n <:bool>")]
    [Option("dark", "-d <:bool>")]
    //
    [Option("year", "-y <:bool>")]
    [Option("season", "-s <:bool>")]
    [Option("month", "-m <:bool>")]
    [Option("week", "-w <:bool>")]
    //
    [Shortcut("查用户")]
    public async Task<MessageContent> OnUser(MessageContext ctx, ParsedArgs args)
    {
        var userArg = args.GetArgument<string>("user");
        var yearArg = args.GetOption<bool>("year");
        var seasonArg = args.GetOption<bool>("season");
        var monthArg = args.GetOption<bool>("month");
        var weekArg = args.GetOption<bool>("week");
        string? userId = null;

        var lastDays = 0;
        if (yearArg) lastDays = 365;
        if (seasonArg) lastDays = 90;
        if (monthArg) lastDays = 30;
        if (weekArg) lastDays = 7;

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
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");
            var image = await ArcaeaImageGenerator.User(userInfo, pref, _auaClient, lastDays, Logger);
            Logger.Debug("Generation done.");
            return ctx.Reply().Image(image);
        }
        catch (Exception e)
        {
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}