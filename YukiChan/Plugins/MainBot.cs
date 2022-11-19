using Flandre.Core.Attributes;
using Flandre.Core.Common;
using Flandre.Core.Extensions;
using Flandre.Core.Messaging;
using YukiChan.Utils;

namespace YukiChan.Plugins;

[Plugin("MainBot")]
public class MainBotPlugin : Plugin
{
    [Command("mainbot")]
    public static async Task<MessageContent> OnMainBot(MessageContext ctx)
    {
        if (ctx.Message.SourceType != MessageSourceType.Channel || ctx.GuildId is null)
            return ctx.Reply("当前不在群聊环境中哦！");
        
        ctx.SetBotAsGuildAssignee();
        await Global.YukiDb.UpdateGuildData(ctx.Platform, ctx.GuildId, ctx.SelfId);
        
        return ctx.Reply("已将本机设置为群内主 Bot~");
    }
}