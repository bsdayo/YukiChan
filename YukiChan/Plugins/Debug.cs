using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Messaging;
using YukiChan.Utils;

namespace YukiChan.Plugins;

[Plugin("Debug")]
public class DebugPlugin : Plugin
{
    [Command("debug.envinfo")]
    public static MessageContent OnEnvInfo(MessageContext ctx)
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