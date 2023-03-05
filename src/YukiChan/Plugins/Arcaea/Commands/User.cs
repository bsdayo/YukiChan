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
    [Command("a.user")]
    [StringShortcut("查用户", AllowArguments = true)]
    public async Task<MessageContent> OnUser(CommandContext ctx,
        [Option(ShortName = 'n')] bool nya,
        [Option(ShortName = 'd')] bool dark,
        [Option(ShortName = 'S')] bool smooth,
        //
        [Option(ShortName = 'y')] bool year,
        [Option(ShortName = 's')] bool season,
        [Option(ShortName = 'm')] bool month,
        [Option(ShortName = 'w')] bool week,
        //
        string user = "")
    {
        string? userId = null;

        var lastDays = 1_000_000_000;
        if (year) lastDays = 365;
        if (season) lastDays = 90;
        if (month) lastDays = 30;
        if (week) lastDays = 7;

        try
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                var userResp = await _yukiClient.Arcaea.GetUser(ctx.Platform, ctx.UserId);
                if (userResp.Code == YukiErrorCode.Arcaea_NotBound)
                    return ctx.Reply()
                        .Text("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n")
                        .Text("你也可以使用 /a user 名称或好友码 直接查询指定用户。");
                if (!userResp.Ok) return ctx.ReplyServerError(userResp);
                userId = userResp.Data.ArcaeaId;
            }

            var recentResp = await _yukiClient.Arcaea.GetRecent(userId ?? user);
            if (!recentResp.Ok) return ctx.ReplyServerError(recentResp);

            var prefResp = await _yukiClient.Arcaea.GetPreferences(ctx.Platform, ctx.UserId);
            var pref = prefResp.Ok ? prefResp.Data.Preferences : new ArcaeaUserPreferences();

            pref.Nya = pref.Nya || nya;
            pref.Dark = pref.Dark || dark;

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