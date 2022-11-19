using Flandre.Core.Extensions;
using Flandre.Core.Messaging;

namespace YukiChan;

public static class Middlewares
{
    public static void QqGuildFilter(MessageContext ctx, Action next)
    {
        if (ctx.Platform == "qqguild"
            && !Global.YukiConfig.QqGuildAllowedChannels.Contains(ctx.ChannelId))
            return;

        next();
    }

    public static async Task HandleGuildAssignee(MessageContext ctx, Action next)
    {
        if (ctx.Message.SourceType == MessageSourceType.Channel
            && ctx.GuildId is not null
            && !ctx.App.IsGuildAssigned(ctx.Platform, ctx.GuildId))
        {
            ctx.SetBotAsGuildAssignee();
            await Global.YukiDb.InsertGuildDataIfNotExists(ctx.Platform, ctx.GuildId, ctx.SelfId);
        }

        next();
    }
}