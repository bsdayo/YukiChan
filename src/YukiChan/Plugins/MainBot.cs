using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChan.Client.Console;
using YukiChan.Shared.Data.Console.Guilds;
using YukiChan.Utils;

namespace YukiChan.Plugins;

public class MainBotPlugin : Plugin
{
    private readonly YukiConsoleClient _yukiClient;

    public MainBotPlugin(YukiConsoleClient yukiClient)
    {
        _yukiClient = yukiClient;
    }

    [Command("mainbot")]
    public async Task<MessageContent> OnMainBot(CommandContext ctx)
    {
        if (ctx.Message.SourceType != MessageSourceType.Channel || ctx.GuildId is null)
            return ctx.Reply("当前不在群聊环境中哦！");

        var resp = await _yukiClient.Guilds.UpdateAssignee(ctx.Platform, ctx.GuildId, new GuildUpdateAssigneeRequest
        {
            NewAssigneeId = ctx.SelfId
        });

        return resp.Ok
            ? ctx.Reply("已将本机设置为群内主 Bot~")
            : ctx.ReplyServerError(resp);
    }
}