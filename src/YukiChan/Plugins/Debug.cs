using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Shared.Utils;

namespace YukiChan.Plugins;

public class DebugPlugin : Plugin
{
    [Command("debug.envinfo")]
    public static MessageContent OnEnvInfo(CommandContext ctx)
    {
        return ctx.Reply()
            .Text("[Debug.EnvInfo]\n")
            .Text($"Platform: {ctx.Platform}\n")
            .Text($"GuildId: {ctx.GuildId}\n")
            .Text($"ChannelId: {ctx.ChannelId}\n")
            .Text($"UserId: {ctx.UserId}\n")
            .Text($"MessageId: {ctx.Message.MessageId}");
    }
}