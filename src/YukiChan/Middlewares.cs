using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YukiChan.Shared.Database;

namespace YukiChan;

public static class Middlewares
{
    public static void QqGroupWarnFilter(MiddlewareContext ctx, Action next)
    {
        next();

        if (ctx.Platform != "onebot") return;
        var isWarn = ctx.Response?.GetText().Contains("未找到指令");
        if (isWarn == null || !isWarn.Value) return;
        if (ctx.App.Services.GetRequiredService<YukiConfig>().NoWarnQqGroups.Contains(ctx.ChannelId))
            ctx.Response = null;
    }

    public static void QqGuildFilter(MiddlewareContext ctx, Action next)
    {
        if (ctx.Platform == "qqguild"
            && !ctx.App.Services.GetRequiredService<YukiConfig>()
                .QqGuildAllowedChannels.Contains(ctx.ChannelId))
            return;

        next();
    }

    public static async Task HandleGuildAssignee(MiddlewareContext ctx, Action next)
    {
        try
        {
            if (ctx.Message.SourceType == MessageSourceType.Channel
                && ctx.GuildId is not null
                && !ctx.App.IsGuildAssigned(ctx.Platform, ctx.GuildId))
            {
                ctx.App.SetGuildAssignee(ctx.Platform, ctx.GuildId, ctx.SelfId);
                await ctx.App.Services.GetRequiredService<YukiDbManager>()
                    .InsertGuildDataIfNotExists(ctx.Platform, ctx.GuildId, ctx.SelfId);
            }
        }
        catch (Exception e)
        {
            ctx.App.Logger.LogError(e, "");
        }

        next();
    }
}