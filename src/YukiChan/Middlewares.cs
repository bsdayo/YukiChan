using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YukiChan.Database;

namespace YukiChan;

public static class Middlewares
{
    public static void QqGuildFilter(MiddlewareContext ctx, Action next)
    {
        if (ctx.Platform == "qqguild"
            && !Global.YukiConfig.QqGuildAllowedChannels.Contains(ctx.ChannelId))
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