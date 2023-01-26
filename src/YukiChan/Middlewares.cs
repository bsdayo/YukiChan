using Flandre.Framework.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using YukiChan.Client.Console;
using YukiChan.Shared.Data;
using YukiChan.Shared.Data.Console;
using YukiChan.Utils;

// ReSharper disable InconsistentNaming

namespace YukiChan;

public static class Middlewares
{
    public static void QQGroupWarnFilter(MiddlewareContext ctx, Action next)
    {
        next();

        if (ctx.Platform != "onebot") return;
        var isWarn = ctx.Response?.GetText().Contains("未找到指令");
        if (isWarn == null || !isWarn.Value) return;
        if (ctx.App.Services.GetRequiredService<IOptionsSnapshot<YukiOptions>>().Value
            .NoWarnQQGroups.Contains(ctx.ChannelId))
            ctx.Response = null;
    }

    public static void QQGuildFilter(MiddlewareContext ctx, Action next)
    {
        if (ctx.Platform == "qqguild"
            && !ctx.App.Services.GetRequiredService<IOptionsSnapshot<YukiOptions>>().Value
                .QQGuildAllowedChannels.Contains(ctx.ChannelId))
            return;

        next();
    }

    public static async Task CommandPrechecker(MiddlewareContext ctx, Action next)
    {
        if (ctx.Command is not null)
        {
            var resp = await ctx.App.Services.GetRequiredService<YukiConsoleClient>()
                .Root.Precheck(new PrecheckRequest
                {
                    Platform = ctx.Platform,
                    GuildId = ctx.GuildId,
                    UserId = ctx.UserId,
                    SelfId = ctx.SelfId,
                    Command = ctx.Command.CommandInfo.Command,
                    CommandText = ctx.Message.GetText()
                });

            if (!resp.Ok)
            {
                if (resp.Code != YukiErrorCode.UserGotBanned)
                    ctx.Response = ctx.ReplyServerError(resp);
                return;
            }

            if (!resp.Data.IsAssignee)
                return;
        }

        next();
    }
}