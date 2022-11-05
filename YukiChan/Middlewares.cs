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
}