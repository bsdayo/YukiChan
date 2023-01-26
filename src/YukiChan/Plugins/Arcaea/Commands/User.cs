using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Data;
using YukiChan.Shared.Models.Arcaea;
using YukiChan.Utils;

// ReSharper disable CheckNamespace

namespace YukiChan.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.user [user:string]")]
    [Option("nya", "-n <:bool>")]
    [Option("dark", "-d <:bool>")]
    [Option("smooth", "-S <:bool>")]
    //
    [Option("year", "-y <:bool>")]
    [Option("season", "-s <:bool>")]
    [Option("month", "-m <:bool>")]
    [Option("week", "-w <:bool>")]
    //
    [Shortcut("查用户")]
    public async Task<MessageContent> OnUser(CommandContext ctx, ParsedArgs args)
    {
        var userArg = args.GetArgument<string>("user");
        var yearArg = args.GetOption<bool>("year");
        var seasonArg = args.GetOption<bool>("season");
        var monthArg = args.GetOption<bool>("month");
        var weekArg = args.GetOption<bool>("week");
        var smooth = args.GetOption<bool>("smooth");
        string? userId = null;

        var lastDays = 1_000_000_000;
        if (yearArg) lastDays = 365;
        if (seasonArg) lastDays = 90;
        if (monthArg) lastDays = 30;
        if (weekArg) lastDays = 7;

        try
        {
            if (string.IsNullOrWhiteSpace(userArg))
            {
                var userResp = await _yukiClient.Arcaea.GetUser(ctx.Platform, ctx.UserId);
                if (userResp.Code == YukiErrorCode.Arcaea_NotBound)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a user 名称或好友码 直接查询指定用户。");
                if (!userResp.Ok) return ctx.ReplyServerError(userResp);
                userId = userResp.Data.ArcaeaId;
            }

            var recentResp = await _yukiClient.Arcaea.GetRecent(userId ?? userArg);
            if (!recentResp.Ok) return ctx.ReplyServerError(recentResp);
            var prefResp = await _yukiClient.Arcaea.GetPreferences(ctx.Platform, ctx.UserId);
            var pref = prefResp.Ok ? prefResp.Data.Preferences : new ArcaeaUserPreferences();
            pref.Dark = pref.Dark || args.GetOption<bool>("dark");
            pref.Nya = pref.Nya || args.GetOption<bool>("nya");
            var image = await _service.ImageGenerator.User(recentResp.Data.User, recentResp.Data.RecentRecord,
                pref, _yukiClient, lastDays, smooth,
                _logger);
            _logger.LogDebug("Generation done.");
            return ctx.Reply().Image(image);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred in a.user");
            return ctx.Reply($"发生了奇怪的错误！({e.Message})");
        }
    }
}